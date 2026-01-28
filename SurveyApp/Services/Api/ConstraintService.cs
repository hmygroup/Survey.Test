namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing constraints via the API.
/// </summary>
public class ConstraintService : ApiService
{
    public ConstraintService(HttpClient httpClient, ILogger<ConstraintService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Creates a new constraint.
    /// </summary>
    public async Task<ConstraintDto?> CreateAsync(
        int order,
        Guid policyId,
        Guid questionId,
        IEnumerable<PolicyRecordsDto> policyRecords,
        CancellationToken cancellationToken = default)
    {
        var constraintData = new
        {
            order = order,
            policyId = policyId,
            questionId = questionId,
            policyRecords = policyRecords
        };

        return await PostAsync<object, ConstraintDto>(
            $"Constraint/{ConnectionId}",
            constraintData,
            cancellationToken);
    }

    /// <summary>
    /// Updates an existing constraint.
    /// </summary>
    public async Task<ConstraintDto?> UpdateAsync(
        Guid id,
        int order,
        Guid policyId,
        Guid questionId,
        IEnumerable<PolicyRecordsDto> policyRecords,
        CancellationToken cancellationToken = default)
    {
        var constraintData = new
        {
            order = order,
            policyId = policyId,
            questionId = questionId,
            policyRecords = policyRecords
        };

        return await PutAsync<object, ConstraintDto>(
            $"Constraint/{ConnectionId}/{id}",
            constraintData,
            cancellationToken);
    }

    /// <summary>
    /// Deletes a constraint.
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await DeleteAsync($"Constraint/{ConnectionId}/{id}", cancellationToken);
    }

    /// <summary>
    /// Gets all constraints for a specific question.
    /// </summary>
    public async Task<IEnumerable<ConstraintDto>?> GetByQuestionIdAsync(
        Guid questionId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<ConstraintDto>>(
            $"Constraint/{ConnectionId}/question/{questionId}",
            cancellationToken);
    }
}
