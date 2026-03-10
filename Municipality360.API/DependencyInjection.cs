// ═══════════════════════════════════════════════════════════════════
//  DependencyInjection.cs
//  API/DependencyInjection.cs
//
//  Enregistre tous les services, repositories et SignalR.
//  Appeler depuis Program.cs : builder.Services.AddApplicationServices(builder.Configuration);
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Application.Services;
using Municipality360.Infrastructure.Data;
using Municipality360.Infrastructure.Repositories;
using Municipality360.Infrastructure.Services;
using Municipality360.API.Hubs;

namespace Municipality360.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Base de données ──────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("Municipality360.Infrastructure")));

        // ── SignalR ──────────────────────────────────────────────────
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 102400; // 100 KB
        });

        // ── CORS (pour SignalR WebSocket — Flutter app) ──────────────
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy
                    .WithOrigins(
                        configuration.GetSection("AllowedOrigins").Get<string[]>()
                        ?? new[] { "http://localhost:3000", "http://localhost:5000" })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());   // obligatoire pour SignalR
        });

        // ── Repositories — Structure existante ───────────────────────
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IDepartementRepository, DepartementRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IPosteRepository, PosteRepository>();
        services.AddScoped<IEmployeRepository, EmployeRepository>();

        // ── Repositories — Nouveaux modules ──────────────────────────
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

        // ── Services — Structure existante ───────────────────────────
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDepartementService, DepartementService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IPosteService, PosteService>();
        services.AddScoped<IEmployeService, EmployeService>();

        // ── Services — Nouveaux modules ───────────────────────────────
        // Bureau d'Ordre
        services.AddScoped<ICourrierEntrantService, CourrierEntrantService>();
        services.AddScoped<ICourrierSortantService, CourrierSortantService>();
        services.AddScoped<IBODossierService, BODossierService>();
        services.AddScoped<IBOContactService, BOContactService>();
        services.AddScoped<IBOArchiveService, BOArchiveService>();

        // Réclamations
        services.AddScoped<ICitoyenService, CitoyenService>();
        services.AddScoped<IReclamationService, ReclamationService>();

        // Permis de Bâtir
        services.AddScoped<IDemandeurService, DemandeurService>();
        services.AddScoped<IArchitecteService, ArchitecteService>();
        services.AddScoped<IDemandePermisBatirService, DemandePermisBatirService>();

        // Notifications (doit être enregistré avant les autres — dépendance)
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }

    /// <summary>
    /// Enregistrer les routes SignalR dans app.UseEndpoints()
    /// Appeler depuis Program.cs : app.MapHubs();
    /// </summary>
    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<NotificationHub>("/hubs/notifications");
        return app;
    }
}