using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SurveyApp.Models;

namespace SurveyApp.Services;

/// <summary>
/// Service for managing session checkpoints, auto-save, and recovery.
/// Provides functionality to save, load, and manage survey session checkpoints.
/// </summary>
public class SessionManager
{
    private readonly ILogger<SessionManager> _logger;
    private readonly string _checkpointDirectory;
    private Timer? _autoSaveTimer;
    private Func<Task<SessionCheckpoint?>>? _getCheckpointCallback;

    /// <summary>
    /// Gets or sets the auto-save interval in seconds. Default is 30 seconds.
    /// </summary>
    public int AutoSaveIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionManager"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SessionManager(ILogger<SessionManager> logger)
    {
        _logger = logger;
        
        // Set checkpoint directory to %APPDATA%\SurveyApp\Sessions\
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _checkpointDirectory = Path.Combine(appDataPath, "SurveyApp", "Sessions");
        
        // Ensure directory exists
        if (!Directory.Exists(_checkpointDirectory))
        {
            Directory.CreateDirectory(_checkpointDirectory);
            _logger.LogInformation("Created checkpoint directory: {Directory}", _checkpointDirectory);
        }
    }

    /// <summary>
    /// Starts auto-save timer with the specified callback.
    /// </summary>
    /// <param name="getCheckpointCallback">Callback function to get the current checkpoint data.</param>
    public void StartAutoSave(Func<Task<SessionCheckpoint?>> getCheckpointCallback)
    {
        _getCheckpointCallback = getCheckpointCallback;
        
        // Stop existing timer if any
        _autoSaveTimer?.Dispose();
        
        // Create new timer
        _autoSaveTimer = new Timer(
            async _ => await AutoSaveCallback(),
            null,
            TimeSpan.FromSeconds(AutoSaveIntervalSeconds),
            TimeSpan.FromSeconds(AutoSaveIntervalSeconds)
        );
        
        _logger.LogInformation("Auto-save started with interval: {Interval} seconds", AutoSaveIntervalSeconds);
    }

    /// <summary>
    /// Stops the auto-save timer.
    /// </summary>
    public void StopAutoSave()
    {
        _autoSaveTimer?.Dispose();
        _autoSaveTimer = null;
        _logger.LogInformation("Auto-save stopped");
    }

    /// <summary>
    /// Auto-save callback that executes periodically.
    /// </summary>
    private async Task AutoSaveCallback()
    {
        try
        {
            if (_getCheckpointCallback == null)
            {
                return;
            }

            var checkpoint = await _getCheckpointCallback();
            if (checkpoint != null)
            {
                await SaveCheckpointAsync(checkpoint);
                _logger.LogDebug("Auto-save completed for Answer: {AnswerId}", checkpoint.AnswerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auto-save failed");
        }
    }

    /// <summary>
    /// Saves a checkpoint to disk with DPAPI encryption.
    /// </summary>
    /// <param name="checkpoint">The checkpoint to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveCheckpointAsync(SessionCheckpoint checkpoint)
    {
        try
        {
            checkpoint.LastUpdatedAt = DateTime.UtcNow;
            
            // Serialize to JSON
            var json = JsonSerializer.Serialize(checkpoint, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            // Encrypt using DPAPI
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var encryptedBytes = ProtectedData.Protect(
                jsonBytes,
                null, // No additional entropy
                DataProtectionScope.CurrentUser
            );
            
            // Save to file
            var fileName = $"{checkpoint.AnswerId}.checkpoint";
            var filePath = Path.Combine(_checkpointDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, encryptedBytes);
            
            _logger.LogInformation("Checkpoint saved: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save checkpoint for Answer: {AnswerId}", checkpoint.AnswerId);
            throw;
        }
    }

    /// <summary>
    /// Loads a checkpoint from disk.
    /// </summary>
    /// <param name="answerId">The Answer ID to load.</param>
    /// <returns>The checkpoint if found, otherwise null.</returns>
    public async Task<SessionCheckpoint?> LoadCheckpointAsync(Guid answerId)
    {
        try
        {
            var fileName = $"{answerId}.checkpoint";
            var filePath = Path.Combine(_checkpointDirectory, fileName);
            
            if (!File.Exists(filePath))
            {
                _logger.LogDebug("Checkpoint not found: {FileName}", fileName);
                return null;
            }
            
            // Read encrypted file
            var encryptedBytes = await File.ReadAllBytesAsync(filePath);
            
            // Decrypt using DPAPI
            var jsonBytes = ProtectedData.Unprotect(
                encryptedBytes,
                null, // No additional entropy
                DataProtectionScope.CurrentUser
            );
            
            // Deserialize from JSON
            var json = Encoding.UTF8.GetString(jsonBytes);
            var checkpoint = JsonSerializer.Deserialize<SessionCheckpoint>(json);
            
            _logger.LogInformation("Checkpoint loaded: {FileName}", fileName);
            return checkpoint;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load checkpoint for Answer: {AnswerId}", answerId);
            return null;
        }
    }

    /// <summary>
    /// Deletes a checkpoint from disk.
    /// </summary>
    /// <param name="answerId">The Answer ID to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteCheckpointAsync(Guid answerId)
    {
        try
        {
            var fileName = $"{answerId}.checkpoint";
            var filePath = Path.Combine(_checkpointDirectory, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Checkpoint deleted: {FileName}", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete checkpoint for Answer: {AnswerId}", answerId);
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets all unfinished sessions from disk.
    /// </summary>
    /// <returns>List of unfinished session checkpoints.</returns>
    public async Task<List<SessionCheckpoint>> GetUnfinishedSessionsAsync()
    {
        var sessions = new List<SessionCheckpoint>();
        
        try
        {
            var files = Directory.GetFiles(_checkpointDirectory, "*.checkpoint");
            
            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (Guid.TryParse(fileName, out var answerId))
                    {
                        var checkpoint = await LoadCheckpointAsync(answerId);
                        if (checkpoint != null && checkpoint.Status == "UNFINISHED")
                        {
                            sessions.Add(checkpoint);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load checkpoint: {File}", file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get unfinished sessions");
        }
        
        return sessions;
    }

    /// <summary>
    /// Cleans up stale checkpoints (older than 7 days).
    /// </summary>
    /// <returns>Number of checkpoints deleted.</returns>
    public async Task<int> CleanupStaleCheckpointsAsync()
    {
        var deletedCount = 0;
        
        try
        {
            var files = Directory.GetFiles(_checkpointDirectory, "*.checkpoint");
            
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    var age = DateTime.UtcNow - fileInfo.LastWriteTimeUtc;
                    
                    if (age.TotalDays > 7)
                    {
                        File.Delete(file);
                        deletedCount++;
                        _logger.LogInformation("Deleted stale checkpoint: {File}", fileInfo.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete checkpoint: {File}", file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup stale checkpoints");
        }
        
        _logger.LogInformation("Cleanup complete. Deleted {Count} stale checkpoints", deletedCount);
        return deletedCount;
    }
}
