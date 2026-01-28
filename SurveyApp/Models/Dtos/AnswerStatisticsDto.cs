namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents statistics for answers of a questionary.
/// </summary>
public record AnswerStatisticsDto
{
    /// <summary>
    /// Gets the questionary ID.
    /// </summary>
    public Guid QuestionaryId { get; init; }

    /// <summary>
    /// Gets the total number of answers.
    /// </summary>
    public int TotalAnswers { get; init; }

    /// <summary>
    /// Gets the number of unfinished answers.
    /// </summary>
    public int UnfinishedCount { get; init; }

    /// <summary>
    /// Gets the number of pending answers.
    /// </summary>
    public int PendingCount { get; init; }

    /// <summary>
    /// Gets the number of completed answers.
    /// </summary>
    public int CompletedCount { get; init; }

    /// <summary>
    /// Gets the number of cancelled answers.
    /// </summary>
    public int CancelledCount { get; init; }

    /// <summary>
    /// Gets the completion rate as a percentage.
    /// </summary>
    public double CompletionRate { get; init; }

    /// <summary>
    /// Gets the average completion time in seconds.
    /// </summary>
    public double? AverageCompletionTimeSeconds { get; init; }

    /// <summary>
    /// Gets the number of responses in the last 24 hours.
    /// </summary>
    public int ResponsesLast24Hours { get; init; }

    /// <summary>
    /// Gets the number of responses in the last 7 days.
    /// </summary>
    public int ResponsesLast7Days { get; init; }
}
