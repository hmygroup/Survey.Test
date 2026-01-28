namespace SurveyApp.Services.StateMachine;

/// <summary>
/// Factory for creating AnswerStateMachine instances.
/// </summary>
public class AnswerStateMachineFactory
{
    private readonly ILogger<AnswerStateMachine> _logger;

    public AnswerStateMachineFactory(ILogger<AnswerStateMachine> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new state machine for an Answer.
    /// </summary>
    /// <param name="answerId">The Answer ID.</param>
    /// <param name="initialState">The initial state.</param>
    /// <param name="user">The user associated with this Answer.</param>
    /// <returns>A configured AnswerStateMachine instance.</returns>
    public AnswerStateMachine Create(Guid answerId, AnswerStatus initialState, string? user = null)
    {
        var stateMachine = new AnswerStateMachine(answerId, initialState, _logger);
        stateMachine.User = user;
        return stateMachine;
    }
}
