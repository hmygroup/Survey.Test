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
        await DeleteAsync($"Question/{ConnectionId}/{id}", cancellationToken);
    }

    /// <summary>
    /// Updates an existing question.
    /// </summary>
    public async Task<QuestionDto?> UpdateAsync(
        Guid id,
        string questionText,
        string questionType,
        Guid questionaryId,
        IEnumerable<ConstraintDto>? constraints = null,
        CancellationToken cancellationToken = default)
    {
        var updateData = new
        {
            id = id,
            questionText = questionText,
            questionType = new { dotNetType = questionType },
            questionaryId = questionaryId,
            constraints = constraints ?? Enumerable.Empty<ConstraintDto>()
        };

        return await PutAsync<object, QuestionDto>(
            $"Question/{ConnectionId}/{id}",
            updateData,
            cancellationToken);
    }

    /// <summary>
    /// Reorders questions in bulk.
    /// </summary>
    public async Task ReorderQuestionsAsync(
        Guid questionaryId,
        IEnumerable<(Guid QuestionId, int Order)> questionOrders,
        CancellationToken cancellationToken = default)
    {
        var reorderData = new
        {
            questionaryId = questionaryId,
            questions = questionOrders.Select(qo => new
            {
                questionId = qo.QuestionId,
                order = qo.Order
            })
        };

        await PatchAsync<object, object>(
            $"Question/{ConnectionId}/reorder",
            reorderData,
            cancellationToken);
    }
}
