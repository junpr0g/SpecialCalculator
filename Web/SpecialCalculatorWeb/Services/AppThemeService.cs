using Microsoft.JSInterop;
using SpecialCalculatorWeb.Models;

namespace SpecialCalculatorWeb.Services;

public sealed class AppThemeService(AppPreferencesStore preferences, IJSRuntime jsRuntime)
{
    public event Action? ThemeChanged;

    public bool IsDark => preferences.Theme == ThemeMode.Dark;

    public async Task InitializeAsync()
    {
        await preferences.InitializeAsync();
        await ApplySavedThemeAsync();
    }

    public async Task ApplySavedThemeAsync()
    {
        ThemeMode theme = await preferences.GetThemeAsync();
        await ApplyThemeAsync(theme);
    }

    public async Task SetThemeAsync(ThemeMode theme)
    {
        await preferences.SetThemeAsync(theme);
        await ApplyThemeAsync(theme);
    }

    private async Task ApplyThemeAsync(ThemeMode theme)
    {
        string themeName = theme == ThemeMode.Dark ? "dark" : "light";
        await jsRuntime.InvokeVoidAsync("themeInterop.applyTheme", themeName);
        ThemeChanged?.Invoke();
    }
}
