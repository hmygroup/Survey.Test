
You are an expert WPF + WinUI 3 developer tasked with building a production-ready questionnaire management system.

**PROJECT STATUS**: Phase 6 READY TO START 🚀 - See docs/phases/PHASE6_PLAN.md for complete specifications
- Phase 1: Foundation established with .NET 8.0, ModernWPF, MVVM, DI, Serilog ✅ **COMPLETE**
- Phase 2: Questionary Management UI with CRUD operations, search/filter, graph caching ✅ **COMPLETE**
- Phase 3: Question Editor ✅ **COMPLETE** (100%)
  - ✅ QuestionEditorView with ListView and drag-drop
  - ✅ Question CRUD operations (Create, Delete, Edit)
  - ✅ QuestionDialogWindow for question creation/editing
  - ✅ **ConstraintEditorViewModel and ConstraintEditorView** (Policy selection, parameter management)
  - ✅ **QuestionEditorFactory** (Factory pattern for all 11 question types)
  - ✅ **ReactiveValidationService** (Rx.NET with debouncing, 11 validation rules)
  - ✅ **Command Pattern** (IUndoableCommand, AddQuestionCommand, DeleteQuestionCommand, CommandHistoryManager)
  - ✅ Navigation fixes (Frame.Content issue resolved, HomeView created)
  - ✅ API endpoints corrected per OpenAPI specification
  - ✅ Connection ID: 10001, Port: 5049
  - ✅ All services registered in DI container
- Phase 4: Response Collection ✅ **COMPLETE** (100% complete - See docs/phases/PHASE4_COMPLETE.md)
  - ✅ **AnswerStateMachine** (Stateless library, 4 states, 5 transitions, audit trail)
  - ✅ **ResponseFormView and ViewModel** (Navigation, progress tracking, auto-save)
  - ✅ Type-specific input controls integration (6 specialized templates)
  - ✅ **Session Checkpoint System** (SessionManager, auto-save every 30s, DPAPI encryption)
  - ✅ Recovery Dialog (unfinished session detection on startup)
  - ✅ Enhanced metadata (time tracking, device info)
  - ✅ Submission enhancements (validation, confirmation, success page)
- Phase 5: Response Analysis ⏸️ **BLOCKED** (Backend APIs required)
- Phase 6: Polish & Optimization 🎯 **CURRENT FOCUS** - Production readiness
- Build verified (0 errors, 0 warnings)
- All core infrastructure in place
- Ready to begin Phase 6: Polish, Testing, Accessibility, Documentation & Deployment

**⚠️ BACKEND API BLOCKERS**: Some features implemented in frontend **cannot persist data** due to missing backend APIs:
- ❌ Question Edit (PUT) - Changes are local only
- ❌ Bulk Question Reorder (PATCH) - Order changes not saved
- ❌ Constraint CRUD - Cannot save/update/delete constraints
- ❌ Answer Retrieval by Questionary - Phase 5 blocked
- ❌ Response Analysis Endpoints - Phase 5 blocked
- 📄 **See BACKEND_API_REQUIREMENTS.md** for complete list of missing APIs and specifications

