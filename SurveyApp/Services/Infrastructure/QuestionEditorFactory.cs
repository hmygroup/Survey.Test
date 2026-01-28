namespace SurveyApp.Services.Infrastructure;

/// <summary>
/// Factory for creating type-specific question editor controls.
/// Implements the Factory Pattern to generate appropriate UI elements based on question type.
/// </summary>
public class QuestionEditorFactory
{
    private readonly ILogger<QuestionEditorFactory> _logger;

    public QuestionEditorFactory(ILogger<QuestionEditorFactory> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates an appropriate editor control based on the question's .NET type.
    /// </summary>
    /// <param name="questionType">The .NET type of the question (e.g., "System.String", "System.Boolean")</param>
    /// <param name="constraints">Optional constraints to configure the editor</param>
    /// <returns>A configured UIElement for editing the question response</returns>
    public UIElement CreateEditor(string questionType, IEnumerable<ConstraintDto>? constraints = null)
    {
        _logger.LogDebug("Creating editor for question type: {QuestionType}", questionType);

        return questionType switch
        {
            "System.String" => CreateTextEditor(constraints),
            "System.Boolean" => CreateBooleanEditor(),
            "System.Int32" => CreateIntegerEditor(constraints),
            "System.Decimal" => CreateDecimalEditor(constraints),
            "System.Double" => CreateDoubleEditor(constraints),
            "System.DateTime" => CreateDateTimeEditor(constraints),
            "System.Guid" => CreateGuidEditor(),
            "System.Byte[]" => CreateFileUploadEditor(constraints),
            _ => CreateDefaultEditor(questionType)
        };
    }

    /// <summary>
    /// Creates a TextBox editor for string input.
    /// </summary>
    private UIElement CreateTextEditor(IEnumerable<ConstraintDto>? constraints)
    {
        var textBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MinHeight = 80,
            Padding = new Thickness(8)
        };

        // Apply constraints if available
        if (constraints != null)
        {
            var maxLength = GetMaxLengthFromConstraints(constraints);
            if (maxLength > 0)
            {
                textBox.MaxLength = maxLength;
            }
        }

        return textBox;
    }

