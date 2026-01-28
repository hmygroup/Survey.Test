using System.Windows;
using System.Windows.Controls;
using SurveyApp.Models;

namespace SurveyApp.Views;

/// <summary>
/// Interaction logic for SessionRecoveryDialog.xaml
/// </summary>
public partial class SessionRecoveryDialog : Window
{
    /// <summary>
    /// Gets the selected recovery action.
    /// </summary>
    public RecoveryAction SelectedAction { get; private set; } = RecoveryAction.StartFresh;

    /// <summary>
    /// Gets the checkpoint to recover.
    /// </summary>
    public SessionCheckpoint? Checkpoint { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionRecoveryDialog"/> class.
    /// </summary>
    /// <param name="checkpoint">The unfinished session checkpoint.</param>
    public SessionRecoveryDialog(SessionCheckpoint checkpoint)
    {
        InitializeComponent();
        Checkpoint = checkpoint;
        DataContext = this;
        
        // Set window properties
        Owner = Application.Current.MainWindow;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        
        // Populate UI
        QuestionaryTitleText.Text = checkpoint.QuestionaryTitle;
        LastSavedText.Text = $"Last saved: {checkpoint.LastUpdatedAt.ToLocalTime():g}";
        ProgressText.Text = $"Progress: {checkpoint.Responses.Count} questions answered";
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = RecoveryAction.Continue;
        DialogResult = true;
        Close();
    }

    private void StartFreshButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = RecoveryAction.StartFresh;
        DialogResult = true;
        Close();
    }

    private void DiscardButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = RecoveryAction.Discard;
        DialogResult = true;
        Close();
    }
}

/// <summary>
/// Enum representing recovery actions.
/// </summary>
public enum RecoveryAction
{
    /// <summary>
    /// Continue from the saved checkpoint.
    /// </summary>
    Continue,

    /// <summary>
    /// Start a fresh session (keep checkpoint).
    /// </summary>
    StartFresh,

    /// <summary>
    /// Discard the checkpoint.
    /// </summary>
    Discard
}
