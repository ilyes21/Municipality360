// ═══════════════════════════════════════════════════════════════════
//  Program.cs  ✅ UPDATED
//  Municipality360.API/Program.cs
//
//  التحديثات:
//  ✅ حذف DependencyInjection.cs القديم — AddInfrastructure() يفعل كل شيء
//  ✅ إضافة app.MapHub<NotificationHub>("/hubs/notifications")
//  ✅ using صحيح: Municipality360.Infrastructure.Hubs
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Municipality360.Infrastructure.Data;
using Municipality360.Infrastructure.Data.Training;
using Municipality360.Infrastructure.Extensions;
using Municipality360.Infrastructure.Hubs;          // ✅ FIXED: Infrastructure لا API
using Municipality360.Infrastructure.Identity;
using Municipality360.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure (DB · Identity · JWT · SignalR · tous les services) ──
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── CORS ──────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileApp", policy =>
          policy
              .AllowAnyOrigin()           // Flutter يعمل على منافذ مختلفة
              .AllowAnyMethod()
              .AllowAnyHeader());

    options.AddPolicy("Municipality.ELAIN.360Policy", policy =>
        policy.WithOrigins(
                "https://localhost:7173",   // API نفسه (Swagger)
                "http://localhost:5155",    // Blazor dev
                "https://localhost:7174",   // Blazor dev HTTPS
                "http://localhost:3000",    // React/Next dev
                "http://localhost:5000",
                "https://localhost:5001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());   // obligatoire pour SignalR
});
// ──  SignalR ───────────────────────────────────────────────────
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// ── Swagger avec JWT ──────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Municipality ELAIN.360 API",
        Version = "v1",
        Description = "API pour le système de gestion municipale Municipality ELAIN.360"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {votre_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddOpenApi();

// ═════════════════════════════════════════════════════════════════
var app = builder.Build();
// ═════════════════════════════════════════════════════════════════

// ── إنشاء ملف بيانات التدريب تلقائياً ─────────────────────────────
await TrainingSamples.EnsureCsvFileExistsAsync();

// ── Seed ──────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbSeeder.SeedAsync(context, userManager, roleManager);
}

// ── Middleware ────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Municipality ELAIN.360 API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
// ── 4. تطبيق CORS  ─────────────────
//app.UseCors("Municipality.ELAIN.360Policy");     // للـ admin dashboard
//app.UseCors("MobileApp");   // للتطبيق المحمول
// ✅ CORS: سياسة واحدة قبل Authentication
// نطبق MobilePolicy على مسارات /api/mobile فقط
app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api/mobile"),
    branch => branch.UseCors("MobileApp"));

// باقي المسارات (Blazor dashboard + SignalR)
app.UseWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/api/mobile"),
    branch => branch.UseCors("Municipality.ELAIN.360Policy"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ SignalR Hub
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
