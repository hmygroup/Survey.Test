namespace SurveyApp.Views;

/// <summary>
/// Interaction logic for QuestionaryListView.xaml
/// </summary>
public partial class QuestionaryListView : Page
{
    private readonly QuestionaryListViewModel _viewModel;

    public QuestionaryListView(QuestionaryListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        // Load questionnaires when page is loaded
        await _viewModel.LoadQuestionnairesCommand.ExecuteAsync(null);
    }

    private async void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // View details on double-click
        if (_viewModel.ViewDetailsCommand.CanExecute(null))
        {
            await _viewModel.ViewDetailsCommand.ExecuteAsync(null);
        }
    }
}
