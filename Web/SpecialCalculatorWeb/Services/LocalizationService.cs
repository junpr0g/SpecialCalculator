using SpecialCalculatorWeb.Models;

namespace SpecialCalculatorWeb.Services;

public sealed class LocalizationService(AppPreferencesStore preferences)
{
    private static readonly Dictionary<string, string> Fi = new()
    {
        ["header_title"] = "Lapin Koti- ja Mökkiyhdistys ry",
        ["header_subtitle"] = "Palvelukustannuslaskuri",
        ["settings"] = "Asetukset",
        ["results_title"] = "Laskelman erittely",
        ["work_net"] = "Työ (alv 0 %)",
        ["travel_net"] = "Matka (alv 0 %)",
        ["subtotal_net"] = "Subtotal (alv 0 %)",
        ["vat_label"] = "ALV",
        ["total"] = "YHTEENSÄ",
        ["input_title"] = "Syötä tiedot",
        ["clear"] = "Tyhjennä",
        ["select_work"] = "Valitse työtyyppi",
        ["select_travel"] = "Valitse matkatyyppi",
        ["hours"] = "Työtunnit (h)",
        ["kilometers"] = "Ajokilometrit (km)",
        ["admin_settings_title"] = "Ylläpito / Asetukset",
        ["hourly_rates"] = "Työhinnat (alv 0 % / h)",
        ["mileage_rates"] = "Matkahinnat (alv 0 % / km)",
        ["save"] = "Tallenna",
        ["reset_defaults"] = "Palauta oletukset",
        ["saved"] = "Tallennettu.",
        ["defaults_restored"] = "Oletusarvot palautettu.",
        ["app_settings"] = "Sovellusasetukset",
        ["app_settings_title"] = "Sovellusasetukset",
        ["language"] = "Kieli",
        ["theme"] = "Teema",
        ["language_fi"] = "Suomi",
        ["language_en"] = "Englanti",
        ["theme_light"] = "Vaalea",
        ["theme_dark"] = "Tumma",
        ["tab_admin"] = "Administraattori",
        ["tab_app"] = "Yleiset asetukset",
        ["notifications"] = "Ilmoitukset",
        ["invalid_hourly_rate"] = "Virheellinen työhinta: {name}.",
        ["invalid_mileage_rate"] = "Virheellinen matkahinta: {name}.",
        ["invalid_vat"] = "ALV ei voi olla negatiivinen.",
        ["rate_perustalkkari"] = "perustalkkari",
        ["rate_digitalkkari"] = "digitalkkari",
        ["rate_konetalkkari"] = "konetalkkari",
        ["rate_perus_ei_jasen"] = "perus ei jäsen",
        ["rate_digi_ei_jasen"] = "digi ei jäsen",
        ["rate_kone_ei_jasen"] = "kone ei jäsen",
        ["rate_julkinen"] = "julkinen",
        ["rate_perus"] = "perus",
        ["rate_lumikola"] = "lumikola",
        ["rate_matkustaja"] = "matkustaja",
        ["rate_perakarry"] = "peräkärry",
        ["validation_hours_negative"] = "Työtunnit ei voi olla negatiivinen.",
        ["validation_km_negative"] = "Ajokilometrit ei voi olla negatiivinen."
    };

    private static readonly Dictionary<string, string> En = new()
    {
        ["header_title"] = "Lapin Home and Cottage Association",
        ["header_subtitle"] = "Service Cost Calculator",
        ["settings"] = "Settings",
        ["results_title"] = "Calculation Breakdown",
        ["work_net"] = "Work (VAT 0%)",
        ["travel_net"] = "Travel (VAT 0%)",
        ["subtotal_net"] = "Subtotal (VAT 0%)",
        ["vat_label"] = "VAT",
        ["total"] = "TOTAL",
        ["input_title"] = "Enter Data",
        ["clear"] = "Clear",
        ["select_work"] = "Select work type",
        ["select_travel"] = "Select travel type",
        ["hours"] = "Work hours (h)",
        ["kilometers"] = "Driving kilometers (km)",
        ["admin_settings_title"] = "Admin / Settings",
        ["hourly_rates"] = "Hourly rates (VAT 0% / h)",
        ["mileage_rates"] = "Mileage rates (VAT 0% / km)",
        ["save"] = "Save",
        ["reset_defaults"] = "Reset defaults",
        ["saved"] = "Saved.",
        ["defaults_restored"] = "Defaults restored.",
        ["app_settings"] = "App Settings",
        ["app_settings_title"] = "App Settings",
        ["language"] = "Language",
        ["theme"] = "Theme",
        ["language_fi"] = "Finnish",
        ["language_en"] = "English",
        ["theme_light"] = "Light",
        ["theme_dark"] = "Dark",
        ["tab_admin"] = "Administrator",
        ["tab_app"] = "General settings",
        ["notifications"] = "Notifications",
        ["invalid_hourly_rate"] = "Invalid hourly rate: {name}.",
        ["invalid_mileage_rate"] = "Invalid mileage rate: {name}.",
        ["invalid_vat"] = "VAT cannot be negative.",
        ["rate_perustalkkari"] = "Basic carpenter",
        ["rate_digitalkkari"] = "Digital carpenter",
        ["rate_konetalkkari"] = "Machine carpenter",
        ["rate_perus_ei_jasen"] = "Basic (non-member)",
        ["rate_digi_ei_jasen"] = "Digital (non-member)",
        ["rate_kone_ei_jasen"] = "Machine (non-member)",
        ["rate_julkinen"] = "Public",
        ["rate_perus"] = "Standard",
        ["rate_lumikola"] = "Snow blower",
        ["rate_matkustaja"] = "Passenger",
        ["rate_perakarry"] = "Trailer",
        ["validation_hours_negative"] = "Work hours cannot be negative.",
        ["validation_km_negative"] = "Driving kilometers cannot be negative."
    };

    public string CurrentLanguage => preferences.Language;

    public string T(string key)
    {
        Dictionary<string, string> source = CurrentLanguage == "en" ? En : Fi;
        return source.TryGetValue(key, out string? value) ? value : key;
    }

    public string RateName(RateItem rate) => RateName(rate.Id, rate.Name);

    public string RateName(string id, string fallbackName)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return fallbackName;
        }

        string key = $"rate_{id}";
        Dictionary<string, string> source = CurrentLanguage == "en" ? En : Fi;
        return source.TryGetValue(key, out string? value) ? value : fallbackName;
    }
}
