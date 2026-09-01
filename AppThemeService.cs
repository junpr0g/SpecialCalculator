namespace SpecialCalculator;

public static class AppThemeService
{
    private static readonly (string ThemedKey, string LightKey, string DarkKey)[] ColorMap =
    [
        ("CalcPageBackground", "CalcPageBackgroundLight", "CalcPageBackgroundDark"),
        ("CalcCardBackground", "CalcCardBackgroundLight", "CalcCardBackgroundDark"),
        ("CalcCardAltBackground", "CalcCardAltBackgroundLight", "CalcCardAltBackgroundDark"),
        ("CalcHeaderBackground", "CalcHeaderBackgroundLight", "CalcHeaderBackgroundDark"),
        ("CalcStroke", "CalcStrokeLight", "CalcStrokeDark"),
        ("CalcAccentStroke", "CalcAccentStrokeLight", "CalcAccentStrokeDark"),
        ("CalcPrimaryText", "CalcPrimaryTextLight", "CalcPrimaryTextDark"),
        ("CalcSecondaryText", "CalcSecondaryTextLight", "CalcSecondaryTextDark"),
        ("CalcBodyText", "CalcBodyTextLight", "CalcBodyTextDark"),
        ("CalcInputText", "CalcInputTextLight", "CalcInputTextDark"),
        ("CalcInputBackground", "CalcInputBackgroundLight", "CalcInputBackgroundDark"),
        ("CalcDropdownBackground", "CalcDropdownBackgroundLight", "CalcDropdownBackgroundDark"),
        ("CalcButtonBackground", "CalcButtonBackgroundLight", "CalcButtonBackgroundDark"),
        ("CalcPlaceholder", "CalcPlaceholderLight", "CalcPlaceholderDark"),
        ("CalcDropdownOptionText", "CalcDropdownOptionTextLight", "CalcDropdownOptionTextDark"),
        ("CalcTotalBand", "CalcTotalBandLight", "CalcTotalBandDark"),
        ("CalcTabBarBackground", "CalcTabBarBackgroundLight", "CalcTabBarBackgroundDark"),
        ("CalcAdminTabActiveBg", "CalcAdminTabActiveBgLight", "CalcAdminTabActiveBgDark"),
        ("CalcAppTabActiveBg", "CalcAppTabActiveBgLight", "CalcAppTabActiveBgDark"),
        ("CalcAppTabActiveText", "CalcAppTabActiveTextLight", "CalcAppTabActiveTextDark"),
        ("CalcAdminPanelBg", "CalcAdminPanelBgLight", "CalcAdminPanelBgDark"),
        ("CalcAppPanelBg", "CalcAppPanelBgLight", "CalcAppPanelBgDark"),
        ("CalcAdminInputBg", "CalcAdminInputBgLight", "CalcAdminInputBgDark"),
        ("CalcAppInputBg", "CalcAppInputBgLight", "CalcAppInputBgDark"),
        ("CalcAppInputStroke", "CalcAppInputStrokeLight", "CalcAppInputStrokeDark"),
        ("CalcAppPrimaryText", "CalcAppPrimaryTextLight", "CalcAppPrimaryTextDark"),
        ("CalcAppBodyText", "CalcAppBodyTextLight", "CalcAppBodyTextDark"),
        ("CalcAppPickerText", "CalcAppPickerTextLight", "CalcAppPickerTextDark"),
        ("CalcPrimaryButton", "CalcPrimaryButtonLight", "CalcPrimaryButtonDark"),
        ("CalcAppPrimaryButton", "CalcAppPrimaryButtonLight", "CalcAppPrimaryButtonDark"),
        ("CalcError", "CalcErrorLight", "CalcErrorDark"),
        ("CalcSuccess", "CalcSuccessLight", "CalcSuccessDark"),
        ("CalcAppSuccess", "CalcAppSuccessLight", "CalcAppSuccessDark")
    ];

    public static event EventHandler? ThemeChanged;

    public static bool IsDark =>
        Application.Current?.RequestedTheme == AppTheme.Dark;

