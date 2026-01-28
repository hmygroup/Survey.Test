namespace SurveyApp.Models;

/// <summary>
/// Represents a saved checkpoint of a survey response session.
/// Used for auto-save and recovery functionality.
/// </summary>
public class SessionCheckpoint
{
    /// <summary>
    /// Gets or sets the unique identifier for this checkpoint.
    /// </summary>
    public Guid CheckpointId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the Answer ID this checkpoint belongs to.
    /// </summary>
    public Guid AnswerId { get; set; }

    /// <summary>
    /// Gets or sets the Questionary ID.
    /// </summary>
    public Guid QuestionaryId { get; set; }

    /// <summary>
    /// Gets or sets the questionary title for display.
    /// </summary>
    public string QuestionaryTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current question index.
    /// </summary>
    public int CurrentQuestionIndex { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of responses (QuestionId -> Response Value).
    /// </summary>
    public Dictionary<Guid, string> Responses { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp when this checkpoint was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when this checkpoint was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the current status of the answer.
    /// </summary>
    public string Status { get; set; } = "UNFINISHED";

    /// <summary>
    /// Gets or sets metadata about the session (device info, time spent, etc.).
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets the age of this checkpoint.
    /// </summary>
    public TimeSpan Age => DateTime.UtcNow - LastUpdatedAt;

    /// <summary>
    /// Gets whether this checkpoint is stale (older than 7 days).
    /// </summary>
    public bool IsStale => Age.TotalDays > 7;
}
