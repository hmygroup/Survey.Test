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
            $"QuestionResponse/{ConnectionId}/response",
            responses,
            cancellationToken);
    }

    /// <summary>
    /// Updates a specific question response.
    /// </summary>
    public async Task<QuestionResponseDto?> UpdateResponseAsync(
        Guid questionResponseId,
        string newValue,
        string? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var encodedNewValue = Uri.EscapeDataString(newValue);
        var url = $"QuestionResponse/{ConnectionId}/response?QuestionResponseID={questionResponseId}&newValue={encodedNewValue}";
        
        if (!string.IsNullOrEmpty(metadata))
        {
            var encodedMetadata = Uri.EscapeDataString(metadata);
            url += $"&metadata={encodedMetadata}";
        }
        
        return await PatchAsync<object, QuestionResponseDto>(
            url,
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
        // Note: This endpoint is not documented in the API
        // This method may not work until the API provides the endpoint
        return await GetAsync<IEnumerable<QuestionResponseDto>>(
            $"QuestionResponse/{ConnectionId}/answer/{answerId}",
            cancellationToken);
    }

    /// <summary>
    /// Deletes a specific question response.
    /// </summary>
    public async Task DeleteResponseAsync(
        Guid questionResponseId,
        CancellationToken cancellationToken = default)
    {
        await DeleteAsync(
            $"QuestionResponse/{ConnectionId}/response/{questionResponseId}",
            cancellationToken);
    }
}
