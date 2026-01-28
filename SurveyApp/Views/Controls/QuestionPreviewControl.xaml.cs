namespace SurveyApp.Views.Controls;

/// <summary>
/// UserControl for previewing how a question will appear to users.
/// </summary>
public partial class QuestionPreviewControl : UserControl
{
    private QuestionEditorFactory _editorFactory;
    private string _currentQuestionType = "System.String";
    private IEnumerable<ConstraintDto>? _currentConstraints;

    public QuestionPreviewControl()
    {
        InitializeComponent();
        
        // Note: Factory will be injected when the control is added to the dialog
        // For now, create a temporary instance with basic logging
        _editorFactory = new QuestionEditorFactory(
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<QuestionEditorFactory>());
        
        // Initialize with default preview
        UpdatePreview();
    }

    /// <summary>
    /// Sets the editor factory (for dependency injection).
    /// </summary>
    public void SetEditorFactory(QuestionEditorFactory factory)
    {
        _editorFactory = factory;
        UpdatePreview();
    }

    /// <summary>
    /// Updates the preview with new question text.
    /// </summary>
    public void UpdateQuestionText(string questionText)
    {
        PreviewQuestionText.Text = string.IsNullOrWhiteSpace(questionText) 
            ? "Your question will appear here..." 
            : questionText;
        
        PreviewQuestionText.Foreground = string.IsNullOrWhiteSpace(questionText)
            ? (Brush)FindResource("SystemControlForegroundBaseMediumBrush")
            : (Brush)FindResource("SystemControlForegroundBaseHighBrush");
    }

    /// <summary>
    /// Updates the preview with new question type and constraints.
    /// </summary>
    public void UpdateQuestionType(string questionType, IEnumerable<ConstraintDto>? constraints = null)
    {
        _currentQuestionType = questionType;
        _currentConstraints = constraints;
        UpdatePreview();
    }

    /// <summary>
    /// Updates the preview display.
    /// </summary>
    private void UpdatePreview()
    {
        // Update type description
        PreviewTypeDescription.Text = $"Type: {QuestionEditorFactory.GetTypeDisplayName(_currentQuestionType)} - {QuestionEditorFactory.GetTypeDescription(_currentQuestionType)}";

        // Create and display the appropriate editor
        try
        {
            var editor = _editorFactory.CreateEditor(_currentQuestionType, _currentConstraints);
            PreviewEditorPresenter.Content = editor;
        }
        catch (Exception ex)
        {
            var errorText = new TextBlock
            {
                Text = $"⚠️ Error creating preview: {ex.Message}",
                Foreground = Brushes.Red,
                TextWrapping = TextWrapping.Wrap
            };
            PreviewEditorPresenter.Content = errorText;
        }
    }

    /// <summary>
    /// Updates constraints and refreshes preview.
    /// </summary>
    public void UpdateConstraints(IEnumerable<ConstraintDto> constraints)
    {
        _currentConstraints = constraints;
        UpdatePreview();
    }
}
