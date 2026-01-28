using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace SurveyApp.Services.Infrastructure;

/// <summary>
/// Service for performing reactive validation of question responses using Rx.NET.
/// Provides debounced validation with support for local and remote validation rules.
/// </summary>
public class ReactiveValidationService : IDisposable
{
    private readonly ILogger<ReactiveValidationService> _logger;
    private readonly Subject<(string value, IEnumerable<ConstraintDto> constraints)> _validationSubject = new();
    private readonly Dictionary<string, IDisposable> _subscriptions = new();
    private bool _disposed;

    public ReactiveValidationService(ILogger<ReactiveValidationService> _logger)
    {
        this._logger = _logger;
    }

    /// <summary>
    /// Creates a validation stream for a text input that debounces input and validates against constraints.
    /// </summary>
    /// <param name="fieldName">Name of the field being validated</param>
    /// <param name="inputObservable">Observable stream of input text changes</param>
    /// <param name="constraints">Validation constraints to apply</param>
    /// <param name="debounceMilliseconds">Debounce time in milliseconds (default: 500ms)</param>
    /// <returns>Observable stream of validation results</returns>
    public IObservable<QuestionValidationResult> CreateValidationStream(
        string fieldName,
        IObservable<string> inputObservable,
        IEnumerable<ConstraintDto> constraints,
        int debounceMilliseconds = 500)
    {
        _logger.LogDebug("Creating validation stream for field: {FieldName} with {ConstraintCount} constraints",
            fieldName, constraints.Count());

        return inputObservable
            .Throttle(TimeSpan.FromMilliseconds(debounceMilliseconds))
            .DistinctUntilChanged()
            .Select(value => ValidateLocal(fieldName, value, constraints))
            .Catch<QuestionValidationResult, Exception>(ex =>
            {
                _logger.LogError(ex, "Error during validation for field: {FieldName}", fieldName);
                return Observable.Return(QuestionValidationResult.Error(fieldName, "Validation error occurred"));
            });
    }

    /// <summary>
    /// Performs local validation against constraints.
    /// </summary>
    private QuestionValidationResult ValidateLocal(string fieldName, string value, IEnumerable<ConstraintDto> constraints)
    {
        if (!constraints.Any())
        {
            return QuestionValidationResult.Success(fieldName);
        }

        foreach (var constraint in constraints)
        {
            var policyRecords = constraint.PolicyRecords ?? Enumerable.Empty<PolicyRecordsDto>();
            
            foreach (var record in policyRecords)
            {
                var validationResult = ValidatePolicyRecord(fieldName, value, record);
                if (!validationResult.IsValid)
                {
                    return validationResult;
                }
            }
        }

        return QuestionValidationResult.Success(fieldName);
    }

