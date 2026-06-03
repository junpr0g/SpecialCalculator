using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpecialCalculatorWeb;
using SpecialCalculatorWeb.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AppPreferencesStore>();
builder.Services.AddScoped<CalculatorSettingsStore>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<AppThemeService>();

await builder.Build().RunAsync();
