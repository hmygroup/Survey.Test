namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Questionary dialog (Create/Edit).
/// </summary>
public partial class QuestionaryDialogViewModel : ObservableObject
{
    private readonly ILogger<QuestionaryDialogViewModel> _logger;

    [ObservableProperty]
    private string _title = "Questionnaire";

    [ObservableProperty]
    private string _headerText = "Create Questionnaire";

    [ObservableProperty]
    private string _subHeaderText = "Fill in the details below";

    [ObservableProperty]
    private string _saveButtonText = "Create";

    [ObservableProperty]
    private string _questionaryName = string.Empty;

    [ObservableProperty]
    private string _questionaryDescription = string.Empty;

    [ObservableProperty]
    private string _validationMessage = string.Empty;

    [ObservableProperty]
    private bool _hasValidationError;

    [ObservableProperty]
    private bool _canSave = true;

    public Guid? QuestionaryId { get; set; }

    public QuestionaryDialogViewModel(ILogger<QuestionaryDialogViewModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Configures the dialog for creating a new questionnaire.
    /// </summary>
    public void ConfigureForCreate()
    {
        Title = "Create Questionnaire";
        HeaderText = "Create New Questionnaire";
        SubHeaderText = "Fill in the details below to create a new questionnaire";
        SaveButtonText = "Create";
        QuestionaryId = null;
        QuestionaryName = string.Empty;
        QuestionaryDescription = string.Empty;
        _logger.LogInformation("Dialog configured for Create mode");
    }

    /// <summary>
    /// Configures the dialog for editing an existing questionnaire.
    /// </summary>
    public void ConfigureForEdit(QuestionaryDto questionary)
    {
        Title = "Edit Questionnaire";
        HeaderText = "Edit Questionnaire";
        SubHeaderText = "Update the questionnaire details below";
        SaveButtonText = "Save";
        QuestionaryId = questionary.Id;
        QuestionaryName = questionary.Name;
        QuestionaryDescription = questionary.Description;
        _logger.LogInformation("Dialog configured for Edit mode: {Id}", questionary.Id);
    }

    /// <summary>
    /// Validates the questionary name when it changes.
    /// </summary>
    partial void OnQuestionaryNameChanged(string value)
    {
        ValidateQuestionaryName();
    }

    /// <summary>
    /// Validates the questionary name.
    /// </summary>
    public bool ValidateQuestionaryName()
    {
        if (string.IsNullOrWhiteSpace(QuestionaryName))
        {
            ValidationMessage = "Name is required";
            HasValidationError = true;
            CanSave = false;
            return false;
        }

        if (QuestionaryName.Length < 3)
        {
            ValidationMessage = "Name must be at least 3 characters";
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
    /// Gets the questionary data for saving.
    /// </summary>
    public (string name, string description) GetQuestionaryData()
    {
        return (QuestionaryName.Trim(), QuestionaryDescription.Trim());
    }
}
