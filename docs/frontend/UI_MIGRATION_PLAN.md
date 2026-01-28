# WPF UI Migration Plan - UI/UX Overhaul

**Project:** Survey Management System  
**Current UI Framework:** ModernWPF (0.9.6)  
**Target UI Framework:** WPF UI (Lepo.co) - Latest version  
**Date:** January 28, 2026  
**Priority:** MEDIUM - Keep SEPARATE from functionality implementation  
**Estimated Duration:** 2-3 weeks (parallel to backend API implementation)

---

## 🎯 Objectives

### Primary Goals
1. **Modernize UI** - Migrate from ModernWPF to WPF UI for Fluent Design 2.0 aesthetics
2. **Improve Usability** - Enhance user experience with better navigation and controls
3. **Zero Functionality Impact** - UI changes ONLY, no business logic modifications
4. **Maintain Feature Parity** - All existing features must work identically after migration

### Why WPF UI?
- ✅ **Modern Fluent Design** - Windows 11-native appearance
- ✅ **Active Development** - Regular updates, better support than ModernWPF
- ✅ **Rich Control Set** - NavigationView, CardControl, InfoBar, Breadcrumb, etc.
- ✅ **Theme Engine** - Superior light/dark theme support
- ✅ **Icon Library** - Built-in Fluent System Icons
- ✅ **Accessibility** - Better WCAG compliance
- ✅ **Performance** - Hardware-accelerated animations

---

## 🚨 CRITICAL SEPARATION OF CONCERNS

### ⚠️ GOLDEN RULE: UI Migration ≠ Functionality Changes

**NEVER mix these two work streams:**

| UI Migration (This Plan) | Functionality Work (Separate) |
|--------------------------|-------------------------------|
| ✅ Change XAML controls | ❌ Add new ViewModels |
| ✅ Update styles/themes | ❌ Modify business logic |
| ✅ Improve navigation UX | ❌ Add API endpoints |
| ✅ Refine layouts | ❌ Change data models |
| ✅ Update icons/colors | ❌ Implement new features |

### Work Stream Isolation
1. **Create UI Migration Branch**: `feature/wpfui-migration`
2. **Functionality stays on**: `main` or separate feature branches
3. **Merge strategy**: UI migration merges ONLY when 100% complete and tested
4. **Testing**: Every UI change must pass regression tests (functionality unchanged)

---

## 📊 Current State Analysis

### Current ModernWPF Usage

**Controls in Use:**
- `NavigationView` (MainWindow.xaml) → Sidebar navigation
- `DataGrid` (QuestionaryListView) → Table display
- `TextBox`, `Button`, `ComboBox` → Standard inputs
- `ListView` (QuestionEditorView) → Question list
- `AutoSuggestBox` → Search functionality
- Theme system → Light/Dark toggle

**Current Theme Configuration:**
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ui:ThemeResources />
            <ui:XamlControlsResources />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

**Known Issues to Address:**
1. ❌ AutoSuggestBox style errors (DefaultAutoSuggestBoxStyle missing)
2. ❌ Theme switching requires app restart
3. ❌ Limited icon support
4. ❌ No breadcrumb navigation
5. ❌ No modern card layouts

---

## 🎨 Target State - WPF UI Design

### New Control Mapping

| Current (ModernWPF) | New (WPF UI) | Improvement |
|---------------------|--------------|-------------|
| `NavigationView` | `Wpf.Ui.Controls.NavigationView` | Better Fluent Design, breadcrumbs |
| `DataGrid` | `Wpf.Ui.Controls.DataGrid` | Modern styling, better performance |
| `TextBox` | `Wpf.Ui.Controls.TextBox` | Fluent appearance, placeholders |
| `Button` | `Wpf.Ui.Controls.Button` | Icon support, accent colors |
| `AutoSuggestBox` | `Wpf.Ui.Controls.AutoSuggestBox` | No style errors, better UX |
| Custom dialogs | `Wpf.Ui.Controls.ContentDialog` | Native Fluent dialogs |
| Status messages | `Wpf.Ui.Controls.InfoBar` | Contextual notifications |
| Theme switcher | `Wpf.Ui.Appearance.Theme` | Real-time switching, no restart |

