using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Municipality360.Web.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private static readonly AuthenticationState _anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string? token;
            try
            {
                // هذا الاستدعاء قد يفشل أثناء prerendering - نتعامل معه بأمان
                token = await _localStorage.GetItemAsync<string>("auth_token");
            }
            catch (InvalidOperationException)
            {
                // أثناء static prerendering - نرجع anonymous
                return _anonymous;
            }
            catch (JSException)
            {
                return _anonymous;
            }

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                await TryClearTokenAsync();
                return _anonymous;
            }

            var jwt = handler.ReadJwtToken(token);

            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await TryClearTokenAsync();
                return _anonymous;
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch
        {
            return _anonymous;
        }
    }

    public async Task NotifyUserLoggedIn(string token)
    {
        try
        {
            await _localStorage.SetItemAsync("auth_token", token);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        catch { /* ignore */ }
    }

    public async Task NotifyUserLoggedOut()
    {
        await TryClearTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private async Task TryClearTokenAsync()
    {
        try { await _localStorage.RemoveItemAsync("auth_token"); }
        catch { /* ignore */ }
    }
}