**🎨 UI MODERNIZATION PLAN**: Parallel UI/UX overhaul in progress - **SEPARATE from functionality**:
- 🔄 Migrating from ModernWPF to WPF UI (https://wpfui.lepo.co/)
- ✨ Modern Fluent Design 2.0 aesthetics (Windows 11 native)
- 📊 Improved usability with card layouts, breadcrumbs, InfoBars
- 🚫 **CRITICAL**: UI changes are isolated - NO business logic modifications
- 📄 **See UI_MIGRATION_PLAN.md** for complete migration strategy and timeline
- ⏱️ Estimated: 2-3 weeks (runs parallel to backend API implementation)

CRITICAL: You MUST read and strictly follow the complete technical documentation located at:
\FRONTEND_TECHNICAL_DOCUMENTATION.md

This file contains ALL specifications including API endpoints, data models, UI/UX rules, validation patterns, and architectural decisions. Refer to it constantly.

PHASE COMPLETION STATUS:
- Phase 1: Foundation ✅ **COMPLETE** (See PHASE1_COMPLETION.md)
- Phase 2: Questionary Management ✅ **COMPLETE** (See PHASE2_COMPLETION.md)
- Phase 3: Question Editor ✅ **COMPLETE** (See PHASE3_IMPLEMENTATION_SUMMARY.md & CONSTRAINT_EDITOR_IMPLEMENTATION.md)
  - Core infrastructure: ✅ Complete
  - Drag-drop reordering: ✅ Complete
  - Question CRUD: ✅ Complete (Create, Delete, Edit)
  - Navigation: ✅ Complete
  - API Integration: ✅ Complete
  - Constraint Editor: ✅ Complete (ConstraintEditorViewModel, ConstraintEditorView, policy selection, parameter management)
  - Question Type Factory: ✅ Complete (QuestionEditorFactory for 11 types: Text, Boolean, Integer, Decimal, Date, Email, Phone, Rating, SingleChoice, MultipleChoice, FileUpload)
  - Reactive Validation: ✅ Complete (ReactiveValidationService with Rx.NET, debouncing, 11 validation rules)
  - Undo/Redo: ✅ Complete (Command pattern: IUndoableCommand, AddQuestionCommand, DeleteQuestionCommand, CommandHistoryManager with graph-based history)
- Phase 4: Response Collection ✅ **COMPLETE** (100% complete - See docs/phases/PHASE4_COMPLETE.md)
  - Answer State Machine: ✅ Complete (Stateless library, 4 states, audit trail)
  - Response Form: ✅ Complete (Navigation, progress tracking, auto-save)
  - Type-Specific Controls: ✅ Complete (6 specialized input templates)
  - Session Checkpoints: ✅ Complete (SessionManager, auto-save, DPAPI encryption)
  - Recovery Dialog: ✅ Complete (startup session detection, 3 recovery options)
  - Enhanced Features: ✅ Complete (metadata, validation, confirmation)
- Phase 5: Response Analysis ⏸️ **BLOCKED** (Waiting for backend APIs - see BACKEND_API_REQUIREMENTS.md)
- Phase 6: Polish & Optimization 🎯 **CURRENT FOCUS** (See docs/phases/PHASE6_PLAN.md)

---

## 🎯 PHASE 6: POLISH & OPTIMIZATION - NEXT TASK

**Complete Documentation:** See [docs/phases/PHASE6_PLAN.md](docs/phases/PHASE6_PLAN.md) for comprehensive specifications.

**Objective:** Prepare the application for production deployment with performance optimization, accessibility compliance, comprehensive testing, documentation, and deployment packaging.

**Duration:** 2-3 weeks

### Priority Tasks for Phase 6:

#### 1. Performance Optimization (Week 1, Days 1-2)
- [ ] Implement UI Virtualization for large lists (VirtualizingStackPanel)
- [ ] Add lazy loading for question details
- [ ] Optimize GraphCacheService with LRU eviction
- [ ] Audit all async operations for ConfigureAwait(false)
- [ ] Add cancellation tokens to long-running operations

#### 2. Unit & Integration Testing (Week 1, Days 3-5)
- [ ] Unit tests for all ViewModels (target: 80% coverage)
- [ ] Unit tests for all Services
- [ ] Integration tests using WireMock.Net for API mocking
- [ ] Test error handling scenarios (404, 500, timeout)
- [ ] Setup Coverlet for code coverage reporting

#### 3. Accessibility Compliance (Week 2, Days 1-2)
- [ ] WCAG 2.1 AAA compliance audit
- [ ] Full keyboard navigation support
- [ ] Screen reader compatibility (AutomationProperties)
- [ ] Color contrast ratio ≥ 7:1 for text
- [ ] High contrast mode support
- [ ] Test with Narrator and Windows Accessibility Insights

#### 4. Documentation (Week 2, Days 3-4)
- [ ] User Guide with screenshots (Getting Started, Features, Troubleshooting)
- [ ] Developer Documentation (Architecture, API integration, Extending)
- [ ] API documentation refinements
- [ ] Create FAQ section
- [ ] In-app help system

#### 5. Deployment Packaging (Week 3, Days 1-2)
- [ ] Create MSIX package for Microsoft Store
- [ ] Setup ClickOnce deployment (alternative)
- [ ] Configure auto-update mechanism
- [ ] Code signing certificate setup
- [ ] Environment-specific configurations (Dev/Staging/Production)
- [ ] Test installation/uninstallation flows

#### 6. Telemetry & Monitoring (Week 3, Days 3-4)
- [ ] Integrate Application Insights (Microsoft.ApplicationInsights.WindowsDesktop)
- [ ] Track key events (Questionary Created, Survey Submitted, Errors)
- [ ] Create Application Insights dashboard
- [ ] Custom analytics (DAU/WAU/MAU, feature usage)
- [ ] Performance monitoring (page load time, API response time)

#### 7. Security Audit & Final QA (Week 3, Day 5)
- [ ] Security audit (input sanitization, HTTPS enforcement)
- [ ] Dependency vulnerability scan
- [ ] Final manual testing checklist
- [ ] Performance testing (10,000+ questions, memory leak test)
- [ ] Cross-version testing (Windows 10/11, different DPI)

### Success Metrics:
- ✅ < 3s application startup time
- ✅ < 500ms page navigation
- ✅ 80%+ test coverage
- ✅ WCAG 2.1 AAA compliance
- ✅ 0 critical security vulnerabilities
- ✅ < 200 MB memory for 1000 questionnaires

### Reference Documentation:
- **Complete Phase 6 Plan:** [docs/phases/PHASE6_PLAN.md](docs/phases/PHASE6_PLAN.md)
- **Frontend Technical Docs:** [docs/frontend/FRONTEND_TECHNICAL_DOCUMENTATION.md](docs/frontend/FRONTEND_TECHNICAL_DOCUMENTATION.md)
- **Backend API Requirements:** [docs/api/BACKEND_API_REQUIREMENTS.md](docs/api/BACKEND_API_REQUIREMENTS.md)
- **Phase 4 Complete Summary:** [docs/phases/PHASE4_COMPLETE.md](docs/phases/PHASE4_COMPLETE.md)

---

ADVANCED REQUIREMENTS - Use Latest Techniques:

1. GRAPH-BASED STATE MANAGEMENT
   - Implement a state machine using a directed graph for Answer status transitions
   - States: UNFINISHED → PENDING → COMPLETED (with CANCELLED as exit state)
   - Use Stateless library (NuGet: Stateless) to manage state transitions
   - Track state history with timestamps for auditing
   - Validate transitions before executing (prevent invalid state changes)
   - Example:
     ```csharp
     var answerStateMachine = new StateMachine<AnswerState, AnswerTrigger>(AnswerState.Unfinished);
     answerStateMachine.Configure(AnswerState.Unfinished)
         .Permit(AnswerTrigger.Complete, AnswerState.Pending)
         .Permit(AnswerTrigger.Cancel, AnswerState.Cancelled);
     answerStateMachine.Configure(AnswerState.Pending)
         .Permit(AnswerTrigger.Approve, AnswerState.Completed)
         .Permit(AnswerTrigger.Reject, AnswerState.Unfinished);
     ```

2. SESSION MANAGEMENT WITH CHECKPOINTS
   - Implement Mark Checkpointing pattern for auto-save and recovery
   - Create SessionManager that saves state snapshots every 30 seconds or on specific triggers
   - Store session data locally using System.Text.Json with encryption (System.Security.Cryptography)
   - Session structure:
     ```csharp
     public class SessionCheckpoint
     {
         public Guid CheckpointId { get; set; }
         public DateTime Timestamp { get; set; }
         public Guid AnswerId { get; set; }
         public Dictionary<Guid, string> QuestionResponses { get; set; }
         public int ProgressPercentage { get; set; }
         public string UserAgent { get; set; }
         public byte[] Hash { get; set; } // For integrity verification
     }
     ```
   - Implement recovery dialog on app restart if unfinished sessions exist
   - Allow user to "Continue where I left off" or "Start fresh"

3. HISTORY TRACKING WITH TEMPORAL GRAPH
   - Use temporal graph pattern to track ALL changes over time
   - Every edit to a questionnaire creates a new version node
   - Link versions with timestamps and user information
   - Enable "View history" feature showing timeline of changes
   - Implement diff viewer to compare versions side-by-side
   - Structure:
     ```csharp
     public class QuestionaryVersion
     {
         public Guid VersionId { get; set; }
         public Guid QuestionaryId { get; set; }
         public int VersionNumber { get; set; }
         public DateTime CreatedAt { get; set; }
         public string CreatedBy { get; set; }
         public string ChangeDescription { get; set; }
         public JObject Snapshot { get; set; } // Complete state
         public Guid? PreviousVersionId { get; set; }
         public List<ChangeOperation> Changes { get; set; }
     }
     ```

4. INTELLIGENT CACHING WITH GRAPH INVALIDATION
   - Implement cache dependency graph using QuickGraph library
   - When Questionary changes, invalidate all dependent cached items (Questions, Constraints, etc.)
   - Use IMemoryCache with custom eviction policies based on relationships
   - Cache structure:
     ```csharp
     public class GraphCache<TKey, TValue>
     {
         private readonly IMemoryCache _cache;
         private readonly BidirectionalGraph<CacheNode, CacheEdge> _dependencyGraph;
         
         public void InvalidateNode(TKey key)
         {
             // Find all descendants in graph and evict them
             var descendants = GetAllDescendants(key);
             foreach (var descendant in descendants)
                 _cache.Remove(descendant);
         }
     }
     ```

5. REAL-TIME VALIDATION WITH REACTIVE EXTENSIONS
   - Use System.Reactive (Rx.NET) for reactive validation
   - Debounce user input (500ms) before triggering validation
   - Throttle API calls to prevent rate limiting
   - Combine multiple validation streams
   - Example:
     ```csharp
     Observable
         .FromEventPattern<TextChangedEventArgs>(questionTextBox, nameof(TextBox.TextChanged))
         .Throttle(TimeSpan.FromMilliseconds(500))
         .Select(e => questionTextBox.Text)
         .DistinctUntilChanged()
         .ObserveOn(SynchronizationContext.Current)
         .Subscribe(async text => await ValidateQuestionTextAsync(text));
     ```

6. UNDO/REDO WITH COMMAND PATTERN GRAPH
   - Implement Command pattern with graph-based history
   - Each command forms a node in the undo graph
   - Support branching: if user undoes then makes new change, create branch
   - Visualize undo history as tree structure
   - Commands to implement:
     ```csharp
     public interface ICommand
     {
         Guid CommandId { get; }
         Guid? ParentCommandId { get; }
         DateTime ExecutedAt { get; }
         Task ExecuteAsync();
         Task UndoAsync();
         Task RedoAsync();
         string Description { get; }
     }
     
     // Concrete commands: AddQuestionCommand, DeleteQuestionCommand,
     // ModifyQuestionCommand, ReorderQuestionsCommand, etc.
     ```

7. PROGRESSIVE WEB ASSEMBLY (OPTIONAL ENHANCEMENT)
   - Consider creating a Blazor WebAssembly twin that shares the same business logic
   - Use shared .NET Standard libraries for Models, Services, Validation
   - This allows web access while maintaining WPF for desktop power users
   - Share 80% of codebase between WPF and Blazor

8. TELEMETRY AND ANALYTICS GRAPH
   - Track user interactions as event graph
   - Events: QuestionaryOpened, QuestionAdded, ConstraintApplied, ResponseStarted, ResponseCompleted
   - Build adjacency list to analyze common user flows
   - Use this data to optimize UI/UX (show most-used features prominently)
   - Example:
     ```csharp
     public class TelemetryGraph
     {
         private readonly DirectedGraph<UserAction, ActionTransition> _flowGraph;
         
         public void RecordAction(UserAction action)
         {
             if (_lastAction != null)
                 _flowGraph.AddEdge(new ActionTransition(_lastAction, action));
             _lastAction = action;
         }
         
         public List<ActionPath> GetMostCommonPaths(int topN = 10)
         {
             // Analyze graph to find most frequent paths
         }
     }
     ```

9. CONFLICT RESOLUTION FOR CONCURRENT EDITS
   - Implement Operational Transformation (OT) or CRDT-like conflict resolution
   - If two users edit the same questionary simultaneously, merge changes intelligently
   - Use vector clocks or Lamport timestamps to order events
   - Show conflict resolution UI when automatic merge isn't possible
   - Example:
     ```csharp
     public class ConflictResolver
     {
         public QuestionaryDto Merge(QuestionaryDto local, QuestionaryDto remote, QuestionaryDto commonAncestor)
         {
             var merged = commonAncestor.Clone();
             
             // Three-way merge algorithm
             // 1. Apply local changes that don't conflict
             // 2. Apply remote changes that don't conflict
             // 3. For conflicts, present to user or use heuristics (last-write-wins, etc.)
             
             return merged;
         }
     }
     ```

10. PERFORMANCE OPTIMIZATIONS
    - Virtualize long lists (ListView.ItemsPanel with VirtualizingStackPanel)
    - Lazy load questions (only load visible + 20 buffer)
    - Use DataTemplate pooling for repeated items
    - Implement pagination for large response sets (100 per page)
    - Background thread for serialization/deserialization
    - Use Span<T> and Memory<T> for high-performance string operations

ARCHITECTURAL PATTERNS TO IMPLEMENT:

✅ Repository Pattern with Unit of Work
✅ CQRS on client side (separate read/write models if complex)
✅ Event Sourcing for audit trail (store all events, rebuild state)
✅ Mediator Pattern for loose coupling (use MediatR on client too if complex)
✅ Specification Pattern for complex validation rules
✅ Builder Pattern for constructing complex DTOs
✅ Factory Pattern for creating UI controls based on QuestionType
✅ Strategy Pattern for different validation strategies
✅ Observer Pattern for real-time updates (SignalR integration future)
✅ Memento Pattern for undo/redo state capture

TESTING REQUIREMENTS:

- Unit tests for all ViewModels (xUnit + Moq)
- Integration tests for API services (WireMock for mocking API)
- UI automation tests (Appium or WinAppDriver)
- Performance tests (BenchmarkDotNet for critical paths)
- Test coverage minimum: 80% for business logic
- Snapshot testing for complex DTOs

CODE QUALITY STANDARDS:

- Enable nullable reference types in csproj (<Nullable>enable</Nullable>)
- Use C# 10+ features: record types, global usings, file-scoped namespaces
- Async all the way (no .Result or .Wait())
- Use ValueTask<T> for hot paths
- Implement IDisposable/IAsyncDisposable properly
- Use ConfigureAwait(false) in library code
- Follow SOLID principles strictly
- Cyclomatic complexity max: 10 per method
- Lines per method max: 50
- Use Code Analyzers: StyleCop, Roslynator, SonarLint

SECURITY REQUIREMENTS:

- Never store connection parameter in plain text (use Windows Credential Manager)
- Encrypt session checkpoints with DPAPI (Data Protection API)
- Sanitize all user input before API calls (prevent injection)
- Validate all API responses (don't trust backend completely)
- Implement rate limiting on client side (max 10 requests/second)
- Use HTTPS only for API calls
- Implement certificate pinning if possible

ACCESSIBILITY (WCAG 2.1 AAA):

- All interactive elements have AutomationProperties.Name
- Keyboard navigation fully functional (Tab, Shift+Tab, Arrow keys)
- Focus indicators clearly visible (2px blue border)
- Color contrast ratio minimum 7:1 for text
- Support screen readers (Narrator, JAWS, NVDA)
- All images have alt text
- Forms have proper labels and error associations
- Support high contrast mode
- Font sizes scalable (respect system DPI settings)

DOCUMENTATION TO GENERATE:

- XML documentation for all public APIs (100% coverage)
- README.md with setup instructions
- ARCHITECTURE.md explaining design decisions
- API_INTEGRATION.md documenting endpoint usage
- USER_GUIDE.md with screenshots
- DEPLOYMENT.md with packaging instructions
- CHANGELOG.md following Keep a Changelog format

DELIVERABLES - PHASE BY PHASE:

Phase 1: Foundation (Week 1) ✅ **COMPLETED**
- [x] Project structure with all folders
- [x] DI container setup in App.xaml.cs
- [x] ApiService base implementation with HttpClientFactory
- [x] MainWindow with NavigationView skeleton
- [x] Light/Dark theme switching
- [x] Logging infrastructure with Serilog
- [x] All DTOs matching API specification (9 DTOs)
- [x] All API services (Questionary, Question, Answer, QuestionResponse)
- [x] Infrastructure services (Navigation, Dialog, Theme)
- [x] ModernWPF integration with Windows 11 styling
- [x] URL encoding for security
- [x] XML documentation (100% coverage)
- [x] Build verified (0 errors, 0 warnings)
- [x] CodeQL security scan passed
- [x] Complete documentation (README, ARCHITECTURE, API_INTEGRATION)

**Status**: Production-ready foundation. See PHASE1_COMPLETION.md for full details.

Phase 2: Questionary Management (Week 2) ✅ **COMPLETE**
- [x] QuestionaryListView with DataGrid
- [x] Search and filter functionality
- [x] Create/Edit/Delete questionary dialogs
- [x] GraphCache implementation for questionaries
- [x] NullToBooleanConverter for UI bindings
- [x] Service registration in DI container
- [x] All CRUD operations working
- [x] Cache invalidation on updates
- [x] Build verified (0 errors, 0 warnings)
- [x] Complete documentation

**Status**: Production-ready. See PHASE2_COMPLETION.md for full details.

Phase 3: Question Editor (Week 3-4) ✅ **COMPLETE**
- [x] Question list with drag-and-drop reordering
- [x] QuestionEditorView with ListView and drag handlers
- [x] QuestionEditorViewModel with state management
- [x] Add question UI with validation
- [x] Edit question UI with validation
- [x] Delete question with confirmation
- [x] QuestionDialogWindow for Create/Edit dialogs
- [x] Navigation from QuestionaryListView (View Details button)
- [x] Navigation fixes (Frame.Content removed, HomeView created)
- [x] API endpoints corrected per OpenAPI 3.0 specification
- [x] Connection ID updated to 10001
- [x] Base URL updated to http://localhost:5049/api/
- [x] CountToVisibilityConverter for UI bindings
- [x] QuestionTypeService for managing question types
- [x] PolicyService for managing policies
- [x] **ConstraintEditorViewModel and ConstraintEditorView** (policy selection, parameter management)
- [x] **QuestionEditorFactory** (Factory pattern for 11 question types)
- [x] **ReactiveValidationService** (Rx.NET with 11 validation rules, debouncing)
- [x] **Command Pattern for Undo/Redo** (IUndoableCommand, CommandHistoryManager, graph-based history)
- [x] All documentation updated with correct endpoints
- [x] Build verified (0 errors, 0 warnings)
- [x] Security scan passed (CodeQL)

**Status**: ✅ **COMPLETE** - All features implemented and documented.  
**Documentation**: See PHASE3_IMPLEMENTATION_SUMMARY.md and CONSTRAINT_EDITOR_IMPLEMENTATION.md.

Phase 4: Response Collection (Week 5) 🔄 **IN PROGRESS (40% COMPLETE)**
- [x] **AnswerStateMachine** - State machine with Stateless library (4 states, 5 transitions, audit trail)
- [x] **ResponseFormView and ViewModel** - Question-by-question navigation, progress tracking
- [x] **Auto-save on navigation** - Saves responses automatically
- [x] **Progress indicators** - Progress bar, question counter, completion percentage
- [x] **Submit functionality** - State transition UNFINISHED → PENDING
- [ ] **Form renderer with type-specific controls** - Integrate QuestionEditorFactory **← CURRENT TASK**
- [ ] **Session Checkpoint System** - SessionManager, auto-save every 30s, DPAPI encryption **← NEXT PRIORITY**

Phase 5: Response Analysis (Week 6)
- [ ] Response list with filtering
- [ ] Response detail view
- [ ] Basic statistics (charts with LiveCharts2)
- [ ] Export to CSV/Excel (EPPlus library)
- [ ] Conflict resolution UI

Phase 6: Polish & Optimization (Week 7-8)
- [ ] Performance optimization (virtualization, lazy loading)
- [ ] Accessibility audit and fixes, 2 & 3):
✅ Project structure (Models, ViewModels, Services folders)
✅ Dependency Injection with Microsoft.Extensions.DependencyInjection
✅ All 9 DTOs (QuestionaryDto, QuestionDto, AnswerDto, etc.)
✅ ApiService base class with HttpClientFactory
✅ QuestionaryService (GetAll, GetById, GetFull, Create, Delete)
✅ QuestionService (GetByQuestionaryIdAsync, CreateAsync, DeleteAsync)
✅ QuestionTypeService (GetAllAsync, GetByIdAsync, AddAsync)
✅ PolicyService (GetAllAsync)
✅ AnswerService (Create, GetById, SetStatus, GetByQuestionaryId)
✅ QuestionResponseService (SaveResponses, UpdateResponse, DeleteResponse, GetByAnswerId)
✅ NavigationService with page instance navigation
✅ DialogService, ThemeService
✅ GraphCacheService with dependency tracking and cascade invalidation
✅ MainWindow with NavigationView (ModernWPF)
✅ HomeView for welcome screen
✅ QuestionaryListView with DataGrid, search/filter, CRUD operations
✅ QuestionaryDialogWindow for Create/Edit dialogs
✅ QuestionEditorView with ListView, drag-drop reordering, question management
✅ QuestionDialogWindow for Add/Edit questions with validation
✅ NullToBooleanConverter, CountToVisibilityConverter for UI bindings
✅ Light/Dark theme toggle with persistence
✅ Serilog logging (file + console)
✅ URL encoding for security
✅ API endpoints corrected per OpenAPI 3.0 specification
✅ Connection ID: 10001
✅ Base URL: http://localhost:5049/a - Remaining 30%):
1. ~~Question list view with drag-and-drop reordering~~ ✅ DONE
2. ~~Add/Delete question UI with validation~~ ✅ DONE
3. Edit question UI (pending backend API support for UPDATE endpoint)
4. Constraint editor with Policy selection - **NEXT PRIORITY**
   - Create ConstraintEditorView UserControl
   - Integrate with QuestionDialogWindow
   - Policy selection from PolicyService
   - PolicyRecords management (key-value pairs)
   - Visual policy builder (no JSON editing)
5. QuestionType selector with Factory pattern
   - Factory creates type-specific editor controls
   - TextQuestionEditor, RatingQuestionEditor, DateQuestionEditor, etc.
   - Type-specific validation rules
   - Preview renderer
6. Real-time validation with Rx.NET
   - Debounced input validation (500ms)
   - Combined local + remote validation
   - Inline validation messages
7. Undo/Redo with Command pattern graph
   - AddQuestionCommand, DeleteQuestionCommand, ModifyQuestionCommand
   - Command history with branching support
   - Keyboard shortcuts (Ctrl+Z, Ctrl+Y)
8. Live preview pane showing question as it will appear to respondents
9. Version history viewer (temporal graph pattern)
10. Unit tests for new components
11. Performance optimization (virtualization for large question lists)

**RECENT FIXES COMPLETED**:
✅ Navigation issue resolved (Frame.Content was blocking navigation)
✅ HomeView created for welcome screen
✅ AutoSuggestBox style error fixed (removed invalid DefaultAutoSuggestBoxStyle reference)
✅ API endpoints corrected to match OpenAPI specification (Questionary, Question, Answer, QuestionResponse)
✅ Connection ID updated to 10001
✅ Base URL updated to port 5049
✅ QuestionService endpoint corrected (GET Question/{ConnectionId}/get?questionaryId={id})
✅ AnswerService.SetStatusAsync changed from POST to PUT
✅ QuestionResponseService parameters updated (QuestionResponseID, newValue, metadata)
✅ QuestionTypeService and PolicyService created and registered
✅ All documentation updated with correct API informationases 1 & 2):
✅ Project structure (Models, ViewModels, Services folders)
✅ Dependency Injection with Microsoft.Extensions.DependencyInjection
✅ All 9 DTOs (QuestionaryDto, QuestionDto, AnswerDto, etc.)
✅ ApiService base class with HttpClientFactory
✅ QuestionaryService (GetAll, GetById, GetFull, Create, Delete)
✅ QuestionService (GetByQuestionaryId, Create, Delete)
✅ AnswerService (Create, SetStatus, GetByQuestionaryId)
✅ QuestionResponseService (SaveResponses, UpdateResponse, GetByAnswerId)
✅ NavigationService, DialogService, ThemeService
✅ GraphCacheService with dependency tracking and cascade invalidation
✅ MainWindow with NavigationView (ModernWPF)
✅ QuestionaryListView with DataGrid, search/filter, CRUD operations
✅ QuestionaryDialogWindow for Create/Edit dialogs
✅ NullToBooleanConverter for UI bindings
✅ Light/Dark theme toggle with persistence
✅ Serilog logging (file + console)
✅ URL encoding for security
✅ Complete documentation

