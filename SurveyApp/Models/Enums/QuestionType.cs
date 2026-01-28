namespace SurveyApp.Models.Enums;

/// <summary>
/// Represents the different types of questions that can be asked in a survey.
/// </summary>
public enum QuestionType
{
    /// <summary>
    /// Text-based question (short or long answer).
    /// </summary>
    Text,
    
    /// <summary>
    /// Date selection question.
    /// </summary>
    Date,
    
    /// <summary>
    /// Integer number question.
    /// </summary>
    Integer,
    
    /// <summary>
    /// Decimal number question.
    /// </summary>
    Decimal,
    
    /// <summary>
    /// Email address question.
    /// </summary>
    Email,
    
    /// <summary>
    /// Phone number question.
    /// </summary>
    Phone,
    
    /// <summary>
    /// Rating question (e.g., 1-5 stars).
    /// </summary>
    Rating,
    
    /// <summary>
    /// Single choice question (radio buttons).
    /// </summary>
    SingleChoice,
    
    /// <summary>
    /// Multiple choice question (checkboxes).
    /// </summary>
    MultipleChoice,
    
    /// <summary>
    /// File upload question.
    /// </summary>
    FileUpload,
    
    /// <summary>
    /// Boolean question (Yes/No, True/False).
    /// </summary>
    Boolean
}
