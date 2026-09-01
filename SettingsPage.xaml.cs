using System.Globalization;
using Microsoft.Maui.Controls.Shapes;

namespace SpecialCalculator;

public partial class SettingsPage : ContentPage
{
    private const int AdminTabIndex = 0;
    private const int AppTabIndex = 1;

    private readonly CultureInfo _fiCulture = CultureInfo.GetCultureInfo("fi-FI");
    private readonly Dictionary<string, Entry> _hourlyEntries = [];
    private readonly Dictionary<string, Entry> _mileageEntries = [];

    private CalculatorSettings _settings = CalculatorSettingsStore.Clone(CalculatorSettingsStore.Defaults);
    private bool _panelsInitialized;
    private bool _suppressAdminPersist;
    private bool _suppressPickerEvents;
    private int _selectedTabIndex = AdminTabIndex;

    private ScrollView? _adminPanel;
    private ScrollView? _appPanel;
    private Entry? _vatSettingsEntry;
    private Label? _adminStatusLabel;
    private Picker? _languagePicker;
    private Picker? _themePicker;

    public SettingsPage()
    {
        InitializeComponent();
        UpdateTabVisualState(AdminTabIndex);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AppThemeService.ThemeChanged += OnAppThemeChanged;
        AppThemeService.ApplySavedTheme();
        RebuildPanels();
    }

    protected override void OnDisappearing()
    {
        AppThemeService.ThemeChanged -= OnAppThemeChanged;
        base.OnDisappearing();
    }

