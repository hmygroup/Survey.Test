namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing question responses via the API.
/// </summary>
public class QuestionResponseService : ApiService
{
    public QuestionResponseService(HttpClient httpClient, ILogger<QuestionResponseService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Creates or updates question responses.
    /// </summary>
    public async Task<IEnumerable<QuestionResponseDto>?> SaveResponsesAsync(
        IEnumerable<object> responses,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<IEnumerable<object>, IEnumerable<QuestionResponseDto>>(
            $"questionresponse/{ConnectionId}/response",
            responses,
            cancellationToken);
    }

    /// <summary>
    /// Updates a specific question response.
    /// </summary>
    public async Task<QuestionResponseDto?> UpdateResponseAsync(
        Guid questionResponseId,
        string response,
        CancellationToken cancellationToken = default)
    {
        return await PatchAsync<object, QuestionResponseDto>(
            $"questionresponse/{ConnectionId}/response?questionResponseId={questionResponseId}&response={response}",
            new { },
            cancellationToken);
    }

    /// <summary>
    /// Retrieves all responses for a specific answer session.
    /// </summary>
    public async Task<IEnumerable<QuestionResponseDto>?> GetByAnswerIdAsync(
        Guid answerId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<QuestionResponseDto>>(
            $"questionresponse/{ConnectionId}/answer/{answerId}",
            cancellationToken);
    }
}
