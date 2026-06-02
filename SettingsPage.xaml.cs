using System.Globalization;

namespace SpecialCalculator;

public partial class SettingsPage : ContentPage
{
    private readonly CultureInfo _fiCulture = CultureInfo.GetCultureInfo("fi-FI");
    private readonly Dictionary<string, Entry> _hourlyEntries = [];
    private readonly Dictionary<string, Entry> _mileageEntries = [];
    private CalculatorSettings _settings = CalculatorSettingsStore.Clone(CalculatorSettingsStore.Defaults);

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        _settings = CalculatorSettingsStore.Load();
        BuildInputs();
    }

    private void ApplyLocalization()
    {
        Title = LocalizationService.T("settings");
        AdminTitleLabel.Text = LocalizationService.T("admin_settings_title");
        OpenAppSettingsButton.Text = LocalizationService.T("app_settings");
        HourlyRatesLabel.Text = LocalizationService.T("hourly_rates");
        MileageRatesLabel.Text = LocalizationService.T("mileage_rates");
        VatPercentLabel.Text = $"{LocalizationService.T("vat_label")} (%)";
        SaveRatesButton.Text = LocalizationService.T("save");
        ResetDefaultsButton.Text = LocalizationService.T("reset_defaults");
    }

    private void BuildInputs()
    {
        _hourlyEntries.Clear();
        _mileageEntries.Clear();
        HourlySettingsRows.Children.Clear();
        MileageSettingsRows.Children.Clear();

        foreach (RateItem rate in _settings.HourlyRates)
        {
            Entry entry = CreateEntry(rate.Value);
            _hourlyEntries[rate.Id] = entry;
            HourlySettingsRows.Children.Add(CreateRow(rate.Name, entry));
        }

        foreach (RateItem rate in _settings.MileageRates)
        {
            Entry entry = CreateEntry(rate.Value);
            _mileageEntries[rate.Id] = entry;
            MileageSettingsRows.Children.Add(CreateRow(rate.Name, entry));
        }

        VatSettingsEntry.Text = _settings.VatPercent.ToString("0.##", _fiCulture);
        SettingsValidationLabel.IsVisible = false;
        SettingsValidationLabel.Text = string.Empty;
    }

    private Entry CreateEntry(decimal value)
    {
        return new Entry
        {
            Keyboard = Keyboard.Numeric,
            Text = value.ToString("0.##", _fiCulture),
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#153D2B"),
            PlaceholderColor = Color.FromArgb("#90A39A"),
            FontSize = 15,
            HeightRequest = 36,
            HorizontalTextAlignment = TextAlignment.End
        };
    }

    private Grid CreateRow(string label, Entry entry)
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
            TextColor = Color.FromArgb("#2F4736"),
            VerticalOptions = LayoutOptions.Center
        });
        row.Add(entry, 1, 0);
        return row;
    }

    private async void OnSaveSettingsClicked(object? sender, EventArgs e)
    {
        if (!TryCollectSettings(out CalculatorSettings? updated, out string validation))
        {
            SettingsValidationLabel.Text = validation;
            SettingsValidationLabel.IsVisible = true;
            return;
        }

        CalculatorSettingsStore.Save(updated!);
        SettingsValidationLabel.Text = LocalizationService.T("saved");
        SettingsValidationLabel.IsVisible = true;
        await Task.Delay(300);
        await Shell.Current.GoToAsync("..");
    }

    private void OnResetDefaultsClicked(object? sender, EventArgs e)
    {
        CalculatorSettingsStore.ResetToDefaults();
        _settings = CalculatorSettingsStore.Clone(CalculatorSettingsStore.Defaults);
        BuildInputs();
        SettingsValidationLabel.Text = LocalizationService.T("defaults_restored");
        SettingsValidationLabel.IsVisible = true;
    }

    private async void OnOpenAppSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AppSettingsPage));
    }

    private bool TryCollectSettings(out CalculatorSettings? settings, out string validation)
    {
        List<RateItem> hourly = [];
        foreach (RateItem rate in _settings.HourlyRates)
        {
            decimal value = ParseDecimal(_hourlyEntries[rate.Id].Text);
            if (value < 0)
            {
                settings = null;
                validation = $"Virheellinen työhinta: {rate.Name}.";
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
                validation = $"Virheellinen matkahinta: {rate.Name}.";
                return false;
            }

            mileage.Add(new RateItem(rate.Id, rate.Name, value));
        }

        decimal vat = ParseDecimal(VatSettingsEntry.Text);
        if (vat < 0)
        {
            settings = null;
            validation = "ALV ei voi olla negatiivinen.";
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
