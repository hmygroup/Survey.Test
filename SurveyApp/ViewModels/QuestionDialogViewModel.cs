namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Question dialog (Create/Edit).
/// </summary>
public partial class QuestionDialogViewModel : ObservableObject
{
    private readonly ILogger<QuestionDialogViewModel> _logger;
    private readonly ReactiveValidationService? _validationService;

    [ObservableProperty]
    private string _title = "Question";

    [ObservableProperty]
    private string _headerText = "Create Question";

    [ObservableProperty]
    private string _subHeaderText = "Fill in the details below";

    [ObservableProperty]
    private string _saveButtonText = "Create";

    [ObservableProperty]
    private string _questionText = string.Empty;

    [ObservableProperty]
    private string _selectedQuestionType = "System.String";

    [ObservableProperty]
    private string _validationMessage = string.Empty;

    [ObservableProperty]
    private bool _hasValidationError;
    
    [ObservableProperty]
    private bool _hasValidationWarning;

    [ObservableProperty]
    private bool _canSave = true;

    public Guid? QuestionId { get; set; }
    public Guid QuestionaryId { get; set; }

    /// <summary>
    /// Constraints for this question.
    /// </summary>
    public ICollection<ConstraintDto> Constraints { get; set; } = new List<ConstraintDto>();

    /// <summary>
    /// Available question types based on .NET types.
    /// </summary>
    public ObservableCollection<string> QuestionTypes { get; } = new()
    {
        "System.String",        // Text
        "System.Boolean",       // Yes/No
        "System.Int32",         // Integer
        "System.Decimal",       // Decimal
        "System.DateTime",      // Date/Time
        "System.Double",        // Double
        "System.Guid",          // Unique Identifier
        "System.Byte[]",        // Binary/File
    };

    public QuestionDialogViewModel(
        ILogger<QuestionDialogViewModel> logger,
        ReactiveValidationService? validationService = null)
    {
        _logger = logger;
        _validationService = validationService;
    }

    /// <summary>
    /// Configures the dialog for creating a new question.
    /// </summary>
    public void ConfigureForCreate(Guid questionaryId)
    {
        Title = "Create Question";
        HeaderText = "Create New Question";
        SubHeaderText = "Fill in the question details below";
        SaveButtonText = "Create";
        QuestionId = null;
        QuestionaryId = questionaryId;
        QuestionText = string.Empty;
        SelectedQuestionType = "System.String";
        Constraints = new List<ConstraintDto>();
        _logger.LogInformation("Dialog configured for Create mode for questionary {QuestionaryId}", questionaryId);
    }

    /// <summary>
    /// Configures the dialog for editing an existing question.
    /// </summary>
    public void ConfigureForEdit(QuestionDto question)
    {
        Title = "Edit Question";
        HeaderText = "Edit Question";
        SubHeaderText = "Update the question details below";
        SaveButtonText = "Save";
        QuestionId = question.Id;
        QuestionText = question.QuestionText;
        SelectedQuestionType = question.QuestionType?.DotNetType ?? "System.String";
        Constraints = question.Constraints?.ToList() ?? new List<ConstraintDto>();
        _logger.LogInformation("Dialog configured for Edit mode: {Id} with {ConstraintCount} constraints", 
            question.Id, Constraints.Count);
    }

    /// <summary>
    /// Validates the question text when it changes.
    /// </summary>
    partial void OnQuestionTextChanged(string value)
    {
        ValidateQuestionText();
    }

    /// <summary>
    /// Validates the question text.
    /// </summary>
    public bool ValidateQuestionText()
    {
        if (string.IsNullOrWhiteSpace(QuestionText))
        {
            ValidationMessage = "Question text is required";
            HasValidationError = true;
            CanSave = false;
            return false;
        }

        if (QuestionText.Length < 5)
        {
            ValidationMessage = "Question text must be at least 5 characters";
            HasValidationError = true;
            CanSave = false;
            return false;
        }

        if (QuestionText.Length > 500)
        {
            ValidationMessage = "Question text must not exceed 500 characters";
            HasValidationError = true;
            CanSave = false;
            return false;
        }

        ValidationMessage = string.Empty;
        HasValidationError = false;
        CanSave = true;
        return true;
    }

    /// <summary>
    /// Gets the question data for saving.
    /// </summary>
    public (string questionText, string questionType, ICollection<ConstraintDto> constraints) GetQuestionData()
    {
        return (QuestionText.Trim(), SelectedQuestionType, Constraints);
    }

    /// <summary>
    /// Updates validation feedback based on a validation result.
    /// </summary>
    public void UpdateValidationFeedback(QuestionValidationResult result)
    {
        ValidationMessage = result.Message;
        
        switch (result.Severity)
        {
            case ValidationSeverity.Error:
                HasValidationError = true;
                HasValidationWarning = false;
                CanSave = false;
                break;
            
            case ValidationSeverity.Warning:
                HasValidationError = false;
                HasValidationWarning = true;
                CanSave = true; // Allow save with warnings
                break;
            
            case ValidationSeverity.Info:
            case ValidationSeverity.None:
            default:
                HasValidationError = false;
                HasValidationWarning = false;
                CanSave = true;
                break;
        }
        
        _logger.LogDebug("Validation feedback updated: {Severity} - {Message}", 
            result.Severity, result.Message);
    }
}
