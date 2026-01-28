namespace SurveyApp.Views.Dialogs;

/// <summary>
/// Interaction logic for QuestionDialogWindow.xaml
/// </summary>
public partial class QuestionDialogWindow : Window
{
    private readonly QuestionDialogViewModel _viewModel;

    public QuestionDialogWindow(QuestionDialogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    /// <summary>
    /// Gets whether the dialog was confirmed (OK/Save clicked).
    /// </summary>
    public bool IsConfirmed { get; private set; }

    /// <summary>
    /// Gets the question data entered by the user.
    /// </summary>
    public (string questionText, string questionType) GetQuestionData()
    {
        return _viewModel.GetQuestionData();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate before saving
        if (!_viewModel.ValidateQuestionText())
        {
            return;
        }

        IsConfirmed = true;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        IsConfirmed = false;
        DialogResult = false;
        Close();
    }
}
