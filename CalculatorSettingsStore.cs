using System.Text.Json;

namespace SpecialCalculator;

public static class CalculatorSettingsStore
{
    private const string SettingsStorageKey = "calculator_settings_v1";

    public static CalculatorSettings Defaults { get; } = new()
    {
        VatPercent = 25.5m,
        HourlyRates =
        [
            new RateItem("perustalkkari", "perustalkkari", 26m),
            new RateItem("digitalkkari", "digitalkkari", 32m),
            new RateItem("konetalkkari", "konetalkkari", 33m),
            new RateItem("perus_ei_jasen", "perus ei jäsen", 36m),
            new RateItem("digi_ei_jasen", "digi ei jäsen", 42m),
            new RateItem("kone_ei_jasen", "kone ei jäsen", 43m)
        ],
        MileageRates =
        [
            new RateItem("julkinen", "julkinen", 0.13m),
            new RateItem("perus", "perus", 0.55m),
            new RateItem("lumikola", "lumikola", 0.59m),
            new RateItem("matkustaja", "matkustaja", 0.59m),
            new RateItem("perakarry", "peräkärry", 0.64m)
        ]
    };

    public static CalculatorSettings Load()
    {
        if (!Preferences.Default.ContainsKey(SettingsStorageKey))
        {
            return Clone(Defaults);
        }

        string payload = Preferences.Default.Get(SettingsStorageKey, string.Empty);
        if (string.IsNullOrWhiteSpace(payload))
        {
            return Clone(Defaults);
        }

        try
        {
            CalculatorSettings? parsed = JsonSerializer.Deserialize<CalculatorSettings>(payload);
            if (parsed is null || parsed.HourlyRates.Count == 0 || parsed.MileageRates.Count == 0 || parsed.VatPercent < 0)
            {
                return Clone(Defaults);
            }

            return Clone(parsed);
        }
        catch
        {
            return Clone(Defaults);
        }
    }

    public static void Save(CalculatorSettings settings)
    {
        Preferences.Default.Set(SettingsStorageKey, JsonSerializer.Serialize(settings));
    }

    public static void ResetToDefaults()
    {
        Preferences.Default.Remove(SettingsStorageKey);
    }

    public static CalculatorSettings Clone(CalculatorSettings source)
    {
        return new CalculatorSettings
        {
            VatPercent = source.VatPercent,
            HourlyRates = source.HourlyRates.Select(x => new RateItem(x.Id, x.Name, x.Value)).ToList(),
            MileageRates = source.MileageRates.Select(x => new RateItem(x.Id, x.Name, x.Value)).ToList()
        };
    }
}

public sealed class CalculatorSettings
{
    public List<RateItem> HourlyRates { get; set; } = [];
    public List<RateItem> MileageRates { get; set; } = [];
    public decimal VatPercent { get; set; }
}

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
