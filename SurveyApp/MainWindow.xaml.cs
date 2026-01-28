namespace SurveyApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly NavigationService _navigationService;

    public MainWindow(MainWindowViewModel viewModel, NavigationService navigationService)
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

    private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is ModernWpf.Controls.NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            
            switch (tag)
            {
                case "Home":
                    _viewModel.NavigateToHomeCommand.Execute(null);
                    break;
                case "Questionnaires":
                    _viewModel.NavigateToQuestionnairesCommand.Execute(null);
                    break;
                case "Responses":
                    _viewModel.NavigateToResponsesCommand.Execute(null);
                    break;
            }
        }
        else if (args.IsSettingsSelected)
        {
            // Navigate to settings page when created
        }
    }
}
