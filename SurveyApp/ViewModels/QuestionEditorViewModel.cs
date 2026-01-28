namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Question Editor view.
/// Manages questions within a questionnaire with CRUD operations, drag-drop reordering, and validation.
/// </summary>
public partial class QuestionEditorViewModel : ObservableObject
{
    private readonly QuestionService _questionService;
    private readonly DialogService _dialogService;
    private readonly NavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuestionEditorViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<QuestionDto> _questions = new();

    [ObservableProperty]
    private QuestionDto? _selectedQuestion;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private QuestionaryDto? _currentQuestionary;

    [ObservableProperty]
    private string _questionaryTitle = string.Empty;

    /// <summary>
    /// Initializes a new instance of the QuestionEditorViewModel.
    /// </summary>
    public QuestionEditorViewModel(
        QuestionService questionService,
        DialogService dialogService,
        NavigationService navigationService,
        IServiceProvider serviceProvider,
        ILogger<QuestionEditorViewModel> logger)
    {
        _questionService = questionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the editor with a specific questionary.
    /// </summary>
    public async Task InitializeAsync(QuestionaryDto questionary)
    {
        CurrentQuestionary = questionary ?? throw new ArgumentNullException(nameof(questionary));
        QuestionaryTitle = questionary.Name;
        await LoadQuestionsAsync();
    }

    /// <summary>
    /// Loads all questions for the current questionnaire.
    /// </summary>
    [RelayCommand]
    private async Task LoadQuestionsAsync()
    {
        if (CurrentQuestionary == null)
        {
            _logger.LogWarning("Cannot load questions: CurrentQuestionary is null");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Loading questions...";
            _logger.LogInformation("Loading questions for questionary {QuestionaryId}", CurrentQuestionary.Id);

            var result = await _questionService.GetByQuestionaryIdAsync(CurrentQuestionary.Id);

            if (result != null)
            {
                // Sort by order if available, otherwise by creation order
                var sortedQuestions = result.OrderBy(q => q.Id).ToList();
                Questions = new ObservableCollection<QuestionDto>(sortedQuestions);
                StatusMessage = $"Loaded {Questions.Count} question(s)";
                _logger.LogInformation("Successfully loaded {Count} questions", Questions.Count);
            }
            else
            {
                Questions.Clear();
                StatusMessage = "No questions found";
                _logger.LogInformation("No questions found for questionary {QuestionaryId}", CurrentQuestionary.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questions for questionary {QuestionaryId}", CurrentQuestionary?.Id);
            StatusMessage = $"Error loading questions: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to load questions: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Creates a new question.
    /// </summary>
    [RelayCommand]
    private async Task AddQuestionAsync()
    {
        if (CurrentQuestionary == null)
        {
            await _dialogService.ShowErrorAsync("Error", "No questionary selected.");
            return;
        }

        try
        {
            _logger.LogInformation("Adding new question to questionary {QuestionaryId}", CurrentQuestionary.Id);

            // Open question dialog
            var dialog = _serviceProvider.GetRequiredService<QuestionDialogWindow>();
            var dialogViewModel = (QuestionDialogViewModel)dialog.DataContext;
            dialogViewModel.ConfigureForCreate(CurrentQuestionary.Id);

            dialog.Owner = Application.Current.MainWindow;
            var result = dialog.ShowDialog();

            if (result == true && dialog.IsConfirmed)
            {
                var (questionText, questionType) = dialog.GetQuestionData();

                IsLoading = true;
                StatusMessage = "Creating question...";

                // Create question object for API
                var newQuestion = new
                {
                    questionText = questionText,
                    questionType = new
                    {
                        dotNetType = questionType
                    }
                };

                // Call API to create question
                var createdQuestions = await _questionService.CreateAsync(
                    CurrentQuestionary.Id,
                    new[] { newQuestion }
                );

                if (createdQuestions != null && createdQuestions.Any())
                {
                    var createdQuestion = createdQuestions.First();
                    Questions.Add(createdQuestion);
                    StatusMessage = "Question created successfully";
                    _logger.LogInformation("Question created successfully: {QuestionId}", createdQuestion.Id);
                    await _dialogService.ShowMessageAsync("Success", "Question created successfully.");
                }
                else
                {
                    StatusMessage = "Failed to create question";
                    await _dialogService.ShowErrorAsync("Error", "Failed to create question - no response from server");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding question");
            StatusMessage = $"Error adding question: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to add question: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Edits the selected question.
    /// </summary>
    [RelayCommand]
    private async Task EditQuestionAsync()
    {
        if (SelectedQuestion == null)
        {
            await _dialogService.ShowErrorAsync("Error", "Please select a question to edit.");
            return;
        }

        try
        {
            _logger.LogInformation("Editing question {QuestionId}", SelectedQuestion.Id);

            // Open question dialog with existing data
            var dialog = _serviceProvider.GetRequiredService<QuestionDialogWindow>();
            var dialogViewModel = (QuestionDialogViewModel)dialog.DataContext;
            dialogViewModel.ConfigureForEdit(SelectedQuestion);

            dialog.Owner = Application.Current.MainWindow;
            var result = dialog.ShowDialog();

            if (result == true && dialog.IsConfirmed)
            {
                var (questionText, questionType) = dialog.GetQuestionData();

                // Update the question in the collection
                var index = Questions.IndexOf(SelectedQuestion);
                if (index >= 0)
                {
                    // Create updated question object
                    var updatedQuestion = SelectedQuestion with
                    {
                        QuestionText = questionText,
                        QuestionType = new QuestionTypeDto
                        {
                            Id = SelectedQuestion.QuestionType?.Id ?? Guid.NewGuid(),
                            DotNetType = questionType
                        }
                    };

                    Questions[index] = updatedQuestion;
                    SelectedQuestion = updatedQuestion;

                    StatusMessage = "Question updated successfully";
                    _logger.LogInformation("Question updated successfully: {QuestionId}", updatedQuestion.Id);
                    await _dialogService.ShowMessageAsync("Success", "Question updated successfully.\n\nNote: API update endpoint not yet available. Changes are local only.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing question {QuestionId}", SelectedQuestion?.Id);
            StatusMessage = $"Error editing question: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to edit question: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes the selected question after confirmation.
    /// </summary>
    [RelayCommand]
    private async Task DeleteQuestionAsync()
    {
        if (SelectedQuestion == null)
        {
            await _dialogService.ShowErrorAsync("Error", "Please select a question to delete.");
            return;
        }

        try
        {
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Delete Question",
                $"Are you sure you want to delete this question?\n\n{SelectedQuestion.QuestionText}\n\nThis action cannot be undone."
            );

            if (!confirmed)
            {
                _logger.LogInformation("Question deletion cancelled by user");
                return;
            }

            IsLoading = true;
            StatusMessage = "Deleting question...";
            _logger.LogInformation("Deleting question {QuestionId}", SelectedQuestion.Id);

            await _questionService.DeleteAsync(SelectedQuestion.Id);

            Questions.Remove(SelectedQuestion);
            SelectedQuestion = null;

            StatusMessage = "Question deleted successfully";
            _logger.LogInformation("Question deleted successfully");
            await _dialogService.ShowMessageAsync("Success", "Question deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question {QuestionId}", SelectedQuestion?.Id);
            StatusMessage = $"Error deleting question: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to delete question: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates back to the questionary list.
    /// </summary>
    [RelayCommand]
    private void NavigateBack()
    {
        _logger.LogInformation("Navigating back to questionary list");
        _navigationService.NavigateTo<QuestionaryListView>();
    }

    /// <summary>
    /// Handles the drag-drop reordering of questions.
    /// </summary>
    public async Task ReorderQuestionsAsync(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= Questions.Count || newIndex < 0 || newIndex >= Questions.Count)
        {
            _logger.LogWarning("Invalid reorder indices: old={OldIndex}, new={NewIndex}", oldIndex, newIndex);
            return;
        }

        try
        {
            _logger.LogInformation("Reordering question from index {OldIndex} to {NewIndex}", oldIndex, newIndex);

            var question = Questions[oldIndex];
            Questions.RemoveAt(oldIndex);
            Questions.Insert(newIndex, question);

            StatusMessage = "Question reordered";
            
            // TODO: Implement bulk order update API call
            // TODO: Add to undo/redo history
            
            _logger.LogInformation("Question reordered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering questions");
            await _dialogService.ShowErrorAsync("Error", $"Failed to reorder questions: {ex.Message}");
        }
    }
}
