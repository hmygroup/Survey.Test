namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing question types via the API.
/// </summary>
public class QuestionTypeService : ApiService
{
    public QuestionTypeService(HttpClient httpClient, ILogger<QuestionTypeService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Retrieves all question types.
    /// </summary>
    public async Task<IEnumerable<QuestionTypeDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<QuestionTypeDto>>(
            $"QuestionType/{ConnectionId}/all", 
            cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific question type by ID.
    /// </summary>
    public async Task<QuestionTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<QuestionTypeDto>(
            $"QuestionType/{ConnectionId}/{id}", 
            cancellationToken);
    }

    /// <summary>
    /// Creates a new question type.
    /// </summary>
    public async Task<QuestionTypeDto?> AddAsync(
        string name, 
        string dotNetType, 
        CancellationToken cancellationToken = default)
    {
        var encodedName = Uri.EscapeDataString(name);
        var encodedDotNetType = Uri.EscapeDataString(dotNetType);
        
        return await PostAsync<object, QuestionTypeDto>(
            $"QuestionType/{ConnectionId}/Add?name={encodedName}&dotNetType={encodedDotNetType}",
            new { },
            cancellationToken);
    }
}