### New Components to Add

1. **`CardControl`** - Modern card layout for questionnaires
2. **`Breadcrumb`** - Navigation trail (Home → Questionnaires → Editor)
3. **`InfoBar`** - Contextual messages (success, warning, error)
4. **`ProgressRing`** - Loading indicators
5. **`NavigationViewItem` with icons** - Fluent System Icons
6. **`TitleBar`** - Custom window title bar
7. **`SnackBar`** - Toast notifications

---

## 📋 Implementation Phases

### Phase 1: Setup & Foundation (Week 1, Days 1-2)

#### Day 1: Package Installation & Configuration

**Tasks:**
1. ✅ Create new branch: `feature/wpfui-migration`
2. ✅ Install NuGet package: `Wpf.Ui` (latest version)
3. ✅ Remove ModernWPF package: `ModernWpfUI`
4. ✅ Update `App.xaml` with WPF UI resources
5. ✅ Create theme service adapter (compatibility layer)
6. ✅ Update GlobalUsings.cs with WPF UI namespaces
7. ✅ Build and verify no compilation errors

**Files to Modify:**
- `SurveyApp.csproj` - Update PackageReferences
- `App.xaml` - Replace ModernWPF resources with WPF UI
- `App.xaml.cs` - Update theme initialization
- `GlobalUsings.cs` - Add WPF UI using statements

**Expected Result:** Project builds successfully with WPF UI installed

---

#### Day 2: Theme System Migration

**Tasks:**
1. ✅ Update `ThemeService.cs` to use `Wpf.Ui.Appearance.Theme`
2. ✅ Implement real-time theme switching (no app restart)
3. ✅ Update theme persistence logic
4. ✅ Create color scheme configuration (accent colors)
5. ✅ Test Light/Dark/High Contrast themes
6. ✅ Update MainWindow theme integration

**New Theme Features:**
- Real-time theme switching
- Custom accent colors
- System theme sync (follows Windows)
- Per-control theme overrides

**Code Example (ThemeService):**
```csharp
public class ThemeService
{
    public void SetTheme(ThemeType theme)
    {
        Wpf.Ui.Appearance.Theme.Apply(
            theme == ThemeType.Light 
                ? Wpf.Ui.Appearance.ThemeType.Light 
                : Wpf.Ui.Appearance.ThemeType.Dark
        );
    }

    public void SyncWithSystem()
    {
        Wpf.Ui.Appearance.Theme.Apply(
            Wpf.Ui.Appearance.ThemeType.Unknown,
            updateAccent: true
        );
    }
}
```

---

### Phase 2: Main Window & Navigation (Week 1, Days 3-5)

#### Day 3: MainWindow Redesign

**Tasks:**
1. ✅ Replace ModernWPF NavigationView with WPF UI NavigationView
2. ✅ Add custom TitleBar with theme toggle and window controls
3. ✅ Update navigation items with Fluent System Icons
4. ✅ Add breadcrumb navigation at top of content area
5. ✅ Implement NavigationService adapter for WPF UI
6. ✅ Test navigation flow (Home → Questionnaires → Questions → Responses)

**Before (ModernWPF):**
```xml
<ui:NavigationView>
    <ui:NavigationView.MenuItems>
        <ui:NavigationViewItem Content="Home" />
        <ui:NavigationViewItem Content="Questionnaires" />
    </ui:NavigationView.MenuItems>
</ui:NavigationView>
```

**After (WPF UI):**
```xml
<wpfui:NavigationView x:Name="RootNavigation">
    <wpfui:NavigationView.MenuItems>
        <wpfui:NavigationViewItem 
            Content="Home" 
            Icon="Home24"
            TargetPageType="{x:Type local:HomeView}"/>
        <wpfui:NavigationViewItem 
            Content="Questionnaires" 
            Icon="DocumentBulletList24"
            TargetPageType="{x:Type local:QuestionaryListView}"/>
    </wpfui:NavigationView.MenuItems>
    <wpfui:NavigationView.Header>
        <wpfui:Breadcrumb x:Name="BreadcrumbNav" />
    </wpfui:NavigationView.Header>
</wpfui:NavigationView>
```

