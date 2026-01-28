namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a validation constraint applied to a question.
/// </summary>
public record ConstraintDto
{
    /// <summary>
    /// Unique identifier for the constraint.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The question this constraint applies to.
    /// </summary>
    public Guid? QuestionId { get; set; }
    
    /// <summary>
    /// The validation policy for this constraint.
    /// </summary>
    public PolicyDto? Policy { get; set; }
    
    /// <summary>
    /// Collection of policy records (validation parameters).
    /// </summary>
    public IEnumerable<PolicyRecordsDto> PolicyRecords { get; set; } = Array.Empty<PolicyRecordsDto>();
}
