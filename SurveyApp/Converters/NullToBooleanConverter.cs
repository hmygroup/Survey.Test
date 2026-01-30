using System.Globalization;
using System.Windows.Data;

namespace SurveyApp.Converters;

/// <summary>
/// Converts null values to boolean (null = false, not null = true).
/// Supports only OneWay binding from source to target.
/// </summary>
public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // This converter only supports OneWay binding
        throw new NotSupportedException("NullToBooleanConverter only supports OneWay binding.");
    }
}
