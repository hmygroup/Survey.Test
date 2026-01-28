namespace SurveyApp.Services.Api;

/// <summary>
/// Service for managing policies via the API.
/// </summary>
public class PolicyService : ApiService
{
    public PolicyService(HttpClient httpClient, ILogger<PolicyService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Retrieves all policies.
    /// </summary>
    public async Task<IEnumerable<PolicyDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<PolicyDto>>(
            $"Policy/{ConnectionId}/all", 
            cancellationToken);
    }
}
