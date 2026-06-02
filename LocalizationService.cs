namespace SpecialCalculator;

public static class LocalizationService
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
        ["theme_dark"] = "Tumma"
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
        ["theme_dark"] = "Dark"
    };

    public static string CurrentLanguage => AppPreferencesStore.GetLanguage();

    public static string T(string key)
    {
        Dictionary<string, string> source = CurrentLanguage == "en" ? En : Fi;
        return source.TryGetValue(key, out string? value) ? value : key;
    }
}
