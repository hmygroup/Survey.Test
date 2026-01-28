using SurveyApp.Services.StateMachine;

namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing answer sessions via the API.
/// Integrates with AnswerStateMachine for state management.
/// </summary>
public class AnswerService : ApiService
{
    private readonly ILogger<AnswerService> _answerLogger;
    private readonly AnswerStateMachineFactory _stateMachineFactory;
    private readonly Dictionary<Guid, AnswerStateMachine> _stateMachines = new();

    public AnswerService(
        HttpClient httpClient, 
        ILogger<AnswerService> logger,
        AnswerStateMachineFactory stateMachineFactory) 
        : base(httpClient, logger)
    {
        _answerLogger = logger;
        _stateMachineFactory = stateMachineFactory;
    }

    /// <summary>
    /// Creates a new answer session for a questionnaire.
    /// </summary>
    public async Task<AnswerDto?> CreateAsync(
        Guid questionaryId,
        string user,
        int cardId,
        CancellationToken cancellationToken = default)
    {
        var encodedUser = Uri.EscapeDataString(user);
        var answer = await PostAsync<object, AnswerDto>(
            $"Answer/{ConnectionId}?questionaryId={questionaryId}&user={encodedUser}&cardId={cardId}",
            new { },
            cancellationToken);

        // Initialize state machine for this answer
        if (answer != null)
        {
            var stateMachine = _stateMachineFactory.Create(
                answer.Id,
                answer.AnswerStatus?.AnswerStatus ?? AnswerStatus.Unfinished,
                user);
            _stateMachines[answer.Id] = stateMachine;
            
            _answerLogger.LogInformation("Created Answer {AnswerId} with state machine in {State}", 
                answer.Id, stateMachine.CurrentState);
        }

        return answer;
    }

    /// <summary>
    /// Gets or creates a state machine for an answer.
    /// </summary>
    public AnswerStateMachine GetStateMachine(Guid answerId, AnswerStatus currentState, string? user = null)
    {
        if (!_stateMachines.TryGetValue(answerId, out var stateMachine))
        {
            stateMachine = _stateMachineFactory.Create(answerId, currentState, user);
            _stateMachines[answerId] = stateMachine;
        }
        return stateMachine;
    }

    /// <summary>
    /// Transitions an answer to a new state using the state machine.
    /// </summary>
    public async Task<bool> TransitionStateAsync(
        Guid answerId,
        AnswerTrigger trigger,
        AnswerStatus currentState,
        string? user = null,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stateMachine = GetStateMachine(answerId, currentState, user);

            // Attempt the transition
            if (!stateMachine.Fire(trigger, notes))
            {
                _answerLogger.LogWarning("State transition failed for Answer {AnswerId}: {Trigger} from {State}",
                    answerId, trigger, currentState);
                return false;
            }

            // Update the status via API
            var newStatus = stateMachine.CurrentState;
            await SetStatusAsync(new[] { answerId }, newStatus, cancellationToken);

            _answerLogger.LogInformation("Answer {AnswerId} transitioned to {NewState} via {Trigger}",
                answerId, newStatus, trigger);
            return true;
        }
        catch (Exception ex)
        {
            _answerLogger.LogError(ex, "Error transitioning state for Answer {AnswerId}", answerId);
            return false;
        }
    }

    /// <summary>
    /// Updates the status of answer sessions.
    /// </summary>
    public async Task SetStatusAsync(
        IEnumerable<Guid> answerIds,
        AnswerStatus status,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            answersId = answerIds,
            ANSWER_STATUS = status.ToString().ToUpper()
        };

        await PutAsync<object, object>(
            $"Answer/setStatus",
            request,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific answer by ID.
    /// </summary>
    public async Task<AnswerDto?> GetByIdAsync(
        Guid answerId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<AnswerDto>(
            $"Answer/{ConnectionId}/{answerId}",
            cancellationToken);
    }

    /// <summary>
    /// Retrieves all answers for a specific questionnaire.
    /// </summary>
    public async Task<IEnumerable<AnswerDto>?> GetByQuestionaryIdAsync(
        Guid questionaryId,
        CancellationToken cancellationToken = default)
    {
        // Note: This endpoint is not documented in the API
        // This method may not work until the API provides the endpoint
        return await GetAsync<IEnumerable<AnswerDto>>(
            $"Answer/{ConnectionId}/questionary/{questionaryId}",
            cancellationToken);
    }

    /// <summary>
    /// Gets the state transition history for an answer.
    /// </summary>
    public IReadOnlyList<StateTransitionHistory>? GetStateHistory(Guid answerId)
    {
        return _stateMachines.TryGetValue(answerId, out var stateMachine) 
            ? stateMachine.History 
            : null;
    }
}
