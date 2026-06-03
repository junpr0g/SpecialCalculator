using SpecialCalculatorWeb.Models;

namespace SpecialCalculatorWeb.Services;

public sealed class AppPreferencesStore(ILocalStorageService storage)
{
    private const string LanguageKey = "app_language";
    private const string ThemeKey = "app_theme";
    private const string NotificationsKey = "app_notifications_enabled";

    private string _language = "fi";
    private ThemeMode _theme = ThemeMode.Light;
    private bool _notificationsEnabled = true;
    private bool _initialized;

    public event Action? PreferencesChanged;

    public string Language => _language;
    public ThemeMode Theme => _theme;
    public bool NotificationsEnabled => _notificationsEnabled;

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        _language = await GetLanguageAsync();
        _theme = await GetThemeAsync();
        _notificationsEnabled = await GetNotificationsEnabledAsync();
        _initialized = true;
    }

    public async Task<string> GetLanguageAsync()
    {
        if (!await storage.ContainsKeyAsync(LanguageKey))
        {
            return "fi";
        }

        string? value = await storage.GetItemAsync(LanguageKey);
        return string.IsNullOrWhiteSpace(value) ? "fi" : value;
    }

    public async Task SetLanguageAsync(string languageCode)
    {
        await storage.SetItemAsync(LanguageKey, languageCode);
        _language = languageCode;
        PreferencesChanged?.Invoke();
    }

    public async Task<ThemeMode> GetThemeAsync()
    {
        if (!await storage.ContainsKeyAsync(ThemeKey))
        {
            return ThemeMode.Light;
        }

        string? stored = await storage.GetItemAsync(ThemeKey);
        return stored switch
        {
            "dark" => ThemeMode.Dark,
            _ => ThemeMode.Light
        };
    }

    public async Task SetThemeAsync(ThemeMode theme)
    {
        string stored = theme == ThemeMode.Dark ? "dark" : "light";
        await storage.SetItemAsync(ThemeKey, stored);
        _theme = theme;
        PreferencesChanged?.Invoke();
    }

    public async Task<bool> GetNotificationsEnabledAsync()
    {
        if (!await storage.ContainsKeyAsync(NotificationsKey))
        {
            return true;
        }

        string? value = await storage.GetItemAsync(NotificationsKey);
        return value is not "false";
    }

    public async Task SetNotificationsEnabledAsync(bool enabled)
    {
        await storage.SetItemAsync(NotificationsKey, enabled ? "true" : "false");
        _notificationsEnabled = enabled;
        PreferencesChanged?.Invoke();
    }
}