    /// <summary>
    /// Creates a CheckBox editor for boolean input.
    /// </summary>
    private UIElement CreateBooleanEditor()
    {
        return new CheckBox
        {
            Content = "Yes / True",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 8)
        };
    }

    /// <summary>
    /// Creates a NumericUpDown-style editor for integer input.
    /// </summary>
    private UIElement CreateIntegerEditor(IEnumerable<ConstraintDto>? constraints)
    {
        var textBox = new TextBox
        {
            MinWidth = 200,
            HorizontalAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(8)
        };

        // Add validation for numeric input
        textBox.PreviewTextInput += (s, e) =>
        {
            // Allow only digits and minus sign at the beginning
            e.Handled = !IsValidIntegerInput(e.Text, ((TextBox)s!).Text);
        };

        return textBox;
    }

    /// <summary>
    /// Creates a TextBox editor for decimal input.
    /// </summary>
    private UIElement CreateDecimalEditor(IEnumerable<ConstraintDto>? constraints)
    {
        var textBox = new TextBox
        {
            MinWidth = 200,
            HorizontalAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(8)
        };

        // Add validation for numeric input with decimals
        textBox.PreviewTextInput += (s, e) =>
        {
            e.Handled = !IsValidDecimalInput(e.Text, ((TextBox)s!).Text);
        };

        return textBox;
    }

    /// <summary>
    /// Creates a TextBox editor for double input.
    /// </summary>
    private UIElement CreateDoubleEditor(IEnumerable<ConstraintDto>? constraints)
    {
        var textBox = new TextBox
        {
            MinWidth = 200,
            HorizontalAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(8)
        };

        // Add validation for numeric input with decimals
        textBox.PreviewTextInput += (s, e) =>
        {
            e.Handled = !IsValidDecimalInput(e.Text, ((TextBox)s!).Text);
        };

        return textBox;
    }

    /// <summary>
    /// Creates a DatePicker editor for DateTime input.
    /// </summary>
    private UIElement CreateDateTimeEditor(IEnumerable<ConstraintDto>? constraints)
    {
        return new DatePicker
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            MinWidth = 200,
            DisplayDateStart = DateTime.Now.AddYears(-100),
            DisplayDateEnd = DateTime.Now.AddYears(100)
        };
    }

    /// <summary>
    /// Creates a read-only TextBox for GUID display.
    /// </summary>
    private UIElement CreateGuidEditor()
    {
        var textBox = new TextBox
        {
            IsReadOnly = true,
            MinWidth = 300,
            HorizontalAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(8),
            Text = "(auto-generated)"
        };

        return textBox;
    }

    /// <summary>
    /// Creates a file upload button for byte array input.
    /// </summary>
    private UIElement CreateFileUploadEditor(IEnumerable<ConstraintDto>? constraints)
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };

        var button = new Button
        {
            Content = "📁 Choose File",
            Padding = new Thickness(16, 8, 16, 8),
            Margin = new Thickness(0, 0, 8, 0)
        };

        var fileLabel = new TextBlock
        {
            Text = "No file selected",
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.Gray)
        };

        button.Click += (s, e) =>
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a file",
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                fileLabel.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                fileLabel.Foreground = new SolidColorBrush(Colors.Black);
            }
        };

        stackPanel.Children.Add(button);
        stackPanel.Children.Add(fileLabel);

        return stackPanel;
    }

    /// <summary>
    /// Creates a default TextBox editor for unknown types.
    /// </summary>
    private UIElement CreateDefaultEditor(string questionType)
    {
        _logger.LogWarning("Unknown question type: {QuestionType}. Using default text editor.", questionType);
        
        var textBox = new TextBox
        {
            MinHeight = 80,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Padding = new Thickness(8),
            Text = $"(Enter value for {questionType})"
        };

        return textBox;
    }

    #region Constraint Helpers

    private int GetMaxLengthFromConstraints(IEnumerable<ConstraintDto> constraints)
    {
        foreach (var constraint in constraints)
        {
            var maxLengthRecord = constraint.PolicyRecords?
                .FirstOrDefault(r => r.Value?.StartsWith("maxlength:", StringComparison.OrdinalIgnoreCase) == true 
                                  || r.Value?.StartsWith("max:", StringComparison.OrdinalIgnoreCase) == true);
            
            if (maxLengthRecord != null)
            {
                var parts = maxLengthRecord.Value.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var maxLength))
                {
                    return maxLength;
                }
            }
        }
        return 0;
    }

    private int GetMinLengthFromConstraints(IEnumerable<ConstraintDto> constraints)
    {
        foreach (var constraint in constraints)
        {
            var minLengthRecord = constraint.PolicyRecords?
                .FirstOrDefault(r => r.Value?.StartsWith("minlength:", StringComparison.OrdinalIgnoreCase) == true 
                                  || r.Value?.StartsWith("min:", StringComparison.OrdinalIgnoreCase) == true);
            
            if (minLengthRecord != null)
            {
                var parts = minLengthRecord.Value.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var minLength))
                {
                    return minLength;
                }
            }
        }
        return 0;
    }

    private int? GetMinValueFromConstraints(IEnumerable<ConstraintDto> constraints)
    {
        foreach (var constraint in constraints)
        {
            var minRecord = constraint.PolicyRecords?
                .FirstOrDefault(r => r.Value?.StartsWith("min:", StringComparison.OrdinalIgnoreCase) == true);
            
            if (minRecord != null)
            {
                var parts = minRecord.Value.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var min))
                {
                    return min;
                }
            }
        }
        return null;
    }

    private int? GetMaxValueFromConstraints(IEnumerable<ConstraintDto> constraints)
    {
        foreach (var constraint in constraints)
        {
            var maxRecord = constraint.PolicyRecords?
                .FirstOrDefault(r => r.Value?.StartsWith("max:", StringComparison.OrdinalIgnoreCase) == true);
            
            if (maxRecord != null)
            {
                var parts = maxRecord.Value.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var max))
                {
                    return max;
                }
            }
        }
        return null;
    }

    #endregion

    #region Input Validation Helpers

    private bool IsValidIntegerInput(string input, string currentText)
    {
        // Allow minus sign only at the beginning
        if (input == "-" && string.IsNullOrEmpty(currentText))
            return true;

        // Allow only digits
        return input.All(char.IsDigit);
    }

    private bool IsValidDecimalInput(string input, string currentText)
    {
        // Allow minus sign only at the beginning
        if (input == "-" && string.IsNullOrEmpty(currentText))
            return true;

        // Allow decimal point only once
        if (input == "." && !currentText.Contains("."))
            return true;

        // Allow only digits
        return input.All(char.IsDigit);
    }

    #endregion

    /// <summary>
    /// Gets a user-friendly display name for a question type.
    /// </summary>
    public static string GetTypeDisplayName(string dotNetType)
    {
        return dotNetType switch
        {
            "System.String" => "Text",
            "System.Boolean" => "Yes/No",
            "System.Int32" => "Integer",
            "System.Decimal" => "Decimal",
            "System.Double" => "Number",
            "System.DateTime" => "Date/Time",
            "System.Guid" => "Unique ID",
            "System.Byte[]" => "File Upload",
            _ => dotNetType
        };
    }

    /// <summary>
    /// Gets a description for a question type.
    /// </summary>
    public static string GetTypeDescription(string dotNetType)
    {
        return dotNetType switch
        {
            "System.String" => "Free-form text input for short or long answers",
            "System.Boolean" => "Yes/No or True/False selection",
            "System.Int32" => "Whole numbers (e.g., age, quantity)",
            "System.Decimal" => "Numbers with decimal places (e.g., price, rating)",
            "System.Double" => "Floating-point numbers",
            "System.DateTime" => "Date and time selection",
            "System.Guid" => "Unique identifier (auto-generated)",
            "System.Byte[]" => "File or document upload",
            _ => "Custom data type"
        };
    }
}
