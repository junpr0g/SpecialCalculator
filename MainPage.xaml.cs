using System.Globalization;

namespace SpecialCalculator;

public partial class MainPage : ContentPage
{
    private const decimal VatRate = 0.255m;
    private const double TwoColumnBreakpoint = 980;
    private const double DropdownRowHeight = 44;
    private readonly CultureInfo _fiCulture = CultureInfo.GetCultureInfo("fi-FI");
    private readonly List<RateOption> _hourlyRates =
    [
        new("26,00 €/h - perustalkkari", 26m),
        new("32,00 €/h - digitalkkari", 32m),
        new("33,00 €/h - konetalkkari", 33m),
        new("36,00 €/h - perus ei jäsen", 36m),
        new("42,00 €/h - digi ei jäsen", 42m),
        new("43,00 €/h - kone ei jäsen", 43m)
    ];

    private readonly List<RateOption> _mileageRates =
    [
        new("0,13 €/km - julkinen", 0.13m),
        new("0,55 €/km - perus", 0.55m),
        new("0,59 €/km - lumikola", 0.59m),
        new("0,59 €/km - matkustaja", 0.59m),
        new("0,64 €/km - peräkärry", 0.64m)
    ];
    private int _selectedHourlyIndex = 0;
    private int _selectedMileageIndex = 1;
    private bool _isTwoColumnLayout;

    public MainPage()
    {
        InitializeComponent();
        InitializeDropdowns();
        ApplyResponsiveLayout(Width);
        CalculateAndRender();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ApplyResponsiveLayout(width);
    }

    private void InitializeDropdowns()
    {
        BuildDropdownOptions(HourlyOptionsPanel, _hourlyRates, SelectHourlyOption);
        BuildDropdownOptions(MileageOptionsPanel, _mileageRates, SelectMileageOption);
        HourlySelectedLabel.Text = _hourlyRates[_selectedHourlyIndex].Label;
        MileageSelectedLabel.Text = _mileageRates[_selectedMileageIndex].Label;
    }

    private static void BuildDropdownOptions(
        VerticalStackLayout panel,
        IReadOnlyList<RateOption> rates,
        Action<RateOption> onSelected)
    {
        panel.Children.Clear();

        foreach (RateOption rate in rates)
        {
            var row = new Grid
            {
                Padding = new Thickness(14, 10),
                BackgroundColor = Color.FromArgb("#F2F8F2"),
                HeightRequest = DropdownRowHeight
            };

            row.Children.Add(new Label
            {
                Text = rate.Label,
                TextColor = Color.FromArgb("#0E3726"),
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            });

            var tap = new TapGestureRecognizer();
            RateOption captured = rate;
            tap.Tapped += (_, _) => onSelected(captured);
            row.GestureRecognizers.Add(tap);
            panel.Children.Add(row);
        }
    }

    private void DismissKeyboard()
    {
        if (HoursEntry.IsFocused)
        {
            HoursEntry.Unfocus();
        }

        if (KilometersEntry.IsFocused)
        {
            KilometersEntry.Unfocus();
        }

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

#if IOS || MACCATALYST
        UIKit.UIApplication.SharedApplication?.KeyWindow?.EndEditing(true);
#endif
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
            Grid.SetColumnSpan(ResultsCard, 1);

            Grid.SetColumn(InputCard, 1);
            Grid.SetRow(InputCard, 0);
            Grid.SetColumnSpan(InputCard, 1);
        }
        else
        {
            CardsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            CardsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            CardsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            Grid.SetColumn(ResultsCard, 0);
            Grid.SetRow(ResultsCard, 0);
            Grid.SetColumnSpan(ResultsCard, 1);

            Grid.SetColumn(InputCard, 0);
            Grid.SetRow(InputCard, 1);
            Grid.SetColumnSpan(InputCard, 1);
        }

        _isTwoColumnLayout = shouldUseTwoColumns;
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

    private void SelectHourlyOption(RateOption selected)
    {
        _selectedHourlyIndex = _hourlyRates.IndexOf(selected);
        HourlySelectedLabel.Text = selected.Label;
        HourlyOptionsBorder.IsVisible = false;
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
        _selectedMileageIndex = 1;
        HourlySelectedLabel.Text = _hourlyRates[_selectedHourlyIndex].Label;
        MileageSelectedLabel.Text = _mileageRates[_selectedMileageIndex].Label;
        HourlyOptionsBorder.IsVisible = false;
        MileageOptionsBorder.IsVisible = false;
        ValidationLabel.Text = string.Empty;
        ValidationLabel.IsVisible = false;
        CalculateAndRender();
    }

    private void SelectMileageOption(RateOption selected)
    {
        _selectedMileageIndex = _mileageRates.IndexOf(selected);
        MileageSelectedLabel.Text = selected.Label;
        MileageOptionsBorder.IsVisible = false;
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
        decimal mileageRate = GetSelectedRate(_mileageRates, _selectedMileageIndex, 1);

        decimal workCost = hours * hourlyRate;
        decimal travelCost = kilometers * mileageRate;
        decimal subtotal = workCost + travelCost;
        decimal vat = subtotal * VatRate;
        decimal total = subtotal + vat;

        WorkValueLabel.Text = FormatEuro(workCost);
        TravelValueLabel.Text = FormatEuro(travelCost);
        SubtotalValueLabel.Text = FormatEuro(subtotal);
        VatValueLabel.Text = FormatEuro(vat);
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

    private decimal ParseDecimal(string? text)
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

    private decimal GetSelectedRate(IReadOnlyList<RateOption> rates, int selectedIndex, int fallbackIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < rates.Count)
        {
            return rates[selectedIndex].Value;
        }

        return rates[fallbackIndex].Value;
    }

    private string FormatEuro(decimal value)
    {
        return string.Format(_fiCulture, "{0:C2}", value);
    }

    private sealed record RateOption(string Label, decimal Value);
}
