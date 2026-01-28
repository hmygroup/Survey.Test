namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents an individual response to a specific question within an answer session.
/// </summary>
public record QuestionResponseDto
{
    /// <summary>
    /// Unique identifier for the question response.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The question being answered.
    /// </summary>
    public Guid QuestionId { get; set; }
    
    /// <summary>
    /// The answer session this response belongs to.
    /// </summary>
    public Guid AnswerId { get; set; }
    
    /// <summary>
    /// The actual response value (stored as string).
    /// </summary>
    public string Response { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional metadata for the response (stored as JSON).
    /// </summary>
    public string Metadata { get; set; } = string.Empty;
    
    /// <summary>
    /// Collection of answers associated with this response.
    /// </summary>
    public ICollection<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
}