---

#### Day 4: Icon System Integration

**Tasks:**
1. ✅ Map all navigation items to Fluent System Icons
2. ✅ Add icons to buttons (Add, Edit, Delete, Refresh, etc.)
3. ✅ Update dialog icons (Success, Warning, Error)
4. ✅ Add loading spinners (ProgressRing)
5. ✅ Create icon resource dictionary

**Icon Mapping:**
| Feature | Icon | WPF UI Name |
|---------|------|-------------|
| Home | 🏠 | `Home24` |
| Questionnaires | 📋 | `DocumentBulletList24` |
| Questions | ❓ | `QuestionCircle24` |
| Responses | 📊 | `DataUsage24` |
| Add | ➕ | `Add24` |
| Edit | ✏️ | `Edit24` |
| Delete | 🗑️ | `Delete24` |
| Refresh | 🔄 | `ArrowSync24` |
| Search | 🔍 | `Search24` |
| Settings | ⚙️ | `Settings24` |

---

#### Day 5: Navigation UX Improvements

**Tasks:**
1. ✅ Implement breadcrumb navigation updates
2. ✅ Add navigation history (back button functionality)
3. ✅ Update NavigationService to support breadcrumbs
4. ✅ Add keyboard shortcuts for navigation (Alt+Left/Right)
5. ✅ Test deep navigation flows
6. ✅ Add navigation animations (smooth transitions)

**Breadcrumb Example:**
```
Home > Questionnaires > Customer Satisfaction Survey > Questions
```

---

### Phase 3: View Redesigns (Week 2, Days 1-5)

#### Day 1: HomeView Redesign

**Tasks:**
1. ✅ Replace basic layout with Card-based dashboard
2. ✅ Add statistics cards (Total Questionnaires, Recent Responses, etc.)
3. ✅ Add quick action buttons (Create Questionary, View Responses)
4. ✅ Add recent activity feed
5. ✅ Update with modern spacing and typography

**New Layout:**
```
┌─────────────────────────────────────────────┐
│ Welcome back, User!                         │
├─────────────────────────────────────────────┤
│ ┌──────┐  ┌──────┐  ┌──────┐  ┌──────┐    │
│ │ 25   │  │ 150  │  │ 89%  │  │ 12   │    │
│ │Quest.│  │Resp. │  │Compl.│  │Today │    │
│ └──────┘  └──────┘  └──────┘  └──────┘    │
├─────────────────────────────────────────────┤
│ Quick Actions                               │
│ [Create Questionary] [View Responses]      │
├─────────────────────────────────────────────┤
│ Recent Activity                             │
│ • Survey "Customer Feedback" created       │
│ • 15 new responses to "Employee Survey"    │
└─────────────────────────────────────────────┘
```

---

#### Day 2: QuestionaryListView Redesign

**Tasks:**
1. ✅ Replace DataGrid with CardControl grid layout
2. ✅ Add search bar with AutoSuggestBox (WPF UI version)
3. ✅ Add filter chips (by date, by creator, by status)
4. ✅ Update toolbar with icon buttons
5. ✅ Add empty state illustration
6. ✅ Implement card hover effects

