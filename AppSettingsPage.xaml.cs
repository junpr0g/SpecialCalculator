namespace SpecialCalculator;

public partial class AppSettingsPage : ContentPage
{
    public AppSettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        BuildPickers();
    }

    private void ApplyLocalization()
    {
        Title = LocalizationService.T("app_settings_title");
        PageTitleLabel.Text = LocalizationService.T("app_settings_title");
        LanguageLabel.Text = LocalizationService.T("language");
        ThemeLabel.Text = LocalizationService.T("theme");
        SaveButton.Text = LocalizationService.T("save");
    }

    private void BuildPickers()
    {
        LanguagePicker.ItemsSource = new List<string>
        {
            LocalizationService.T("language_fi"),
            LocalizationService.T("language_en")
        };
        LanguagePicker.SelectedIndex = AppPreferencesStore.GetLanguage() == "en" ? 1 : 0;

        ThemePicker.ItemsSource = new List<string>
        {
            LocalizationService.T("theme_light"),
            LocalizationService.T("theme_dark")
        };
        ThemePicker.SelectedIndex = AppPreferencesStore.GetTheme() == AppTheme.Dark ? 1 : 0;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string language = LanguagePicker.SelectedIndex == 1 ? "en" : "fi";
        AppTheme theme = ThemePicker.SelectedIndex == 1 ? AppTheme.Dark : AppTheme.Light;

        AppPreferencesStore.SetLanguage(language);
        AppPreferencesStore.SetTheme(theme);
        if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = theme;
        }

        StatusLabel.Text = LocalizationService.T("saved");
        StatusLabel.IsVisible = true;
        await Task.Delay(250);
        await Shell.Current.GoToAsync("..");
    }
}
