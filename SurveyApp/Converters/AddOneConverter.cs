using System.Globalization;
using System.Windows.Data;

namespace SurveyApp.Converters;

/// <summary>
/// Converts an integer value by adding one.
/// Useful for converting 0-based indices to 1-based display numbers.
/// </summary>
public class AddOneConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue + 1;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue - 1;
        }
        return value;
    }
}