**Card Layout (Instead of DataGrid):**
```xml
<ItemsControl ItemsSource="{Binding Questionnaires}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <wpfui:CardControl Margin="8">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                    </Grid.RowDefinitions>
                    
                    <TextBlock Text="{Binding Name}" 
                               FontSize="18" 
                               FontWeight="SemiBold"/>
                    
                    <TextBlock Grid.Row="1" 
                               Text="{Binding Description}"
                               Foreground="{DynamicResource TextFillColorSecondaryBrush}"/>
                    
                    <StackPanel Grid.Row="2" Orientation="Horizontal">
                        <wpfui:Button Content="View Details" 
                                     Icon="Open24"
                                     Appearance="Primary"/>
                        <wpfui:Button Content="Edit" 
                                     Icon="Edit24"
                                     Margin="8,0,0,0"/>
                    </StackPanel>
                </Grid>
            </wpfui:CardControl>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

#### Day 3: QuestionEditorView Redesign

**Tasks:**
1. ✅ Update ListView with modern card styling
2. ✅ Add drag handle icons (visual improvement)
3. ✅ Update toolbar with icon buttons
4. ✅ Add question type badges with colors
5. ✅ Improve drag-drop visual feedback
6. ✅ Add constraint count badge styling
7. ✅ Update empty state

**Question Card Design:**
```
┌────────────────────────────────────────┐
│ ≡  1. What is your name?               │
│    Type: Text  |  2 Constraints        │
│    [Edit] [Delete]                     │
└────────────────────────────────────────┘
```

---

#### Day 4: Dialog Redesigns

**Tasks:**
1. ✅ Replace custom dialogs with `ContentDialog`
2. ✅ Update QuestionaryDialogWindow
3. ✅ Update QuestionDialogWindow
4. ✅ Add dialog icons and better spacing
5. ✅ Update validation message styling
6. ✅ Add confirmation dialogs with InfoBar styling

**ContentDialog Example:**
```xml
<wpfui:ContentDialog x:Name="CreateQuestionaryDialog"
                     Title="Create New Questionary"
                     PrimaryButtonText="Create"
                     SecondaryButtonText="Cancel"
                     DefaultButton="Primary">
    <StackPanel Spacing="16">
        <wpfui:TextBox 
            Header="Name"
            PlaceholderText="Enter questionary name"
            Text="{Binding Name}"/>
        
        <wpfui:TextBox 
            Header="Description"
            PlaceholderText="Enter description"
            Text="{Binding Description}"
            AcceptsReturn="True"
            Height="100"/>
    </StackPanel>
</wpfui:ContentDialog>
```

---

#### Day 5: ResponseFormView Redesign

**Tasks:**
1. ✅ Update progress bar styling
2. ✅ Add modern question card layout
3. ✅ Update navigation buttons with icons
4. ✅ Add submit button with accent color
5. ✅ Improve loading overlay
6. ✅ Add success animation on submit

---

### Phase 4: Component Updates (Week 2-3, Days 1-3)

#### Day 1: InfoBar Integration

**Tasks:**
1. ✅ Replace MessageBox calls with InfoBar
2. ✅ Add success InfoBars (Create, Update, Delete operations)
3. ✅ Add warning InfoBars (validation messages)
4. ✅ Add error InfoBars (API failures)
5. ✅ Add info InfoBars (helpful tips)
6. ✅ Implement auto-dismiss timers

**InfoBar Examples:**
```xml
<!-- Success -->
<wpfui:InfoBar 
    Title="Success"
    Message="Questionary created successfully"
    Severity="Success"
    IsOpen="{Binding ShowSuccessMessage}"/>

<!-- Error -->
<wpfui:InfoBar 
    Title="Error"
    Message="{Binding ErrorMessage}"
    Severity="Error"
    IsOpen="{Binding HasError}"/>
```

---

#### Day 2: Loading States & Animations

**Tasks:**
1. ✅ Replace loading overlays with ProgressRing
2. ✅ Add skeleton loaders for lists
3. ✅ Add smooth transitions between views
4. ✅ Add button loading states
5. ✅ Implement page load animations

**Loading Example:**
```xml
<wpfui:ProgressRing 
    IsIndeterminate="True"
    Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibilityConverter}}"/>
