# API Integration Guide

This document describes how the Survey Management System integrates with the backend API.

## Base Configuration

**API Base URL**: `http://localhost:5030/api/`

All API services inherit from `ApiService` which provides common HTTP methods and automatic JSON serialization/deserialization.

## Connection Parameter

**IMPORTANT**: All API endpoints require a `connection` parameter that identifies the multi-tenant database.

### Setting the Connection ID

```csharp
// In your service or ViewModel
var questionaryService = serviceProvider.GetRequiredService<QuestionaryService>();
questionaryService.ConnectionId = 1; // Set to your database connection ID
```

The default connection ID is `1`.

## Available Services

### 1. QuestionaryService

Manages questionnaires (surveys).

#### Get All Questionnaires
```csharp
var questionnaries = await questionaryService.GetAllAsync();
// GET /api/questionary/{connection}/all
```

#### Get Single Questionnaire
```csharp
var questionary = await questionaryService.GetByIdAsync(questionaryId);
// GET /api/questionary/{connection}/{id}
```

#### Get Full Questionnaire (with Questions)
```csharp
var fullQuestionary = await questionaryService.GetFullAsync(questionaryId);
// GET /api/questionary/{connection}/{id}/full
```

#### Create Questionnaire
```csharp
var newQuestionary = await questionaryService.CreateAsync("Customer Satisfaction Survey");
// POST /api/questionary/{connection}/New/{name}
```

#### Delete Questionnaire
```csharp
await questionaryService.DeleteAsync(questionaryId);
// DELETE /api/questionary/{connection}/{id}
```

### 2. QuestionService

Manages questions within questionnaires.

#### Get Questions by Questionnaire
```csharp
var questions = await questionService.GetByQuestionaryIdAsync(questionaryId);
// GET /api/question/{connection}/questionary/{questionaryId}
```

#### Create Questions
```csharp
var newQuestions = new[]
{
    new { questionText = "What is your age?", questionTypeId = typeId },
    new { questionText = "How satisfied are you?", questionTypeId = typeId }
};

var created = await questionService.CreateAsync(questionaryId, newQuestions);
// POST /api/question/new/{connection}?questionaryId={id}
```

#### Delete Question
```csharp
await questionService.DeleteAsync(questionId);
// DELETE /api/question/{connection}/{id}
```

### 3. AnswerService

Manages answer sessions (when users fill out surveys).

#### Create Answer Session
```csharp
var answer = await answerService.CreateAsync(
    questionaryId: questionaryId,
    user: "john.doe@company.com",
    cardId: 12345
);
// POST /api/answer/{connection}?questionaryId={id}&user={email}&cardId={cardId}
// Returns: AnswerDto with Id to use for saving responses
```

#### Update Answer Status
```csharp
var answerIds = new[] { answerId1, answerId2 };
await answerService.SetStatusAsync(answerIds, AnswerStatus.Completed);
// PUT /api/answer/setStatus
// Body: { answersId: ["guid1", "guid2"], ANSWER_STATUS: "COMPLETED" }
```

#### Get Answers by Questionnaire
```csharp
var answers = await answerService.GetByQuestionaryIdAsync(questionaryId);
// GET /api/answer/{connection}/questionary/{questionaryId}
```

### 4. QuestionResponseService

Manages individual question responses within an answer session.

#### Save Responses
```csharp
var responses = new[]
{
    new { questionId = q1Id, answerId = sessionAnswerId, response = "25" },
    new { questionId = q2Id, answerId = sessionAnswerId, response = "Very satisfied" }
};

var saved = await questionResponseService.SaveResponsesAsync(responses);
// POST /api/questionresponse/{connection}/response
```

#### Update Single Response
```csharp
var updated = await questionResponseService.UpdateResponseAsync(
    questionResponseId: responseId,
    response: "Updated answer"
);
// PATCH /api/questionresponse/{connection}/response?questionResponseId={id}&response={value}
```

#### Get Responses by Answer Session
```csharp
var responses = await questionResponseService.GetByAnswerIdAsync(answerId);
// GET /api/questionresponse/{connection}/answer/{answerId}
```

## Complete Survey Workflow

### Step 1: Load the Questionnaire
```csharp
// Get questionnaire with all questions
var questionary = await questionaryService.GetFullAsync(questionaryId);
```

### Step 2: Start an Answer Session
```csharp
// Create a new answer session
var answer = await answerService.CreateAsync(
    questionaryId: questionary.Id,
    user: currentUser.Email,
    cardId: currentUser.CardId
);

var answerId = answer.Id; // Save this for the session
```

### Step 3: Save User Responses
```csharp
// As user fills in questions, save responses
var responses = new List<object>();

foreach (var question in questionary.Questions)
{
    responses.Add(new
    {
        questionId = question.Id,
        answerId = answerId,
        response = userAnswers[question.Id], // User's answer as string
        metadata = JsonSerializer.Serialize(new
        {
            timeSpent = elapsedSeconds,
            device = "desktop"
        })
    });
}

await questionResponseService.SaveResponsesAsync(responses);
```

