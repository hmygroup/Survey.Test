namespace SurveyApp.Services.Infrastructure.Commands;

/// <summary>
/// Command for adding a new question to a questionary.
/// </summary>
public class AddQuestionCommand : IUndoableCommand
{
    private readonly QuestionService _questionService;
    private readonly ILogger<AddQuestionCommand> _logger;
    private readonly Guid _questionaryId;
    private readonly string _questionText;
    private readonly string _questionType;
    private readonly ICollection<ConstraintDto> _constraints;
    
    private Guid? _createdQuestionId;

    public Guid Id { get; } = Guid.NewGuid();
    public string Description { get; }
    public DateTime ExecutedAt { get; private set; }

    public AddQuestionCommand(
        QuestionService questionService,
        ILogger<AddQuestionCommand> logger,
        Guid questionaryId,
        string questionText,
        string questionType,
        ICollection<ConstraintDto> constraints)
    {
        _questionService = questionService;
        _logger = logger;
        _questionaryId = questionaryId;
        _questionText = questionText;
        _questionType = questionType;
        _constraints = constraints;
        Description = $"Add question: {questionText.Substring(0, Math.Min(50, questionText.Length))}...";
    }

    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Adding question to questionary {QuestionaryId}", _questionaryId);
            
            // Create question object for API
            var newQuestion = new
            {
                questionText = _questionText,
                questionType = new
                {
                    dotNetType = _questionType
                }
            };
            
            var createdQuestions = await _questionService.CreateAsync(
                _questionaryId,
                new[] { newQuestion });

            if (createdQuestions != null && createdQuestions.Any())
            {
                _createdQuestionId = createdQuestions.First().Id;
                ExecutedAt = DateTime.UtcNow;
                _logger.LogInformation("Question added successfully: {QuestionId}", _createdQuestionId);
                return true;
            }

            _logger.LogWarning("Failed to add question");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding question");
            return false;
        }
    }

    public async Task<bool> UndoAsync()
    {
        if (_createdQuestionId == null)
        {
            _logger.LogWarning("Cannot undo: question was not created");
            return false;
        }

        try
        {
            _logger.LogInformation("Undoing question creation: {QuestionId}", _createdQuestionId);
            
            await _questionService.DeleteAsync(_createdQuestionId.Value);
            
            _logger.LogInformation("Question deleted successfully (undo): {QuestionId}", _createdQuestionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error undoing question creation");
            return false;
        }
    }

    public async Task<bool> RedoAsync()
    {
        // Re-execute the command
        return await ExecuteAsync();
    }
}
