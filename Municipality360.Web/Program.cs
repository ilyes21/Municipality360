using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Municipality360.Web.Components;
using Municipality360.Web.Services;
using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;

var builder = WebApplication.CreateBuilder(args);

// ── Razor Components ──────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient داخلي لتحميل الموارد (AITranslationService يحتاجه)
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

// خدمة الترجمة التلقائية بـ Claude API
builder.Services.AddScoped<AITranslationService>();

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

// ── Bureau d'Ordre Services ───────────────────────────────
builder.Services.AddScoped<ICourrierEntrantApiService, CourrierEntrantApiService>();
builder.Services.AddScoped<ICourrierSortantApiService, CourrierSortantApiService>();
builder.Services.AddScoped<IBODossierApiService, BODossierApiService>();
builder.Services.AddScoped<IBOContactApiService, BOContactApiService>();

// ── Auth State Provider ───────────────────────────────────
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

// ── Cascading Auth State ──────────────────────────────────
builder.Services.AddCascadingAuthenticationState();

//------Reclamation---------
builder.Services.AddScoped<IReclamationApiService, ReclamationApiService>();
builder.Services.AddScoped<ICitoyenApiService, CitoyenApiService>();
builder.Services.AddScoped<ITypeReclamationApiService, TypeReclamationApiService>();
builder.Services.AddScoped<ICategorieReclamationApiService, CategorieReclamationApiService>();

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