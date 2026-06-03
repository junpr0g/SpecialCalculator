using System.Text.Json;
using SpecialCalculatorWeb.Models;

namespace SpecialCalculatorWeb.Services;

public sealed class CalculatorSettingsStore(ILocalStorageService storage)
{
    private const string SettingsStorageKey = "calculator_settings_v1";

    public CalculatorSettings Defaults { get; } = new()
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

    public async Task<CalculatorSettings> LoadAsync()
    {
        if (!await storage.ContainsKeyAsync(SettingsStorageKey))
        {
            return Clone(Defaults);
        }

        string? payload = await storage.GetItemAsync(SettingsStorageKey);
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

    public async Task SaveAsync(CalculatorSettings settings) =>
        await storage.SetItemAsync(SettingsStorageKey, JsonSerializer.Serialize(settings));

    public async Task ResetToDefaultsAsync() =>
        await storage.RemoveItemAsync(SettingsStorageKey);

    public static CalculatorSettings Clone(CalculatorSettings source) =>
        new()
        {
            VatPercent = source.VatPercent,
            HourlyRates = source.HourlyRates.Select(x => new RateItem(x.Id, x.Name, x.Value)).ToList(),
            MileageRates = source.MileageRates.Select(x => new RateItem(x.Id, x.Name, x.Value)).ToList()
        };
}