**WHAT'S NEXT** (Phase 3 priorities):
1. Question list view with drag-and-drop reordering
2. Add/Edit/Delete question UI with QuestionType selector
3. Constraint editor with Policy selection
4. Real-time validation with Rx.NET
5. Undo/Redo with Command pattern graph
6. Live preview pane
7. Version history viewer
8. Unit tests for new components

Begin Phase 3 by understanding the existing codebase, then continue implementing advanced Question Editor features.

---

## 🔧 RECENT SESSION UPDATES (January 28, 2026)

### Navigation & Infrastructure Fixes ✅
- **Fixed Frame Navigation**: Removed hardcoded Frame.Content in MainWindow.xaml that was blocking navigation
- **Created HomeView**: New page for welcome screen (previously embedded in Frame.Content)
- **Navigation Flow Working**: Home → Questionnaires → Question Editor → Back navigation all functional

### API Configuration Updates ✅
- **Port Changed**: Updated from 5030 to **5049** across all services
- **Connection ID**: Updated from 1 to **10001** in ApiService
- **Base URL**: All HttpClient configurations now use `http://localhost:5049/api/`

### API Endpoint Corrections ✅ (Per OpenAPI 3.0 Specification)

**QuestionaryService**:
- `GET Questionary/10001/all` (was: questionary/{connection}/all)
- `GET Questionary/10001/{id}`
- `GET Questionary/10001/{id}/full`
- `POST Questionary/10001/New/{name}`
- `DELETE Questionary/10001/{id}` (not in API spec - may not work)

