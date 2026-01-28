namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a question in a questionnaire.
/// </summary>
public record QuestionDto
{
    /// <summary>
    /// Unique identifier for the question.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The text of the question.
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of the question (text, boolean, choice, etc.).
    /// </summary>
    public QuestionTypeDto? QuestionType { get; set; }
    
    /// <summary>
    /// Collection of responses to this question.
    /// </summary>
    public ICollection<QuestionResponseDto> QuestionResponses { get; set; } = new List<QuestionResponseDto>();
    
    /// <summary>
    /// Collection of distinct answers associated with this question.
    /// </summary>
    public ICollection<AnswerDto> DistinctAnswers { get; set; } = new List<AnswerDto>();
    
    /// <summary>
    /// Collection of validation constraints applied to this question.
    /// </summary>
    public ICollection<ConstraintDto> Constraints { get; set; } = new List<ConstraintDto>();
}