    private void OnAppThemeChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RebuildPanels);
    }

    private void RebuildPanels()
    {
        int position = _panelsInitialized ? _selectedTabIndex : AdminTabIndex;
        _panelsInitialized = false;
        _adminPanel = null;
        _appPanel = null;
        _vatSettingsEntry = null;
        _adminStatusLabel = null;
        _languagePicker = null;
        _themePicker = null;
        SettingsContentHost.Children.Clear();
        EnsurePanelsInitialized();
        SelectTab(position);
        ApplyLocalization();
        _settings = CalculatorSettingsStore.Load();
        BuildAdminInputs();
        BuildAppPickers();
    }

    private void EnsurePanelsInitialized()
    {
        if (_panelsInitialized)
        {
            return;
        }

        _adminPanel = CreateAdminPanel();
        _appPanel = CreateAppPanel();
        SettingsContentHost.Children.Add(_adminPanel);
        SettingsContentHost.Children.Add(_appPanel);
        _panelsInitialized = true;
    }

    private ScrollView CreateAdminPanel()
    {
        var hourlyRows = new VerticalStackLayout { Spacing = 8 };
        var mileageRows = new VerticalStackLayout { Spacing = 8 };

        _vatSettingsEntry = CreateAdminEntry(0m);
        _vatSettingsEntry.Placeholder = "25,5";
        _vatSettingsEntry.HeightRequest = 44;
        _vatSettingsEntry.HorizontalTextAlignment = TextAlignment.Start;
        _vatSettingsEntry.TextChanged += OnAdminSettingTextChanged;

        var resetButton = new Button
        {
            BackgroundColor = Colors.Transparent,
            TextColor = AppThemeService.PrimaryText,
            BorderColor = AppThemeService.AccentStroke,
            BorderWidth = 1,
            CornerRadius = 8,
            HeightRequest = 38,
            FontSize = 13,
            Padding = new Thickness(14, 0)
        };
        resetButton.Clicked += OnResetDefaultsClicked;

        _adminStatusLabel = new Label
        {
            TextColor = AppThemeService.Success,
            FontSize = 12,
            IsVisible = false
        };

        HourlySettingsRows = hourlyRows;
        MileageSettingsRows = mileageRows;

        var content = new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                CreateTitleLabel(LocalizationService.T("admin_settings_title"), AppThemeService.PrimaryText),
                CreateSectionLabel(LocalizationService.T("hourly_rates")),
                hourlyRows,
                CreateSectionLabel(LocalizationService.T("mileage_rates"), topMargin: 8),
                mileageRows,
                CreateSectionLabel($"{LocalizationService.T("vat_label")} (%)", topMargin: 8),
                CreateSageInputBorder(_vatSettingsEntry),
                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 10,
                    Margin = new Thickness(0, 10, 0, 0),
                    Children = { resetButton }
                },
                _adminStatusLabel
            }
        };

        resetButton.Text = LocalizationService.T("reset_defaults");

        return new ScrollView
        {
            Content = CreateCard(content, AppThemeService.AdminPanelBg, AppThemeService.Stroke)
        };
    }

    private ScrollView CreateAppPanel()
    {
        _languagePicker = CreatePicker();
        _languagePicker.SelectedIndexChanged += OnLanguagePickerChanged;
        _themePicker = CreatePicker();
        _themePicker.SelectedIndexChanged += OnThemePickerChanged;

        var content = new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                CreateTitleLabel(LocalizationService.T("app_settings_title"), AppThemeService.AppPrimaryText),
                CreateSectionLabel(LocalizationService.T("language"), AppThemeService.AppBodyText),
                CreateBlueInputBorder(_languagePicker),
                CreateSectionLabel(LocalizationService.T("theme"), AppThemeService.AppBodyText, topMargin: 8),
                CreateBlueInputBorder(_themePicker)
            }
        };

        return new ScrollView
        {
            Content = CreateCard(content, AppThemeService.AppPanelBg, AppThemeService.AppInputStroke)
        };
    }

    private VerticalStackLayout HourlySettingsRows { get; set; } = null!;
    private VerticalStackLayout MileageSettingsRows { get; set; } = null!;

    private static Label CreateTitleLabel(string text, Color color) =>
        new()
        {
            Text = text,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = color
        };

    private static Label CreateSectionLabel(string text, Color? color = null, double topMargin = 0) =>
        new()
        {
            Text = text,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = color ?? AppThemeService.BodyText,
            Margin = new Thickness(0, topMargin, 0, 0)
        };

    private static Picker CreatePicker() =>
        new()
        {
            BackgroundColor = Colors.Transparent,
            TextColor = AppThemeService.AppPickerText,
            TitleColor = AppThemeService.SecondaryText,
            FontSize = 15,
            HeightRequest = 44
        };

    private static Border CreateSageInputBorder(View content) =>
        new()
        {
            BackgroundColor = AppThemeService.AdminInputBg,
            Stroke = AppThemeService.AccentStroke,
            StrokeThickness = 1.2,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Padding = new Thickness(10, 4),
            Content = content
        };

    private static Border CreateBlueInputBorder(View content) =>
        new()
        {
            BackgroundColor = AppThemeService.AppInputBg,
            Stroke = AppThemeService.AppInputStroke,
            StrokeThickness = 1.2,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Padding = new Thickness(10, 4),
            Content = content
        };

    private static Border CreateCard(View content, Color background, Color stroke) =>
        new()
        {
            BackgroundColor = background,
            Stroke = stroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Padding = 16,
            Margin = new Thickness(0, 0, 0, 8),
            Content = content
        };

    private void ApplyLocalization()
    {
        Title = LocalizationService.T("settings");
        AdminTabButton.Text = LocalizationService.T("tab_admin");
        AppTabButton.Text = LocalizationService.T("tab_app");
    }

    private void OnAdminTabClicked(object? sender, EventArgs e) => SelectTab(AdminTabIndex);

    private void OnAppTabClicked(object? sender, EventArgs e) => SelectTab(AppTabIndex);

    private void SelectTab(int index)
    {
        EnsurePanelsInitialized();

        if (_adminPanel is null || _appPanel is null)
        {
            return;
        }

        _selectedTabIndex = index;
        _adminPanel.IsVisible = index == AdminTabIndex;
        _appPanel.IsVisible = index == AppTabIndex;
        UpdateTabVisualState(index);
    }

    private void UpdateTabVisualState(int selectedIndex)
    {
        bool adminSelected = selectedIndex == AdminTabIndex;
        AdminTabButton.BackgroundColor = adminSelected ? AppThemeService.AdminTabActiveBg : Colors.Transparent;
        AdminTabButton.TextColor = adminSelected ? AppThemeService.PrimaryText : AppThemeService.SecondaryText;
        AdminTabButton.FontAttributes = adminSelected ? FontAttributes.Bold : FontAttributes.None;

        bool appSelected = selectedIndex == AppTabIndex;
        AppTabButton.BackgroundColor = appSelected ? AppThemeService.AppTabActiveBg : Colors.Transparent;
        AppTabButton.TextColor = appSelected ? AppThemeService.AppTabActiveText : AppThemeService.SecondaryText;
        AppTabButton.FontAttributes = appSelected ? FontAttributes.Bold : FontAttributes.None;
    }

    private void BuildAdminInputs()
    {
        _suppressAdminPersist = true;
        _hourlyEntries.Clear();
        _mileageEntries.Clear();
        HourlySettingsRows.Children.Clear();
        MileageSettingsRows.Children.Clear();

        foreach (RateItem rate in _settings.HourlyRates)
        {
            Entry entry = CreateAdminEntry(rate.Value);
            entry.TextChanged += OnAdminSettingTextChanged;
            _hourlyEntries[rate.Id] = entry;
            HourlySettingsRows.Children.Add(CreateAdminRow(LocalizationService.RateName(rate), entry));
        }

        foreach (RateItem rate in _settings.MileageRates)
        {
            Entry entry = CreateAdminEntry(rate.Value);
            entry.TextChanged += OnAdminSettingTextChanged;
            _mileageEntries[rate.Id] = entry;
            MileageSettingsRows.Children.Add(CreateAdminRow(LocalizationService.RateName(rate), entry));
        }

        if (_vatSettingsEntry is not null)
        {
            _vatSettingsEntry.Text = _settings.VatPercent.ToString("0.##", _fiCulture);
        }

        if (_adminStatusLabel is not null)
        {
            _adminStatusLabel.IsVisible = false;
            _adminStatusLabel.Text = string.Empty;
        }

        _suppressAdminPersist = false;
    }

    private void BuildAppPickers()
    {
        if (_languagePicker is null || _themePicker is null)
        {
            return;
        }

        _suppressPickerEvents = true;
        _languagePicker.ItemsSource = new List<string>
        {
            LocalizationService.T("language_fi"),
            LocalizationService.T("language_en")
        };
        _languagePicker.SelectedIndex = AppPreferencesStore.GetLanguage() == "en" ? 1 : 0;

        _themePicker.ItemsSource = new List<string>
        {
            LocalizationService.T("theme_light"),
            LocalizationService.T("theme_dark")
        };
        _themePicker.SelectedIndex = AppPreferencesStore.GetTheme() == AppTheme.Dark ? 1 : 0;
        _suppressPickerEvents = false;
    }

    private static Entry CreateAdminEntry(decimal value) =>
        new()
        {
            Keyboard = Keyboard.Numeric,
            Text = value == 0m ? string.Empty : value.ToString("0.##", CultureInfo.GetCultureInfo("fi-FI")),
            BackgroundColor = Colors.Transparent,
            TextColor = AppThemeService.InputText,
            PlaceholderColor = AppThemeService.Placeholder,
            FontSize = 15,
            HeightRequest = 36,
            HorizontalTextAlignment = TextAlignment.End
        };

    private static Grid CreateAdminRow(string label, Entry entry)
    {
        Grid row = new()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 10
        };

        row.Add(new Label
        {
            Text = label,
            FontSize = 13,
            TextColor = AppThemeService.BodyText,
            VerticalOptions = LayoutOptions.Center
        });
        row.Add(entry, 1, 0);
        return row;
    }

    private void OnAdminSettingTextChanged(object? sender, TextChangedEventArgs e) =>
        PersistAdminSettingsIfValid();

    private void OnLanguagePickerChanged(object? sender, EventArgs e)
    {
        if (_suppressPickerEvents || _languagePicker is null || _languagePicker.SelectedIndex < 0)
        {
            return;
        }

        string language = _languagePicker.SelectedIndex == 1 ? "en" : "fi";
        if (AppPreferencesStore.GetLanguage() == language)
        {
            return;
        }

        AppPreferencesStore.SetLanguage(language);
        ApplyLocalization();
        RefreshAdminRateLabels();
        _suppressPickerEvents = true;
        BuildAppPickers();
        _suppressPickerEvents = false;
    }

    private void OnThemePickerChanged(object? sender, EventArgs e)
    {
        if (_suppressPickerEvents || _themePicker is null || _themePicker.SelectedIndex < 0)
        {
            return;
        }

        AppTheme theme = _themePicker.SelectedIndex == 1 ? AppTheme.Dark : AppTheme.Light;
        if (AppPreferencesStore.GetTheme() == theme)
        {
            return;
        }

        AppThemeService.SetTheme(theme);
    }

    private void RefreshAdminRateLabels()
    {
        for (int i = 0; i < _settings.HourlyRates.Count; i++)
        {
            if (HourlySettingsRows.Children[i] is Grid hourlyRow &&
                hourlyRow.Children.FirstOrDefault() is Label hourlyLabel)
            {
                hourlyLabel.Text = LocalizationService.RateName(_settings.HourlyRates[i]);
            }
        }

        for (int i = 0; i < _settings.MileageRates.Count; i++)
        {
            if (MileageSettingsRows.Children[i] is Grid mileageRow &&
                mileageRow.Children.FirstOrDefault() is Label mileageLabel)
            {
                mileageLabel.Text = LocalizationService.RateName(_settings.MileageRates[i]);
            }
        }
    }

    private void PersistAdminSettingsIfValid()
    {
        if (_suppressAdminPersist || _adminStatusLabel is null)
        {
            return;
        }

        if (!TryCollectAdminSettings(out CalculatorSettings? updated, out string validation))
        {
            _adminStatusLabel.Text = validation;
            _adminStatusLabel.TextColor = AppThemeService.Error;
            _adminStatusLabel.IsVisible = true;
            return;
        }

        CalculatorSettingsStore.Save(updated!);
        _settings = CalculatorSettingsStore.Clone(updated!);
        _adminStatusLabel.IsVisible = false;
        _adminStatusLabel.Text = string.Empty;
    }

    private void OnResetDefaultsClicked(object? sender, EventArgs e)
    {
        if (_adminStatusLabel is null)
        {
            return;
        }

        CalculatorSettingsStore.ResetToDefaults();
        _settings = CalculatorSettingsStore.Clone(CalculatorSettingsStore.Defaults);
        BuildAdminInputs();
        _adminStatusLabel.Text = LocalizationService.T("defaults_restored");
        _adminStatusLabel.TextColor = AppThemeService.Success;
        _adminStatusLabel.IsVisible = true;
    }

    private bool TryCollectAdminSettings(out CalculatorSettings? settings, out string validation)
    {
        if (_vatSettingsEntry is null)
        {
            settings = null;
            validation = LocalizationService.T("invalid_vat");
            return false;
        }

        List<RateItem> hourly = [];
        foreach (RateItem rate in _settings.HourlyRates)
        {
            decimal value = ParseDecimal(_hourlyEntries[rate.Id].Text);
            if (value < 0)
            {
                settings = null;
                validation = LocalizationService.T("invalid_hourly_rate").Replace("{name}", LocalizationService.RateName(rate));
                return false;
            }

            hourly.Add(new RateItem(rate.Id, rate.Name, value));
        }

        List<RateItem> mileage = [];
        foreach (RateItem rate in _settings.MileageRates)
        {
            decimal value = ParseDecimal(_mileageEntries[rate.Id].Text);
            if (value < 0)
            {
                settings = null;
                validation = LocalizationService.T("invalid_mileage_rate").Replace("{name}", LocalizationService.RateName(rate));
                return false;
            }

            mileage.Add(new RateItem(rate.Id, rate.Name, value));
        }

        decimal vat = ParseDecimal(_vatSettingsEntry.Text);
        if (vat < 0)
        {
            settings = null;
            validation = LocalizationService.T("invalid_vat");
            return false;
        }

        settings = new CalculatorSettings
        {
            HourlyRates = hourly,
            MileageRates = mileage,
            VatPercent = vat
        };
        validation = string.Empty;
        return true;
    }

    private static decimal ParseDecimal(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0m;
        }

        string normalized = text.Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
            ? value
            : 0m;
    }
}
