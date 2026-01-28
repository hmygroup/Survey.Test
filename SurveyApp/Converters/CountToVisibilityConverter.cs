namespace SurveyApp.Converters;

/// <summary>
/// Converts integer count to Visibility.
/// By default: count > 0 = Visible, count == 0 = Collapsed.
/// With "Inverse" parameter: count > 0 = Collapsed, count == 0 = Visible.
/// </summary>
public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = 0;
        
        if (value is int intValue)
        {
            count = intValue;
        }
        else if (value != null && int.TryParse(value.ToString(), out var parsedValue))
        {
            count = parsedValue;
        }

        var isInverse = parameter?.ToString()?.Equals("Inverse", StringComparison.OrdinalIgnoreCase) ?? false;
        
        if (isInverse)
        {
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("CountToVisibilityConverter only supports OneWay binding.");
    }
}
