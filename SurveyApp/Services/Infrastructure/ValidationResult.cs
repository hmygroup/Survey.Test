namespace SurveyApp.Services.Infrastructure;

/// <summary>
/// Represents the result of a validation operation for questions.
/// </summary>
public class QuestionValidationResult
{
    /// <summary>
    /// Gets or sets the validation message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity of the validation result.
    /// </summary>
    public ValidationSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the field that was validated.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets whether the validation passed.
    /// </summary>
    public bool IsValid => Severity == ValidationSeverity.None || Severity == ValidationSeverity.Info;

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static QuestionValidationResult Success(string field) => new()
    {
        Field = field,
        Severity = ValidationSeverity.None,
        Message = string.Empty
    };

    /// <summary>
    /// Creates an error validation result.
    /// </summary>
    public static QuestionValidationResult Error(string field, string message) => new()
    {
        Field = field,
        Severity = ValidationSeverity.Error,
        Message = message
    };

    /// <summary>
    /// Creates a warning validation result.
    /// </summary>
    public static QuestionValidationResult Warning(string field, string message) => new()
    {
        Field = field,
        Severity = ValidationSeverity.Warning,
        Message = message
    };

    /// <summary>
    /// Creates an info validation result.
    /// </summary>
    public static QuestionValidationResult Info(string field, string message) => new()
    {
        Field = field,
        Severity = ValidationSeverity.Info,
        Message = message
    };
}

/// <summary>
/// Defines the severity levels for validation results.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// No validation issues.
    /// </summary>
    None,

    /// <summary>
    /// Informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Error message (validation failed).
    /// </summary>
    Error
}
