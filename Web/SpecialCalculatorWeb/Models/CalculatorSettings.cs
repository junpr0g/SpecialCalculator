namespace SpecialCalculatorWeb.Models;

public sealed class CalculatorSettings
{
    public List<RateItem> HourlyRates { get; set; } = [];
    public List<RateItem> MileageRates { get; set; } = [];
    public decimal VatPercent { get; set; }
}
