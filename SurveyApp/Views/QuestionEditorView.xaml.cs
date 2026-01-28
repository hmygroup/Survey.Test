namespace SurveyApp.Views;

/// <summary>
/// Interaction logic for QuestionEditorView.xaml
/// </summary>
public partial class QuestionEditorView : Page
{
    private readonly QuestionEditorViewModel _viewModel;
    private Point _startPoint;
    private bool _isDragging;

    public QuestionEditorView(QuestionEditorViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    /// <summary>
    /// Initializes the view with a specific questionary.
    /// </summary>
    public async Task InitializeAsync(QuestionaryDto questionary)
    {
        await _viewModel.InitializeAsync(questionary);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        // Note: Questions are loaded via InitializeAsync called from navigation
        // This ensures proper initialization with questionary context
    }

    #region Drag-Drop Event Handlers

    private void QuestionsListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
        _isDragging = false;
    }

    private void QuestionsListView_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            Point currentPosition = e.GetPosition(null);
            Vector diff = _startPoint - currentPosition;

            // Check if drag threshold is exceeded
            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Get the dragged ListViewItem
                var listView = sender as ListView;
                if (listView?.SelectedItem is QuestionDto question)
                {
                    _isDragging = true;
                    
                    // Create drag data
                    var dragData = new DataObject(typeof(QuestionDto), question);
                    
                    // Initiate drag-drop
                    DragDrop.DoDragDrop(listView, dragData, DragDropEffects.Move);
                    
                    _isDragging = false;
                }
            }
        }
    }

    private void QuestionsListView_DragOver(object sender, DragEventArgs e)
    {
        // Provide visual feedback
        if (e.Data.GetDataPresent(typeof(QuestionDto)))
        {
            e.Effects = DragDropEffects.Move;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        
        e.Handled = true;
    }

    private async void QuestionsListView_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(QuestionDto)))
        {
            var droppedQuestion = e.Data.GetData(typeof(QuestionDto)) as QuestionDto;
            if (droppedQuestion == null) return;

            var listView = sender as ListView;
            if (listView == null) return;

            // Find the drop target
            var targetItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
            if (targetItem != null && targetItem.DataContext is QuestionDto targetQuestion)
            {
                int oldIndex = _viewModel.Questions.IndexOf(droppedQuestion);
                int newIndex = _viewModel.Questions.IndexOf(targetQuestion);

                if (oldIndex != -1 && newIndex != -1 && oldIndex != newIndex)
                {
                    await _viewModel.ReorderQuestionsAsync(oldIndex, newIndex);
                }
            }
        }
    }

    /// <summary>
    /// Finds an ancestor of a specific type in the visual tree.
    /// </summary>
    private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T ancestor)
            {
                return ancestor;
            }
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    #endregion
}
