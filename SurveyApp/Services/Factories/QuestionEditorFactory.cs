namespace SurveyApp.Services.Factories;

/// <summary>
/// Factory for creating type-specific question editor controls.
/// Implements the Factory Pattern for dynamic UI generation based on QuestionType.
/// </summary>
public class QuestionEditorFactory
{
    private readonly ILogger<QuestionEditorFactory> _logger;

    public QuestionEditorFactory(ILogger<QuestionEditorFactory> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates an appropriate editor control based on the question type.
    /// </summary>
    /// <param name="questionType">The type of question.</param>
    /// <returns>A FrameworkElement configured for the question type.</returns>
    public FrameworkElement CreateEditor(QuestionType questionType)
    {
        _logger.LogDebug("Creating editor for question type: {QuestionType}", questionType);

        return questionType switch
        {
            QuestionType.Text => CreateTextEditor(),
            QuestionType.Boolean => CreateBooleanEditor(),
            QuestionType.Integer => CreateIntegerEditor(),
            QuestionType.Decimal => CreateDecimalEditor(),
            QuestionType.Date => CreateDateEditor(),
            QuestionType.Email => CreateEmailEditor(),
            QuestionType.Phone => CreatePhoneEditor(),
            QuestionType.Rating => CreateRatingEditor(),
            QuestionType.SingleChoice => CreateSingleChoiceEditor(),
            QuestionType.MultipleChoice => CreateMultipleChoiceEditor(),
            QuestionType.FileUpload => CreateFileUploadEditor(),
            _ => CreateTextEditor() // Default to text
        };
    }

    /// <summary>
    /// Creates a text editor (TextBox).
    /// </summary>
    private FrameworkElement CreateTextEditor()
    {
        var textBox = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MinHeight = 100,
            MaxLength = 1000
        };
        
        return textBox;
    }

    /// <summary>
    /// Creates a boolean editor (CheckBox).
    /// </summary>
    private FrameworkElement CreateBooleanEditor()
    {
        var checkBox = new CheckBox
        {
            Content = "Yes / True",
            VerticalAlignment = VerticalAlignment.Center
        };
        
        return checkBox;
    }

    /// <summary>
    /// Creates an integer editor (numeric TextBox with validation).
    /// </summary>
    private FrameworkElement CreateIntegerEditor()
    {
        var textBox = new TextBox
        {
            MinWidth = 200,
            MaxLength = 10
        };
        
        // Add numeric validation
        textBox.PreviewTextInput += (s, e) =>
        {
            e.Handled = !IsNumeric(e.Text, allowDecimal: false);
        };
        
        return textBox;
    }

    /// <summary>
    /// Creates a decimal editor (numeric TextBox with decimal validation).
    /// </summary>
    private FrameworkElement CreateDecimalEditor()
    {
        var textBox = new TextBox
        {
            MinWidth = 200,
            MaxLength = 15
        };
        
        // Add decimal validation
        textBox.PreviewTextInput += (s, e) =>
        {
            var currentText = ((TextBox)s).Text;
            e.Handled = !IsNumeric(e.Text, allowDecimal: true) || 
                       (e.Text == "." && currentText.Contains('.'));
        };
        
        return textBox;
    }

    /// <summary>
    /// Creates a date editor (DatePicker).
    /// </summary>
    private FrameworkElement CreateDateEditor()
    {
        var datePicker = new DatePicker
        {
            MinWidth = 200,
            DisplayDate = DateTime.Today
        };
        
        return datePicker;
    }

    /// <summary>
    /// Creates an email editor (TextBox with email validation).
    /// </summary>
    private FrameworkElement CreateEmailEditor()
    {
        var textBox = new TextBox
        {
            MinWidth = 300,
            MaxLength = 255
        };
        
        return textBox;
    }

    /// <summary>
    /// Creates a phone editor (TextBox with phone validation).
    /// </summary>
    private FrameworkElement CreatePhoneEditor()
    {
        var textBox = new TextBox
        {
            MinWidth = 200,
            MaxLength = 20
        };
        
        return textBox;
    }

    /// <summary>
    /// Creates a rating editor (Slider).
    /// </summary>
    private FrameworkElement CreateRatingEditor()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        var slider = new Slider
        {
            Minimum = 1,
            Maximum = 5,
            Value = 3,
            TickFrequency = 1,
            IsSnapToTickEnabled = true,
            MinWidth = 300
        };

        var label = new TextBlock
        {
            Text = "Rating: 3",
            Margin = new Thickness(0, 8, 0, 0),
            FontSize = 14
        };

        slider.ValueChanged += (s, e) =>
        {
            label.Text = $"Rating: {(int)e.NewValue}";
        };

        stackPanel.Children.Add(slider);
        stackPanel.Children.Add(label);

        return stackPanel;
    }

    /// <summary>
    /// Creates a single choice editor (RadioButtons).
    /// </summary>
    private FrameworkElement CreateSingleChoiceEditor()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        // Placeholder options - in real implementation, these would come from question configuration
        var options = new[] { "Option 1", "Option 2", "Option 3", "Other" };

        foreach (var option in options)
        {
            var radioButton = new RadioButton
            {
                Content = option,
                GroupName = "SingleChoice",
                Margin = new Thickness(0, 4, 0, 4)
            };
            stackPanel.Children.Add(radioButton);
        }

        return stackPanel;
    }

    /// <summary>
    /// Creates a multiple choice editor (CheckBoxes).
    /// </summary>
    private FrameworkElement CreateMultipleChoiceEditor()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        // Placeholder options - in real implementation, these would come from question configuration
        var options = new[] { "Option 1", "Option 2", "Option 3", "Option 4" };

        foreach (var option in options)
        {
            var checkBox = new CheckBox
            {
                Content = option,
                Margin = new Thickness(0, 4, 0, 4)
            };
            stackPanel.Children.Add(checkBox);
        }

        return stackPanel;
    }

    /// <summary>
    /// Creates a file upload editor (Button with file picker).
    /// </summary>
    private FrameworkElement CreateFileUploadEditor()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        var button = new Button
        {
            Content = "Choose File",
            Padding = new Thickness(24, 8, 24, 8),
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var fileNameLabel = new TextBlock
        {
            Text = "No file selected",
            Margin = new Thickness(0, 8, 0, 0),
            FontSize = 12,
            Foreground = new SolidColorBrush(Colors.Gray)
        };

        button.Click += (s, e) =>
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Select a file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                fileNameLabel.Text = $"Selected: {System.IO.Path.GetFileName(openFileDialog.FileName)}";
                fileNameLabel.Foreground = new SolidColorBrush(Colors.Black);
            }
        };

        stackPanel.Children.Add(button);
        stackPanel.Children.Add(fileNameLabel);

        return stackPanel;
    }

    /// <summary>
    /// Validates if a string is numeric.
    /// </summary>
    private bool IsNumeric(string text, bool allowDecimal)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        if (allowDecimal && text == ".")
            return true;

        foreach (char c in text)
        {
            if (!char.IsDigit(c) && !(allowDecimal && c == '.') && c != '-')
                return false;
        }

        return true;
    }
}