    /// <summary>
    /// Validates a single policy record against a value.
    /// </summary>
    private QuestionValidationResult ValidatePolicyRecord(string fieldName, string value, PolicyRecordsDto record)
    {
        if (string.IsNullOrEmpty(record.Value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        // Parse the record value (format: "key:value")
        var parts = record.Value.Split(':', 2);
        if (parts.Length != 2)
        {
            _logger.LogWarning("Invalid policy record format: {RecordValue}", record.Value);
            return QuestionValidationResult.Success(fieldName);
        }

        var key = parts[0].Trim().ToLowerInvariant();
        var paramValue = parts[1].Trim();

        return key switch
        {
            "required" => ValidateRequired(fieldName, value, paramValue),
            "minlength" or "min" => ValidateMinLength(fieldName, value, paramValue),
            "maxlength" or "max" => ValidateMaxLength(fieldName, value, paramValue),
            "pattern" => ValidatePattern(fieldName, value, paramValue),
            "range" => ValidateRange(fieldName, value, paramValue),
            "email" => ValidateEmail(fieldName, value),
            "phone" => ValidatePhone(fieldName, value),
            "url" => ValidateUrl(fieldName, value),
            _ => QuestionValidationResult.Success(fieldName)
        };
    }

    #region Validation Rules

    private QuestionValidationResult ValidateRequired(string fieldName, string value, string paramValue)
    {
        var isRequired = bool.TryParse(paramValue, out var req) && req;
        
        if (isRequired && string.IsNullOrWhiteSpace(value))
        {
            return QuestionValidationResult.Error(fieldName, "This field is required");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidateMinLength(string fieldName, string value, string paramValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        if (!int.TryParse(paramValue, out var minLength))
        {
            _logger.LogWarning("Invalid minlength parameter: {ParamValue}", paramValue);
            return QuestionValidationResult.Success(fieldName);
        }

        if (value.Length < minLength)
        {
            return QuestionValidationResult.Error(fieldName, $"Must be at least {minLength} characters");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidateMaxLength(string fieldName, string value, string paramValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        if (!int.TryParse(paramValue, out var maxLength))
        {
            _logger.LogWarning("Invalid maxlength parameter: {ParamValue}", paramValue);
            return QuestionValidationResult.Success(fieldName);
        }

        if (value.Length > maxLength)
        {
            return QuestionValidationResult.Error(fieldName, $"Must not exceed {maxLength} characters");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidatePattern(string fieldName, string value, string paramValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        try
        {
            var regex = new Regex(paramValue, RegexOptions.None, TimeSpan.FromSeconds(1));
            if (!regex.IsMatch(value))
            {
                return QuestionValidationResult.Error(fieldName, "Value does not match the required pattern");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid regex pattern: {Pattern}", paramValue);
            return QuestionValidationResult.Warning(fieldName, "Pattern validation unavailable");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidateRange(string fieldName, string value, string paramValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        // Format: "min,max" or "min-max"
        var separators = new[] { ',', '-' };
        var rangeParts = paramValue.Split(separators, 2);
        
        if (rangeParts.Length != 2)
        {
            _logger.LogWarning("Invalid range format: {ParamValue}", paramValue);
            return QuestionValidationResult.Success(fieldName);
        }

        if (!double.TryParse(value, out var numValue))
        {
            return QuestionValidationResult.Error(fieldName, "Must be a valid number");
        }

        if (!double.TryParse(rangeParts[0], out var min) || !double.TryParse(rangeParts[1], out var max))
        {
            _logger.LogWarning("Invalid range values: {ParamValue}", paramValue);
            return QuestionValidationResult.Success(fieldName);
        }

        if (numValue < min || numValue > max)
        {
            return QuestionValidationResult.Error(fieldName, $"Must be between {min} and {max}");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidateEmail(string fieldName, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        // Basic email validation pattern
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        var regex = new Regex(emailPattern, RegexOptions.IgnoreCase);
        
        if (!regex.IsMatch(value))
        {
            return QuestionValidationResult.Error(fieldName, "Must be a valid email address");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidatePhone(string fieldName, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        // Basic phone validation (digits, spaces, dashes, parentheses, plus sign)
        var phonePattern = @"^[\d\s\-\(\)\+]+$";
        var regex = new Regex(phonePattern);
        
        if (!regex.IsMatch(value))
        {
            return QuestionValidationResult.Error(fieldName, "Must be a valid phone number");
        }

        if (value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "").Length < 10)
        {
            return QuestionValidationResult.Warning(fieldName, "Phone number seems too short");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    private QuestionValidationResult ValidateUrl(string fieldName, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return QuestionValidationResult.Success(fieldName);
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return QuestionValidationResult.Error(fieldName, "Must be a valid URL");
        }

        return QuestionValidationResult.Success(fieldName);
    }

    #endregion

    /// <summary>
    /// Combines multiple validation streams into a single stream.
    /// </summary>
    public IObservable<QuestionValidationResult> CombineValidationStreams(params IObservable<QuestionValidationResult>[] streams)
    {
        return Observable.CombineLatest(streams)
            .Select(results =>
            {
                // Return the first error or warning, or success if all pass
                var error = results.FirstOrDefault(r => r.Severity == ValidationSeverity.Error);
                if (error != null) return error;

                var warning = results.FirstOrDefault(r => r.Severity == ValidationSeverity.Warning);
                if (warning != null) return warning;

                var info = results.FirstOrDefault(r => r.Severity == ValidationSeverity.Info);
                if (info != null) return info;

                return results.First();
            });
    }

    /// <summary>
    /// Disposes resources used by the service.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        _validationSubject.Dispose();
        
        foreach (var subscription in _subscriptions.Values)
        {
            subscription.Dispose();
        }
        _subscriptions.Clear();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
