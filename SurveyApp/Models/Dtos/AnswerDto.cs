namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a survey session - when a user starts answering a questionnaire.
/// </summary>
public record AnswerDto
{
    /// <summary>
    /// Unique identifier for the answer session.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The questionnaire being answered.
    /// </summary>
    public Guid QuestionaryId { get; set; }
    
    /// <summary>
    /// The user responding to the questionnaire.
    /// </summary>
    public string User { get; set; } = string.Empty;
    
    /// <summary>
    /// Card ID associated with this answer session.
    /// </summary>
    public int CardId { get; set; }
    
    /// <summary>
    /// Current status of the answer session.
    /// </summary>
    public AnswerStatusDto? AnswerStatus { get; set; }
}
