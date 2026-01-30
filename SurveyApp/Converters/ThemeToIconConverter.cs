using System.Globalization;
using System.Windows.Data;

namespace SurveyApp.Converters;

/// <summary>
/// Converter that converts theme string to appropriate icon symbol.
/// </summary>
public class ThemeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string theme)
        {
            return theme == "Light" ? "WeatherSunny20" : "WeatherMoon20";
        }
        return "WeatherMoon20";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}