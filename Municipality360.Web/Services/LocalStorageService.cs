using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace Municipality360.Web.Services;

public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;

    public LocalStorageService(Blazored.LocalStorage.ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            return await _localStorage.GetItemAsync<T>(key);
        }
        catch (InvalidOperationException)
        {
            // JS interop غير متاح (static prerendering)
            return default;
        }
        catch (JSException)
        {
            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            await _localStorage.SetItemAsync(key, value);
        }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _localStorage.RemoveItemAsync(key);
        }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }
}
