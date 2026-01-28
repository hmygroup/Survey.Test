namespace SurveyApp.Services.Infrastructure;

/// <summary>
/// Service for navigating between views in the application.
/// </summary>
public class NavigationService
{
    private Frame? _navigationFrame;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Sets the navigation frame for the service.
    /// </summary>
    public void SetNavigationFrame(Frame frame)
    {
        _navigationFrame = frame;
    }

    /// <summary>
    /// Navigates to a specific page type.
    /// </summary>
    public void NavigateTo<TPage>() where TPage : Page
    {
        if (_navigationFrame == null)
        {
            _logger.LogWarning("Navigation frame not set");
            return;
        }

        try
        {
            var page = _serviceProvider.GetRequiredService<TPage>();
            _navigationFrame.Navigate(page);
            _logger.LogInformation("Navigated to {PageType}", typeof(TPage).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to {PageType}", typeof(TPage).Name);
        }
    }

    /// <summary>
    /// Navigates to a specific page type with a parameter.
    /// </summary>
    public void NavigateTo<TPage>(object parameter) where TPage : Page
    {
        if (_navigationFrame == null)
        {
            _logger.LogWarning("Navigation frame not set");
            return;
        }

        try
        {
            var page = _serviceProvider.GetRequiredService<TPage>();
            _navigationFrame.Navigate(page, parameter);
            _logger.LogInformation("Navigated to {PageType} with parameter", typeof(TPage).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to {PageType}", typeof(TPage).Name);
        }
    }

    /// <summary>
    /// Navigates back if possible.
    /// </summary>
    public void GoBack()
    {
        if (_navigationFrame?.CanGoBack == true)
        {
            _navigationFrame.GoBack();
            _logger.LogInformation("Navigated back");
        }
    }

    /// <summary>
    /// Gets whether the navigation service can go back.
    /// </summary>
    public bool CanGoBack => _navigationFrame?.CanGoBack ?? false;
}
