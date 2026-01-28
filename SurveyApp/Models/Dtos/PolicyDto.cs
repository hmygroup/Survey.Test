namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents a validation policy that can be applied to questions.
/// </summary>
public record PolicyDto
{
    /// <summary>
    /// Unique identifier for the policy.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the policy (e.g., "Email Validation", "Range Check").
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
