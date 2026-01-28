# Survey Management System - WPF Application

A modern, production-ready WPF application for managing questionnaires and surveys, built with .NET 8.0 and ModernWPF.

## 🚀 Features

- **Modern UI**: Built with ModernWPF for a contemporary Windows 11-style interface
- **MVVM Architecture**: Clean separation of concerns using the MVVM pattern with CommunityToolkit.Mvvm
- **Dependency Injection**: Full DI container setup with Microsoft.Extensions.DependencyInjection
- **HTTP Client Factory**: Proper HttpClient management for API communication
- **Logging**: Comprehensive logging with Serilog (file and console output)
- **Theme Switching**: Built-in Light/Dark theme toggle
- **State Management**: Graph-based state machine using Stateless library
- **Reactive Validation**: Real-time validation with System.Reactive

## 📋 Prerequisites

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Windows 10** version 1809+ or **Windows 11**
- **Visual Studio 2022** (recommended) or **Visual Studio Code** with C# extension
- **Backend API** running at `http://localhost:5049/api/` with Connection ID `10001`

## 🛠️ Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Survey.Test/SurveyApp
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the Application

```bash
dotnet run
```

Or open `SurveyApp.sln` in Visual Studio 2022 and press F5.

## 📦 NuGet Packages

The project uses the following key packages:

- **CommunityToolkit.Mvvm** (8.2.2) - MVVM helpers and source generators
- **Microsoft.Extensions.DependencyInjection** (8.0.0) - Dependency injection
- **Microsoft.Extensions.Http** (8.0.0) - HttpClient factory
- **ModernWpfUI** (0.9.6) - Modern WinUI 3 controls for WPF
- **Serilog** (3.1.1) - Structured logging
- **Serilog.Sinks.File** (5.0.0) - File logging
- **Stateless** (5.16.0) - State machine implementation
- **System.Reactive** (6.0.0) - Reactive extensions for validation

## 🏗️ Project Structure

```
SurveyApp/
├── Models/
│   ├── Dtos/                  # Data Transfer Objects
│   │   ├── QuestionaryDto.cs
│   │   ├── QuestionDto.cs
│   │   ├── AnswerDto.cs
│   │   └── ...
│   └── Enums/                 # Enumerations
│       └── AnswerStatus.cs
├── ViewModels/                # ViewModels (MVVM)
│   └── MainWindowViewModel.cs
├── Views/                     # Views (XAML)
│   └── (To be added in Phase 2)
├── Services/
│   ├── Api/                   # API Services
│   │   ├── ApiService.cs      # Base HTTP client service
│   │   ├── QuestionaryService.cs
│   │   ├── QuestionService.cs
│   │   ├── AnswerService.cs
│   │   └── QuestionResponseService.cs
│   └── Infrastructure/        # Infrastructure Services
│       ├── NavigationService.cs
│       ├── DialogService.cs
│       └── ThemeService.cs
├── Helpers/                   # Helper classes
├── Resources/                 # Resources (themes, icons)
│   └── Themes/
├── App.xaml                   # Application resources
├── App.xaml.cs                # Application startup & DI configuration
├── MainWindow.xaml            # Main window UI
├── MainWindow.xaml.cs         # Main window code-behind
└── GlobalUsings.cs            # Global using directives

```

## 🎨 Features Implemented (Phase 1)

### ✅ Foundation
- [x] Project structure with proper folders
- [x] DI container setup in App.xaml.cs
- [x] ApiService base implementation with HttpClientFactory
- [x] MainWindow with NavigationView skeleton
- [x] Light/Dark theme switching capability
- [x] Logging infrastructure with Serilog

### 📊 Models & DTOs
- [x] QuestionaryDto
- [x] FullQuestionaryDto
- [x] QuestionDto
- [x] QuestionTypeDto
- [x] AnswerDto
- [x] AnswerStatusDto
- [x] QuestionResponseDto
- [x] ConstraintDto
- [x] PolicyDto
- [x] PolicyRecordsDto

### 🔧 Services
- [x] ApiService (base HTTP client wrapper)
- [x] QuestionaryService
- [x] QuestionService
- [x] AnswerService
- [x] QuestionResponseService
- [x] NavigationService
- [x] DialogService
- [x] ThemeService

## 🔍 API Configuration

The application connects to the backend API at:
```
Base URL: http://localhost:5049/api/
Connection ID: 10001
```

The default connection ID is set to `1`. You can modify this in the `ApiService.ConnectionId` property.

## 📝 Logging

Logs are stored in:
```
%APPDATA%\SurveyApp\Logs\app-YYYYMMDD.log
```

Log files are rotated daily and retained for 7 days.

## 🎨 Theme Switching

The application supports Light and Dark themes:

- Click the theme toggle button in the title bar
- Theme preference is saved and persisted across sessions
- Uses ModernWPF's built-in theming system

## 🔐 Architecture Highlights

### Dependency Injection
All services are registered in `App.xaml.cs`:
- API services are registered with HttpClient factory
- Infrastructure services are singletons
- ViewModels and Views are transient

### MVVM Pattern
- ViewModels use `CommunityToolkit.Mvvm` source generators
- Commands use `[RelayCommand]` attribute
- Properties use `[ObservableProperty]` attribute
- Strict separation between View and ViewModel

### Error Handling
- All API calls include try-catch blocks
- Errors are logged using Serilog
- User-friendly error messages via DialogService

## 🚦 Next Steps (Phase 2)

- [ ] QuestionaryListView with DataGrid
- [ ] Search and filter functionality
- [ ] Create/Edit/Delete questionary dialogs
- [ ] GraphCache implementation
- [ ] SessionManager with checkpointing
- [ ] Unit tests for services

## 🤝 Contributing

1. Follow SOLID principles
2. Use async/await for all I/O operations
3. Add XML documentation comments to public APIs
4. Enable nullable reference types
5. Use C# 12 features (file-scoped namespaces, records)
6. Keep methods under 50 lines
7. Maintain cyclomatic complexity below 10

## 📄 License

Copyright © 2026 Survey Enterprise

## 🐛 Troubleshooting

### Application won't start
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Check that the backend API is running at `http://localhost:5049/api/`
- Review logs in `%APPDATA%\SurveyApp\Logs\`

### Theme not switching
- Delete settings: `%LOCALAPPDATA%\SurveyApp\`
- Restart the application

### API connection errors
- Verify backend is running: `curl http://localhost:5049/api/`
- Check connection ID in `ApiService.ConnectionId`
- Review network logs in Serilog output

## 📞 Support

For issues and questions, please refer to the technical documentation:
- `FRONTEND_TECHNICAL_DOCUMENTATION.md`
- `prompt.md`
