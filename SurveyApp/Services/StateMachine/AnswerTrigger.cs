namespace SurveyApp.Services.StateMachine;

/// <summary>
/// Represents a trigger that can cause a state transition in the Answer state machine.
/// </summary>
public enum AnswerTrigger
{
    /// <summary>
    /// Start working on the survey (UNFINISHED → UNFINISHED).
    /// </summary>
    Start,
    
    /// <summary>
    /// Complete the survey and submit for review (UNFINISHED → PENDING).
    /// </summary>
    Complete,
    
    /// <summary>
    /// Approve a pending survey (PENDING → COMPLETED).
    /// </summary>
    Approve,
    
    /// <summary>
    /// Reject a pending survey, sending it back (PENDING → UNFINISHED).
    /// </summary>
    Reject,
    
    /// <summary>
    /// Cancel the survey from any state (* → CANCELLED).
    /// </summary>
    Cancel
}
