using SurveyApp.Services.Infrastructure;
using SurveyApp.Views;

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

    private void NavigationView_SelectionChanged(Wpf.Ui.Controls.NavigationView sender, System.Windows.RoutedEventArgs args)
    {
        if (sender.SelectedItem is Wpf.Ui.Controls.NavigationViewItem selectedItem)
        {
            switch (selectedItem.Name)
            {
                case "HomeNavItem":
                    _viewModel.NavigateToHomeCommand.Execute(null);
                    break;
                case "QuestionnairesNavItem":
                    _viewModel.NavigateToQuestionnairesCommand.Execute(null);
                    break;
                case "ResponsesNavItem":
                    _viewModel.NavigateToResponsesCommand.Execute(null);
                    break;
                case "ThemeNavItem":
                    _viewModel.ToggleThemeCommand.Execute(null);
                    break;
            }
        }
    }
}
