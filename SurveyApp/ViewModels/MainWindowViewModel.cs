using System.Collections.ObjectModel;
using System.Linq;

namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the main application window.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly ThemeService _themeService;
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private string _title = "Survey Management System";

    [ObservableProperty]
    private string _currentTheme = "Light";

    [ObservableProperty]
    private ObservableCollection<string> _breadcrumbItems = new();

    [ObservableProperty]
    private string _currentPageTitle = "";

    public MainWindowViewModel(
        NavigationService navigationService,
        ThemeService themeService,
        ILogger<MainWindowViewModel> logger)
    {
        _navigationService = navigationService;
        _themeService = themeService;
        _logger = logger;
        
        CurrentTheme = _themeService.CurrentTheme;
        UpdateBreadcrumb("Home");
    }

    /// <summary>
    /// Updates the breadcrumb navigation.
    /// </summary>
    private void UpdateBreadcrumb(params string[] items)
    {
        BreadcrumbItems.Clear();
        foreach (var item in items)
        {
            BreadcrumbItems.Add(item);
        }
        CurrentPageTitle = items.LastOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Toggles the application theme between Light and Dark.
    /// </summary>
    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.ToggleTheme();
        CurrentTheme = _themeService.CurrentTheme;
        _logger.LogInformation("Theme toggled to: {Theme}", CurrentTheme);
    }

    /// <summary>
    /// Navigates to the Home view.
    /// </summary>
    [RelayCommand]
    private void NavigateToHome()
    {
        _logger.LogInformation("Navigating to Home");
        _navigationService.NavigateTo<HomeView>();
        UpdateBreadcrumb("Home");
    }

    /// <summary>
    /// Navigates to the Questionnaires view.
    /// </summary>
    [RelayCommand]
    private void NavigateToQuestionnaires()
    {
        _logger.LogInformation("Navigating to Questionnaires");
        _navigationService.NavigateTo<QuestionaryListView>();
        UpdateBreadcrumb("Home", "Questionnaires");
    }

    /// <summary>
    /// Navigates to the Responses view.
    /// </summary>
    [RelayCommand]
    private void NavigateToResponses()
    {
        _logger.LogInformation("Navigating to Responses");
        // TODO: Implement Responses view when created
        UpdateBreadcrumb("Home", "Responses");
    }
}
