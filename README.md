# Survey Management System

A modern WPF-based questionnaire management system built with .NET 8.0, featuring advanced UI/UX with WPF UI framework.

## 📚 Documentation

### Getting Started
- **[Development Guide](prompt.md)** - Main development instructions and requirements
- **[Project Architecture](docs/architecture/)** - Technical architecture and design patterns

### API Documentation
- **[API Documentation](docs/api/API_DOCUMENTATION.md)** - Complete API reference
- **[Backend Requirements](docs/api/BACKEND_API_REQUIREMENTS.md)** - Required backend endpoints
- **[API Summary](docs/api/BACKEND_API_SUMMARY.md)** - Quick API overview
- **[Backend Implementation](docs/api/BACKEND_IMPLEMENTATION_SUMMARY.md)** - Backend implementation status

### Frontend Documentation
- **[Frontend Technical Docs](docs/frontend/FRONTEND_TECHNICAL_DOCUMENTATION.md)** - Complete frontend specifications
- **[UI Migration Plan](docs/frontend/UI_MIGRATION_PLAN.md)** - ModernWPF to WPF UI migration guide

### Phase Completion Reports
- **[Phase 2](docs/phases/PHASE2_COMPLETION.md)** - Questionary Management (Complete)
- **[Phase 3](docs/phases/PHASE3_IMPLEMENTATION_SUMMARY.md)** - Question Editor (Complete)
- **[Phase 3 Progress](docs/phases/PHASE3_PROGRESS_DAY1.md)** - Day 1 progress
- **[Phase 4](docs/phases/PHASE4_IMPLEMENTATION_SUMMARY.md)** - Response Collection (In Progress)
- **[Phase 4 Complete](docs/phases/PHASE4_COMPLETE.md)** - Phase 4 completion report

### Architecture & Implementation
- **[Constraint Editor](docs/architecture/CONSTRAINT_EDITOR_IMPLEMENTATION.md)** - Constraint editor implementation details
- **[Task Completion Summary](docs/architecture/TASK_COMPLETION_SUMMARY.md)** - Overall task completion status

## 🚀 Quick Start

1. **Prerequisites**
   - .NET 8.0 SDK
   - Visual Studio 2022 or JetBrains Rider
   - Windows 10/11

2. **Build**
   ```bash
   dotnet build Survey.Test.sln
   ```

3. **Run**
   ```bash
   dotnet run --project SurveyApp
   ```

## 🎨 Current Status

- ✅ Phase 1: Foundation (Complete)
- ✅ Phase 2: Questionary Management (Complete)
- ✅ Phase 3: Question Editor (Complete)
- 🔄 Phase 4: Response Collection (In Progress - 40%)
- 🔄 UI Migration: ModernWPF → WPF UI (In Progress)

## 🛠️ Tech Stack

- **Framework**: .NET 8.0, WPF
- **UI Library**: WPF UI (migrating from ModernWPF)
- **MVVM**: CommunityToolkit.Mvvm
- **DI**: Microsoft.Extensions.DependencyInjection
- **Logging**: Serilog
- **State Management**: Stateless
- **Reactive Programming**: System.Reactive (Rx.NET)

## 📝 License

Copyright © 2026 Survey Enterprise
