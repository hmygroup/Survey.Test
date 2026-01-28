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
        var encodedUser = Uri.EscapeDataString(user);
        return await PostAsync<object, AnswerDto>(
            $"Answer/{ConnectionId}?questionaryId={questionaryId}&user={encodedUser}&cardId={cardId}",
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
}
