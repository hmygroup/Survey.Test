namespace SurveyApp.Views;

/// <summary>
/// Interaction logic for AnswerAnalysisView.xaml
/// </summary>
public partial class AnswerAnalysisView : Page
{
    private readonly AnswerAnalysisViewModel _viewModel;

    public AnswerAnalysisView(AnswerAnalysisViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    /// <summary>
    /// Initializes the view with a questionary.
    /// </summary>
    public async Task InitializeAsync(QuestionaryDto questionary)
    {
        await _viewModel.InitializeAsync(questionary);
    }
}
