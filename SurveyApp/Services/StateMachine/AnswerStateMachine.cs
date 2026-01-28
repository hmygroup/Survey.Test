using Stateless;

namespace SurveyApp.Services.StateMachine;

/// <summary>
/// State machine for managing Answer status transitions.
/// Uses the Stateless library to enforce valid state transitions and prevent invalid state changes.
/// </summary>
public class AnswerStateMachine
{
    private readonly StateMachine<AnswerStatus, AnswerTrigger> _stateMachine;
    private readonly ILogger<AnswerStateMachine> _logger;
    private readonly List<StateTransitionHistory> _history = new();
    
    /// <summary>
    /// The current state of the Answer.
    /// </summary>
    public AnswerStatus CurrentState => _stateMachine.State;
    
    /// <summary>
    /// The Answer ID this state machine is managing.
    /// </summary>
    public Guid AnswerId { get; }
    
    /// <summary>
    /// User associated with this Answer session.
    /// </summary>
    public string? User { get; set; }
    
    /// <summary>
    /// Gets the state transition history.
    /// </summary>
    public IReadOnlyList<StateTransitionHistory> History => _history.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the AnswerStateMachine.
    /// </summary>
    /// <param name="answerId">The Answer ID.</param>
    /// <param name="initialState">The initial state of the Answer.</param>
    /// <param name="logger">Logger instance.</param>
    public AnswerStateMachine(
        Guid answerId,
        AnswerStatus initialState,
        ILogger<AnswerStateMachine> logger)
    {
        AnswerId = answerId;
        _logger = logger;
        _stateMachine = new StateMachine<AnswerStatus, AnswerTrigger>(initialState);

        ConfigureStateMachine();
        
        _logger.LogInformation("Answer state machine initialized for {AnswerId} with state {State}", 
            answerId, initialState);
    }

    /// <summary>
    /// Configures the state machine with valid transitions.
    /// </summary>
    private void ConfigureStateMachine()
    {
        // UNFINISHED state: can complete survey or cancel
        _stateMachine.Configure(AnswerStatus.Unfinished)
            .Permit(AnswerTrigger.Complete, AnswerStatus.Pending)
            .Permit(AnswerTrigger.Cancel, AnswerStatus.Cancelled)
            .OnEntry(() => LogStateEntry(AnswerStatus.Unfinished))
            .OnExit(() => LogStateExit(AnswerStatus.Unfinished));

        // PENDING state: can be approved, rejected, or cancelled
        _stateMachine.Configure(AnswerStatus.Pending)
            .Permit(AnswerTrigger.Approve, AnswerStatus.Completed)
            .Permit(AnswerTrigger.Reject, AnswerStatus.Unfinished)
            .Permit(AnswerTrigger.Cancel, AnswerStatus.Cancelled)
            .OnEntry(() => LogStateEntry(AnswerStatus.Pending))
            .OnExit(() => LogStateExit(AnswerStatus.Pending));

        // COMPLETED state: terminal state (no transitions out)
        _stateMachine.Configure(AnswerStatus.Completed)
            .OnEntry(() => LogStateEntry(AnswerStatus.Completed));

        // CANCELLED state: terminal state (no transitions out)
        _stateMachine.Configure(AnswerStatus.Cancelled)
            .OnEntry(() => LogStateEntry(AnswerStatus.Cancelled));
    }

    /// <summary>
    /// Fires a trigger to transition to a new state.
    /// </summary>
    /// <param name="trigger">The trigger to fire.</param>
    /// <param name="notes">Optional notes about the transition.</param>
    /// <returns>True if the transition was successful, false otherwise.</returns>
    public bool Fire(AnswerTrigger trigger, string? notes = null)
    {
        try
        {
            var fromState = CurrentState;
            
            if (!CanFire(trigger))
            {
                _logger.LogWarning("Invalid transition: Cannot fire {Trigger} from {State} for Answer {AnswerId}",
                    trigger, fromState, AnswerId);
                return false;
            }

            _stateMachine.Fire(trigger);
            var toState = CurrentState;

            // Record transition in history
            var transition = new StateTransitionHistory
            {
                AnswerId = AnswerId,
                FromState = fromState,
                ToState = toState,
                Trigger = trigger.ToString(),
                TransitionedBy = User,
                Notes = notes,
                TransitionedAt = DateTime.UtcNow
            };
            
            _history.Add(transition);

            _logger.LogInformation("State transition: {From} → {To} via {Trigger} for Answer {AnswerId}",
                fromState, toState, trigger, AnswerId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during state transition for Answer {AnswerId}", AnswerId);
            return false;
        }
    }

    /// <summary>
    /// Checks if a trigger can be fired from the current state.
    /// </summary>
    /// <param name="trigger">The trigger to check.</param>
    /// <returns>True if the trigger can be fired, false otherwise.</returns>
    public bool CanFire(AnswerTrigger trigger)
    {
        return _stateMachine.CanFire(trigger);
    }

    /// <summary>
    /// Gets all permitted triggers from the current state.
    /// </summary>
    /// <returns>List of permitted triggers.</returns>
    public IEnumerable<AnswerTrigger> GetPermittedTriggers()
    {
        return _stateMachine.PermittedTriggers;
    }

    /// <summary>
    /// Gets a description of the current state and permitted transitions.
    /// </summary>
    public string GetStateDescription()
    {
        var permitted = string.Join(", ", GetPermittedTriggers());
        return $"Current: {CurrentState}, Permitted: [{permitted}]";
    }

    private void LogStateEntry(AnswerStatus state)
    {
        _logger.LogDebug("Entering state {State} for Answer {AnswerId}", state, AnswerId);
    }

    private void LogStateExit(AnswerStatus state)
    {
        _logger.LogDebug("Exiting state {State} for Answer {AnswerId}", state, AnswerId);
    }
}
