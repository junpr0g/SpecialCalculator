namespace SpecialCalculator;

public static class AppPreferencesStore
{
    private const string LanguageKey = "app_language";
    private const string ThemeKey = "app_theme";
    private const string NotificationsKey = "app_notifications_enabled";

    public static string GetLanguage()
    {
        return Preferences.Default.Get(LanguageKey, "fi");
    }

    public static void SetLanguage(string languageCode)
    {
        Preferences.Default.Set(LanguageKey, languageCode);
    }

    public static AppTheme GetTheme()
    {
        string stored = Preferences.Default.Get(ThemeKey, "light");
        return stored switch
        {
            "dark" => AppTheme.Dark,
            _ => AppTheme.Light
        };
    }

    public static void SetTheme(AppTheme theme)
    {
        string stored = theme == AppTheme.Dark ? "dark" : "light";
        Preferences.Default.Set(ThemeKey, stored);
    }

    public static bool GetNotificationsEnabled()
    {
        return Preferences.Default.Get(NotificationsKey, true);
    }

    public static void SetNotificationsEnabled(bool enabled)
    {
        Preferences.Default.Set(NotificationsKey, enabled);
    }
}
