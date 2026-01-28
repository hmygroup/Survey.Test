
You are an expert WPF + WinUI 3 developer tasked with building a production-ready questionnaire management system.

**PROJECT STATUS**: Phase 2 COMPLETED ✅ - See PHASE2_COMPLETION.md for full details
- Phase 1: Foundation established with .NET 8.0, ModernWPF, MVVM, DI, Serilog ✅
- Phase 2: Questionary Management UI with CRUD operations, search/filter, graph caching ✅
- All DTOs, API services, infrastructure services, and UI components implemented
- Build verified (0 errors, 0 warnings), CodeQL passed
- Ready for Phase 3: Question Editor

CRITICAL: You MUST read and strictly follow the complete technical documentation located at:
\FRONTEND_TECHNICAL_DOCUMENTATION.md

This file contains ALL specifications including API endpoints, data models, UI/UX rules, validation patterns, and architectural decisions. Refer to it constantly.

PHASE COMPLETION STATUS:
- Phase 1: Foundation ✅ COMPLETE (See PHASE1_COMPLETION.md)
- Phase 2: Questionary Management ✅ COMPLETE (See PHASE2_COMPLETION.md)
- Phase 3: Question Editor ⏭️ NEXT (Current focus)

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

Phase 2: Questionary Management (Week 2) ✅ **COMPLETED**
- [x] QuestionaryListView with DataGrid
- [x] Search and filter functionality
- [x] Create/Edit/Delete questionary dialogs
- [x] GraphCache implementation for questionaries
- [x] NullToBooleanConverter for UI bindings
- [x] Service registration in DI container
- [x] Build verified (0 errors, 0 warnings)
- [x] CodeQL security scan passed

**Status**: Production-ready questionary management. See PHASE2_COMPLETION.md for full details.

Phase 3: Question Editor (Week 3-4) ⏭️ **NEXT**
- [ ] Question list with drag-and-drop reordering
- [ ] Add/Edit/Delete question UI
- [ ] QuestionType selector with Factory pattern
- [ ] Constraint editor with Policy selection
- [ ] Real-time validation with Rx.NET
- [ ] Undo/Redo with Command pattern graph
- [ ] Live preview pane
- [ ] Version history viewer

Phase 4: Response Collection (Week 5)
- [ ] Form renderer based on QuestionType
- [ ] State machine for Answer status
- [ ] Progress tracking UI
- [ ] Auto-save with SessionCheckpoint
- [ ] Recovery dialog for unfinished sessions
- [ ] Metadata collection (time spent, device info)
- [ ] Submission confirmation

Phase 5: Response Analysis (Week 6)
- [ ] Response list with filtering
- [ ] Response detail view
- [ ] Basic statistics (charts with LiveCharts2)
- [ ] Export to CSV/Excel (EPPlus library)
- [ ] Conflict resolution UI

Phase 6: Polish & Optimization (Week 7-8)
- [ ] Performance optimization (virtualization, lazy loading)
- [ ] Accessibility audit and fixes
- [ ] UI/UX refinement
- [ ] Telemetry implementation
- [ ] Comprehensive testing
- [ ] Documentation completion
- [ ] Deployment packaging (MSIX)

REMEMBER:
- Phase 1 is COMPLETE ✅ - Foundation is production-ready
- Phase 2 is COMPLETE ✅ - Questionary Management UI is production-ready
- Current focus: Phase 3 - Question Editor
- Constantly refer to FRONTEND_TECHNICAL_DOCUMENTATION.md for ALL specifications
- Refer to PHASE1_COMPLETION.md and PHASE2_COMPLETION.md to understand existing implementation
- Every API call MUST include the connection parameter
- Follow the exact endpoint signatures documented
- Implement Answer (session) vs QuestionResponse (individual answer) correctly
- Handle all four Answer states: UNFINISHED, PENDING, COMPLETED, CANCELLED
- Use Constraints with Policies for validation rules
- Maintain session checkpoints for recovery
- Track history with temporal graph pattern
- Implement state machine for Answer transitions
- Write clean, maintainable, testable code
- Build upon existing services (QuestionaryService, QuestionService, GraphCacheService, etc.) - DO NOT recreate them

**WHAT'S ALREADY DONE** (from Phases 1 & 2):
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

Begin Phase 3 by understanding the existing codebase, then implement the Question Editor interface.
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

### Phase 3: Question Editor (Current Priority)
The API documentation is now complete and ready to support Phase 3 development. Focus remains on:

- [ ] Question list view with drag-and-drop reordering
- [ ] Add/Edit/Delete question UI
- [ ] QuestionType selector with Factory pattern
- [ ] Constraint editor with Policy selection
- [ ] Real-time validation with Rx.NET
- [ ] Undo/Redo with Command pattern graph
- [ ] Live preview pane
- [ ] Version history viewer

### Future Phases (4-6)
- [ ] Response Collection (Phase 4)
- [ ] Response Analysis (Phase 5)
- [ ] Polish & Optimization (Phase 6)

---

## 📝 Additional Copilot Chat Prompts for Specific Features

### 🎯 PHASE 3: Question Editor (CURRENT FOCUS)

#### For Question List View with Drag-and-Drop
```
@workspace Create a Question Editor view for managing questions within a questionary.

Requirements from FRONTEND_TECHNICAL_DOCUMENTATION.md:
- Display all questions for selected questionary in ListView
- Enable drag-and-drop reordering (update Order property)
- Show question text, type, and order number
- Add/Edit/Delete question buttons
- Persist order changes via API (bulk update)
- Visual feedback during drag (highlight drop zones)
- Integrate with existing QuestionService

Implement:
1. QuestionEditorView.xaml with ListView and drag-drop handlers
2. QuestionEditorViewModel with question collection management
3. Drag-drop behavior using WPF DragDrop events
4. Order persistence logic
5. Cache integration with GraphCacheService
6. Navigate from QuestionaryListView (double-click or Edit button)
```

#### For Question Type Selector with Factory Pattern
```
@workspace Implement dynamic question type selector using Factory Pattern.

Per documentation, QuestionType enum includes:
- Text
- Date  
- Integer
- Decimal
- Email
- Phone
- Rating
- SingleChoice
- MultipleChoice
- FileUpload

Requirements:
- Factory creates appropriate editor control based on QuestionType
- Each type has specific validation rules
- Type-specific constraints (min/max for numeric, pattern for text, etc.)
- Preview updates in real-time as type changes
- Use DataTemplateSelector or Factory Pattern

Create:
1. QuestionEditorFactory with Create method
2. Type-specific editor controls (TextQuestionEditor, RatingQuestionEditor, etc.)
3. QuestionType to UI mapper
4. Validation rules per type
5. Preview renderer
```

#### For Constraint Editor with Policy Integration
```
@workspace Build a constraint editor for applying validation policies to questions.

From FRONTEND_TECHNICAL_DOCUMENTATION.md:
- Constraints link Questions to Policies via ConstraintDto
- Policies contain PolicyRecords (key-value validation rules)
- Support multiple constraints per question
- Common policies: Required, MinLength, MaxLength, Pattern, Range, Custom
- Visual policy builder UI (no manual JSON editing)

Implement:
1. ConstraintEditorView with policy selection
2. PolicyRecord editor (key-value pairs)
3. Constraint list management (add/remove constraints)
4. Integration with existing QuestionService
5. Policy template library (predefined common policies)
6. Real-time validation preview
7. Save constraints via API
```

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
