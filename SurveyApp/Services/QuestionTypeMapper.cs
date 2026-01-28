namespace SurveyApp.Services;

/// <summary>
/// Maps between .NET types and QuestionType enum values.
/// Provides conversion and metadata for question types.
/// </summary>
public static class QuestionTypeMapper
{
    private static readonly Dictionary<string, QuestionType> _dotNetTypeToQuestionType = new()
    {
        { "System.String", QuestionType.Text },
        { "System.Boolean", QuestionType.Boolean },
        { "System.Int32", QuestionType.Integer },
        { "System.Decimal", QuestionType.Decimal },
        { "System.DateTime", QuestionType.Date },
        { "System.Double", QuestionType.Decimal },
        { "System.Byte[]", QuestionType.FileUpload }
    };

    private static readonly Dictionary<QuestionType, string> _questionTypeToDotNetType = new()
    {
        { QuestionType.Text, "System.String" },
        { QuestionType.Boolean, "System.Boolean" },
        { QuestionType.Integer, "System.Int32" },
        { QuestionType.Decimal, "System.Decimal" },
        { QuestionType.Date, "System.DateTime" },
        { QuestionType.Email, "System.String" },
        { QuestionType.Phone, "System.String" },
        { QuestionType.Rating, "System.Int32" },
        { QuestionType.SingleChoice, "System.String" },
        { QuestionType.MultipleChoice, "System.String" },
        { QuestionType.FileUpload, "System.Byte[]" }
    };

    private static readonly Dictionary<QuestionType, string> _questionTypeDescriptions = new()
    {
        { QuestionType.Text, "Text input (short answer or paragraph)" },
        { QuestionType.Boolean, "Yes/No or True/False question" },
        { QuestionType.Integer, "Whole number (age, quantity, etc.)" },
        { QuestionType.Decimal, "Decimal number (price, rating, etc.)" },
        { QuestionType.Date, "Date selection" },
        { QuestionType.Email, "Email address with validation" },
        { QuestionType.Phone, "Phone number with validation" },
        { QuestionType.Rating, "Rating scale (e.g., 1-5 stars)" },
        { QuestionType.SingleChoice, "Single choice from options (radio buttons)" },
        { QuestionType.MultipleChoice, "Multiple choices from options (checkboxes)" },
        { QuestionType.FileUpload, "File upload (documents, images)" }
    };

    /// <summary>
    /// Converts a .NET type string to a QuestionType enum.
    /// </summary>
    public static QuestionType FromDotNetType(string dotNetType)
    {
        if (_dotNetTypeToQuestionType.TryGetValue(dotNetType, out var questionType))
        {
            return questionType;
        }
        
        // Default to Text for unknown types
        return QuestionType.Text;
    }

    /// <summary>
    /// Converts a QuestionType enum to a .NET type string.
    /// </summary>
    public static string ToDotNetType(QuestionType questionType)
    {
        if (_questionTypeToDotNetType.TryGetValue(questionType, out var dotNetType))
        {
            return dotNetType;
        }
        
        // Default to String for unknown types
        return "System.String";
    }

    /// <summary>
    /// Gets a user-friendly description for a question type.
    /// </summary>
    public static string GetDescription(QuestionType questionType)
    {
        if (_questionTypeDescriptions.TryGetValue(questionType, out var description))
        {
            return description;
        }
        
        return "Custom question type";
    }

    /// <summary>
    /// Gets all available question types.
    /// </summary>
    public static IEnumerable<QuestionType> GetAllTypes()
    {
        return Enum.GetValues<QuestionType>();
    }

    /// <summary>
    /// Gets a display name for a question type.
    /// </summary>
    public static string GetDisplayName(QuestionType questionType)
    {
        return questionType switch
        {
            QuestionType.Text => "Text",
            QuestionType.Boolean => "Yes/No",
            QuestionType.Integer => "Number (Integer)",
            QuestionType.Decimal => "Number (Decimal)",
            QuestionType.Date => "Date",
            QuestionType.Email => "Email",
            QuestionType.Phone => "Phone",
            QuestionType.Rating => "Rating",
            QuestionType.SingleChoice => "Single Choice",
            QuestionType.MultipleChoice => "Multiple Choice",
            QuestionType.FileUpload => "File Upload",
            _ => questionType.ToString()
        };
    }
}
