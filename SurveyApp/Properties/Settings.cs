namespace SurveyApp.Properties;

[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.0.0")]
internal sealed partial class Settings : System.Configuration.ApplicationSettingsBase
{
    private static Settings? defaultInstance = ((Settings)(System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

    public static Settings Default
    {
        get
        {
            return defaultInstance!;
        }
    }

    [System.Configuration.UserScopedSettingAttribute()]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Configuration.DefaultSettingValueAttribute("Light")]
    public string Theme
    {
        get
        {
            return ((string)(this["Theme"]));
        }
        set
        {
            this["Theme"] = value;
        }
    }
}
