namespace SurveyApp.Models.Dtos;

/// <summary>
/// Represents the status information for an Answer.
/// </summary>
public record AnswerStatusDto
{
    /// <summary>
    /// Unique identifier for the status.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the status.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The answer status enum value.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AnswerStatus AnswerStatus { get; set; }
}
