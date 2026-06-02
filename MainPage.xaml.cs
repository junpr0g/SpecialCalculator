using System.Globalization;

namespace SpecialCalculator;

public partial class MainPage : ContentPage
{
    private const double TwoColumnBreakpoint = 980;
    private const double DropdownRowHeight = 44;
    private readonly CultureInfo _fiCulture = CultureInfo.GetCultureInfo("fi-FI");
    private List<RateItem> _hourlyRates = [];
    private List<RateItem> _mileageRates = [];
    private decimal _vatPercent;
    private int _selectedHourlyIndex;
    private int _selectedMileageIndex = 1;

    public MainPage()
    {
        InitializeComponent();
        ApplyLocalization();
        ReloadSettingsFromStore();
        ApplyResponsiveLayout(Width);
        CalculateAndRender();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        ReloadSettingsFromStore();
        CalculateAndRender();
    }

    private void ApplyLocalization()
    {
        HeaderTitleLabel.Text = LocalizationService.T("header_title");
        HeaderSubtitleLabel.Text = LocalizationService.T("header_subtitle");
        OpenSettingsButton.Text = LocalizationService.T("settings");
        ResultsTitleLabel.Text = LocalizationService.T("results_title");
        WorkCaptionLabel.Text = LocalizationService.T("work_net");
        TravelCaptionLabel.Text = LocalizationService.T("travel_net");
        SubtotalCaptionLabel.Text = LocalizationService.T("subtotal_net");
        TotalCaptionLabel.Text = LocalizationService.T("total");
        InputTitleLabel.Text = LocalizationService.T("input_title");
        ClearInputsButton.Text = LocalizationService.T("clear");
        WorkTypeLabel.Text = LocalizationService.T("select_work");
        TravelTypeLabel.Text = LocalizationService.T("select_travel");
        HoursInputLabel.Text = LocalizationService.T("hours");
        KilometersInputLabel.Text = LocalizationService.T("kilometers");
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ApplyResponsiveLayout(width);
    }

    private void ReloadSettingsFromStore()
    {
        CalculatorSettings settings = CalculatorSettingsStore.Load();
        _hourlyRates = settings.HourlyRates;
        _mileageRates = settings.MileageRates;
        _vatPercent = settings.VatPercent;

        if (_hourlyRates.Count == 0 || _mileageRates.Count == 0)
        {
            CalculatorSettings fallback = CalculatorSettingsStore.Clone(CalculatorSettingsStore.Defaults);
            _hourlyRates = fallback.HourlyRates;
            _mileageRates = fallback.MileageRates;
            _vatPercent = fallback.VatPercent;
        }

        _selectedHourlyIndex = Math.Clamp(_selectedHourlyIndex, 0, _hourlyRates.Count - 1);
        _selectedMileageIndex = Math.Clamp(_selectedMileageIndex, 0, _mileageRates.Count - 1);

        BuildDropdownOptions(HourlyOptionsPanel, _hourlyRates, "h", SelectHourlyOption);
        BuildDropdownOptions(MileageOptionsPanel, _mileageRates, "km", SelectMileageOption);
        RefreshSelectedLabels();
    }

    private void ApplyResponsiveLayout(double width)
    {
        bool shouldUseTwoColumns = width >= TwoColumnBreakpoint;
        CardsGrid.RowDefinitions.Clear();
        CardsGrid.ColumnDefinitions.Clear();

        if (shouldUseTwoColumns)
        {
            CardsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            CardsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            CardsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(ResultsCard, 0);
            Grid.SetRow(ResultsCard, 0);
            Grid.SetColumn(InputCard, 1);
            Grid.SetRow(InputCard, 0);
        }
        else
        {
            CardsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            CardsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            CardsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(ResultsCard, 0);
            Grid.SetRow(ResultsCard, 0);
            Grid.SetColumn(InputCard, 0);
            Grid.SetRow(InputCard, 1);
        }
    }

    private static void BuildDropdownOptions(
        VerticalStackLayout panel,
        IReadOnlyList<RateItem> rates,
        string unit,
        Action<RateItem> onSelected)
    {
        panel.Children.Clear();

        foreach (RateItem rate in rates)
        {
            Grid row = new()
            {
                Padding = new Thickness(14, 10),
                BackgroundColor = Color.FromArgb("#F2F8F2"),
                HeightRequest = DropdownRowHeight
            };
            row.Children.Add(new Label
            {
                Text = $"{rate.Value.ToString("0.00", CultureInfo.GetCultureInfo("fi-FI"))} €/ {unit} - {rate.Name}",
                TextColor = Color.FromArgb("#0E3726"),
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            });

            TapGestureRecognizer tap = new();
            RateItem captured = rate;
            tap.Tapped += (_, _) => onSelected(captured);
            row.GestureRecognizers.Add(tap);
            panel.Children.Add(row);
        }
    }

    private void RefreshSelectedLabels()
    {
        HourlySelectedLabel.Text = BuildHeaderLabel(_hourlyRates[_selectedHourlyIndex], "h");
        MileageSelectedLabel.Text = BuildHeaderLabel(_mileageRates[_selectedMileageIndex], "km");
        string vatToken = LocalizationService.T("vat_label");
        VatCaptionLabel.Text = $"{vatToken} ({_vatPercent.ToString("0.##", _fiCulture)} %)";
    }

