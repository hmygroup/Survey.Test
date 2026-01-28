namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Questionary List view.
/// Manages the list of questionnaires with search, filter, and CRUD operations.
/// </summary>
public partial class QuestionaryListViewModel : ObservableObject
{
    private readonly QuestionaryService _questionaryService;
    private readonly DialogService _dialogService;
    private readonly ILogger<QuestionaryListViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<QuestionaryDto> _questionnaires = new();

    [ObservableProperty]
    private ObservableCollection<QuestionaryDto> _filteredQuestionnaires = new();

    [ObservableProperty]
    private QuestionaryDto? _selectedQuestionary;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public QuestionaryListViewModel(
        QuestionaryService questionaryService,
        DialogService dialogService,
        ILogger<QuestionaryListViewModel> logger)
    {
        _questionaryService = questionaryService;
        _dialogService = dialogService;
        _logger = logger;
    }

    /// <summary>
    /// Loads all questionnaires from the API.
    /// </summary>
    [RelayCommand]
    private async Task LoadQuestionnairesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading questionnaires...";
            _logger.LogInformation("Loading questionnaires");

            var result = await _questionaryService.GetAllAsync();
            
            if (result != null)
            {
                Questionnaires = new ObservableCollection<QuestionaryDto>(result);
                FilteredQuestionnaires = new ObservableCollection<QuestionaryDto>(result);
                StatusMessage = $"Loaded {Questionnaires.Count} questionnaire(s)";
                _logger.LogInformation("Successfully loaded {Count} questionnaires", Questionnaires.Count);
            }
            else
            {
                Questionnaires.Clear();
                FilteredQuestionnaires.Clear();
                StatusMessage = "No questionnaires found";
                _logger.LogWarning("No questionnaires returned from API");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questionnaires");
            StatusMessage = $"Error loading questionnaires: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to load questionnaires: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Filters the questionnaire list based on search text.
    /// </summary>
    partial void OnSearchTextChanged(string value)
    {
        FilterQuestionnaires();
    }

    /// <summary>
    /// Applies search filter to the questionnaire list.
    /// </summary>
    private void FilterQuestionnaires()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredQuestionnaires = new ObservableCollection<QuestionaryDto>(Questionnaires);
            return;
        }

        var searchLower = SearchText.ToLowerInvariant();
        var filtered = Questionnaires.Where(q =>
            q.Name.ToLowerInvariant().Contains(searchLower) ||
            q.Description.ToLowerInvariant().Contains(searchLower) ||
            q.CreatedBy.ToLowerInvariant().Contains(searchLower)
        ).ToList();

        FilteredQuestionnaires = new ObservableCollection<QuestionaryDto>(filtered);
        _logger.LogInformation("Filtered to {Count} questionnaires with search: {SearchText}", 
            filtered.Count, SearchText);
    }

    /// <summary>
    /// Creates a new questionnaire.
    /// </summary>
    [RelayCommand]
    private async Task CreateQuestionaryAsync()
    {
        _logger.LogInformation("Create questionary command invoked");
        // TODO: Implement create dialog
        await _dialogService.ShowMessageAsync("Info", "Create questionary dialog will be implemented in the next step");
    }

    /// <summary>
    /// Edits the selected questionnaire.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task EditQuestionaryAsync()
    {
        if (SelectedQuestionary == null) return;

        _logger.LogInformation("Edit questionary: {Id}", SelectedQuestionary.Id);
        // TODO: Implement edit dialog
        await _dialogService.ShowMessageAsync("Info", $"Edit dialog for '{SelectedQuestionary.Name}' will be implemented in the next step");
    }

    /// <summary>
    /// Deletes the selected questionnaire.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task DeleteQuestionaryAsync()
    {
        if (SelectedQuestionary == null) return;

        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Confirm Delete",
            $"Are you sure you want to delete '{SelectedQuestionary.Name}'? This action cannot be undone.");

        if (!confirmed)
        {
            _logger.LogInformation("Delete cancelled by user");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = $"Deleting '{SelectedQuestionary.Name}'...";
            _logger.LogInformation("Deleting questionary: {Id}", SelectedQuestionary.Id);

            await _questionaryService.DeleteAsync(SelectedQuestionary.Id);

            await _dialogService.ShowMessageAsync("Success", $"'{SelectedQuestionary.Name}' deleted successfully");
            _logger.LogInformation("Successfully deleted questionary: {Id}", SelectedQuestionary.Id);

            // Reload the list
            await LoadQuestionnairesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting questionary: {Id}", SelectedQuestionary.Id);
            StatusMessage = $"Error deleting questionary: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to delete questionary: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refreshes the questionnaire list.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        _logger.LogInformation("Refresh command invoked");
        await LoadQuestionnairesAsync();
    }

    /// <summary>
    /// Views details of the selected questionnaire.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task ViewDetailsAsync()
    {
        if (SelectedQuestionary == null) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading questionnaire details...";
            _logger.LogInformation("Loading full details for questionary: {Id}", SelectedQuestionary.Id);

            var fullQuestionary = await _questionaryService.GetFullAsync(SelectedQuestionary.Id);
            
            if (fullQuestionary != null)
            {
                var questionCount = fullQuestionary.Questions?.Count() ?? 0;
                await _dialogService.ShowMessageAsync(
                    fullQuestionary.Name,
                    $"Description: {fullQuestionary.Description}\n\n" +
                    $"Questions: {questionCount}");
                _logger.LogInformation("Displayed details for questionary: {Id}", SelectedQuestionary.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questionary details: {Id}", SelectedQuestionary.Id);
            await _dialogService.ShowErrorAsync("Error", $"Failed to load questionary details: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StatusMessage = string.Empty;
        }
    }

    /// <summary>
    /// Determines if edit or delete commands can execute.
    /// </summary>
    private bool CanEditOrDelete() => SelectedQuestionary != null;

    /// <summary>
    /// Called when the selected questionary changes.
    /// </summary>
    partial void OnSelectedQuestionaryChanged(QuestionaryDto? value)
    {
        EditQuestionaryCommand.NotifyCanExecuteChanged();
        DeleteQuestionaryCommand.NotifyCanExecuteChanged();
        ViewDetailsCommand.NotifyCanExecuteChanged();
    }
}
