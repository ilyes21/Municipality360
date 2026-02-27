using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace Municipality360.Web.Services;

/// <summary>
/// يقرأ التوكن من:
///   1) Cookie → متاح في أول HTTP request قبل JS (يحل مشكلة F5)
///   2) LocalStorage → بعد أن يكون JS جاهزاً
///   3) Cache في الذاكرة → بعد أول قراءة ناجحة
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private AuthenticationState? _cachedState;

    private static readonly AuthenticationState _anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private const string TokenKey = "auth_token";

    public CustomAuthStateProvider(
        ILocalStorageService localStorage,
        IHttpContextAccessor httpContextAccessor)
    {
        _localStorage = localStorage;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // ✅ 1) Cache → نرجعه فوراً بدون أي I/O
        if (_cachedState != null)
            return _cachedState;

        // ✅ 2) Cookie → متاح قبل JS (يحل F5)
        var token = TryGetTokenFromCookie();

        // ✅ 3) LocalStorage → fallback بعد JS يكون جاهزاً
        if (string.IsNullOrWhiteSpace(token))
        {
            try { token = await _localStorage.GetItemAsync<string>(TokenKey); }
            catch (InvalidOperationException) { }
            catch (JSException) { }
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            _cachedState = _anonymous;
            return _anonymous;
        }

        var state = BuildStateFromToken(token);
        if (state == _anonymous)
            await TryClearAsync();

        _cachedState = state;
        return _cachedState;
    }

    public async Task NotifyUserLoggedIn(string token)
    {
        try
        {
            await _localStorage.SetItemAsync(TokenKey, token);
            SetTokenCookie(token);                              // ✅ حفظ في Cookie

            _cachedState = BuildStateFromToken(token);
            NotifyAuthenticationStateChanged(Task.FromResult(_cachedState));
        }
        catch { }
    }

    public async Task NotifyUserLoggedOut()
    {
        await TryClearAsync();
        _cachedState = _anonymous;
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    // ── بناء State من التوكن ─────────────────────────────
    private AuthenticationState BuildStateFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return _anonymous;

            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow) return _anonymous;

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch { return _anonymous; }
    }

    // ── قراءة Cookie (متاح قبل JS) ──────────────────────
    private string? TryGetTokenFromCookie()
    {
        try { return _httpContextAccessor.HttpContext?.Request.Cookies[TokenKey]; }
        catch { return null; }
    }

    // ── حفظ التوكن في Cookie ─────────────────────────────
    private void SetTokenCookie(string token)
    {
        try
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return;

            var handler = new JwtSecurityTokenHandler();
            var expires = handler.ReadJwtToken(token).ValidTo;

            ctx.Response.Cookies.Append(TokenKey, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            });
        }
        catch { }
    }

    // ── مسح التوكن من LocalStorage و Cookie ──────────────
    private async Task TryClearAsync()
    {
        try { await _localStorage.RemoveItemAsync(TokenKey); } catch { }
        try { _httpContextAccessor.HttpContext?.Response.Cookies.Delete(TokenKey); } catch { }
    }
}