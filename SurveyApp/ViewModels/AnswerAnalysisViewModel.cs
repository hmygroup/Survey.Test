namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Answer Analysis view.
/// Provides filtering, searching, and statistics for answer sessions.
/// </summary>
public partial class AnswerAnalysisViewModel : ObservableObject
{
    private readonly AnswerService _answerService;
    private readonly QuestionResponseService _questionResponseService;
    private readonly DialogService _dialogService;
    private readonly NavigationService _navigationService;
    private readonly ILogger<AnswerAnalysisViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<AnswerDto> _answers = new();

    [ObservableProperty]
    private AnswerDto? _selectedAnswer;

    [ObservableProperty]
    private AnswerStatisticsDto? _statistics;

    [ObservableProperty]
    private QuestionaryDto? _currentQuestionary;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _searchUser = string.Empty;

    [ObservableProperty]
    private string? _filterStatus;

    [ObservableProperty]
    private DateTime? _filterFromDate;

    [ObservableProperty]
    private DateTime? _filterToDate;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _pageSize = 50;

    [ObservableProperty]
    private int _totalPages;

    [ObservableProperty]
    private int _totalCount;

    /// <summary>
    /// Gets whether there is a next page.
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Gets whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Gets the available status filter options.
    /// </summary>
    public List<string> StatusOptions { get; } = new()
    {
        "All",
        "UNFINISHED",
        "PENDING",
        "COMPLETED",
        "CANCELLED"
    };

    public AnswerAnalysisViewModel(
        AnswerService answerService,
        QuestionResponseService questionResponseService,
        DialogService dialogService,
        NavigationService navigationService,
        ILogger<AnswerAnalysisViewModel> logger)
    {
        _answerService = answerService;
        _questionResponseService = questionResponseService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _logger = logger;

        // Set default filter
        FilterStatus = "All";
    }

    /// <summary>
    /// Initializes the view with a questionary.
    /// </summary>
    public async Task InitializeAsync(QuestionaryDto questionary)
    {
        CurrentQuestionary = questionary ?? throw new ArgumentNullException(nameof(questionary));
        await LoadDataAsync();
    }

    /// <summary>
    /// Loads answers and statistics.
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (CurrentQuestionary == null)
        {
            _logger.LogWarning("Cannot load data: CurrentQuestionary is null");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Loading answers...";
            _logger.LogInformation("Loading answers for questionary {QuestionaryId}", CurrentQuestionary.Id);

            // Load answers with filters
            var status = FilterStatus == "All" ? null : FilterStatus;
            var result = await _answerService.GetByQuestionaryIdAsync(
                CurrentQuestionary.Id,
                status,
                FilterFromDate,
                FilterToDate,
                string.IsNullOrWhiteSpace(SearchUser) ? null : SearchUser,
                CurrentPage,
                PageSize
            );

            if (result != null)
            {
                Answers = new ObservableCollection<AnswerDto>(result.Items);
                TotalCount = result.TotalCount;
                TotalPages = result.TotalPages;
                StatusMessage = $"Loaded {Answers.Count} of {TotalCount} answer(s)";
                _logger.LogInformation("Successfully loaded {Count} answers", Answers.Count);
            }
            else
            {
                Answers.Clear();
                TotalCount = 0;
                TotalPages = 0;
                StatusMessage = "No answers found";
            }

            // Load statistics
            await LoadStatisticsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading answers for questionary {QuestionaryId}", CurrentQuestionary?.Id);
            StatusMessage = $"Error loading answers: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to load answers: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads statistics for the questionary.
    /// </summary>
    private async Task LoadStatisticsAsync()
    {
        if (CurrentQuestionary == null) return;

        try
        {
            Statistics = await _answerService.GetStatisticsAsync(CurrentQuestionary.Id);
            _logger.LogInformation("Loaded statistics for questionary {QuestionaryId}", CurrentQuestionary.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading statistics");
        }
    }

    /// <summary>
    /// Moves to the next page.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasNextPage))]
    private async Task NextPage()
    {
        CurrentPage++;
        await LoadDataAsync();
    }

    /// <summary>
    /// Moves to the previous page.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasPreviousPage))]
    private async Task PreviousPage()
    {
        CurrentPage--;
        await LoadDataAsync();
    }

    /// <summary>
    /// Applies the current filters.
    /// </summary>
    [RelayCommand]
    private async Task ApplyFilters()
    {
        CurrentPage = 1; // Reset to first page
        await LoadDataAsync();
    }

    /// <summary>
    /// Clears all filters.
    /// </summary>
    [RelayCommand]
    private async Task ClearFilters()
    {
        FilterStatus = "All";
        SearchUser = string.Empty;
        FilterFromDate = null;
        FilterToDate = null;
        CurrentPage = 1;
        await LoadDataAsync();
    }

    /// <summary>
    /// Exports answers to CSV.
    /// </summary>
    [RelayCommand]
    private async Task ExportToCsv()
    {
        if (CurrentQuestionary == null)
        {
            await _dialogService.ShowErrorAsync("Error", "No questionary selected.");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Exporting to CSV...";
            _logger.LogInformation("Exporting answers to CSV for questionary {QuestionaryId}", CurrentQuestionary.Id);

            var status = FilterStatus == "All" ? null : FilterStatus;
            var csvData = await _answerService.ExportToCsvAsync(
                CurrentQuestionary.Id,
                status,
                FilterFromDate,
                FilterToDate
            );

            if (csvData != null && csvData.Length > 0)
            {
                // Save file dialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    FileName = $"{CurrentQuestionary.Name}_Answers_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    DefaultExt = ".csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, csvData);
                    StatusMessage = "Export completed successfully";
                    _logger.LogInformation("Exported {Bytes} bytes to {FileName}", csvData.Length, saveFileDialog.FileName);
                    await _dialogService.ShowMessageAsync("Success", $"Exported {TotalCount} answer(s) to CSV successfully.");
                }
            }
            else
            {
                StatusMessage = "Export failed - no data";
                await _dialogService.ShowErrorAsync("Error", "Failed to export data.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting answers to CSV");
            StatusMessage = $"Error exporting: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to export: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Views details of the selected answer.
    /// </summary>
    [RelayCommand]
    private async Task ViewAnswerDetails()
    {
        if (SelectedAnswer == null)
        {
            await _dialogService.ShowErrorAsync("Error", "Please select an answer to view.");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Loading answer details...";

            // Load question responses for this answer
            var responses = await _questionResponseService.GetByAnswerIdAsync(SelectedAnswer.Id);
            
            if (responses != null && responses.Any())
            {
                var details = responses.Select((r, idx) => 
                    $"Response #{idx + 1}: {r.Response ?? "No response"}")
                    .ToArray();

                var message = string.Join("\n\n", details);
                await _dialogService.ShowMessageAsync(
                    $"Answer Details - {SelectedAnswer.User}",
                    message);
            }
            else
            {
                await _dialogService.ShowMessageAsync(
                    "No Responses",
                    "No question responses found for this answer.");
            }

            StatusMessage = "Ready";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading answer details");
            StatusMessage = $"Error loading details: {ex.Message}";
            await _dialogService.ShowErrorAsync("Error", $"Failed to load answer details: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refreshes the data.
    /// </summary>
    [RelayCommand]
    private async Task Refresh()
    {
        await LoadDataAsync();
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
}
