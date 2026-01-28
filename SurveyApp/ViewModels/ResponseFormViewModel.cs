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
    private readonly ILogger<ResponseFormViewModel> _logger;

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
        ILogger<ResponseFormViewModel> logger)
    {
        _questionService = questionService;
        _questionResponseService = questionResponseService;
        _answerService = answerService;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the form with an Answer session.
    /// </summary>
    public async Task InitializeAsync(AnswerDto answer)
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading questions...";
            CurrentAnswer = answer;

            // Load questions for the questionary
            var questions = await _questionService.GetByQuestionaryIdAsync(answer.QuestionaryId);
            if (questions != null && questions.Any())
            {
                Questions = new ObservableCollection<QuestionDto>(questions.OrderBy(q => q.Id));
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions.FirstOrDefault();
                
                UpdateProgressPercentage();
                StatusMessage = $"Question 1 of {Questions.Count}";
                
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
        await SaveCurrentResponse();
        
        CurrentQuestionIndex++;
        CurrentQuestion = Questions[CurrentQuestionIndex];
        UpdateProgressPercentage();
        StatusMessage = $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";
    }

    /// <summary>
    /// Moves to the previous question.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasPreviousQuestion))]
    private async Task PreviousQuestion()
    {
        await SaveCurrentResponse();
        
        CurrentQuestionIndex--;
        CurrentQuestion = Questions[CurrentQuestionIndex];
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
                // Save or update response
                var questionResponse = new
                {
                    questionId = CurrentQuestion.Id,
                    answerId = CurrentAnswer.Id,
                    response = responseValue,
                    metadata = $"{{\"timestamp\":\"{DateTime.UtcNow:O}\"}}"
                };

                await _questionResponseService.SaveResponsesAsync(
                    new[] { questionResponse });

                _logger.LogInformation("Saved response for Question {QuestionId}", CurrentQuestion.Id);
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
            IsLoading = true;
            StatusMessage = "Submitting responses...";

            // Save current response
            await SaveCurrentResponse();

            // Transition state to PENDING
            if (CurrentAnswer != null)
            {
                var success = await _answerService.TransitionStateAsync(
                    CurrentAnswer.Id,
                    AnswerTrigger.Complete,
                    CurrentAnswer.AnswerStatus?.AnswerStatus ?? AnswerStatus.Unfinished,
                    CurrentAnswer.User,
                    "Survey completed and submitted");

                if (success)
                {
                    StatusMessage = "Survey submitted successfully!";
                    _logger.LogInformation("Answer {AnswerId} submitted successfully", CurrentAnswer.Id);
                }
                else
                {
                    StatusMessage = "Failed to submit survey";
                    _logger.LogWarning("Failed to submit Answer {AnswerId}", CurrentAnswer.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting Answer {AnswerId}", CurrentAnswer?.Id);
            StatusMessage = "Error submitting survey";
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
}