**QuestionService**:
- `GET Question/10001/get?questionaryId={id}` (was: Question/{connection}/questionary/{id})
- `POST Question/new/10001?questionaryId={id}`
- `DELETE Question/10001/{id}` (not in API spec - may not work)

**AnswerService**:
- `POST Answer/10001?questionaryId={id}&user={email}&cardId={cardId}`
- `PUT Answer/setStatus` (changed from POST to PUT)
- `GET Answer/10001/{id}` (newly added)
- `GET Answer/10001/questionary/{questionaryId}` (not in API spec - may not work)

**QuestionResponseService**:
- `POST QuestionResponse/10001/response`
- `PATCH QuestionResponse/10001/response?QuestionResponseID={id}&newValue={text}&metadata={data}`
- `DELETE QuestionResponse/10001/response/{questionResponseId}` (newly added)
- Parameters updated: `QuestionResponseID` (not questionResponseId), `newValue` (not response)

### New Services Created ✅
- **QuestionTypeService**: GET all types, GET by ID, POST add new type
- **PolicyService**: GET all policies

### XAML Fixes ✅
- **AutoSuggestBox Error**: Removed invalid `DefaultAutoSuggestBoxStyle` reference in QuestionaryListView
- Direct property setting instead of custom style

### Documentation Updates ✅
- API_INTEGRATION.md: All endpoints updated with correct URLs and parameters
- README.md: Updated base URL and connection ID
- ARCHITECTURE.md: Updated API configuration examples
- FRONTEND_TECHNICAL_DOCUMENTATION.md: Updated API specifications
- PHASE1_COMPLETION.md: Updated API references

