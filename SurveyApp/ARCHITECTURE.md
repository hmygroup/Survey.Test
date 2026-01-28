# Architecture Documentation - Survey Management System

## Overview

This document describes the architectural design of the Survey Management System WPF application, built following SOLID principles, MVVM pattern, and modern .NET 8.0 practices.

## Technology Stack

### Core Framework
- **.NET 8.0** - Latest LTS version with C# 12 features
- **WPF (Windows Presentation Foundation)** - Desktop UI framework
- **ModernWPF** - Modern UI controls library providing WinUI-style controls for WPF

### Key Libraries
- **CommunityToolkit.Mvvm** - MVVM source generators and helpers
- **Microsoft.Extensions.DependencyInjection** - Built-in DI container
- **Microsoft.Extensions.Http** - HttpClient factory for API calls
- **Serilog** - Structured logging framework
- **Stateless** - State machine implementation (for future Answer status management)
- **System.Reactive** - Reactive extensions (for future validation features)

## Architectural Patterns

### 1. MVVM (Model-View-ViewModel)

The application strictly follows the MVVM pattern:

```
┌─────────────┐         ┌──────────────┐         ┌────────────┐
│    View     │────────▶│  ViewModel   │────────▶│   Model    │
│   (XAML)    │◀────────│   (Logic)    │◀────────│   (Data)   │
└─────────────┘         └──────────────┘         └────────────┘
     │                         │                        │
     │                         │                        │
  Binding              Commands/Properties         DTOs/Services
```

**View Layer:**
- XAML files for UI definition
- Minimal code-behind (only for initialization)
- Data binding to ViewModels

**ViewModel Layer:**
- Business logic and presentation logic
- Observable properties using `[ObservableProperty]`
- Commands using `[RelayCommand]`
- No direct references to Views

**Model Layer:**
- DTOs (Data Transfer Objects) for API communication
- Service classes for data access
- Domain entities and business rules

### 2. Dependency Injection

All components are registered in the DI container (`App.xaml.cs`):

```csharp
// API Services (Scoped via HttpClient factory)
services.AddHttpClient<QuestionaryService>(...);
services.AddHttpClient<QuestionService>(...);

// Infrastructure Services (Singleton)
services.AddSingleton<NavigationService>();
services.AddSingleton<DialogService>();
services.AddSingleton<ThemeService>();

// ViewModels (Transient)
services.AddTransient<MainWindowViewModel>();

// Views (Transient)
services.AddTransient<MainWindow>();
```

**Benefits:**
- Loose coupling between components
- Easy testing with mock services
- Centralized configuration
- Lifetime management

### 3. Repository Pattern (Via Services)

API services act as repositories, abstracting data access:

```csharp
QuestionaryService
    ├── GetAllAsync()           // Retrieve all questionnaires
    ├── GetByIdAsync(id)        // Get single questionnaire
    ├── GetFullAsync(id)        // Get with all questions
    ├── CreateAsync(name)       // Create new
    └── DeleteAsync(id)         // Delete
```

### 4. Service Layer Architecture

```
┌─────────────────────────────────────────────┐
│          Application Layer                  │
│  (ViewModels, Views, Commands)             │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│        Infrastructure Services              │
│  NavigationService, DialogService, Theme   │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│            API Services                     │
│  Questionary, Question, Answer, Response   │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│           Base ApiService                   │
│  (HTTP methods: GET, POST, PUT, PATCH)     │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│           HttpClient Factory                │
│  (Connection pooling, retry policies)      │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Backend REST API                    │
│    http://localhost:5049/api/              │
└─────────────────────────────────────────────┘
```

## Project Structure

