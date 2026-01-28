using SurveyApp.Services.Infrastructure;

namespace SurveyApp.Views.Dialogs;

/// <summary>
/// Interaction logic for QuestionDialogWindow.xaml
/// </summary>
public partial class QuestionDialogWindow : Window
{
    private readonly QuestionDialogViewModel _viewModel;
    private readonly ConstraintEditorViewModel _constraintEditorViewModel;
    private readonly SurveyApp.Services.Infrastructure.QuestionEditorFactory _editorFactory;

    public QuestionDialogWindow(
        QuestionDialogViewModel viewModel,
        ConstraintEditorViewModel constraintEditorViewModel,
        SurveyApp.Services.Infrastructure.QuestionEditorFactory editorFactory)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _constraintEditorViewModel = constraintEditorViewModel;
        _editorFactory = editorFactory;
        DataContext = _viewModel;
        
        // Set the constraint editor's DataContext
        ConstraintEditor.DataContext = _constraintEditorViewModel;
        
        // Inject the factory into the preview control
        QuestionPreview.SetEditorFactory(_editorFactory);
        
        // Wire up preview updates
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.QuestionText))
        {
            QuestionPreview.UpdateQuestionText(_viewModel.QuestionText);
        }
        else if (e.PropertyName == nameof(_viewModel.SelectedQuestionType))
        {
            QuestionPreview.UpdateQuestionType(_viewModel.SelectedQuestionType, _viewModel.Constraints);
        }
    }

    /// <summary>
    /// Initializes the constraint editor asynchronously.
    /// </summary>
    public async Task InitializeConstraintEditorAsync()
    {
        await _constraintEditorViewModel.InitializeAsync(_viewModel.Constraints);
    }

    /// <summary>
    /// Gets whether the dialog was confirmed (OK/Save clicked).
    /// </summary>
    public bool IsConfirmed { get; private set; }

    /// <summary>
    /// Gets the question data entered by the user.
    /// </summary>
    public (string questionText, string questionType, ICollection<ConstraintDto> constraints) GetQuestionData()
    {
        // Update constraints from the constraint editor
        _viewModel.Constraints = _constraintEditorViewModel.GetConstraints();
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
