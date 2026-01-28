namespace SurveyApp.Services.Infrastructure;

/// <summary>
/// Service for displaying dialogs and messages to the user.
/// </summary>
public class DialogService
{
    private readonly ILogger<DialogService> _logger;

    public DialogService(ILogger<DialogService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Shows an information message.
    /// </summary>
    public async Task ShowMessageAsync(string title, string message)
    {
        _logger.LogInformation("Showing message: {Title}", title);
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    /// <summary>
    /// Shows an error message.
    /// </summary>
    public async Task ShowErrorAsync(string title, string message)
    {
        _logger.LogError("Showing error: {Title} - {Message}", title, message);
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    public async Task ShowWarningAsync(string title, string message)
    {
        _logger.LogWarning("Showing warning: {Title}", title);
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        });
    }

    /// <summary>
    /// Shows a confirmation dialog.
    /// </summary>
    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        _logger.LogInformation("Showing confirmation: {Title}", title);
        var result = false;
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialogResult = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            result = dialogResult == MessageBoxResult.Yes;
        });
        return result;
    }

    /// <summary>
    /// Shows an input dialog (simplified implementation).
    /// </summary>
    public async Task<string?> ShowInputDialogAsync(string title, string prompt)
    {
        _logger.LogInformation("Showing input dialog: {Title}", title);
        
        // For a production app, use a custom dialog window
        // This is a simplified version using MessageBox
        string? result = null;
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // In a real app, create a custom dialog window
            // For now, return null to indicate cancellation
            result = null;
        });
        return result;
    }
}