### Services Registration ✅
All new services properly registered in App.xaml.cs DI container:
```csharp
services.AddHttpClient<QuestionTypeService>(...);
services.AddHttpClient<PolicyService>(...);
```

### Known Limitations (Documented)
- Edit Question: Changes are local only (UPDATE endpoint not available in API)
- Delete Question: Endpoint not documented (may fail)
- Question Order Persistence: Reorder works in UI but doesn't persist (bulk update endpoint needed)
- Some GET endpoints not in API spec (fallback behavior implemented)

### Build Status ✅
- **Build**: SUCCESS (0 errors, 0 warnings)
- **Runtime**: All navigation working, API calls succeeding
- **Endpoints**: Verified against OpenAPI specification
```

---

## ✅ COMPLETED WORK - API Documentation Update (January 2026)

### Summary of Completed Tasks

#### ✅ API Documentation Consolidation
A comprehensive API documentation file has been created at `/API_DOCUMENTATION.md` that includes:

1. **Complete Endpoint Documentation** - All API endpoints documented with:
   - Consistent Connection ID (10001) across all endpoints
   - Full request/response examples with JSON payloads
   - HTTP method, URL, parameters clearly specified
   - cURL examples for testing each endpoint

2. **Organized by Resource Type**:
   - ✅ Questionary Endpoints (5 endpoints)
     - Get all questionnaires
     - Get by ID
     - Get by name
     - Get full questionary with questions
     - Create new questionary
   
   - ✅ Question Endpoints (3 endpoints)
     - Get questions by questionary ID
     - Create questions
     - Get questions with specific responses
   
   - ✅ Answer Endpoints (3 endpoints)
     - Create answer session
     - Get answer by ID
     - Set answer status
   
   - ✅ Question Response Endpoints (3 endpoints)
     - Save question responses
     - Update question response
     - Delete question response
   
   - ✅ Question Type Endpoints (3 endpoints)
     - Get all question types
     - Get question type by ID
     - Add question type
   
   - ✅ Policy Endpoints (3 endpoints)
     - Get all policies
     - Get policy by ID
     - Create policy
   
   - ✅ Attachment Endpoints (2 endpoints)
     - Create attachment
     - Get attachment by ID

3. **Complete Workflow Examples**:
   - ✅ Creating a new survey (3 steps)
   - ✅ User filling out a survey (4 steps)
   - ✅ Reviewing survey responses (2 steps)

4. **Error Handling Documentation**:
   - ✅ Common HTTP status codes
   - ✅ Error response format
   - ✅ Best practices for error handling

### What This Accomplishes

- **Consistency**: All endpoints now explicitly use Connection ID 10001
- **Completeness**: Every endpoint is documented with full examples
- **Usability**: Developers can copy-paste cURL examples for testing
- **Clarity**: Clear workflow examples show how to use endpoints together
- **Maintainability**: Single source of truth for API documentation

### Files Modified/Created

- ✅ **CREATED**: `/API_DOCUMENTATION.md` - Complete API reference with all endpoints using Connection ID 10001
- ✅ **UPDATED**: `/prompt.md` - Added this completion summary

---

## 📋 REMAINING WORK

### Phase 4: Response Collection 🔄 **CURRENT PRIORITY** (40% Complete)

**✅ COMPLETED:**
- Answer State Machine (Stateless library, audit trail, 4 states, 5 transitions)
- Response Form View/ViewModel (navigation, progress tracking, auto-save)
- Basic response capture with TextBox

**⏳ REMAINING (60%):**

#### Priority 1: Session Checkpoint System **← NEXT TASK**
- [ ] **SessionCheckpoint Model** - Data structure for auto-save checkpoints
- [ ] **SessionManager Service** - Auto-save logic, encryption, storage
- [ ] **Auto-Save Timer** - Save every 30 seconds automatically
- [ ] **DPAPI Encryption** - Encrypt checkpoints with Windows Data Protection API
- [ ] **Local Storage** - Store in %APPDATA%\SurveyApp\Sessions\
- [ ] **Checkpoint Cleanup** - Delete checkpoints older than 7 days

#### Priority 2: Type-Specific Input Controls
- [ ] **Boolean Questions** - Checkbox/Toggle (factory created, needs integration)
- [ ] **Integer/Decimal Questions** - Numeric input with validation
- [ ] **Date Questions** - DatePicker control
- [ ] **Rating Questions** - Slider/Stars control
- [ ] **Single/Multiple Choice** - Radio buttons/Checkboxes
- [ ] **File Upload Questions** - File selection dialog
- [ ] **Email/Phone Questions** - Specialized TextBox with validation
- [ ] Integrate QuestionEditorFactory with ResponseFormView

#### Priority 3: Recovery Dialog
- [ ] **Recovery UI** - Dialog shown on app startup
- [ ] **Session Detection** - Check for unfinished sessions
- [ ] **Options** - Continue, Start Fresh, Discard
- [ ] **Session Restoration** - Load saved responses and state

#### Priority 4: Enhanced Features
- [ ] **Metadata Collection** - Time per question, device info, interaction events
- [ ] **Pre-Submit Validation** - Ensure required fields completed
- [ ] **Confirmation Dialog** - "Are you sure?" before submission
- [ ] **Success Page** - Thank you message after submission
- [ ] **Error Retry** - Handle network failures gracefully

### Phase 5: Response Analysis (Not Started)
- [ ] Response list with filtering
- [ ] Response detail view
- [ ] Basic statistics (charts with LiveCharts2)
- [ ] Export to CSV/Excel (EPPlus library)
- [ ] Conflict resolution UI

### Phase 6: Polish & Optimization (Not Started)
- [ ] Performance optimization (virtualization, lazy loading)
- [ ] Accessibility audit and fixes (WCAG 2.1 AAA compliance)
- [ ] Complete unit test coverage (80%+ target)
- [ ] Integration tests with WireMock
- [ ] User documentation and guides
- [ ] Deployment packaging (MSIX/ClickOnce)
- [ ] Telemetry and analytics

---

## 📝 Additional Copilot Chat Prompts for Specific Features

### ✅ PHASE 4: Response Collection (COMPLETE)

Phase 4 has been successfully completed. See [docs/phases/PHASE4_COMPLETE.md](docs/phases/PHASE4_COMPLETE.md) for implementation details.

All features implemented:
- ✅ Answer State Machine with Stateless library
- ✅ Response Form with navigation and progress tracking
- ✅ Type-specific input controls (6 templates)
- ✅ Session Checkpoint System with DPAPI encryption
- ✅ Recovery Dialog for unfinished sessions
- ✅ Enhanced metadata collection and tracking
- ✅ Submission enhancements with validation

---

### 🎯 PHASE 6: Polish & Optimization (CURRENT FOCUS)

**Complete specifications:** See [docs/phases/PHASE6_PLAN.md](docs/phases/PHASE6_PLAN.md)

#### For Performance Optimization (Week 1, Priority 1)
```
@workspace Implement performance optimizations for the WPF questionnaire application.