```

---

#### Day 3: Form Controls Update

**Tasks:**
1. ✅ Update all TextBox controls to WPF UI TextBox
2. ✅ Add placeholder text to inputs
3. ✅ Update ComboBox styling
4. ✅ Update CheckBox and RadioButton styling
5. ✅ Add input validation visual states
6. ✅ Update DatePicker styling

---

### Phase 5: Polish & Testing (Week 3, Days 4-5)

#### Day 4: Visual Polish

**Tasks:**
1. ✅ Review all spacing and margins (consistent 8px grid)
2. ✅ Update typography (font sizes, weights)
3. ✅ Add micro-animations (hover, click, etc.)
4. ✅ Update color scheme (accent colors)
5. ✅ Add focus indicators (accessibility)
6. ✅ Update high contrast mode
7. ✅ Add window resize animations

**Design System:**
- **Spacing**: 4px, 8px, 16px, 24px, 32px
- **Border Radius**: 4px (small), 8px (medium), 12px (large)
- **Font Sizes**: 12px (caption), 14px (body), 16px (subtitle), 18px (title), 24px (header)
- **Accent Color**: System accent (respects user preference)

---

#### Day 5: Comprehensive Testing

**Testing Checklist:**

**Visual Regression:**
- [ ] All views render correctly in Light theme
- [ ] All views render correctly in Dark theme
- [ ] All views render correctly in High Contrast theme
- [ ] No layout breaks at different window sizes
- [ ] All icons display correctly
- [ ] All colors follow theme

**Functional Regression:**
- [ ] All navigation works (no broken links)
- [ ] All forms submit correctly
- [ ] All dialogs open/close properly
- [ ] Theme switching works in real-time
- [ ] Drag-drop still functions
- [ ] Search/filter still works
- [ ] All buttons trigger correct actions

**Accessibility:**
- [ ] Keyboard navigation works (Tab, Enter, Esc)
- [ ] Screen reader announces all elements
- [ ] Focus indicators visible
- [ ] Color contrast meets WCAG 2.1 AAA
- [ ] All interactive elements have labels

**Performance:**
- [ ] App startup time not degraded
- [ ] View transitions smooth (60 FPS)
- [ ] Large lists render without lag
- [ ] Memory usage stable
- [ ] No visual stuttering

---

## 🔄 Migration Strategy

### Phased Rollout Approach

**Option A: Big Bang (NOT RECOMMENDED)**
- Migrate all views at once
- ❌ High risk of breaking changes
- ❌ Difficult to test thoroughly
- ❌ Cannot rollback easily

**Option B: Incremental (RECOMMENDED)**
- Migrate one view at a time
- ✅ Lower risk
- ✅ Easier to test
- ✅ Can rollback individual views
- ✅ Can ship partially migrated app

**Recommended Order:**
1. MainWindow + Navigation (foundation)
2. HomeView (simple, low risk)
3. QuestionaryListView (moderate complexity)
4. QuestionEditorView (high complexity)
5. Dialogs (low risk, high impact)
6. ResponseFormView (moderate complexity)

### Compatibility Layer

Create abstraction to support both UI frameworks during migration:

```csharp
// INavigationService (unchanged)
public interface INavigationService
{
    void Navigate(Type pageType, object? parameter = null);
    bool CanGoBack { get; }
    void GoBack();
}

// WpfUiNavigationService (new implementation)
public class WpfUiNavigationService : INavigationService
{
    private readonly Wpf.Ui.Controls.NavigationView _navigationView;
    
    public void Navigate(Type pageType, object? parameter = null)
    {
        _navigationView.Navigate(pageType, parameter);
    }
    
