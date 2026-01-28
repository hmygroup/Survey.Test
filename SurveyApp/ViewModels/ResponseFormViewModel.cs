namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Response Form.
/// Handles displaying questions and collecting responses for an Answer session.
/// </summary>
public partial class ResponseFormViewModel : ObservableObject
{
    private readonly QuestionService _questionService;
    private readonly QuestionResponseService _questionResponseService;
    private readonly AnswerService _answerService;
    private readonly SessionManager _sessionManager;
    private readonly ILogger<ResponseFormViewModel> _logger;
    private string _questionaryTitle = string.Empty;
    
    // Metadata tracking
    private readonly Dictionary<Guid, DateTime> _questionStartTimes = new();
    private readonly Dictionary<Guid, TimeSpan> _questionDurations = new();
    private readonly Dictionary<Guid, int> _questionInteractions = new();
    private DateTime _sessionStartTime;
    private readonly string _deviceInfo;

    [ObservableProperty]
    private ObservableCollection<QuestionDto> _questions = new();

    [ObservableProperty]
    private Dictionary<Guid, string> _responses = new();

    [ObservableProperty]
    private AnswerDto? _currentAnswer;

    [ObservableProperty]
    private QuestionDto? _currentQuestion;

    [ObservableProperty]
    private int _currentQuestionIndex;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private int _progressPercentage;

    /// <summary>
    /// Gets whether there is a next question.
    /// </summary>
    public bool HasNextQuestion => CurrentQuestionIndex < Questions.Count - 1;

    /// <summary>
    /// Gets whether there is a previous question.
    /// </summary>
    public bool HasPreviousQuestion => CurrentQuestionIndex > 0;

    public ResponseFormViewModel(
        QuestionService questionService,
        QuestionResponseService questionResponseService,
        AnswerService answerService,
        SessionManager sessionManager,
        ILogger<ResponseFormViewModel> logger)
    {
        _questionService = questionService;
        _questionResponseService = questionResponseService;
        _answerService = answerService;
        _sessionManager = sessionManager;
        _logger = logger;
        
        // Collect device info
        _deviceInfo = $"OS: {Environment.OSVersion}, " +
                      $"CLR: {Environment.Version}, " +
                      $"Machine: {Environment.MachineName}, " +
                      $"User: {Environment.UserName}, " +
                      $"64-bit: {Environment.Is64BitOperatingSystem}";
    }

    /// <summary>
    /// Initializes the form with an Answer session.
    /// </summary>
    public async Task InitializeAsync(AnswerDto answer, string questionaryTitle = "")
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading questions...";
            CurrentAnswer = answer;
            _questionaryTitle = questionaryTitle;
            _sessionStartTime = DateTime.UtcNow;

