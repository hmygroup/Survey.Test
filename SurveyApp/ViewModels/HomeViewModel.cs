using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SurveyApp.Services.Api;

namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Home dashboard view.
/// </summary>
public partial class HomeViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly QuestionaryService _questionaryService;
    private readonly AnswerService _answerService;
    private readonly ILogger<HomeViewModel> _logger;

    [ObservableProperty]
    private int _totalQuestionnaires;

    [ObservableProperty]
    private int _totalResponses;

    [ObservableProperty]
    private double _completionRate;

    [ObservableProperty]
    private int _todayItems;

    [ObservableProperty]
    private ObservableCollection<string> _recentActivities = new();

    public HomeViewModel(
        NavigationService navigationService,
        QuestionaryService questionaryService,
        AnswerService answerService,
        ILogger<HomeViewModel> logger)
    {
        _navigationService = navigationService;
        _questionaryService = questionaryService;
        _answerService = answerService;
        _logger = logger;

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var questionaries = await _questionaryService.GetAllAsync();
            TotalQuestionnaires = questionaries?.Count() ?? 0;

            int totalResponses = 0;
            double totalCompletionRate = 0.0;
            int todayItems = 0;

            if (questionaries != null)
            {
                foreach (var questionary in questionaries)
                {
                    var stats = await _answerService.GetStatisticsAsync(questionary.Id);
                    if (stats != null)
                    {
                        totalResponses += stats.TotalAnswers;
                        totalCompletionRate += stats.CompletionRate;
                        todayItems += stats.ResponsesLast24Hours;
                    }
                }

                CompletionRate = questionaries.Count() > 0 ? totalCompletionRate / questionaries.Count() : 0.0;
            }

            TotalResponses = totalResponses;
            TodayItems = todayItems;

            // TODO: Load actual recent activities from services
            RecentActivities.Clear();
            RecentActivities.Add("Dashboard initialized");
            RecentActivities.Add("Statistics loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            RecentActivities.Clear();
            RecentActivities.Add("Error loading dashboard data");
        }
    }

    /// <summary>
    /// Command to create a new questionnaire.
    /// </summary>
    [RelayCommand]
    private void CreateQuestionary()
    {
        _logger.LogInformation("Navigating to create questionnaire");
        _navigationService.NavigateTo<QuestionaryListView>();
    }

    /// <summary>
    /// Command to view responses.
    /// </summary>
    [RelayCommand]
    private void ViewResponses()
    {
        _logger.LogInformation("Navigating to responses view");
        // TODO: Implement responses view navigation when available
    }
}