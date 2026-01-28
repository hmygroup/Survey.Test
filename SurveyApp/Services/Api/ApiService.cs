namespace SurveyApp.Services.Api;

/// <summary>
/// Base service for making HTTP requests to the Survey API.
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Gets or sets the current connection ID for multi-tenant database access.
    /// </summary>
    public int ConnectionId { get; set; } = 1;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    /// <summary>
    /// Sends a GET request to the specified URI.
    /// </summary>
    protected async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GET request to {Uri}", requestUri);
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during GET request to {Uri}", requestUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GET request to {Uri}", requestUri);
            throw;
        }
    }

    /// <summary>
    /// Sends a POST request to the specified URI.
    /// </summary>
    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string requestUri, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("POST request to {Uri}", requestUri);
            var response = await _httpClient.PostAsJsonAsync(requestUri, data, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during POST request to {Uri}", requestUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during POST request to {Uri}", requestUri);
            throw;
        }
    }

    /// <summary>
    /// Sends a POST request to the specified URI without expecting a response body.
    /// </summary>
    protected async Task PostAsync<TRequest>(
        string requestUri, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("POST request to {Uri}", requestUri);
            var response = await _httpClient.PostAsJsonAsync(requestUri, data, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during POST request to {Uri}", requestUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during POST request to {Uri}", requestUri);
            throw;
        }
    }

    /// <summary>
    /// Sends a PUT request to the specified URI.
    /// </summary>
    protected async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string requestUri, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("PUT request to {Uri}", requestUri);
            var response = await _httpClient.PutAsJsonAsync(requestUri, data, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during PUT request to {Uri}", requestUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PUT request to {Uri}", requestUri);
            throw;
        }
    }

    /// <summary>
    /// Sends a PATCH request to the specified URI.
    /// </summary>
    protected async Task<TResponse?> PatchAsync<TRequest, TResponse>(
        string requestUri, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("PATCH request to {Uri}", requestUri);
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
            {
                Content = JsonContent.Create(data, options: _jsonOptions)
            };
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during PATCH request to {Uri}", requestUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PATCH request to {Uri}", requestUri);
            throw;
        }
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI.
    /// </summary>
    protected async Task DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("DELETE request to {Uri}", requestUri);
            var response = await _httpClient.DeleteAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during DELETE request to {Uri}", requestUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DELETE request to {Uri}", requestUri);
            throw;
        }
    }
}