Requirements from docs/phases/PHASE6_PLAN.md:
1. UI Virtualization
   - Add VirtualizingStackPanel to QuestionaryListView DataGrid
   - Enable UI virtualization for QuestionEditorView question lists
   - Implement recycling mode for better memory usage
   - Add paging for large result sets (100 items per page)

2. Lazy Loading
   - Implement lazy loading for questionary questions (load on demand)
   - Defer loading of response statistics until requested
   - Background loading with progress indicators
   - Cache loaded data to prevent redundant API calls

3. Graph Cache Optimization
   - Add LRU (Least Recently Used) eviction policy to GraphCacheService
   - Set cache size limit (max 1000 nodes)
   - Add telemetry for cache hit/miss rates
   - Optimize dependency graph traversal (use BFS)

4. Async/Await Best Practices
   - Audit all API calls and add ConfigureAwait(false) where appropriate
   - Add CancellationToken support to long-running operations
   - Use Task.WhenAll for parallel operations
   - Replace synchronous file I/O with async alternatives

Success Metrics:
- Application startup < 3 seconds
- Page navigation < 500ms
- Memory usage < 200 MB for 1000 questionnaires
- Smooth scrolling with 10,000+ items

Files to modify:
- Views/QuestionaryListView.xaml (add virtualization)
- Views/QuestionEditorView.xaml (add virtualization)
- Services/GraphCacheService.cs (add LRU eviction)
- All ViewModels (add ConfigureAwait, CancellationToken)
```

#### For Unit & Integration Testing (Week 1, Priority 2)
```
@workspace Create comprehensive test suite for the questionnaire application.

Requirements:
1. Unit Tests (Target: 80% coverage)
   - Test all ViewModels using AAA pattern (Arrange, Act, Assert)
   - Test all Services with mocked dependencies (use Moq or NSubstitute)
   - Test StateMachine transitions
   - Test ValidationService rules
   - Test GraphCacheService invalidation
   - Test Converters (NullToBooleanConverter, etc.)

2. Integration Tests
   - Use WireMock.Net to mock backend APIs
   - Test all API services (QuestionaryService, QuestionService, AnswerService, etc.)
   - Test error handling (404, 500, timeout scenarios)
   - Test retry logic for transient failures
   - Test data serialization/deserialization

3. Code Coverage
   - Install Coverlet NuGet package
   - Configure coverage reporting with ReportGenerator
   - Target: 80%+ line coverage
   - Exclude UI code-behind from coverage metrics

Framework: xUnit or NUnit
Mocking: Moq or NSubstitute
Coverage: Coverlet + ReportGenerator

