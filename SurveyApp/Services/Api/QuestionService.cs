namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing questions via the API.
/// </summary>
public class QuestionService : ApiService
{
    public QuestionService(HttpClient httpClient, ILogger<QuestionService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Retrieves all questions for a specific questionnaire.
    /// </summary>
    public async Task<IEnumerable<QuestionDto>?> GetByQuestionaryIdAsync(
        Guid questionaryId, 
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<QuestionDto>>(
            $"Question/{ConnectionId}/get?questionaryId={questionaryId}", 
            cancellationToken);
    }

    /// <summary>
    /// Creates new questions for a questionnaire.
    /// </summary>
    public async Task<IEnumerable<QuestionDto>?> CreateAsync(
        Guid questionaryId,
        IEnumerable<object> questions,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<IEnumerable<object>, IEnumerable<QuestionDto>>(
            $"Question/new/{ConnectionId}?questionaryId={questionaryId}",
            questions,
            cancellationToken);
    }

    /// <summary>
    /// Deletes a question.
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Note: DELETE endpoint for Question is not documented in the API
        // This method may not work until the API provides the endpoint
        await DeleteAsync($"Question/{ConnectionId}/{id}", cancellationToken);
    }
}