    // ... rest of implementation
}
```

**Benefits:**
- ViewModels unchanged (no business logic impact)
- Can switch implementations via DI
- Easy A/B testing

---

## 📏 Design Guidelines

### Layout Principles

1. **8px Grid System** - All spacing in multiples of 8px
2. **Consistent Margins** - 16px between major sections, 8px between related items
3. **Card-Based Layouts** - Group related content in cards
4. **Whitespace** - Don't overcrowd, give content room to breathe
5. **Hierarchy** - Use size/weight to establish visual hierarchy

### Typography Scale

```
Display:  32px / SemiBold  (Page titles)
Title:    24px / SemiBold  (Section headers)
Subtitle: 18px / SemiBold  (Card headers)
Body:     14px / Normal    (Content)
Caption:  12px / Normal    (Metadata, hints)
```

### Color Usage

**Semantic Colors:**
- **Primary**: Accent color (system-defined)
- **Success**: Green (#10b981)
- **Warning**: Amber (#f59e0b)
- **Error**: Red (#ef4444)
- **Info**: Blue (#3b82f6)

**Text Colors:**
- **Primary Text**: Default foreground
- **Secondary Text**: 70% opacity
- **Tertiary Text**: 50% opacity (captions)
- **Disabled Text**: 38% opacity

### Component Patterns

**Buttons:**
- **Primary**: Filled accent color (main actions)
- **Secondary**: Subtle outline (secondary actions)
- **Tertiary**: Text only (low-priority actions)

**Cards:**
- 8px border radius
- 2px border (subtle)
- 8px padding
- Shadow on hover

**Input Fields:**
- Clear labels above field
- Placeholder text for examples
- Validation messages below field
- Red border for errors

---

## 🚀 Implementation Code Snippets

### App.xaml Changes

**Remove:**
```xml
<ui:ThemeResources />
<ui:XamlControlsResources />
```

**Add:**
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <wpfui:ThemesDictionary Theme="Dark" />
            <wpfui:ControlsDictionary />
        </ResourceDictionary.MergedDictionaries>
        
        <!-- Custom resources -->
        <Style TargetType="wpfui:CardControl" BasedOn="{StaticResource {x:Type wpfui:CardControl}}">
            <Setter Property="Margin" Value="8"/>
            <Setter Property="Padding" Value="16"/>
        </Style>
    </ResourceDictionary>
</Application.Resources>
```

### App.xaml.cs Changes

**Add to OnStartup:**
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    // Initialize WPF UI
    Wpf.Ui.Appearance.Watcher.Watch(
        this,
        Wpf.Ui.Appearance.BackgroundType.Mica,
        updateAccents: true
    );
    
    // Rest of startup code...
}
```

### SurveyApp.csproj Changes

**Remove:**
```xml
<PackageReference Include="ModernWpfUI" Version="0.9.6" />
```

**Add:**
```xml
<PackageReference Include="WPF-UI" Version="3.0.4" />
```

### GlobalUsings.cs Changes

**Add:**
```csharp
global using Wpf.Ui;
global using Wpf.Ui.Controls;
global using Wpf.Ui.Appearance;
```

---

## 🧪 Testing Strategy

### Unit Tests (ViewModels - Should NOT Break)

All existing ViewModel unit tests must pass without modification:
- QuestionaryListViewModel tests
- QuestionEditorViewModel tests
- ResponseFormViewModel tests
- All other ViewModel tests

**If tests break**: You've mixed UI and logic - ROLLBACK

### UI Tests (Manual - Phase by Phase)

**Test Script Template:**
```
Feature: [Feature Name]
View: [View Name]
Theme: [Light/Dark/High Contrast]

Test Cases:
1. View loads without errors
2. All controls visible and aligned
3. All interactions work (click, type, select)
4. Navigation to/from view works
5. Theme changes apply correctly
6. Responsive at different window sizes
7. No console errors or warnings