Files to create:
- SurveyApp.Tests/ViewModels/*.cs
- SurveyApp.Tests/Services/*.cs
- SurveyApp.Tests/Integration/*.cs
- SurveyApp.Tests/SurveyApp.Tests.csproj

Example test structure:
```csharp
[Fact]
public async Task CreateQuestionaryAsync_ValidInput_CreatesSuccessfully()
{
    // Arrange
    var mockService = new Mock<IQuestionaryService>();
    var viewModel = new QuestionaryDialogViewModel(mockService.Object);
    viewModel.Title = "Test Survey";
    
    // Act
    await viewModel.SaveAsync();
    
    // Assert
    mockService.Verify(x => x.CreateAsync(It.IsAny<QuestionaryDto>()), Times.Once);
    Assert.True(viewModel.IsSuccess);
}
```
```

#### For Accessibility Compliance (Week 2, Priority 3)
```
@workspace Implement WCAG 2.1 AAA accessibility compliance for the WPF application.

Requirements from docs/phases/PHASE6_PLAN.md:
1. Keyboard Navigation
   - Full keyboard navigation for all views (Tab, Shift+Tab)
   - Keyboard shortcuts: Ctrl+N (New), Ctrl+S (Save), Ctrl+F (Find), Ctrl+Z/Y (Undo/Redo)
   - Focus indicators visible on all focusable elements (3:1 contrast ratio)
   - Logical tab order for forms
   - Escape key closes dialogs, Enter submits forms

2. Screen Reader Support
   - Add AutomationProperties.Name to all controls
   - Add AutomationProperties.HelpText for complex controls
   - ARIA labels for dynamic content
   - Announce state changes (loading, success, error)
   - Descriptive button labels (not just icons)

3. Visual Accessibility
   - Color contrast ratio ≥ 7:1 for text (AAA standard)
   - Color contrast ratio ≥ 4.5:1 for UI components
   - No information conveyed by color alone (use icons + text)
   - Support 200% zoom without horizontal scrolling
   - High Contrast mode support

4. Text Alternatives
   - Alt text for all images and icons
   - Tooltips for icon-only buttons
   - Descriptive link text (avoid "Click here")
   - Labels for all form inputs

Testing:
- Test with keyboard only (no mouse)
- Test with Windows Narrator screen reader
- Use Microsoft Accessibility Insights tool
- Test in High Contrast mode
- Use Color Contrast Analyzer

Files to modify:
- All XAML views (add AutomationProperties)
- All ViewModels (add descriptive labels)
- App.xaml (ensure proper focus styling)
```

#### For Documentation (Week 2, Priority 4)
```
@workspace Create comprehensive user and developer documentation.

Requirements:
1. User Guide (docs/user-guide/)
   - Getting Started (installation, first launch, configuration)
   - Creating Questionnaries (step-by-step with screenshots)
   - Managing Questions (add, edit, delete, reorder, constraints)
   - Collecting Responses (starting survey, navigation, submission)
   - Session Recovery (handling interrupted surveys)
   - Analyzing Results (when backend APIs available)
   - Troubleshooting common issues
   - FAQ section

2. Developer Documentation (docs/developer-guide/)
   - Architecture Overview (MVVM, DI, services structure)
   - API Integration Guide (endpoints, authentication, error handling)
   - Adding New Question Types (using factory pattern)
   - Adding New Validation Rules (ReactiveValidationService)
   - Extending State Machine (new states, transitions)
   - Caching Strategy (GraphCacheService usage)
   - Testing Guidelines (unit tests, integration tests)
   - Contributing Guidelines (code style, PR process)

3. API Documentation
   - Update OpenAPI/Swagger specifications
   - Add request/response examples for each endpoint
   - Document authentication requirements
   - Document rate limiting rules
   - Document error codes and handling

Format: Markdown with code samples and screenshots
Location: /docs/user-guide/ and /docs/developer-guide/

Include diagrams for:
- Application architecture
- Data flow
- State machine transitions
- Deployment architecture
```

#### For Deployment Packaging (Week 3, Priority 5)
```
@workspace Create deployment packages for production distribution.

Requirements from docs/phases/PHASE6_PLAN.md:
1. MSIX Packaging (Preferred)
   - Create MSIX manifest with app metadata
   - Configure app capabilities (internetClient, localFiles)
   - Add application icons (multiple sizes: 16x16, 32x32, 48x48, 64x64, 128x128, 256x256)
   - Configure auto-update from Microsoft Store
   - Setup code signing certificate
   - Test installation/uninstallation flows

2. ClickOnce Deployment (Alternative)
   - Create ClickOnce publish profile
   - Configure auto-update (check on startup)
   - Setup deployment URL
   - Digital signature for trusted installation

3. Configuration Management
   - Move hardcoded settings to appsettings.json
   - Environment-specific configs (Development, Staging, Production)
   - User settings persistence (theme preference, window size/position)
   - Feature flags for gradual rollout

Example appsettings.json:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://api.production.com",
    "ConnectionId": "10001",
    "TimeoutSeconds": 30
  },
  "Features": {
    "EnableTelemetry": true,
    "EnableAutoSave": true,
    "AutoSaveIntervalSeconds": 30
  },
  "Logging": {
    "MinimumLevel": "Information",
    "RetentionDays": 7
  }
}
```

Files to create:
- Package.appxmanifest (MSIX)
- SurveyApp.Deployment/ project (ClickOnce)
- appsettings.json, appsettings.Development.json, appsettings.Production.json
```

#### For Telemetry & Monitoring (Week 3, Priority 6)
```
@workspace Integrate Application Insights for telemetry and monitoring.

Requirements:
1. Application Insights Setup
   - Install Microsoft.ApplicationInsights.WindowsDesktop NuGet package
   - Configure instrumentation key (from Azure portal)
   - Initialize TelemetryClient in App.xaml.cs
   - Configure telemetry processors for filtering

2. Event Tracking
   - Page views (navigation events)
   - User actions (Questionary Created, Question Added, Survey Submitted, etc.)
   - Exceptions and errors (automatic + manual)
   - Performance metrics (page load time, API response time)
   - Custom metrics (cache hit rate, session recovery rate)

3. Analytics Dashboard
   - Create Application Insights dashboard in Azure portal
   - Key metrics: DAU/WAU/MAU, feature usage, error rate
   - Performance charts: response times, memory usage
   - User journey funnel: questionary creation → question addition → survey submission

Example telemetry code:
```csharp
public class TelemetryService : ITelemetryService
{
    private readonly TelemetryClient _telemetry;

    public TelemetryService(TelemetryConfiguration config)
    {
        _telemetry = new TelemetryClient(config);
    }

    public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
    {
        _telemetry.TrackEvent(eventName, properties);
    }

    public void TrackException(Exception exception)
    {
        _telemetry.TrackException(exception);
    }

    public void TrackMetric(string metricName, double value)
    {
        _telemetry.TrackMetric(metricName, value);
    }
}
```

Events to track:
- Application.Started
- Questionary.Created
- Question.Added / Edited / Deleted
- Survey.Started / Submitted / Recovered
- Error.Occurred (with exception details)

Files to create:
- Services/Telemetry/TelemetryService.cs
- Services/Telemetry/ITelemetryService.cs

Files to modify:
- App.xaml.cs (initialize Application Insights)
- All ViewModels (add telemetry tracking)
```

---

### ARCHIVED: Previous Phase Prompts

#### For Session Checkpoint System (NEXT TASK)
```
@workspace Implement a session checkpoint system for auto-saving user progress when filling out questionnaires.

Requirements from prompt.md:
- Save checkpoint every 30 seconds automatically
- Save on manual trigger (Save Draft button)
- Encrypt checkpoints using Windows DPAPI (System.Security.Cryptography.ProtectedData)
- Store locally in %APPDATA%\SurveyApp\Sessions\
- Include: AnswerId, QuestionResponses (Dictionary<Guid, string>), progress %, timestamp, hash for integrity
- On app restart, check for unfinished sessions and offer recovery
- Recovery dialog with options: Continue, Start Fresh, Discard
- Clear old checkpoints (>7 days) on app start

Create:
1. SessionCheckpoint record/class
2. SessionManager service with IDisposable
3. Auto-save timer (DispatcherTimer, 30s interval)
4. Encryption/Decryption methods using DPAPI
5. File I/O for checkpoint storage
6. Integrity hash verification (SHA256)
7. Unit tests for save/load/recovery scenarios

Files to create:
- Models/SessionCheckpoint.cs
- Services/SessionManagement/SessionManager.cs
- Services/SessionManagement/ISessionManager.cs

Integration points:
- ResponseFormViewModel: Call SessionManager on timer and navigation
- App.xaml.cs: Check for unfinished sessions on startup
```

#### For Recovery Dialog
```
@workspace Create a recovery dialog to restore unfinished survey sessions on app startup.

Requirements:
- Modal window shown before main window if unfinished sessions exist
- Display session info: Questionary name, progress %, last saved timestamp
- Three action buttons: Continue, Start Fresh, Discard
- "Continue" loads checkpoint into ResponseFormView
- "Start Fresh" deletes checkpoint and creates new Answer
- "Discard" deletes checkpoint without creating new session
- Handle multiple unfinished sessions (show list)

Create:
1. Views/Dialogs/SessionRecoveryDialog.xaml
2. Views/Dialogs/SessionRecoveryDialog.xaml.cs
3. ViewModels/SessionRecoveryViewModel.cs

Integration:
- App.xaml.cs: Check SessionManager.GetUnfinishedSessions() before MainWindow.Show()
- Auto-navigate to ResponseFormView on "Continue"
```

#### For Type-Specific Input Controls Integration
```
@workspace Integrate QuestionEditorFactory type-specific controls into ResponseFormView.

Current state:
- QuestionEditorFactory exists at Services/Factories/QuestionEditorFactory.cs
- Factory creates controls for 11 question types
- ResponseFormView currently uses generic TextBox

Requirements:
- Replace static TextBox in ResponseFormView with ContentPresenter
- Bind ContentPresenter to dynamic control from factory
- Use CurrentQuestion.QuestionType to select appropriate control
- Extract response value from different control types
- Update SaveCurrentResponse() to handle all control types
- Maintain auto-save functionality

Modify:
1. ResponseFormView.xaml - Replace TextBox with ContentPresenter
2. ResponseFormViewModel.cs - Add CreateEditorControl() method
3. Add value extraction logic for each control type

Control types to support:
- TextBox (Text, Email, Phone)
- CheckBox (Boolean)
- TextBox with numeric validation (Integer, Decimal)
- DatePicker (Date)
- Slider (Rating)
- RadioButton group (SingleChoice)
- CheckBox group (MultipleChoice)
- File picker button (FileUpload)
```

### 🎓 PHASE 3: Question Editor ✅ **COMPLETED** (Reference Only)

Phase 3 is fully complete. All features implemented:
- ✅ Question list with drag-and-drop reordering
- ✅ Add/Edit/Delete question UI
- ✅ QuestionType selector with Factory pattern (QuestionEditorFactory)
- ✅ Constraint editor with Policy selection (ConstraintEditorView/ViewModel)
- ✅ Real-time validation with Rx.NET (ReactiveValidationService)
- ✅ Undo/Redo with Command pattern graph (CommandHistoryManager, IUndoableCommand)

See PHASE3_IMPLEMENTATION_SUMMARY.md and CONSTRAINT_EDITOR_IMPLEMENTATION.md for details.

#### For Undo/Redo Command Pattern
```
@workspace Implement undo/redo functionality using Command Pattern with graph-based history.

Requirements from prompt.md:
- Each edit creates a Command node in undo graph
- Support branching: undo then new edit creates branch
- Visualize history as tree structure
- Commands: AddQuestionCommand, DeleteQuestionCommand, ModifyQuestionCommand, ReorderQuestionsCommand
- Ctrl+Z for undo, Ctrl+Y for redo
- Maintain command history for session
- Show undo stack in UI (optional)

Create:
1. ICommand interface with Execute/Undo/Redo
2. Concrete command classes for each operation
3. CommandHistoryManager with graph structure
4. UI integration (menu items, keyboard shortcuts)
5. State capture/restoration logic
6. Limit history depth (e.g., 50 commands)
```

### 📋 PHASE 2: Completed Features (Reference)

### 📋 PHASE 2: Completed Features (Reference)

#### Graph-Based Caching (✅ IMPLEMENTED)
The GraphCacheService has been successfully implemented with:
- Dependency tracking between cache nodes
- Cascade invalidation when parent nodes change
- Thread-safe operations
- Statistics tracking
- See PHASE2_COMPLETION.md for details

### 🔄 PHASE 4+: Future Features

### For State Machine Implementation
```
@workspace Create a robust state machine for Answer status management using the Stateless library.

Requirements:
- States: UNFINISHED, PENDING, COMPLETED, CANCELLED
- Triggers: Start, Complete, Approve, Reject, Cancel
- Valid transitions (refer to FRONTEND_TECHNICAL_DOCUMENTATION.md section on Answer states)
- Log all transitions with timestamp and user
- Prevent invalid transitions with meaningful error messages
- Integrate with PUT /api/answer/setStatus endpoint
- Unit tests for all valid and invalid transitions

Include complete implementation with:
1. AnswerStateMachine class
2. Integration in AnswerService
3. UI feedback for state changes (InfoBar notifications)
4. State history tracking in database/local storage

NOTE: To be implemented in Phase 4 (Response Collection).
```
```
@workspace Create a robust state machine for Answer status management using the Stateless library.

Requirements:
- States: UNFINISHED, PENDING, COMPLETED, CANCELLED
- Triggers: Start, Complete, Approve, Reject, Cancel
- Valid transitions (refer to FRONTEND_TECHNICAL_DOCUMENTATION.md section on Answer states)
- Log all transitions with timestamp and user
- Prevent invalid transitions with meaningful error messages
- Integrate with PUT /api/answer/setStatus endpoint
- Unit tests for all valid and invalid transitions

Include complete implementation with:
1. AnswerStateMachine class
2. Integration in AnswerService
3. UI feedback for state changes (InfoBar notifications)
4. State history tracking in database/local storage

NOTE: To be implemented in Phase 4 (Response Collection).
```

### For Session Checkpoint System
```
@workspace Implement a session checkpoint system for auto-saving user progress when filling out questionnaires.

Requirements from documentation:
- Save checkpoint every 30 seconds automatically
- Save on manual trigger (Save Draft button)
- Encrypt checkpoints using Windows DPAPI
- Store locally in %APPDATA%\SurveyApp\Sessions\
- Include: AnswerId, QuestionResponses (Dictionary<Guid, string>), progress %, timestamp, hash for integrity
- On app restart, check for unfinished sessions and offer recovery
- Recovery dialog with options: Continue, Start Fresh, Discard
- Clear old checkpoints (>7 days) on app start

Create:
1. SessionCheckpoint model
2. SessionManager service
3. Recovery dialog UI
4. Background timer for auto-save
5. Unit tests for save/load/recovery scenarios

NOTE: To be implemented in Phase 4 (Response Collection).
```

### For History & Versioning
```
@workspace Implement temporal graph-based version history for questionnaires.

Per FRONTEND_TECHNICAL_DOCUMENTATION.md specifications:
- Track every change to Questionary and Questions
- Create QuestionaryVersion nodes linked as temporal graph
- Store complete snapshot (JSON) + delta changes
- Enable viewing history timeline
- Implement diff viewer showing side-by-side comparison
- Allow restore to previous version (creates new version, doesn't overwrite)
- Use colors: green for additions, red for deletions, yellow for modifications

Implement:
1. QuestionaryVersion model with graph structure
2. VersioningService with graph operations
3. HistoryViewer UI with timeline
4. DiffViewer UI with side-by-side comparison
5. Restore functionality
6. Storage (local SQLite for caching, API for persistence)

NOTE: To be implemented in Phase 3 (Question Editor - advanced features).
```

### For Reactive Validation
```
@workspace Set up reactive validation using Rx.NET for question input.

According to documentation:
- Debounce text input 500ms before validating
- Combine multiple validation streams (local + remote)
- Show inline validation messages with severity (error, warning, info)
- Validate against Constraints and Policies
- Throttle API validation calls to max 1 per second
- Display validation spinner during async checks
- Use color coding: red border for errors, yellow for warnings

Create:
1. ReactiveValidationService using System.Reactive
2. ValidationResult model with message, severity, field
3. UI integration with TextBox (binding to HasErrors, ErrorMessage)
4. Constraint interpreters for different PolicyRecord types
5. Local validators (pattern, length, range)
6. Remote validators (uniqueness checks via API)

NOTE: To be implemented in Phase 3 (Question Editor).
```

### For Conflict Resolution
```
@workspace Build a three-way merge conflict resolver for concurrent questionary edits.

As specified in documentation:
- Detect conflicts when saving (compare timestamps, version numbers)
- Use three-way merge: local changes + remote changes + common ancestor
- Auto-merge non-conflicting changes
- Present conflicts to user in clear UI
- Show: Your Change | Current Version | Merged Result
- Allow manual resolution: Accept Yours, Accept Theirs, Edit Manually
- Use vector clocks or version numbers for ordering

Implement:
1. ConflictResolver with three-way merge algorithm
2. ConflictDetectionService
3. ConflictResolutionDialog UI
4. Diff visualization component
5. Merge strategy: last-write-wins with user override
6. Tests for various conflict scenarios

NOTE: To be implemented in Phase 5 (Response Analysis - advanced features).
```

---

## 🔴 BACKEND API REQUIREMENTS

**Critical Issue**: Frontend development is **BLOCKED** by missing backend APIs.

### Missing Endpoints (13 total)
The frontend has implemented features that **cannot persist data** or **cannot function** without backend support:

#### CRITICAL (Blocks Phase 3 completeness):
1. ❌ `PUT /api/Question/{ConnectionId}/{id}` - Update question
2. ❌ `PATCH /api/Question/{ConnectionId}/reorder` - Bulk reorder questions  
3. ❌ `DELETE /api/Question/{ConnectionId}/{id}` - Delete question
4. ❌ `POST/PUT/DELETE /api/Constraint/{ConnectionId}` - Constraint CRUD

#### HIGH Priority (Blocks Phase 5):
5. ❌ `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}` - Get answers with filters
6. ❌ `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}/statistics` - Answer statistics
7. ❌ `POST /api/Answer/{ConnectionId}/search` - Advanced answer search
8. ❌ `GET /api/QuestionResponse/{ConnectionId}/question/{questionId}` - Response aggregation

#### MEDIUM Priority (Enhances functionality):
9. ❌ `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}/export/csv` - Export to CSV
10. ❌ `POST /api/Question/{ConnectionId}/batch` - Batch create questions
11. ❌ `POST /api/Questionary/{ConnectionId}/{id}/duplicate` - Duplicate questionary
12. ❌ `GET /api/Answer/{ConnectionId}/{id}/history` - State transition history
13. ❌ `PATCH /api/Answer/{ConnectionId}/{id}/metadata` - Update answer metadata

### 📄 Complete Specification
See **[BACKEND_API_REQUIREMENTS.md](./BACKEND_API_REQUIREMENTS.md)** for:
- Full API specifications with request/response examples
- cURL examples for testing
- Performance requirements
- Security & validation rules
- Database schema considerations
- Implementation priority and timeline
- Integration testing plan

### Frontend Status
- ✅ All frontend features implemented and tested locally
- ✅ Build passing (0 errors, 0 warnings)
- ⏸️ **Waiting for backend APIs** to enable full functionality
- 🟢 Ready to integrate immediately upon API availability

### Next Steps
1. **Backend Team**: Review BACKEND_API_REQUIREMENTS.md
2. **Backend Team**: Implement Phase 1 (Critical) endpoints - Week 1
3. **Backend Team**: Update OpenAPI specification
4. **Frontend Team**: Integration testing once APIs deployed
5. **Both Teams**: Joint testing and performance tuning

---