            // Load questions for the questionary
            var questions = await _questionService.GetByQuestionaryIdAsync(answer.QuestionaryId);
            if (questions != null && questions.Any())
            {
                Questions = new ObservableCollection<QuestionDto>(questions.OrderBy(q => q.Id));
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions.FirstOrDefault();
                
                // Start tracking time for first question
                if (CurrentQuestion != null)
                {
                    _questionStartTimes[CurrentQuestion.Id] = DateTime.UtcNow;
                }
                
                UpdateProgressPercentage();
                StatusMessage = $"Question 1 of {Questions.Count}";
                
                // Start auto-save
                _sessionManager.StartAutoSave(GetCurrentCheckpoint);
                
                _logger.LogInformation("Loaded {Count} questions for Answer {AnswerId}", 
                    Questions.Count, answer.Id);
            }
            else
            {
                StatusMessage = "No questions found for this questionary";
                _logger.LogWarning("No questions found for Questionary {QuestionaryId}", answer.QuestionaryId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questions for Answer {AnswerId}", answer.Id);
            StatusMessage = "Error loading questions";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Moves to the next question.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasNextQuestion))]
    private async Task NextQuestion()
    {
        // Track time spent on current question
        if (CurrentQuestion != null && _questionStartTimes.TryGetValue(CurrentQuestion.Id, out var startTime))
        {
            _questionDurations[CurrentQuestion.Id] = DateTime.UtcNow - startTime;
        }
        
        await SaveCurrentResponse();
        
        CurrentQuestionIndex++;
        CurrentQuestion = Questions[CurrentQuestionIndex];
        
        // Start tracking time for new question
        if (CurrentQuestion != null)
        {
            _questionStartTimes[CurrentQuestion.Id] = DateTime.UtcNow;
        }
        
        UpdateProgressPercentage();
        StatusMessage = $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";
    }

    /// <summary>
    /// Moves to the previous question.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasPreviousQuestion))]
    private async Task PreviousQuestion()
    {
        // Track time spent on current question
        if (CurrentQuestion != null && _questionStartTimes.TryGetValue(CurrentQuestion.Id, out var startTime))
        {
            _questionDurations[CurrentQuestion.Id] = DateTime.UtcNow - startTime;
        }
        
        await SaveCurrentResponse();
        
        CurrentQuestionIndex--;
        CurrentQuestion = Questions[CurrentQuestionIndex];
        
        // Start tracking time for new question
        if (CurrentQuestion != null)
        {
            _questionStartTimes[CurrentQuestion.Id] = DateTime.UtcNow;
        }
        
        UpdateProgressPercentage();
        StatusMessage = $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";
    }

    /// <summary>
    /// Saves the response for the current question.
    /// </summary>
    private async Task SaveCurrentResponse()
    {
        if (CurrentQuestion == null || CurrentAnswer == null)
            return;

        try
        {
            if (Responses.TryGetValue(CurrentQuestion.Id, out var responseValue))
            {
                // Calculate time spent on this question
                var timeSpent = _questionDurations.ContainsKey(CurrentQuestion.Id)
                    ? _questionDurations[CurrentQuestion.Id]
                    : (_questionStartTimes.ContainsKey(CurrentQuestion.Id)
                        ? DateTime.UtcNow - _questionStartTimes[CurrentQuestion.Id]
                        : TimeSpan.Zero);
                
                // Get interaction count
                var interactions = _questionInteractions.ContainsKey(CurrentQuestion.Id)
                    ? _questionInteractions[CurrentQuestion.Id]
                    : 0;
                
                // Build metadata JSON
                var metadata = $"{{" +
                    $"\"timestamp\":\"{DateTime.UtcNow:O}\"," +
                    $"\"timeSpentSeconds\":{timeSpent.TotalSeconds:F2}," +
                    $"\"interactions\":{interactions}," +
                    $"\"deviceInfo\":\"{_deviceInfo.Replace("\"", "\\\"")}\"" +
                    $"}}";

                // Save or update response
                var questionResponse = new
                {
                    questionId = CurrentQuestion.Id,
                    answerId = CurrentAnswer.Id,
                    response = responseValue,
                    metadata = metadata
                };

                await _questionResponseService.SaveResponsesAsync(
                    new[] { questionResponse });

                _logger.LogInformation("Saved response for Question {QuestionId} (Time: {Time}s, Interactions: {Count})", 
                    CurrentQuestion.Id, timeSpent.TotalSeconds, interactions);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving response for Question {QuestionId}", CurrentQuestion.Id);
        }
    }

    /// <summary>
    /// Updates the response for the current question.
    /// </summary>
    public void UpdateResponse(string value)
    {
        if (CurrentQuestion != null)
        {
            Responses[CurrentQuestion.Id] = value;
            
            // Track interaction
            if (!_questionInteractions.ContainsKey(CurrentQuestion.Id))
            {
                _questionInteractions[CurrentQuestion.Id] = 0;
            }
            _questionInteractions[CurrentQuestion.Id]++;
            
            UpdateProgressPercentage();
        }
    }

    /// <summary>
    /// Submits the form and completes the Answer.
    /// </summary>
    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            // Validation: Check if all questions are answered
            var unansweredCount = Questions.Count - Responses.Count;
            if (unansweredCount > 0)
            {
                StatusMessage = $"Warning: {unansweredCount} question(s) not answered";
                _logger.LogWarning("Attempting to submit with {Count} unanswered questions", unansweredCount);
                
                // Could show a confirmation dialog here, but for minimal changes, just proceed
            }
            
            IsLoading = true;
            StatusMessage = "Submitting responses...";

            // Track final question time
            if (CurrentQuestion != null && _questionStartTimes.TryGetValue(CurrentQuestion.Id, out var startTime))
            {
                _questionDurations[CurrentQuestion.Id] = DateTime.UtcNow - startTime;
            }

            // Save current response with final metadata
            await SaveCurrentResponse();

            // Calculate total session time
            var totalSessionTime = DateTime.UtcNow - _sessionStartTime;

            // Transition state to PENDING
            if (CurrentAnswer != null)
            {
                var notes = $"Survey completed. Total time: {totalSessionTime.TotalMinutes:F2} minutes. " +
                           $"Questions answered: {Responses.Count}/{Questions.Count}. " +
                           $"Device: {_deviceInfo}";
                
                var success = await _answerService.TransitionStateAsync(
                    CurrentAnswer.Id,
                    AnswerTrigger.Complete,
                    CurrentAnswer.AnswerStatus?.AnswerStatus ?? AnswerStatus.Unfinished,
                    CurrentAnswer.User,
                    notes);

                if (success)
                {
                    StatusMessage = $"✓ Survey submitted successfully! Total time: {totalSessionTime.TotalMinutes:F1} minutes";
                    _logger.LogInformation("Answer {AnswerId} submitted successfully. Time: {Time} minutes, Responses: {Count}/{Total}",
                        CurrentAnswer.Id, totalSessionTime.TotalMinutes, Responses.Count, Questions.Count);
                    
                    // Delete checkpoint after successful submission
                    await _sessionManager.DeleteCheckpointAsync(CurrentAnswer.Id);
                }
                else
                {
                    StatusMessage = "❌ Failed to submit survey. Please try again.";
                    _logger.LogWarning("Failed to submit Answer {AnswerId}", CurrentAnswer.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting Answer {AnswerId}", CurrentAnswer?.Id);
            StatusMessage = "❌ Error submitting survey. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Calculates and updates the progress percentage.
    /// </summary>
    private void UpdateProgressPercentage()
    {
        if (Questions.Count == 0)
        {
            ProgressPercentage = 0;
            return;
        }

        var answeredCount = Responses.Count;
        ProgressPercentage = (int)((double)answeredCount / Questions.Count * 100);
    }

    /// <summary>
    /// Gets the current session checkpoint.
    /// </summary>
    private Task<SessionCheckpoint?> GetCurrentCheckpoint()
    {
        if (CurrentAnswer == null)
            return Task.FromResult<SessionCheckpoint?>(null);

        var checkpoint = new SessionCheckpoint
        {
            AnswerId = CurrentAnswer.Id,
            QuestionaryId = CurrentAnswer.QuestionaryId,
            QuestionaryTitle = _questionaryTitle,
            CurrentQuestionIndex = CurrentQuestionIndex,
            Responses = new Dictionary<Guid, string>(Responses),
            Status = CurrentAnswer.AnswerStatus?.AnswerStatus.ToString() ?? "UNFINISHED",
            Metadata = new Dictionary<string, object>
            {
                ["totalQuestions"] = Questions.Count,
                ["progressPercentage"] = ProgressPercentage,
                ["sessionStartTime"] = _sessionStartTime,
                ["deviceInfo"] = _deviceInfo,
                ["questionDurations"] = _questionDurations.ToDictionary(
                    kvp => kvp.Key.ToString(), 
                    kvp => kvp.Value.TotalSeconds),
                ["questionInteractions"] = _questionInteractions.ToDictionary(
                    kvp => kvp.Key.ToString(), 
                    kvp => kvp.Value)
            }
        };

        return Task.FromResult<SessionCheckpoint?>(checkpoint);
    }

    /// <summary>
    /// Restores the session from a checkpoint.
    /// </summary>
    public void RestoreFromCheckpoint(SessionCheckpoint checkpoint)
    {
        CurrentQuestionIndex = checkpoint.CurrentQuestionIndex;
        Responses = new Dictionary<Guid, string>(checkpoint.Responses);
        
        if (CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Questions.Count)
        {
            CurrentQuestion = Questions[CurrentQuestionIndex];
        }
        
        UpdateProgressPercentage();
        StatusMessage = $"Question {CurrentQuestionIndex + 1} of {Questions.Count} (Restored)";
        
        _logger.LogInformation("Session restored from checkpoint for Answer {AnswerId}", checkpoint.AnswerId);
    }

    /// <summary>
    /// Cleans up resources when the form is closed.
    /// </summary>
    public async Task CleanupAsync()
    {
        // Stop auto-save
        _sessionManager.StopAutoSave();
        
        // Save final checkpoint if session is still unfinished
        if (CurrentAnswer != null && 
            CurrentAnswer.AnswerStatus?.AnswerStatus == AnswerStatus.Unfinished)
        {
            var checkpoint = await GetCurrentCheckpoint();
            if (checkpoint != null)
            {
                await _sessionManager.SaveCheckpointAsync(checkpoint);
            }
        }
        // Delete checkpoint if session is completed or cancelled
        else if (CurrentAnswer != null)
        {
            await _sessionManager.DeleteCheckpointAsync(CurrentAnswer.Id);
        }
    }
}