Pass/Fail: ___
Notes: ___
```

### Automated UI Tests (Appium/WinAppDriver)

**Critical Paths to Automate:**
1. Navigate to Questionary List
2. Create new questionary
3. View questionary details
4. Add question
5. Delete question
6. Switch theme
7. Search questionaries

### Performance Benchmarks

**Metrics to Track:**
- App cold start time (baseline: < 2s)
- View navigation time (baseline: < 200ms)
- Theme switch time (target: < 100ms)
- Memory usage (baseline: ~150MB)
- CPU usage (idle: < 5%)

**Tool:** BenchmarkDotNet for .NET performance testing

---

## 📦 Deliverables

### Code Deliverables

1. ✅ Updated `SurveyApp.csproj` (WPF UI package)
2. ✅ Updated `App.xaml` and `App.xaml.cs` (theme system)
3. ✅ Updated `MainWindow.xaml` (NavigationView)
4. ✅ Updated all View XAML files (controls, layouts)
5. ✅ Updated `ThemeService.cs` (WPF UI adapter)
6. ✅ Updated `NavigationService.cs` (WPF UI adapter)
7. ✅ New ResourceDictionary for icons
8. ✅ New ResourceDictionary for custom styles

### Documentation Deliverables

1. ✅ This migration plan document
2. ✅ Before/After screenshots
3. ✅ Design system documentation
4. ✅ Component usage guide
5. ✅ Testing report
6. ✅ Performance comparison report

---

## 🛡️ Risk Management

### Identified Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Breaking changes in WPF UI API | High | Low | Pin package version, review changelog |
| Performance degradation | High | Medium | Benchmark before/after, optimize as needed |
| Theme compatibility issues | Medium | Medium | Test all themes early, create fallbacks |
| Navigation structure changes | High | Low | Abstract navigation service, test thoroughly |
| Deadline pressure mixing concerns | **CRITICAL** | High | Separate branches, strict code reviews |

### Rollback Plan

**If migration fails or introduces critical bugs:**

1. Revert `SurveyApp.csproj` to ModernWPF package
2. Revert `App.xaml` resources
3. Revert all XAML files from Git
4. Revert service adapters
5. Rebuild and test
6. Estimated rollback time: **< 30 minutes**

**Rollback triggers:**
- More than 5 critical bugs found
- Performance degradation > 20%
- Key functionality broken
- Accessibility regression
- Unable to meet deadline

---

## 📅 Timeline & Milestones

### Week 1: Foundation & Navigation
- **Day 1-2**: Setup, package installation, theme system
- **Day 3-5**: MainWindow, navigation, icons, breadcrumbs
- **Milestone**: App launches with new theme and navigation

### Week 2: View Redesigns
- **Day 1**: HomeView redesign
- **Day 2**: QuestionaryListView redesign
- **Day 3**: QuestionEditorView redesign
- **Day 4**: Dialog redesigns
- **Day 5**: ResponseFormView redesign
- **Milestone**: All major views migrated

### Week 3: Polish & Testing
- **Day 1-2**: Component updates (InfoBar, ProgressRing, forms)
- **Day 3**: Visual polish, animations, accessibility
- **Day 4**: Comprehensive testing
- **Day 5**: Bug fixes, performance tuning
- **Milestone**: Production-ready, all tests passing

### Week 4 (Buffer): Integration & Deployment
- Merge `feature/wpfui-migration` to `main`
- Deploy to staging
- User acceptance testing
- Production deployment

---

## 🎓 Training & Documentation

### Developer Onboarding

**For team members joining UI work:**

1. **Read WPF UI Documentation**: https://wpfui.lepo.co/documentation/
2. **Review this migration plan** (mandatory)
3. **Study design system** (spacing, colors, typography)
4. **Run app locally** with both themes
5. **Make small PR** (update one icon) to learn process

### Design System Documentation

Create `DESIGN_SYSTEM.md`:
- Color palette
- Typography scale
- Spacing system
- Component library
- Icon library
- Animation guidelines
- Accessibility standards

### Component Usage Guide

Create `COMPONENT_GUIDE.md`:
- When to use each WPF UI control
- Common patterns (card layouts, dialogs, etc.)
- Code snippets
- Do's and Don'ts
- Examples from codebase

---

## ✅ Success Criteria

### Must Haves (Launch Blockers)

- [ ] All views migrated to WPF UI
- [ ] All functionality works identically
- [ ] All existing tests pass
- [ ] No console errors or warnings
- [ ] Light & Dark themes work
- [ ] Accessibility standards met (WCAG 2.1 AA minimum)
- [ ] Performance benchmarks met or exceeded
- [ ] Code review approved
- [ ] User acceptance testing passed

### Nice to Haves (Post-Launch)

- [ ] High Contrast theme optimized
- [ ] Custom accent color picker
- [ ] Advanced animations
- [ ] Skeleton loaders for all lists
- [ ] Tooltip improvements
- [ ] Context menu modernization
- [ ] Keyboard shortcut discoverability

---

## 🔗 Resources

### WPF UI Resources
- **Official Docs**: https://wpfui.lepo.co/documentation/
- **GitHub Repo**: https://github.com/lepoco/wpfui
- **NuGet Package**: https://www.nuget.org/packages/WPF-UI/
- **Community Discord**: (check GitHub for link)
- **Fluent Design System**: https://fluent2.microsoft.design/

### Migration References
- ModernWPF GitHub Issues (common migration problems)
- WPF UI Examples Gallery (sample code)
- Fluent Design Guidelines (Microsoft)

### Tools
- **Design**: Figma (for mockups)
- **Icons**: Fluent System Icons (built into WPF UI)
- **Testing**: WinAppDriver, Appium
- **Performance**: BenchmarkDotNet, dotTrace

---

## 📞 Communication Plan

### Stakeholder Updates

**Weekly Status Report Template:**
```
Week: [1/2/3]
Completion: [X]%
Completed:
- [Task 1]
- [Task 2]

