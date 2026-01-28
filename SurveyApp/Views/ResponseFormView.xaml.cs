namespace SurveyApp.Views;

/// <summary>
/// Interaction logic for ResponseFormView.xaml
/// </summary>
public partial class ResponseFormView : Page
{
    private readonly ResponseFormViewModel _viewModel;

    public ResponseFormView(ResponseFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        // Subscribe to property changes to update input controls
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        
        // Subscribe to Loaded event to wire up input controls
        this.Loaded += ResponseFormView_Loaded;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.CurrentQuestion))
        {
            LoadCurrentResponse();
        }
    }

    private void ResponseFormView_Loaded(object sender, RoutedEventArgs e)
    {
        WireUpInputControls();
    }

    private void WireUpInputControls()
    {
        // Find the ContentControl that displays the question input
        var contentControl = FindVisualChild<ContentControl>(this);
        if (contentControl == null) return;

        // Wire up event handlers based on control type
        var textBox = FindVisualChild<TextBox>(contentControl);
        if (textBox != null)
        {
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.TextChanged += TextBox_TextChanged;
            LoadCurrentResponse();
            return;
        }

        var checkBox = FindVisualChild<CheckBox>(contentControl);
        if (checkBox != null)
        {
            checkBox.Checked -= CheckBox_Changed;
            checkBox.Unchecked -= CheckBox_Changed;
            checkBox.Checked += CheckBox_Changed;
            checkBox.Unchecked += CheckBox_Changed;
            LoadCurrentResponse();
            return;
        }

        var numberBox = FindVisualChild<ModernWpf.Controls.NumberBox>(contentControl);
        if (numberBox != null)
        {
            numberBox.ValueChanged -= NumberBox_ValueChanged;
            numberBox.ValueChanged += NumberBox_ValueChanged;
            LoadCurrentResponse();
            return;
        }

        var datePicker = FindVisualChild<DatePicker>(contentControl);
        if (datePicker != null)
        {
            datePicker.SelectedDateChanged -= DatePicker_SelectedDateChanged;
            datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
            LoadCurrentResponse();
            return;
        }

        var slider = FindVisualChild<Slider>(contentControl);
        if (slider != null)
        {
            slider.ValueChanged -= Slider_ValueChanged;
            slider.ValueChanged += Slider_ValueChanged;
            LoadCurrentResponse();
            return;
        }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox && _viewModel.CurrentQuestion != null)
        {
            _viewModel.UpdateResponse(textBox.Text);
        }
    }

    private void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && _viewModel.CurrentQuestion != null)
        {
            _viewModel.UpdateResponse(checkBox.IsChecked == true ? "true" : "false");
        }
    }

    private void NumberBox_ValueChanged(ModernWpf.Controls.NumberBox sender, ModernWpf.Controls.NumberBoxValueChangedEventArgs args)
    {
        if (_viewModel.CurrentQuestion != null && !double.IsNaN(args.NewValue))
        {
            _viewModel.UpdateResponse(args.NewValue.ToString());
        }
    }

    private void DatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is DatePicker datePicker && _viewModel.CurrentQuestion != null && datePicker.SelectedDate.HasValue)
        {
            _viewModel.UpdateResponse(datePicker.SelectedDate.Value.ToString("O"));
        }
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is Slider slider && _viewModel.CurrentQuestion != null)
        {
            _viewModel.UpdateResponse(slider.Value.ToString());
        }
    }

    private void LoadCurrentResponse()
    {
        if (_viewModel.CurrentQuestion == null) return;

        // Find the input control
        var contentControl = FindVisualChild<ContentControl>(this);
        if (contentControl == null) return;

        if (_viewModel.Responses.TryGetValue(_viewModel.CurrentQuestion.Id, out var response))
        {
            var textBox = FindVisualChild<TextBox>(contentControl);
            if (textBox != null)
            {
                textBox.Text = response;
                return;
            }

            var checkBox = FindVisualChild<CheckBox>(contentControl);
            if (checkBox != null)
            {
                checkBox.IsChecked = response.Equals("true", StringComparison.OrdinalIgnoreCase);
                return;
            }

            var numberBox = FindVisualChild<ModernWpf.Controls.NumberBox>(contentControl);
            if (numberBox != null && double.TryParse(response, out var number))
            {
                numberBox.Value = number;
                return;
            }

            var datePicker = FindVisualChild<DatePicker>(contentControl);
            if (datePicker != null && DateTime.TryParse(response, out var date))
            {
                datePicker.SelectedDate = date;
                return;
            }

            var slider = FindVisualChild<Slider>(contentControl);
            if (slider != null && double.TryParse(response, out var rating))
            {
                slider.Value = rating;
                return;
            }
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
                return typedChild;

            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }
        return null;
    }

    /// <summary>
    /// Initializes the form with an Answer.
    /// </summary>
    public async Task InitializeAsync(AnswerDto answer, string questionaryTitle = "")
    {
        await _viewModel.InitializeAsync(answer, questionaryTitle);
        
        // Wire up controls after initialization
        await Dispatcher.InvokeAsync(() =>
        {
            WireUpInputControls();
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }
}
