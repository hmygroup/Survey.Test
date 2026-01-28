namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a parameter/value for a validation policy.
/// </summary>
public record PolicyRecordsDto
{
    /// <summary>
    /// Unique identifier for the policy record.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The constraint this record belongs to.
    /// </summary>
    public Guid ConstraintId { get; set; }
    
    /// <summary>
    /// The value/parameter for the policy (e.g., "pattern:^[a-z]+$", "min:5").
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
