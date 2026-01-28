using System.Windows;
using System.Windows.Controls;
using SurveyApp.Models.Dtos;

namespace SurveyApp.Views.Selectors;

/// <summary>
/// DataTemplateSelector for choosing the appropriate input control based on question type.
/// </summary>
public class QuestionInputSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the template for text questions (System.String).
    /// </summary>
    public DataTemplate? TextTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for boolean questions (System.Boolean).
    /// </summary>
    public DataTemplate? BooleanTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for integer questions (System.Int32).
    /// </summary>
    public DataTemplate? IntegerTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for decimal questions (System.Decimal, System.Double).
    /// </summary>
    public DataTemplate? DecimalTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for date questions (System.DateTime).
    /// </summary>
    public DataTemplate? DateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for rating questions.
    /// </summary>
    public DataTemplate? RatingTemplate { get; set; }

    /// <summary>
    /// Selects the appropriate template based on the question type.
    /// </summary>
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not QuestionDto question || question.QuestionType == null)
            return TextTemplate;

        var dotNetType = question.QuestionType.DotNetType;

        return dotNetType switch
        {
            "System.Boolean" => BooleanTemplate ?? TextTemplate,
            "System.Int32" => IntegerTemplate ?? TextTemplate,
            "System.Decimal" or "System.Double" => DecimalTemplate ?? TextTemplate,
            "System.DateTime" => DateTemplate ?? TextTemplate,
            _ when dotNetType.Contains("Rating", StringComparison.OrdinalIgnoreCase) => RatingTemplate ?? TextTemplate,
            _ => TextTemplate
        };
    }
}
