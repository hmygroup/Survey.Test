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
            $"question/{ConnectionId}/questionary/{questionaryId}", 
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
            $"question/new/{ConnectionId}?questionaryId={questionaryId}",
            questions,
            cancellationToken);
    }

    /// <summary>
    /// Deletes a question.
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await DeleteAsync($"question/{ConnectionId}/{id}", cancellationToken);
    }
}