    private static string BuildHeaderLabel(RateItem rate, string unit)
    {
        return $"{rate.Value.ToString("0.00", CultureInfo.GetCultureInfo("fi-FI"))} €/ {unit} - {rate.Name}";
    }

    private void DismissKeyboard()
    {
        if (HoursEntry.IsFocused) HoursEntry.Unfocus();
        if (KilometersEntry.IsFocused) KilometersEntry.Unfocus();

#if ANDROID
        var activity = Platform.CurrentActivity;
        if (activity?.CurrentFocus is Android.Views.View focusView)
        {
            var inputManager = (Android.Views.InputMethods.InputMethodManager?)
                activity.GetSystemService(Android.Content.Context.InputMethodService);
            inputManager?.HideSoftInputFromWindow(
                focusView.WindowToken,
                Android.Views.InputMethods.HideSoftInputFlags.None);
            focusView.ClearFocus();
        }
#endif

    }

    private async void OnOpenSettingsClicked(object? sender, EventArgs e)
    {
        HourlyOptionsBorder.IsVisible = false;
        MileageOptionsBorder.IsVisible = false;
        DismissKeyboard();
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private void OnHourlyDropdownTapped(object? sender, EventArgs e)
    {
        DismissKeyboard();
        MileageOptionsBorder.IsVisible = false;
        HourlyOptionsBorder.IsVisible = !HourlyOptionsBorder.IsVisible;
    }

    private void OnMileageDropdownTapped(object? sender, EventArgs e)
    {
        DismissKeyboard();
        HourlyOptionsBorder.IsVisible = false;
        MileageOptionsBorder.IsVisible = !MileageOptionsBorder.IsVisible;
    }

    private void SelectHourlyOption(RateItem selected)
    {
        _selectedHourlyIndex = _hourlyRates.FindIndex(x => x.Id == selected.Id);
        if (_selectedHourlyIndex < 0) _selectedHourlyIndex = 0;
        RefreshSelectedLabels();
        HourlyOptionsBorder.IsVisible = false;
        CalculateAndRender();
    }

    private void SelectMileageOption(RateItem selected)
    {
        _selectedMileageIndex = _mileageRates.FindIndex(x => x.Id == selected.Id);
        if (_selectedMileageIndex < 0) _selectedMileageIndex = 0;
        RefreshSelectedLabels();
        MileageOptionsBorder.IsVisible = false;
        CalculateAndRender();
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        CalculateAndRender();
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        HoursEntry.Text = string.Empty;
        KilometersEntry.Text = string.Empty;
        _selectedHourlyIndex = 0;
        _selectedMileageIndex = Math.Min(1, _mileageRates.Count - 1);
        RefreshSelectedLabels();
        HourlyOptionsBorder.IsVisible = false;
        MileageOptionsBorder.IsVisible = false;
        ValidationLabel.Text = string.Empty;
        ValidationLabel.IsVisible = false;
        CalculateAndRender();
    }

    private void CalculateAndRender()
    {
        if (!TryValidate(out decimal hours, out decimal kilometers, out string validationMessage))
        {
            ValidationLabel.Text = validationMessage;
            ValidationLabel.IsVisible = true;
            return;
        }

        ValidationLabel.Text = string.Empty;
        ValidationLabel.IsVisible = false;

        decimal hourlyRate = GetSelectedRate(_hourlyRates, _selectedHourlyIndex, 0);
        decimal mileageRate = GetSelectedRate(_mileageRates, _selectedMileageIndex, 0);
        decimal workCost = hours * hourlyRate;
        decimal travelCost = kilometers * mileageRate;
        decimal subtotal = workCost + travelCost;
        decimal vatAmount = subtotal * (_vatPercent / 100m);
        decimal total = subtotal + vatAmount;

        WorkValueLabel.Text = FormatEuro(workCost);
        TravelValueLabel.Text = FormatEuro(travelCost);
        SubtotalValueLabel.Text = FormatEuro(subtotal);
        VatValueLabel.Text = FormatEuro(vatAmount);
        TotalValueLabel.Text = FormatEuro(total);
    }

    private bool TryValidate(out decimal hours, out decimal kilometers, out string message)
    {
        hours = ParseDecimal(HoursEntry.Text);
        kilometers = ParseDecimal(KilometersEntry.Text);

        if (hours < 0)
        {
            message = "Työtunnit ei voi olla negatiivinen.";
            return false;
        }

        if (kilometers < 0)
        {
            message = "Ajokilometrit ei voi olla negatiivinen.";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private static decimal ParseDecimal(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0m;
        string normalized = text.Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
            ? value
            : 0m;
    }

    private static decimal GetSelectedRate(IReadOnlyList<RateItem> rates, int selectedIndex, int fallbackIndex)
    {
        if (rates.Count == 0) return 0m;
        if (selectedIndex >= 0 && selectedIndex < rates.Count) return rates[selectedIndex].Value;
        return rates[Math.Clamp(fallbackIndex, 0, rates.Count - 1)].Value;
    }

    private string FormatEuro(decimal value) => string.Format(_fiCulture, "{0:C2}", value);
}
