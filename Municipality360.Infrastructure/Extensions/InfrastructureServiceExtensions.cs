using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Application.Services;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;
using Municipality360.Infrastructure.Data.Training;
using Municipality360.Infrastructure.Hubs;
using Municipality360.Infrastructure.Identity;
using Municipality360.Infrastructure.Repositories;
using Municipality360.Infrastructure.Services;
using Municipality360.Infrastructure.Services.AI;
using System.Text;

namespace Municipality360.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Base de données ───────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // ── Identity ──────────────────────────────────────────────────
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // ── JWT Authentication ────────────────────────────────────────
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key manquant dans appsettings.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            // ✅ SignalR: lire le token depuis le query string
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var accessToken = ctx.Request.Query["access_token"];
                    var path = ctx.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        ctx.Token = accessToken;
                    return Task.CompletedTask;
                }
            };
        });

        // ── SignalR ───────────────────────────────────────────────────
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 102400;
        });

        // ── Repositories — Structure (existant) ───────────────────────
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IDepartementRepository, DepartementRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IPosteRepository, PosteRepository>();
        services.AddScoped<IEmployeRepository, EmployeRepository>();

        // ── Repositories — Nouveaux modules ───────────────────────────
        services.AddScoped<ISequenceRepository, SequenceRepository>();

        // Bureau d'Ordre
        services.AddScoped<IBOContactRepository, BOContactRepository>();
        services.AddScoped<IBOCategorieCourrierRepository, BOCategorieCourrierRepository>();
        services.AddScoped<IBODossierRepository, BODossierRepository>();
        services.AddScoped<IBOCourrierEntrantRepository, BOCourrierEntrantRepository>();
        services.AddScoped<IBOCircuitTraitementRepository, BOCircuitTraitementRepository>();
        services.AddScoped<IBOCourrierSortantRepository, BOCourrierSortantRepository>();
        services.AddScoped<IBOArchiveRepository, BOArchiveRepository>();

        // Réclamations
        services.AddScoped<ICitoyenRepository, CitoyenRepository>();
        services.AddScoped<ITypeReclamationRepository, TypeReclamationRepository>();
        services.AddScoped<ICategorieReclamationRepository, CategorieReclamationRepository>();
        services.AddScoped<IReclamationRepository, ReclamationRepository>();

        // Permis de Bâtir
        services.AddScoped<IDemandeurRepository, DemandeurRepository>();
        services.AddScoped<IArchitecteRepository, ArchitecteRepository>();
        services.AddScoped<ICommissionExamenRepository, CommissionExamenRepository>();
        services.AddScoped<IDemandePermisBatirRepository, DemandePermisBatirRepository>();

        // Notifications
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // ── Services — Structure (existant) ───────────────────────────
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDepartementService, DepartementService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IPosteService, PosteService>();
        services.AddScoped<IEmployeService, EmployeService>();

        // ── Services — Notifications (en premier — dépendance) ────────
        services.AddScoped<INotificationService, NotificationService>();

        // ── Services — Bureau d'Ordre ──────────────────────────────────
        services.AddScoped<ICourrierEntrantService, CourrierEntrantService>();
        services.AddScoped<ICourrierSortantService, CourrierSortantService>();
        services.AddScoped<IBODossierService, BODossierService>();
        services.AddScoped<IBOContactService, BOContactService>();
        services.AddScoped<IBOArchiveService, BOArchiveService>();

        // ── Services — Réclamations ────────────────────────────────────
        services.AddScoped<ICitoyenService, CitoyenService>();
        services.AddScoped<IReclamationService, ReclamationService>();
        // Réclamations — référentiels
        services.AddScoped<ITypeReclamationService, TypeReclamationService>();
        services.AddScoped<ICategorieReclamationService, CategorieReclamationService>();

        // ── Services — Permis de Bâtir ─────────────────────────────────
        services.AddScoped<IDemandeurService, DemandeurService>();
        services.AddScoped<IArchitecteService, ArchitecteService>();
        services.AddScoped<IDemandePermisBatirService, DemandePermisBatirService>();

        // ── . تسجيل ICitoyenAuthService في DI ─────────────────────────
        services.AddScoped<ICitoyenAuthService, CitoyenAuthService>();

        // ── ML.NET PredictionEnginePool ────────────────────────────────
        // ✅ FIXED: المسار يُقرأ من TrainingSamples لضمان التناسق
        var modelPath = TrainingSamples.GetDefaultModelPath();

        services.AddPredictionEnginePool<ReclamationMLInput, ReclamationMLOutput>()
                .FromFile(
                    modelName: "ComplaintClassifier",
                    filePath: modelPath,
                    watchForChanges: true);  // يُعيد تحميل النموذج تلقائياً عند تحديثه

        // ── OpenAI ────────────────────────────────────────────────────
        services.Configure<OpenAIOptions>(
            configuration.GetSection(OpenAIOptions.Section));

        services.AddHttpClient("OpenAI", (sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;
            client.DefaultRequestHeaders.Add(
                "Authorization", $"Bearer {opts.ApiKey}");
            client.DefaultRequestHeaders.Add(
                "User-Agent", "Municipality360/1.0");
            client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds + 5);
        });

        // ── AI Services ───────────────────────────────────────────────
        services.AddScoped<MLComplaintClassifier>();
        services.AddScoped<AutoResponseGenerator>();
        services.AddScoped<IComplaintIntelligenceService, ComplaintIntelligenceService>();



        return services;
    }
}