    public static void ApplySavedTheme()
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.UserAppTheme = AppPreferencesStore.GetTheme();
        OnApplicationThemeChanged();
    }

    public static void OnApplicationThemeChanged()
    {
        SyncThemedResources();
        ThemeChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void SetTheme(AppTheme theme)
    {
        AppPreferencesStore.SetTheme(theme);

        if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = theme;
        }

        OnApplicationThemeChanged();
    }

    public static void SyncThemedResources()
    {
        if (Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        foreach ((string themedKey, string lightKey, string darkKey) in ColorMap)
        {
            string sourceKey = IsDark ? darkKey : lightKey;
            if (resources.TryGetValue(sourceKey, out object? value) && value is Color color)
            {
                resources[themedKey] = color;
            }
        }
    }

    public static Color Resolve(string themedKey, string lightKey, string darkKey)
    {
        if (Application.Current?.Resources is ResourceDictionary resources)
        {
            string sourceKey = IsDark ? darkKey : lightKey;
            if (resources.TryGetValue(sourceKey, out object? value) && value is Color color)
            {
                return color;
            }
        }

        return Colors.Transparent;
    }

    public static Color PageBackground => Resolve("CalcPageBackground", "CalcPageBackgroundLight", "CalcPageBackgroundDark");
    public static Color CardBackground => Resolve("CalcCardBackground", "CalcCardBackgroundLight", "CalcCardBackgroundDark");
    public static Color CardAltBackground => Resolve("CalcCardAltBackground", "CalcCardAltBackgroundLight", "CalcCardAltBackgroundDark");
    public static Color HeaderBackground => Resolve("CalcHeaderBackground", "CalcHeaderBackgroundLight", "CalcHeaderBackgroundDark");
    public static Color Stroke => Resolve("CalcStroke", "CalcStrokeLight", "CalcStrokeDark");
    public static Color AccentStroke => Resolve("CalcAccentStroke", "CalcAccentStrokeLight", "CalcAccentStrokeDark");
    public static Color PrimaryText => Resolve("CalcPrimaryText", "CalcPrimaryTextLight", "CalcPrimaryTextDark");
    public static Color SecondaryText => Resolve("CalcSecondaryText", "CalcSecondaryTextLight", "CalcSecondaryTextDark");
    public static Color BodyText => Resolve("CalcBodyText", "CalcBodyTextLight", "CalcBodyTextDark");
    public static Color InputText => Resolve("CalcInputText", "CalcInputTextLight", "CalcInputTextDark");
    public static Color InputBackground => Resolve("CalcInputBackground", "CalcInputBackgroundLight", "CalcInputBackgroundDark");
    public static Color DropdownBackground => Resolve("CalcDropdownBackground", "CalcDropdownBackgroundLight", "CalcDropdownBackgroundDark");
    public static Color ButtonBackground => Resolve("CalcButtonBackground", "CalcButtonBackgroundLight", "CalcButtonBackgroundDark");
    public static Color Placeholder => Resolve("CalcPlaceholder", "CalcPlaceholderLight", "CalcPlaceholderDark");
    public static Color DropdownOptionText => Resolve("CalcDropdownOptionText", "CalcDropdownOptionTextLight", "CalcDropdownOptionTextDark");
    public static Color TotalBand => Resolve("CalcTotalBand", "CalcTotalBandLight", "CalcTotalBandDark");
    public static Color TabBarBackground => Resolve("CalcTabBarBackground", "CalcTabBarBackgroundLight", "CalcTabBarBackgroundDark");
    public static Color AdminTabActiveBg => Resolve("CalcAdminTabActiveBg", "CalcAdminTabActiveBgLight", "CalcAdminTabActiveBgDark");
    public static Color AppTabActiveBg => Resolve("CalcAppTabActiveBg", "CalcAppTabActiveBgLight", "CalcAppTabActiveBgDark");
    public static Color AppTabActiveText => Resolve("CalcAppTabActiveText", "CalcAppTabActiveTextLight", "CalcAppTabActiveTextDark");
    public static Color AdminPanelBg => Resolve("CalcAdminPanelBg", "CalcAdminPanelBgLight", "CalcAdminPanelBgDark");
    public static Color AppPanelBg => Resolve("CalcAppPanelBg", "CalcAppPanelBgLight", "CalcAppPanelBgDark");
    public static Color AdminInputBg => Resolve("CalcAdminInputBg", "CalcAdminInputBgLight", "CalcAdminInputBgDark");
    public static Color AppInputBg => Resolve("CalcAppInputBg", "CalcAppInputBgLight", "CalcAppInputBgDark");
    public static Color AppInputStroke => Resolve("CalcAppInputStroke", "CalcAppInputStrokeLight", "CalcAppInputStrokeDark");
    public static Color AppPrimaryText => Resolve("CalcAppPrimaryText", "CalcAppPrimaryTextLight", "CalcAppPrimaryTextDark");
    public static Color AppBodyText => Resolve("CalcAppBodyText", "CalcAppBodyTextLight", "CalcAppBodyTextDark");
    public static Color AppPickerText => Resolve("CalcAppPickerText", "CalcAppPickerTextLight", "CalcAppPickerTextDark");
    public static Color PrimaryButton => Resolve("CalcPrimaryButton", "CalcPrimaryButtonLight", "CalcPrimaryButtonDark");
    public static Color AppPrimaryButton => Resolve("CalcAppPrimaryButton", "CalcAppPrimaryButtonLight", "CalcAppPrimaryButtonDark");
    public static Color Error => Resolve("CalcError", "CalcErrorLight", "CalcErrorDark");
    public static Color Success => Resolve("CalcSuccess", "CalcSuccessLight", "CalcSuccessDark");
    public static Color AppSuccess => Resolve("CalcAppSuccess", "CalcAppSuccessLight", "CalcAppSuccessDark");
}
