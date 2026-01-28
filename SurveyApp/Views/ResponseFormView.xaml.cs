namespace SurveyApp.Views;

/// <summary>
/// Interaction logic for ResponseFormView.xaml
/// </summary>
public partial class ResponseFormView : Page
{
    private readonly ResponseFormViewModel _viewModel;

    public ResponseFormView(ResponseFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Wire up text box change event to update response
        ResponseTextBox.TextChanged += ResponseTextBox_TextChanged;
    }

    private void ResponseTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_viewModel.CurrentQuestion != null)
        {
            _viewModel.UpdateResponse(ResponseTextBox.Text);
        }
    }

    /// <summary>
    /// Initializes the form with an Answer.
    /// </summary>
    public async Task InitializeAsync(AnswerDto answer)
    {
        await _viewModel.InitializeAsync(answer);
        
        // Load existing response if available
        if (_viewModel.CurrentQuestion != null 
            && _viewModel.Responses.TryGetValue(_viewModel.CurrentQuestion.Id, out var response))
        {
            ResponseTextBox.Text = response;
        }
    }
}