```
SurveyApp/
├── Models/
│   ├── Dtos/                    # API Data Transfer Objects
│   │   ├── QuestionaryDto       # Questionnaire definition
│   │   ├── QuestionDto          # Question definition
│   │   ├── AnswerDto            # Survey session
│   │   ├── QuestionResponseDto  # Individual answer
│   │   ├── ConstraintDto        # Validation constraint
│   │   └── PolicyDto            # Validation policy
│   └── Enums/
│       └── AnswerStatus         # UNFINISHED, PENDING, COMPLETED, CANCELLED
│
├── ViewModels/
│   └── MainWindowViewModel      # Main window logic
│
├── Views/
│   └── (To be added in future phases)
│
├── Services/
│   ├── Api/                     # Backend API communication
│   │   ├── ApiService           # Base HTTP service
│   │   ├── QuestionaryService   # Questionnaire operations
│   │   ├── QuestionService      # Question operations
│   │   ├── AnswerService        # Answer session operations
│   │   └── QuestionResponseService  # Response operations
│   │
│   └── Infrastructure/          # App-level services
│       ├── NavigationService    # Page navigation
│       ├── DialogService        # User dialogs
│       └── ThemeService         # Theme management
│
├── Helpers/                     # Utility classes
├── Resources/                   # Assets and resources
│   └── Themes/                  # Theme definitions
│
├── App.xaml                     # Application resources
├── App.xaml.cs                  # DI configuration & startup
├── MainWindow.xaml              # Main window UI
├── MainWindow.xaml.cs           # Main window code-behind
└── GlobalUsings.cs              # Global using directives
```

## Data Flow

### 1. Questionnaire Retrieval Flow

```
User Action
    │
    ▼
MainWindowViewModel.NavigateToQuestionnaires()
    │
    ▼
QuestionaryService.GetAllAsync()
    │
    ▼
ApiService.GetAsync<IEnumerable<QuestionaryDto>>()
    │
    ▼
HttpClient → GET /api/questionary/{connection}/all
    │
    ▼
Deserialize JSON → List<QuestionaryDto>
    │
    ▼
Update ViewModel.Questionnaires (ObservableCollection)
    │
    ▼
UI automatically updates via data binding
```

### 2. Answer Session Flow

```
User starts survey
    │
    ▼
Create Answer Session
    │
POST /api/answer/{connection}?questionaryId={id}&user={email}
    │
    ▼
Receive AnswerId (Guid)
    │
    ▼
User fills in questions
    │
    ▼
Save responses incrementally
    │
POST /api/questionresponse/{connection}/response
    │
Body: [{ questionId, answerId, response }, ...]
    │
    ▼
User completes survey
    │
    ▼
Update Answer status
    │
PUT /api/answer/setStatus
    │
Body: { answersId: [guid], ANSWER_STATUS: "COMPLETED" }
```

## API Integration

### Connection Parameter

All API calls require a `connection` parameter (integer) that identifies the multi-tenant database:

```csharp
// Set globally in ApiService
apiService.ConnectionId = 1;

// Automatically included in all requests
GET /api/questionary/1/all
POST /api/question/new/1?questionaryId={id}
```

### HTTP Client Configuration

```csharp
services.AddHttpClient<QuestionaryService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5049/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

**Benefits:**
- Automatic connection pooling
- Proper disposal
- Centralized configuration
- Easy to add retry policies, circuit breakers, etc.

## Logging Strategy

### Log Levels

- **Debug**: Detailed flow information
- **Information**: General informational messages
- **Warning**: Recoverable errors or unexpected behavior
- **Error**: Unhandled exceptions or failures
- **Fatal**: Application crashes

### Log Outputs

1. **Console**: For development debugging
2. **File**: Persistent logs in `%APPDATA%/SurveyApp/Logs/`
   - Daily rotation
   - 7-day retention
   - Structured format with timestamps

### Example Log Entry

```
2026-01-15 10:23:45.123 +00:00 [INF] GET request to questionary/1/all
2026-01-15 10:23:45.456 +00:00 [INF] Navigating to Questionnaires
2026-01-15 10:23:45.789 +00:00 [ERR] HTTP error during GET request to questionary/1/all
System.Net.Http.HttpRequestException: Connection refused
```

## Theme Management

### Implementation

- Uses ModernWPF's built-in theming system
- Supports Light and Dark modes
- Persists user preference via `Settings.Default`

### Architecture

```
User clicks theme toggle
    │
    ▼
MainWindowViewModel.ToggleThemeCommand
    │
    ▼
