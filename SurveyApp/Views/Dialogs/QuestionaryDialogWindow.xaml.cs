namespace SurveyApp.Views.Dialogs;

/// <summary>
/// Interaction logic for QuestionaryDialogWindow.xaml
/// </summary>
public partial class QuestionaryDialogWindow : Window
{
    private readonly QuestionaryDialogViewModel _viewModel;

    public QuestionaryDialogWindow(QuestionaryDialogViewModel viewModel)
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
    /// Gets the questionary data entered by the user.
    /// </summary>
    public (string name, string description) GetQuestionaryData()
    {
        return _viewModel.GetQuestionaryData();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate before saving
        if (!_viewModel.ValidateQuestionaryName())
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
