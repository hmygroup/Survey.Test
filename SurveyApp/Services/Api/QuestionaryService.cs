namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing questionnaires via the API.
/// </summary>
public class QuestionaryService : ApiService
{
    public QuestionaryService(HttpClient httpClient, ILogger<QuestionaryService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Retrieves all questionnaires.
    /// </summary>
    public async Task<IEnumerable<QuestionaryDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<QuestionaryDto>>(
            $"questionary/{ConnectionId}/all", 
            cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific questionnaire by ID.
    /// </summary>
    public async Task<QuestionaryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<QuestionaryDto>(
            $"questionary/{ConnectionId}/{id}", 
            cancellationToken);
    }

    /// <summary>
    /// Retrieves a questionnaire with all its questions.
    /// </summary>
    public async Task<FullQuestionaryDto?> GetFullAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<FullQuestionaryDto>(
            $"questionary/{ConnectionId}/{id}/full", 
            cancellationToken);
    }

    /// <summary>
    /// Creates a new questionnaire.
    /// </summary>
    public async Task<QuestionaryDto?> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        return await PostAsync<object, QuestionaryDto>(
            $"questionary/{ConnectionId}/New/{name}", 
            new { }, 
            cancellationToken);
    }

    /// <summary>
    /// Deletes a questionnaire.
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await DeleteAsync($"questionary/{ConnectionId}/{id}", cancellationToken);
    }
}
