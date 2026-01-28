namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a questionnaire with its questions.
/// </summary>
public record FullQuestionaryDto
{
    /// <summary>
    /// Unique identifier for the questionnaire.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the questionnaire.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the questionnaire.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Collection of questions in this questionnaire.
    /// </summary>
    public IEnumerable<QuestionDto> Questions { get; set; } = Array.Empty<QuestionDto>();
}