In Progress:
- [Task 3]

Blockers:
- [Issue if any]

Next Week:
- [Planned tasks]

Risks:
- [Any new risks]
```

### Team Communication

- **Daily**: Quick sync on UI work (5 min standup)
- **Weekly**: Demo migrated views to team
- **Ad-hoc**: Immediate notification of blockers
- **Slack Channel**: #ui-migration-wpfui

---

## 🎯 Post-Migration Roadmap

### Future UI Enhancements (After Migration Complete)

**Phase A: Advanced Controls**
- Advanced data visualization (charts with LiveCharts2)
- Rich text editor for descriptions
- Drag-drop file upload with preview
- Timeline view for answer history

**Phase B: Animations**
- Page transition animations
- Loading state animations
- Success/error animations
- Micro-interactions

**Phase C: Customization**
- User-selectable accent colors
- Layout density options (comfortable/compact)
- Font size preferences
- Custom themes

**Phase D: Accessibility**
- Screen reader optimization
- Voice control support
- High contrast improvements
- Keyboard shortcut customization

---

## 🚨 CRITICAL REMINDERS

### ⛔ DO NOT DO THESE DURING UI MIGRATION

1. ❌ **Add new API endpoints** → Wait for backend team
2. ❌ **Change data models** → Separate work stream
3. ❌ **Modify ViewModels logic** → Only UI changes
4. ❌ **Add new features** → UI migration only
5. ❌ **Change business rules** → Separate concern
6. ❌ **Update database schema** → Not UI work
7. ❌ **Refactor services** → Separate task

### ✅ ONLY DO THESE

1. ✅ Change XAML markup
2. ✅ Update styles and templates
3. ✅ Replace ModernWPF controls with WPF UI
4. ✅ Improve layouts and spacing
5. ✅ Add icons and visual polish
6. ✅ Update theme system
7. ✅ Enhance accessibility (UI level)

---

**Document Version**: 1.0  
**Last Updated**: January 28, 2026  
**Next Review**: Start of each milestone week  
**Owner**: Frontend UI Team  
**Stakeholders**: All developers, UX designer, Product owner

---

## 🎬 Getting Started

**To begin this migration:**

1. Read this entire document
2. Review WPF UI documentation
3. Create branch: `git checkout -b feature/wpfui-migration`
4. Follow Phase 1, Day 1 tasks
5. Commit frequently with clear messages: "UI: Migrate MainWindow to WPF UI"
6. Test after each change
7. Keep functionality team informed of progress

**Questions?** Contact UI migration lead or post in #ui-migration-wpfui Slack channel.

---

**Ready to transform Survey Management into a modern Fluent Design masterpiece! 🎨✨**
