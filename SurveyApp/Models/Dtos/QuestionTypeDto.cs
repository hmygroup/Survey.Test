namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents the type of a question.
/// </summary>
public record QuestionTypeDto
{
    /// <summary>
    /// Unique identifier for the question type.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The .NET type that this question type maps to (e.g., System.String, System.Boolean).
    /// </summary>
    public string DotNetType { get; set; } = string.Empty;
}
