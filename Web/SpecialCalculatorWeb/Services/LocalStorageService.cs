using Microsoft.JSInterop;

namespace SpecialCalculatorWeb.Services;

public sealed class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    public async Task<bool> ContainsKeyAsync(string key) =>
        await jsRuntime.InvokeAsync<bool>("localStorageInterop.containsKey", key);

    public async Task<string?> GetItemAsync(string key) =>
        await jsRuntime.InvokeAsync<string?>("localStorageInterop.getItem", key);

    public async Task SetItemAsync(string key, string value) =>
        await jsRuntime.InvokeVoidAsync("localStorageInterop.setItem", key, value);

    public async Task RemoveItemAsync(string key) =>
        await jsRuntime.InvokeVoidAsync("localStorageInterop.removeItem", key);
}