### Step 4: Complete the Survey
```csharp
// Mark the answer session as completed
await answerService.SetStatusAsync(
    new[] { answerId },
    AnswerStatus.Completed
);
```

## Error Handling

All service methods can throw:
- `HttpRequestException` - Network or HTTP errors
- `JsonException` - Invalid response from server
- `Exception` - Other unexpected errors

### Recommended Pattern

```csharp
try
{
    var questionnaries = await questionaryService.GetAllAsync();
    // Process data
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Failed to connect to API");
    await _dialogService.ShowErrorAsync(
        "Connection Error",
        "Unable to reach the server. Please check your connection.");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    await _dialogService.ShowErrorAsync(
        "Error",
        "An unexpected error occurred. Please try again.");
}
```

## DTOs Reference

### QuestionaryDto
```csharp
{
    Id: Guid,
    Name: string,
    Description: string,
    CreatedBy: string,
    CreationDate: DateTimeOffset?
}
```

### FullQuestionaryDto
```csharp
{
    Id: Guid,
    Name: string,
    Description: string,
    Questions: IEnumerable<QuestionDto>
}
```

### QuestionDto
```csharp
{
    Id: Guid,
    QuestionText: string,
    QuestionType: QuestionTypeDto,
    QuestionResponses: ICollection<QuestionResponseDto>,
    DistinctAnswers: ICollection<AnswerDto>,
    Constraints: ICollection<ConstraintDto>
}
```

### AnswerDto
```csharp
{
    Id: Guid,
    QuestionaryId: Guid,
    User: string,
    CardId: int,
    AnswerStatus: AnswerStatusDto
}
```

### AnswerStatusDto
```csharp
{
    Id: Guid,
    Name: string,
    AnswerStatus: AnswerStatus // UNFINISHED, PENDING, COMPLETED, CANCELLED
}
```

### QuestionResponseDto
```csharp
{
    Id: Guid,
    QuestionId: Guid,
    AnswerId: Guid,
    Response: string,
    Metadata: string, // JSON
    Answers: ICollection<AnswerDto>
}
```

## Testing API Connection

### Quick Test in ViewModel

```csharp
public async Task TestConnectionAsync()
{
    try
    {
        _logger.LogInformation("Testing API connection...");
        
        var questionnaries = await _questionaryService.GetAllAsync();
        
        _logger.LogInformation(
            "Successfully connected. Found {Count} questionnaires", 
            questionnaries?.Count() ?? 0);
        
        await _dialogService.ShowMessageAsync(
            "Success",
            $"Connected to API. Found {questionnaries?.Count() ?? 0} questionnaires.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Connection test failed");
        await _dialogService.ShowErrorAsync(
            "Connection Failed",
            $"Could not connect to API: {ex.Message}");
    }
}
```

## Configuration Options

### Change API Base URL

In `App.xaml.cs`, modify:

```csharp
services.AddHttpClient<QuestionaryService>(client =>
{
    client.BaseAddress = new Uri("https://production-api.example.com/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### Add Authentication Header

```csharp
services.AddHttpClient<QuestionaryService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5030/api/");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_TOKEN");
});
```

### Add Retry Policy (using Polly - future enhancement)

```csharp
services.AddHttpClient<QuestionaryService>()
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## API Response Examples

### Successful Response
```json
// GET /api/questionary/1/all
[
    {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "name": "Customer Satisfaction Survey",
        "description": "Annual customer feedback",
        "createdBy": "admin@company.com",
        "creationDate": "2026-01-15T10:00:00Z"
    }
]
```

### Error Response
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
        "Name": ["The Name field is required."]
    }
}
```

## Best Practices

1. **Always use async/await** - Never block on `.Result` or `.Wait()`
2. **Handle cancellation** - Pass CancellationToken for long operations
3. **Log all API calls** - Use ILogger for debugging
4. **Validate input** - Check data before sending to API
5. **Set ConnectionId** - Ensure it's set before making calls
6. **Use try-catch** - Handle network errors gracefully
7. **Show loading UI** - Indicate to user when waiting for API
8. **Cache when appropriate** - Reduce unnecessary API calls

## Troubleshooting

### API not responding
- Check if backend is running: `curl http://localhost:5030/api/`
- Verify firewall settings
- Check logs in `%APPDATA%/SurveyApp/Logs/`

### Deserialization errors
- Ensure DTO properties match API response exactly
- Check for null values in required fields
- Verify JSON property name casing

### Timeout errors
- Increase timeout in HttpClient configuration
- Check network latency
- Consider implementing retry logic

## Next Steps

See `FRONTEND_TECHNICAL_DOCUMENTATION.md` for complete API endpoint specifications and business flows.
