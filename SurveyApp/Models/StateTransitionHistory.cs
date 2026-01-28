namespace SurveyApp.Models;

/// <summary>
/// Represents a state transition in the Answer lifecycle.
/// Used for auditing and tracking the history of state changes.
/// </summary>
public record StateTransitionHistory
{
    /// <summary>
    /// Unique identifier for this transition.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// The Answer ID this transition belongs to.
    /// </summary>
    public Guid AnswerId { get; init; }
    
    /// <summary>
    /// The state before the transition.
    /// </summary>
    public AnswerStatus FromState { get; init; }
    
    /// <summary>
    /// The state after the transition.
    /// </summary>
    public AnswerStatus ToState { get; init; }
    
    /// <summary>
    /// The trigger that caused the transition.
    /// </summary>
    public string Trigger { get; init; } = string.Empty;
    
    /// <summary>
    /// When the transition occurred.
    /// </summary>
    public DateTime TransitionedAt { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// User who triggered the transition.
    /// </summary>
    public string? TransitionedBy { get; init; }
    
    /// <summary>
    /// Optional notes or reason for the transition.
    /// </summary>
    public string? Notes { get; init; }
}
