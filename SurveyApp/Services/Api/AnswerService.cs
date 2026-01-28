namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing answer sessions via the API.
/// </summary>
public class AnswerService : ApiService
{
    public AnswerService(HttpClient httpClient, ILogger<AnswerService> logger) 
        : base(httpClient, logger)
    {
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
        return await PostAsync<object, AnswerDto>(
            $"answer/{ConnectionId}?questionaryId={questionaryId}&user={user}&cardId={cardId}",
            new { },
            cancellationToken);
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

        await PostAsync(
            $"answer/setStatus",
            request,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves all answers for a specific questionnaire.
    /// </summary>
    public async Task<IEnumerable<AnswerDto>?> GetByQuestionaryIdAsync(
        Guid questionaryId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<AnswerDto>>(
            $"answer/{ConnectionId}/questionary/{questionaryId}",
            cancellationToken);
    }
}
