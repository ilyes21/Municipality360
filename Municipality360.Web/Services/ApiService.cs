using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace Municipality360.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public ApiService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    private async Task<bool> SetAuthHeaderAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                return true;
            }

            _http.DefaultRequestHeaders.Authorization = null;
            return false;
        }
        catch (InvalidOperationException) { return false; }
        catch (JSException) { return false; }
        catch { return false; }
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var hasToken = await SetAuthHeaderAsync();
        if (!hasToken) return default;

        try
        {
            return await _http.GetFromJsonAsync<T>(url);
        }
        catch (HttpRequestException ex) when
            (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        { return default; }
        catch (HttpRequestException) { return default; }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _http.PostAsJsonAsync(url, data);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch { return default; }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _http.PutAsJsonAsync(url, data);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch { return default; }
    }

    /// <summary>PATCH – يُستخدم لـ ToggleActive وغيره من العمليات الجزئية</summary>
    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _http.PatchAsJsonAsync(url, data);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch { return default; }
    }

    public async Task<bool> DeleteAsync(string url)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
