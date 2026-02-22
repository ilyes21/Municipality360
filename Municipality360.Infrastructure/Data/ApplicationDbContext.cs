using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Identity;

namespace Municipality360.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Departement> Departements => Set<Departement>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Poste> Postes => Set<Poste>();
    public DbSet<Employe> Employes => Set<Employe>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Departement
        builder.Entity<Departement>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Nom).IsRequired().HasMaxLength(100);
            e.Property(d => d.Code).HasMaxLength(20);
            e.Property(d => d.Description).HasMaxLength(500);
            e.HasIndex(d => d.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(d => !d.IsDeleted);
        });

        // Service
        builder.Entity<Service>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Nom).IsRequired().HasMaxLength(100);
            e.Property(s => s.Code).HasMaxLength(20);
            e.HasIndex(s => s.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(s => !s.IsDeleted);
            e.HasOne(s => s.Departement)
             .WithMany(d => d.Services)
             .HasForeignKey(s => s.DepartementId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Poste
        builder.Entity<Poste>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Titre).IsRequired().HasMaxLength(100);
            e.Property(p => p.Code).HasMaxLength(20);
            e.Property(p => p.SalaireMin).HasColumnType("decimal(18,2)");
            e.Property(p => p.SalaireMax).HasColumnType("decimal(18,2)");
            e.HasIndex(p => p.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(p => !p.IsDeleted);
        });

        // Employe
        builder.Entity<Employe>(e =>
        {
            e.HasKey(emp => emp.Id);
            e.Property(emp => emp.Identifiant).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Cin).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Prenom).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Nom).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Email).HasMaxLength(100);
            e.Property(emp => emp.Telephone).HasMaxLength(20);
            e.Property(emp => emp.Salaire).HasColumnType("decimal(18,2)");
            e.Property(emp => emp.Adresse).HasMaxLength(200);
            e.Property(emp => emp.Genre).HasConversion<string>();
            e.Property(emp => emp.Statut).HasConversion<string>();
            e.HasIndex(emp => emp.Cin).IsUnique();
            e.HasQueryFilter(emp => !emp.IsDeleted);
            e.Ignore(emp => emp.NomComplet);

            e.HasOne(emp => emp.Service)
             .WithMany(s => s.Employes)
             .HasForeignKey(emp => emp.ServiceId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(emp => emp.Poste)
             .WithMany(p => p.Employes)
             .HasForeignKey(emp => emp.PosteId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Rename Identity tables with prefix
        builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
    }
}
