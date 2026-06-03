namespace SpecialCalculatorWeb.Models;

public sealed class RateItem
{
    public RateItem() { }

    public RateItem(string id, string name, decimal value)
    {
        Id = id;
        Name = name;
        Value = value;
    }

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
