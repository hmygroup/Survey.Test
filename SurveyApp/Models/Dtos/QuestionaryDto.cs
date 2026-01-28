namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a questionnaire in the system.
/// </summary>
public record QuestionaryDto
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
    /// User who created the questionnaire.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the questionnaire was created.
    /// </summary>
    public DateTimeOffset? CreationDate { get; set; }
}
