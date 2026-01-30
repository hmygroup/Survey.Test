namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Questionary List view.
/// Manages the list of questionnaires with search, filter, and CRUD operations.
/// </summary>
public partial class QuestionaryListViewModel : ObservableObject
{
    private readonly QuestionaryService _questionaryService;
    private readonly DialogService _dialogService;
    private readonly NavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
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
        NavigationService navigationService,
        IServiceProvider serviceProvider,
        ILogger<QuestionaryListViewModel> logger)
    {
        _questionaryService = questionaryService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
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
            (q.Description?.ToLowerInvariant().Contains(searchLower) ?? false) ||
            (q.CreatedBy?.ToLowerInvariant().Contains(searchLower) ?? false)
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

        try
        {
            var dialog = _serviceProvider.GetRequiredService<QuestionaryDialogWindow>();
            var dialogViewModel = (QuestionaryDialogViewModel)dialog.DataContext;
            dialogViewModel.ConfigureForCreate();

            var result = dialog.ShowDialog();
            
            if (result == true && dialog.IsConfirmed)
            {
                var questionaryData = dialog.GetQuestionaryData();
                var name = questionaryData.name;
                var description = questionaryData.description;
                
                IsLoading = true;
                StatusMessage = $"Creating '{name}'...";
                _logger.LogInformation("Creating questionary: {Name}", name);

                var newQuestionary = await _questionaryService.CreateAsync(name);
                
                if (newQuestionary != null)
                {
                    await _dialogService.ShowMessageAsync("Success", $"Questionnaire '{name}' created successfully");
                    _logger.LogInformation("Successfully created questionary: {Id}", newQuestionary.Id);
                    
                    // Reload the list
                    await LoadQuestionnairesAsync();
                }
                else
                {
                    await _dialogService.ShowErrorAsync("Error", "Failed to create questionnaire - no response from server");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating questionary");
            StatusMessage = $"Error creating questionary: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to create questionnaire: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Edits the selected questionnaire.
    /// </summary>
    [RelayCommand]
    private async Task EditQuestionaryAsync(QuestionaryDto? questionary)
    {
        var targetQuestionary = questionary ?? SelectedQuestionary;
        if (targetQuestionary == null) return;

        _logger.LogInformation("Edit questionary: {Id}", targetQuestionary.Id);
        
        // Note: The API doesn't have an update endpoint yet, so we'll show a message
        await _dialogService.ShowMessageAsync("Info", 
            "Edit functionality requires an Update endpoint in the API.\n" +
            "This will be implemented when the backend provides PUT /api/questionary/{connectionId}/{id} endpoint.");
        
        /* When API supports update, use this code:
        try
        {
            var dialog = _serviceProvider.GetRequiredService<QuestionaryDialogWindow>();
            var dialogViewModel = (QuestionaryDialogViewModel)dialog.DataContext;
            dialogViewModel.ConfigureForEdit(targetQuestionary);

            var result = dialog.ShowDialog();
            
            if (result == true && dialog.IsConfirmed)
            {
                var (name, description) = dialog.GetQuestionaryData();
                
                IsLoading = true;
                StatusMessage = $"Updating '{name}'...";
                _logger.LogInformation("Updating questionary: {Id}", targetQuestionary.Id);

                await _questionaryService.UpdateAsync(targetQuestionary.Id, name, description);
                
                await _dialogService.ShowMessageAsync("Success", $"Questionnaire '{name}' updated successfully");
                _logger.LogInformation("Successfully updated questionary: {Id}", targetQuestionary.Id);
                
                // Reload the list
                await LoadQuestionnairesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating questionary: {Id}", targetQuestionary.Id);
            StatusMessage = $"Error updating questionary: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to update questionnaire: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
        */
    }

    /// <summary>
    /// Deletes the selected questionnaire.
    /// </summary>
    [RelayCommand]
    private async Task DeleteQuestionaryAsync(QuestionaryDto? questionary)
    {
        var targetQuestionary = questionary ?? SelectedQuestionary;
        if (targetQuestionary == null) return;

        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Confirm Delete",
            $"Are you sure you want to delete '{targetQuestionary.Name}'? This action cannot be undone.");

        if (!confirmed)
        {
            _logger.LogInformation("Delete cancelled by user");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = $"Deleting '{targetQuestionary.Name}'...";
            _logger.LogInformation("Deleting questionary: {Id}", targetQuestionary.Id);

            await _questionaryService.DeleteAsync(targetQuestionary.Id);

            await _dialogService.ShowMessageAsync("Success", $"'{targetQuestionary.Name}' deleted successfully");
            _logger.LogInformation("Successfully deleted questionary: {Id}", targetQuestionary.Id);

            // Reload the list
            await LoadQuestionnairesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting questionary: {Id}", targetQuestionary.Id);
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
    /// Opens the Question Editor for managing questions.
    /// </summary>
    [RelayCommand]
    private async Task ViewDetailsAsync(QuestionaryDto? questionary)
    {
        var targetQuestionary = questionary ?? SelectedQuestionary;
        if (targetQuestionary == null) return;

        try
        {
            _logger.LogInformation("Opening question editor for questionary: {Id}", targetQuestionary.Id);

            // Get the QuestionEditorView from DI
            var questionEditorView = _serviceProvider.GetRequiredService<QuestionEditorView>();
            
            // Initialize it with the current questionary
            await questionEditorView.InitializeAsync(targetQuestionary);
            
            // Navigate to the initialized editor instance
            _navigationService.NavigateToInstance(questionEditorView);
            
            _logger.LogInformation("Navigated to question editor for questionary: {Id}", targetQuestionary.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening question editor: {Id}", targetQuestionary.Id);
            await _dialogService.ShowErrorAsync("Error", $"Failed to open question editor: {ex.Message}");
        }
    }

    /// <summary>
    /// Views answer responses and analysis for the selected questionnaire.
    /// Opens the Answer Analysis view.
    /// </summary>
    [RelayCommand]
    private async Task ViewResponsesAsync(QuestionaryDto? questionary)
    {
        var targetQuestionary = questionary ?? SelectedQuestionary;
        if (targetQuestionary == null) return;

        try
        {
            _logger.LogInformation("Opening answer analysis for questionary: {Id}", targetQuestionary.Id);

            // Get the AnswerAnalysisView from DI
            var answerAnalysisView = _serviceProvider.GetRequiredService<AnswerAnalysisView>();
            
            // Initialize it with the current questionary
            await answerAnalysisView.InitializeAsync(targetQuestionary);
            
            // Navigate to the initialized view instance
            _navigationService.NavigateToInstance(answerAnalysisView);
            
            _logger.LogInformation("Navigated to answer analysis for questionary: {Id}", targetQuestionary.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening answer analysis: {Id}", targetQuestionary.Id);
            await _dialogService.ShowErrorAsync("Error", $"Failed to open answer analysis: {ex.Message}");
        }
    }


    /// Called when the selected questionary changes.
    /// </summary>
    partial void OnSelectedQuestionaryChanged(QuestionaryDto? value)
    {
        // Commands can now execute with or without parameters
    }
}
