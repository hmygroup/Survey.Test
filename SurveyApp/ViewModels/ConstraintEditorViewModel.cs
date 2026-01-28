namespace SurveyApp.ViewModels;

/// <summary>
/// ViewModel for the Constraint Editor.
/// Manages constraints and policy records for a question.
/// </summary>
public partial class ConstraintEditorViewModel : ObservableObject
{
    private readonly PolicyService _policyService;
    private readonly ILogger<ConstraintEditorViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<ConstraintDto> _constraints = new();

    [ObservableProperty]
    private ObservableCollection<PolicyDto> _availablePolicies = new();

    [ObservableProperty]
    private ConstraintDto? _selectedConstraint;

    [ObservableProperty]
    private PolicyDto? _selectedPolicy;

    [ObservableProperty]
    private string _newPolicyRecordValue = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Gets the policy records for the selected constraint.
    /// </summary>
    public ObservableCollection<PolicyRecordsDto> PolicyRecords { get; } = new();

    public ConstraintEditorViewModel(
        PolicyService policyService,
        ILogger<ConstraintEditorViewModel> logger)
    {
        _policyService = policyService;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the editor with constraints from a question.
    /// </summary>
    public async Task InitializeAsync(ICollection<ConstraintDto>? constraints = null)
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading policies...";

            // Load available policies
            var policies = await _policyService.GetAllAsync();
            if (policies != null)
            {
                AvailablePolicies = new ObservableCollection<PolicyDto>(policies);
                _logger.LogInformation("Loaded {Count} policies", AvailablePolicies.Count);
            }

            // Load existing constraints if provided
            if (constraints != null && constraints.Any())
            {
                Constraints = new ObservableCollection<ConstraintDto>(constraints);
                _logger.LogInformation("Loaded {Count} existing constraints", Constraints.Count);
            }

            StatusMessage = $"Ready - {AvailablePolicies.Count} policies available";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize constraint editor");
            StatusMessage = "Failed to load policies";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Adds a new constraint with the selected policy.
    /// </summary>
    [RelayCommand]
    private void AddConstraint()
    {
        if (SelectedPolicy == null)
        {
            StatusMessage = "Please select a policy first";
            return;
        }

        var newConstraint = new ConstraintDto
        {
            Id = Guid.NewGuid(),
            Policy = SelectedPolicy,
            PolicyRecords = new List<PolicyRecordsDto>()
        };

        Constraints.Add(newConstraint);
        SelectedConstraint = newConstraint;
        
        _logger.LogInformation("Added constraint with policy {PolicyName}", SelectedPolicy.Name);
        StatusMessage = $"Added constraint: {SelectedPolicy.Name}";
    }

    /// <summary>
    /// Removes the specified constraint or the selected constraint.
    /// </summary>
    [RelayCommand]
    private void RemoveConstraint(ConstraintDto? constraint = null)
    {
        var constraintToRemove = constraint ?? SelectedConstraint;
        
        if (constraintToRemove != null)
        {
            var policyName = constraintToRemove.Policy?.Name ?? "Unknown";
            Constraints.Remove(constraintToRemove);
            
            // Clear policy records if we're removing the selected constraint
            if (constraintToRemove == SelectedConstraint)
            {
                PolicyRecords.Clear();
                SelectedConstraint = null;
            }
            
            _logger.LogInformation("Removed constraint: {PolicyName}", policyName);
            StatusMessage = $"Removed constraint: {policyName}";
        }
    }

    /// <summary>
    /// Adds a new policy record to the selected constraint.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddPolicyRecord))]
    private void AddPolicyRecord()
    {
        if (SelectedConstraint == null || string.IsNullOrWhiteSpace(NewPolicyRecordValue))
            return;

        var newRecord = new PolicyRecordsDto
        {
            Id = Guid.NewGuid(),
            ConstraintId = SelectedConstraint.Id,
            Value = NewPolicyRecordValue.Trim()
        };

        // Find the index first (before creating new constraint)
        var index = Constraints.ToList().FindIndex(c => c.Id == SelectedConstraint.Id);

        // Add to constraint's policy records
        var records = SelectedConstraint.PolicyRecords.ToList();
        records.Add(newRecord);
        var updatedConstraint = SelectedConstraint with { PolicyRecords = records };

        // Update the constraint in the collection using the index
        if (index >= 0)
        {
            Constraints[index] = updatedConstraint;
            SelectedConstraint = updatedConstraint;
        }

        // Update UI collection
        PolicyRecords.Add(newRecord);
        
        _logger.LogInformation("Added policy record: {Value}", NewPolicyRecordValue);
        StatusMessage = $"Added record: {NewPolicyRecordValue}";
        
        NewPolicyRecordValue = string.Empty;
    }

    private bool CanAddPolicyRecord() => 
        SelectedConstraint != null && !string.IsNullOrWhiteSpace(NewPolicyRecordValue);

    /// <summary>
    /// Removes a policy record from the selected constraint.
    /// </summary>
    [RelayCommand]
    private void RemovePolicyRecord(PolicyRecordsDto record)
    {
        if (SelectedConstraint == null || record == null)
            return;

        PolicyRecords.Remove(record);

        // Find the index first (before creating new constraint)
        var index = Constraints.ToList().FindIndex(c => c.Id == SelectedConstraint.Id);

        // Update the constraint
        var records = SelectedConstraint.PolicyRecords.Where(r => r.Id != record.Id).ToList();
        var updatedConstraint = SelectedConstraint with { PolicyRecords = records };

        // Update the constraint in the collection using the index
        if (index >= 0)
        {
            Constraints[index] = updatedConstraint;
            SelectedConstraint = updatedConstraint;
        }

        _logger.LogInformation("Removed policy record: {Value}", record.Value);
        StatusMessage = $"Removed record: {record.Value}";
    }

    /// <summary>
    /// Updates the policy records display when constraint selection changes.
    /// </summary>
    partial void OnSelectedConstraintChanged(ConstraintDto? value)
    {
        PolicyRecords.Clear();
        
        if (value?.PolicyRecords != null)
        {
            foreach (var record in value.PolicyRecords)
            {
                PolicyRecords.Add(record);
            }
        }
    }

    /// <summary>
    /// Gets all constraints for saving.
    /// </summary>
    public ICollection<ConstraintDto> GetConstraints()
    {
        return Constraints.ToList();
    }
}
