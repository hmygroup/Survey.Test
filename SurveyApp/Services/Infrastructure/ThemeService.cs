namespace SurveyApp.Services.Infrastructure;

/// <summary>
/// Service for managing application theme (Light/Dark mode).
/// </summary>
public class ThemeService
{
    private readonly ILogger<ThemeService> _logger;

    public ThemeService(ILogger<ThemeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the current theme.
    /// </summary>
    public string CurrentTheme { get; private set; } = "Light";

    /// <summary>
    /// Applies the specified theme to the application.
    /// </summary>
    public void ApplyTheme(string theme)
    {
        try
        {
            _logger.LogInformation("Applying theme: {Theme}", theme);
            
            // Update ModernWpf theme
            ModernWpf.ThemeManager.Current.ApplicationTheme = theme == "Dark" 
                ? ModernWpf.ApplicationTheme.Dark 
                : ModernWpf.ApplicationTheme.Light;

            CurrentTheme = theme;
            SaveThemePreference(theme);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying theme: {Theme}", theme);
        }
    }

    /// <summary>
    /// Toggles between Light and Dark themes.
    /// </summary>
    public void ToggleTheme()
    {
        var newTheme = CurrentTheme == "Light" ? "Dark" : "Light";
        ApplyTheme(newTheme);
    }

    /// <summary>
    /// Loads the saved theme preference.
    /// </summary>
    public void LoadThemePreference()
    {
        try
        {
            var savedTheme = Properties.Settings.Default.Theme;
            if (!string.IsNullOrEmpty(savedTheme))
            {
                ApplyTheme(savedTheme);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading theme preference");
        }
    }

    /// <summary>
    /// Saves the theme preference.
    /// </summary>
    private void SaveThemePreference(string theme)
    {
        try
        {
            Properties.Settings.Default.Theme = theme;
            Properties.Settings.Default.Save();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving theme preference");
        }
    }
}
