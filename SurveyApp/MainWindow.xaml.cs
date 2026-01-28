using SurveyApp.Services.Infrastructure;

namespace SurveyApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
{
    private readonly MainWindowViewModel _viewModel;
    private readonly SurveyApp.Services.Infrastructure.NavigationService _navigationService;

    public MainWindow(MainWindowViewModel viewModel, SurveyApp.Services.Infrastructure.NavigationService navigationService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _navigationService = navigationService;

        DataContext = _viewModel;

        // Set the navigation frame
        _navigationService.SetNavigationFrame(ContentFrame);

        // Navigate to Home view on startup
        _navigationService.NavigateTo<HomeView>();
    }
}
