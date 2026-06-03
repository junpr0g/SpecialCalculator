using System.Globalization;
using SpecialCalculatorWeb.Models;

namespace SpecialCalculatorWeb.Services;

public static class CalculatorEngine
{
    private static readonly CultureInfo FiCulture = CultureInfo.GetCultureInfo("fi-FI");

    public static decimal ParseDecimal(string? text)
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

    public static decimal GetSelectedRate(IReadOnlyList<RateItem> rates, int selectedIndex, int fallbackIndex)
    {
        if (rates.Count == 0)
        {
            return 0m;
        }

        if (selectedIndex >= 0 && selectedIndex < rates.Count)
        {
            return rates[selectedIndex].Value;
        }

        return rates[Math.Clamp(fallbackIndex, 0, rates.Count - 1)].Value;
    }

    public static string FormatEuro(decimal value) =>
        string.Format(FiCulture, "{0:C2}", value);

    public static string FormatRateOptionLabel(RateItem rate, string unit, LocalizationService localization) =>
        $"{rate.Value.ToString("0.00", FiCulture)} €/ {unit} - {localization.RateName(rate)}";

    public static string FormatVatPercent(decimal vatPercent) =>
        vatPercent.ToString("0.##", FiCulture);

    public sealed record CalculationResult(
        decimal WorkCost,
        decimal TravelCost,
        decimal Subtotal,
        decimal VatAmount,
        decimal Total);

    public static CalculationResult Calculate(
        decimal hours,
        decimal kilometers,
        decimal hourlyRate,
        decimal mileageRate,
        decimal vatPercent)
    {
        decimal workCost = hours * hourlyRate;
        decimal travelCost = kilometers * mileageRate;
        decimal subtotal = workCost + travelCost;
        decimal vatAmount = subtotal * (vatPercent / 100m);
        decimal total = subtotal + vatAmount;

        return new CalculationResult(workCost, travelCost, subtotal, vatAmount, total);
    }

    public static bool TryValidate(
        string? hoursText,
        string? kilometersText,
        LocalizationService localization,
        out decimal hours,
        out decimal kilometers,
        out string message)
    {
        hours = ParseDecimal(hoursText);
        kilometers = ParseDecimal(kilometersText);

        if (hours < 0)
        {
            message = localization.T("validation_hours_negative");
            return false;
        }

        if (kilometers < 0)
        {
            message = localization.T("validation_km_negative");
            return false;
        }

        message = string.Empty;
        return true;
    }
}
