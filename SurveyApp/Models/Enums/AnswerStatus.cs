namespace SurveyApp.Models.Enums;

/// <summary>
/// Represents the status of an Answer (survey session).
/// </summary>
public enum AnswerStatus
{
    /// <summary>
    /// The survey has been started but not yet completed.
    /// </summary>
    Unfinished,
    
    /// <summary>
    /// The survey has been submitted and is pending approval.
    /// </summary>
    Pending,
    
    /// <summary>
    /// The survey has been completed and approved.
    /// </summary>
    Completed,
    
    /// <summary>
    /// The survey has been cancelled.
    /// </summary>
    Cancelled
}
