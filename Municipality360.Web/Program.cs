using Municipality360.Web.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Municipality360.Web.Services;
using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<Municipality360.Web.Services.ILocalStorageService,
                            Municipality360.Web.Services.LocalStorageService>();

builder.Services.AddAuthorizationCore();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7173";
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IDepartementApiService, DepartementApiService>();
builder.Services.AddScoped<IServiceApiService, ServiceApiService>();
builder.Services.AddScoped<IPosteApiService, PosteApiService>();
builder.Services.AddScoped<IEmployeApiService, EmployeApiService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
