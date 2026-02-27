using Municipality360.Web.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Municipality360.Web.Services;
using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;

var builder = WebApplication.CreateBuilder(args);

// ── Razor Components ──────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Local Storage ─────────────────────────────────────────
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<Municipality360.Web.Services.ILocalStorageService,
                            Municipality360.Web.Services.LocalStorageService>();

// ── Authentication & Authorization ───────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();

// ✅ مطلوب لـ CustomAuthStateProvider لقراءة Cookie قبل JS
builder.Services.AddHttpContextAccessor();

// ── HTTP Client → API Backend ─────────────────────────────
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7173";
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// ── Application Services ──────────────────────────────────
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IDepartementApiService, DepartementApiService>();
builder.Services.AddScoped<IServiceApiService, ServiceApiService>();
builder.Services.AddScoped<IPosteApiService, PosteApiService>();
builder.Services.AddScoped<IEmployeApiService, EmployeApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();

// ── Auth State Provider ───────────────────────────────────
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

// ── Cascading Auth State ──────────────────────────────────
builder.Services.AddCascadingAuthenticationState();

// ═══════════════════════════════════════════════════════════
var app = builder.Build();
// ═══════════════════════════════════════════════════════════

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();