ThemeService.ToggleTheme()
    │
    ▼
ModernWpf.ThemeManager.Current.ApplicationTheme = Dark/Light
    │
    ▼
Save to Settings.Default.Theme
    │
    ▼
UI automatically updates (all controls inherit theme)
```

## Navigation System

### Current Implementation

- NavigationView from ModernWPF
- Frame-based navigation
- Menu items: Home, Questionnaires, Responses

### Future Enhancement

```csharp
NavigationService.NavigateTo<QuestionaryListView>();
NavigationService.NavigateTo<QuestionaryEditorView>(questionaryId);
NavigationService.GoBack();
```

## Error Handling Strategy

### Layers

1. **API Service Layer**: 
   - Catch `HttpRequestException`
   - Log error
   - Rethrow to caller

2. **ViewModel Layer**:
   - Try-catch around service calls
   - Show user-friendly error via DialogService
   - Update UI state (loading, error messages)

3. **Global Handler**:
   - Catch unhandled exceptions in `App.xaml.cs`
   - Log fatal errors
   - Show critical error dialog

### Example

```csharp
// In ViewModel
try
{
    var questionnaires = await _questionaryService.GetAllAsync();
    Questionnaires = new ObservableCollection<QuestionaryDto>(questionnaires);
}
catch (HttpRequestException ex)
{
    await _dialogService.ShowErrorAsync(
        "Connection Error", 
        "Unable to connect to the server. Please check your connection.");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error loading questionnaires");
    await _dialogService.ShowErrorAsync(
        "Error", 
        "An unexpected error occurred. Please try again.");
}
```

## Future Enhancements (Planned)

### Phase 2: State Management
- Implement Stateless library for Answer status transitions
- State machine: UNFINISHED → PENDING → COMPLETED
- Track state history with timestamps

### Phase 3: Reactive Validation
- Use System.Reactive for real-time validation
- Debounce user input (500ms)
- Combine multiple validation streams
- Display inline validation messages

### Phase 4: Caching
- Implement intelligent caching with dependency graph
- Cache invalidation on related entity changes
- Use IMemoryCache with custom eviction policies

### Phase 5: Session Management
- Auto-save user progress every 30 seconds
- Checkpoint pattern for crash recovery
- Encrypted local storage for session data

## Security Considerations

### Current Implementation
- HTTPS recommended for production API
- Connection parameter for multi-tenancy
- Input validation before API calls

### Planned Enhancements
- Windows Credential Manager for connection strings
- DPAPI for encrypting session checkpoints
- Certificate pinning for API calls
- Rate limiting on client side

## Performance Optimizations

### Current
- HttpClient pooling via factory
- Async/await throughout (no blocking)
- Nullable reference types (prevent null reference exceptions)

### Planned
- Virtualized lists for large datasets
- Lazy loading for questions
- DataTemplate pooling
- Background threads for serialization
- Span<T> for high-performance string operations

## Testing Strategy (Future)

```
Tests/
├── Unit/
│   ├── ViewModels/          # Test ViewModel logic
│   ├── Services/            # Test service methods with mocks
│   └── Validation/          # Test validation rules
│
├── Integration/
│   └── Api/                 # Test API integration (WireMock)
│
└── UI/
    └── Automation/          # Test UI flows (WinAppDriver)
```

## Build and Deployment

### Development Build
```bash
dotnet restore
dotnet build
dotnet run
```

### Release Build
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

### Future: MSIX Packaging
- Create MSIX package for Microsoft Store
- Enable auto-updates
- Proper app installation/uninstallation

## Conclusion

This architecture provides:
- ✅ **Maintainability**: Clear separation of concerns
- ✅ **Testability**: Dependency injection and loose coupling
- ✅ **Scalability**: Modular design allows easy feature additions
- ✅ **Reliability**: Comprehensive error handling and logging
- ✅ **Performance**: Async operations and proper resource management
- ✅ **User Experience**: Modern UI with theme support

The foundation is production-ready for Phase 1 requirements and designed to support all advanced features planned for future phases.
