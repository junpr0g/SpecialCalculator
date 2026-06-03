namespace SpecialCalculatorWeb.Services;

public interface ILocalStorageService
{
    Task<bool> ContainsKeyAsync(string key);
    Task<string?> GetItemAsync(string key);
    Task SetItemAsync(string key, string value);
    Task RemoveItemAsync(string key);
}
