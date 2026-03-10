// ═══════════════════════════════════════════════════════════════════
//  ApplicationDbContext.cs  ✅ FIXED
//  Infrastructure/Data/ApplicationDbContext.cs
//
//  إصلاحات EF Core:
//  ✅ إضافة HasQueryFilter على جميع الـ child entities
//     التي تربط بـ parent يملك global query filter.
//     يحل تحذيرات: "Entity X has a global query filter defined
//     and is the required end of a relationship with entity Y"
//
//  Child entities المُصلحة:
//    BOCircuitTraitement      → filter على CourrierEntrantId
//    BOPieceJointeEntrant     → filter على CourrierEntrantId
//    BOPieceJointeSortant     → filter على CourrierSortantId
//    DocumentPermis           → filter على DemandeId
//    InspectionPermis         → filter على DemandeId
//    PermisDelivre            → filter على DemandeId
//    SuiviPermis              → filter على DemandeId
//    TaxePermis               → filter على DemandeId
//    SuiviReclamation         → filter على ReclamationId
//    PieceJointeReclamation   → filter على ReclamationId
//
//  ⚠️ COMMANDE MIGRATION (dans PMC, Default project = Municipality360.Infrastructure):
//    Add-Migration AddAllModulesV1 -StartupProject Municipality360.API
//    Update-Database -StartupProject Municipality360.API
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Identity;

namespace Municipality360.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── Structure organisationnelle ──────────────────────────────────
    public DbSet<Departement> Departements => Set<Departement>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Poste> Postes => Set<Poste>();
    public DbSet<Employe> Employes => Set<Employe>();

    // ── Bureau d'Ordre ───────────────────────────────────────────────
    public DbSet<BOContact> BOContacts => Set<BOContact>();
    public DbSet<BOCategorieCourrier> BOCategoriesCourrier => Set<BOCategorieCourrier>();
    public DbSet<BODossier> BODossiers => Set<BODossier>();
    public DbSet<BOCourrierEntrant> BOCourriersEntrants => Set<BOCourrierEntrant>();
    public DbSet<BOPieceJointeEntrant> BOPiecesJointesEntrant => Set<BOPieceJointeEntrant>();
    public DbSet<BOCircuitTraitement> BOCircuits => Set<BOCircuitTraitement>();
    public DbSet<BOCourrierSortant> BOCourriersSortants => Set<BOCourrierSortant>();
    public DbSet<BOPieceJointeSortant> BOPiecesJointesSortant => Set<BOPieceJointeSortant>();
    public DbSet<BOArchive> BOArchives => Set<BOArchive>();
    public DbSet<NumeroSequence> NumeroSequences => Set<NumeroSequence>();

    // ── Réclamations ─────────────────────────────────────────────────
    public DbSet<Citoyen> Citoyens => Set<Citoyen>();
    public DbSet<TypeReclamation> TypesReclamation => Set<TypeReclamation>();
    public DbSet<CategorieReclamation> CategoriesReclamation => Set<CategorieReclamation>();
    public DbSet<Reclamation> Reclamations => Set<Reclamation>();
    public DbSet<SuiviReclamation> SuivisReclamation => Set<SuiviReclamation>();
    public DbSet<PieceJointeReclamation> PiecesJointesReclamation => Set<PieceJointeReclamation>();

    // ── Permis de Bâtir ──────────────────────────────────────────────
    public DbSet<ZonageUrbanisme> ZonagesUrbanisme => Set<ZonageUrbanisme>();
    public DbSet<TypeDemandePermis> TypesDemandePermis => Set<TypeDemandePermis>();
    public DbSet<Demandeur> Demandeurs => Set<Demandeur>();
    public DbSet<Architecte> Architectes => Set<Architecte>();
    public DbSet<CommissionExamen> CommissionsExamen => Set<CommissionExamen>();
    public DbSet<DemandePermisBatir> DemandesPermisBatir => Set<DemandePermisBatir>();
    public DbSet<DocumentPermis> DocumentsPermis => Set<DocumentPermis>();
    public DbSet<PermisDelivre> PermisDelivres => Set<PermisDelivre>();
    public DbSet<TypeTaxe> TypesTaxe => Set<TypeTaxe>();
    public DbSet<TaxePermis> TaxesPermis => Set<TaxePermis>();
    public DbSet<SuiviPermis> SuivisPermis => Set<SuiviPermis>();
    public DbSet<InspectionPermis> InspectionsPermis => Set<InspectionPermis>();

    // ── Notifications ─────────────────────────────────────────────────
    public DbSet<Notification> Notifications => Set<Notification>();

    // ════════════════════════════════════════════════════════════════

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureStructure(builder);
        ConfigureBureauOrdre(builder);
        ConfigureReclamations(builder);
        ConfigurePermisBatir(builder);
        ConfigureNotifications(builder);
    }

    // ── Structure organisationnelle ──────────────────────────────────

    private static void ConfigureStructure(ModelBuilder b)
    {
        b.Entity<Departement>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Nom).IsRequired().HasMaxLength(100);
            e.Property(d => d.Code).HasMaxLength(20);
            e.Property(d => d.Description).HasMaxLength(500);
            e.HasIndex(d => d.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(d => !d.IsDeleted);
        });

        b.Entity<Service>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Nom).IsRequired().HasMaxLength(100);
            e.Property(s => s.Code).HasMaxLength(20);
            e.HasIndex(s => s.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(s => !s.IsDeleted);
            e.HasOne(s => s.Departement).WithMany(d => d.Services)
             .HasForeignKey(s => s.DepartementId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Poste>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Titre).IsRequired().HasMaxLength(100);
            e.Property(p => p.Code).HasMaxLength(20);
            e.Property(p => p.SalaireMin).HasColumnType("decimal(18,2)");
            e.Property(p => p.SalaireMax).HasColumnType("decimal(18,2)");
            e.HasIndex(p => p.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            e.HasQueryFilter(p => !p.IsDeleted);
        });

        b.Entity<Employe>(e =>
        {
            e.HasKey(emp => emp.Id);
            e.Property(emp => emp.Identifiant).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Cin).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Prenom).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Nom).IsRequired().HasMaxLength(50);
            e.Property(emp => emp.Salaire).HasColumnType("decimal(18,2)");
            e.Property(emp => emp.Genre).HasConversion<string>();
            e.Property(emp => emp.Statut).HasConversion<string>();
            e.HasIndex(emp => emp.Cin).IsUnique();
            e.HasQueryFilter(emp => !emp.IsDeleted);
            e.Ignore(emp => emp.NomComplet);
            e.HasOne(emp => emp.Service).WithMany(s => s.Employes)
             .HasForeignKey(emp => emp.ServiceId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(emp => emp.Poste).WithMany(p => p.Employes)
             .HasForeignKey(emp => emp.PosteId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<ApplicationUser>().ToTable("AspNetUsers");
    }

    // ── Bureau d'Ordre ───────────────────────────────────────────────

    private static void ConfigureBureauOrdre(ModelBuilder b)
    {
        b.Entity<BOContact>(e =>
        {
            e.Property(c => c.TypeContact).HasConversion<string>().HasMaxLength(30);
            e.HasQueryFilter(c => !c.IsDeleted);
            e.Ignore(c => c.NomComplet);
        });

        b.Entity<BOCategorieCourrier>(e =>
        {
            e.Property(c => c.Code).IsRequired().HasMaxLength(30);
            e.HasIndex(c => c.Code).IsUnique();
            e.HasQueryFilter(c => !c.IsDeleted);
        });

        b.Entity<BODossier>(e =>
        {
            e.Property(d => d.NumeroDossier).IsRequired().HasMaxLength(50);
            e.Property(d => d.StatutDossier).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(d => d.NumeroDossier).IsUnique();
            e.HasQueryFilter(d => !d.IsDeleted);
            e.HasOne(d => d.ServiceResponsable).WithMany()
             .HasForeignKey(d => d.ServiceResponsableId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<BOCourrierEntrant>(e =>
        {
            e.Property(c => c.NumeroOrdre).IsRequired().HasMaxLength(60);
            e.Property(c => c.TypeDocument).HasConversion<string>().HasMaxLength(50);
            e.Property(c => c.ModeReception).HasConversion<string>().HasMaxLength(30);
            e.Property(c => c.Priorite).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.Statut).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(c => c.NumeroOrdre).IsUnique();
            e.HasQueryFilter(c => !c.IsDeleted);
            e.HasOne(c => c.Categorie).WithMany()
             .HasForeignKey(c => c.CategorieId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Dossier).WithMany(d => d.CourriersEntrants)
             .HasForeignKey(c => c.DossierId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.ExpediteurContact).WithMany()
             .HasForeignKey(c => c.ExpediteurContactId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.ServiceDestinataire).WithMany()
             .HasForeignKey(c => c.ServiceDestinataireId).OnDelete(DeleteBehavior.SetNull);
        });

        // ✅ FIX: HasQueryFilter sur BOPieceJointeEntrant
        //    pour correspondre au filter du parent BOCourrierEntrant
        b.Entity<BOPieceJointeEntrant>(e =>
        {
            e.Property(p => p.TypePiece).HasConversion<string>().HasMaxLength(50);
            e.HasOne(p => p.CourrierEntrant).WithMany(c => c.PiecesJointes)
             .HasForeignKey(p => p.CourrierEntrantId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(p => !p.CourrierEntrant!.IsDeleted);
        });

        // ✅ FIX: HasQueryFilter sur BOCircuitTraitement
        b.Entity<BOCircuitTraitement>(e =>
        {
            e.Property(c => c.TypeAction).HasConversion<string>().HasMaxLength(40);
            e.Property(c => c.StatutEtape).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(c => new { c.CourrierEntrantId, c.NumeroEtape }).IsUnique();
            e.HasOne(c => c.CourrierEntrant).WithMany(ce => ce.Circuit)
             .HasForeignKey(c => c.CourrierEntrantId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(c => c.ServiceEmetteur).WithMany()
             .HasForeignKey(c => c.ServiceEmetteurId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(c => c.ServiceRecepteur).WithMany()
             .HasForeignKey(c => c.ServiceRecepteurId).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(c => !c.CourrierEntrant!.IsDeleted);
        });

        b.Entity<BOCourrierSortant>(e =>
        {
            e.Property(c => c.NumeroOrdre).IsRequired().HasMaxLength(60);
            e.Property(c => c.TypeDocument).HasConversion<string>().HasMaxLength(50);
            e.Property(c => c.ModeEnvoi).HasConversion<string>().HasMaxLength(30);
            e.Property(c => c.Priorite).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.Statut).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(c => c.NumeroOrdre).IsUnique();
            e.HasQueryFilter(c => !c.IsDeleted);
            e.HasOne(c => c.CourrierEntrantRef).WithMany(ce => ce.Reponses)
             .HasForeignKey(c => c.CourrierEntrantRefId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Categorie).WithMany()
             .HasForeignKey(c => c.CategorieId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Dossier).WithMany(d => d.CourriersSortants)
             .HasForeignKey(c => c.DossierId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.ServiceEmetteur).WithMany()
             .HasForeignKey(c => c.ServiceEmetteurId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.DestinataireContact).WithMany()
             .HasForeignKey(c => c.DestinataireContactId).OnDelete(DeleteBehavior.SetNull);
        });

        // ✅ FIX: HasQueryFilter sur BOPieceJointeSortant
        b.Entity<BOPieceJointeSortant>(e =>
        {
            e.Property(p => p.TypePiece).HasConversion<string>().HasMaxLength(50);
            e.HasOne(p => p.CourrierSortant).WithMany(c => c.PiecesJointes)
             .HasForeignKey(p => p.CourrierSortantId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(p => !p.CourrierSortant!.IsDeleted);
        });

        b.Entity<BOArchive>(e =>
        {
            e.Property(a => a.NumeroArchive).IsRequired().HasMaxLength(60);
            e.Property(a => a.Classification).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(a => a.NumeroArchive).IsUnique();
            e.HasOne(a => a.CourrierEntrant).WithOne(c => c.Archive)
             .HasForeignKey<BOArchive>(a => a.CourrierEntrantId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(a => a.CourrierSortant).WithOne(c => c.Archive)
             .HasForeignKey<BOArchive>(a => a.CourrierSortantId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<NumeroSequence>(e =>
        {
            e.HasIndex(n => new { n.Prefixe, n.Annee }).IsUnique();
        });
    }

    // ── Réclamations ─────────────────────────────────────────────────

    private static void ConfigureReclamations(ModelBuilder b)
    {
        b.Entity<Citoyen>(e =>
        {
            e.Property(c => c.CIN).IsRequired().HasMaxLength(20);
            e.Property(c => c.Nom).IsRequired().HasMaxLength(200);
            e.Property(c => c.Prenom).IsRequired().HasMaxLength(200);
            e.Property(c => c.Telephone).IsRequired().HasMaxLength(50);
            e.Property(c => c.Sexe).HasConversion<string>().HasMaxLength(10);
            e.Property(c => c.SituationFamiliale).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(c => c.CIN).IsUnique();
            e.HasQueryFilter(c => !c.IsDeleted);
            e.Ignore(c => c.NomComplet);
        });

        b.Entity<TypeReclamation>(e =>
        {
            e.Property(t => t.Code).IsRequired().HasMaxLength(50);
            e.HasIndex(t => t.Code).IsUnique();
            e.HasQueryFilter(t => !t.IsDeleted);
            e.HasOne(t => t.ServiceResponsable).WithMany()
             .HasForeignKey(t => t.ServiceResponsableId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<CategorieReclamation>(e =>
        {
            e.Property(c => c.Code).IsRequired().HasMaxLength(50);
            e.HasIndex(c => c.Code).IsUnique();
            e.HasQueryFilter(c => !c.IsDeleted);
            e.HasOne(c => c.Parent).WithMany(p => p.SousCategories)
             .HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Reclamation>(e =>
        {
            e.Property(r => r.NumeroReclamation).IsRequired().HasMaxLength(50);
            e.Property(r => r.Priorite).HasConversion<string>().HasMaxLength(20);
            e.Property(r => r.Statut).HasConversion<string>().HasMaxLength(50);
            e.Property(r => r.Canal).HasConversion<string>().HasMaxLength(20);
            e.Property(r => r.Longitude).HasColumnType("decimal(10,8)");
            e.Property(r => r.Latitude).HasColumnType("decimal(10,8)");
            e.HasIndex(r => r.NumeroReclamation).IsUnique();
            e.HasQueryFilter(r => !r.IsDeleted);
            e.HasOne(r => r.TypeReclamation).WithMany()
             .HasForeignKey(r => r.TypeReclamationId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Categorie).WithMany()
             .HasForeignKey(r => r.CategorieId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Citoyen).WithMany(c => c.Reclamations)
             .HasForeignKey(r => r.CitoyenId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.ServiceConcerne).WithMany()
             .HasForeignKey(r => r.ServiceConcerneId).OnDelete(DeleteBehavior.SetNull);
        });

        // ✅ FIX: HasQueryFilter sur SuiviReclamation
        b.Entity<SuiviReclamation>(e =>
        {
            e.Property(s => s.StatutPrecedent).HasConversion<string>().HasMaxLength(50);
            e.Property(s => s.NouveauStatut).HasConversion<string>().HasMaxLength(50);
            e.HasOne(s => s.Reclamation).WithMany(r => r.Suivis)
             .HasForeignKey(s => s.ReclamationId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(s => !s.Reclamation!.IsDeleted);
        });

        // ✅ FIX: HasQueryFilter sur PieceJointeReclamation
        b.Entity<PieceJointeReclamation>(e =>
        {
            e.HasOne(p => p.Reclamation).WithMany(r => r.PiecesJointes)
             .HasForeignKey(p => p.ReclamationId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(p => !p.Reclamation!.IsDeleted);
        });
    }

    // ── Permis de Bâtir ──────────────────────────────────────────────

    private static void ConfigurePermisBatir(ModelBuilder b)
    {
        b.Entity<ZonageUrbanisme>(e =>
        {
            e.Property(z => z.Code).IsRequired().HasMaxLength(50);
            e.Property(z => z.CoefficientOccupationSol).HasColumnType("decimal(5,2)");
            e.Property(z => z.CoefficientUtilisationSol).HasColumnType("decimal(5,2)");
            e.Property(z => z.HauteurMaximale).HasColumnType("decimal(5,2)");
            e.HasIndex(z => z.Code).IsUnique();
            e.HasQueryFilter(z => !z.IsDeleted);
        });

        b.Entity<TypeDemandePermis>(e =>
        {
            e.Property(t => t.Code).IsRequired().HasMaxLength(50);
            e.Property(t => t.TarifBase).HasColumnType("decimal(18,2)");
            e.HasIndex(t => t.Code).IsUnique();
            e.HasQueryFilter(t => !t.IsDeleted);
        });

        b.Entity<Demandeur>(e =>
        {
            e.Property(d => d.Type).HasConversion<string>().HasMaxLength(50);
            e.Property(d => d.Adresse).IsRequired().HasMaxLength(500);
            e.Property(d => d.Telephone).IsRequired().HasMaxLength(50);
            e.HasQueryFilter(d => !d.IsDeleted);
            e.Ignore(d => d.NomComplet);
        });

        b.Entity<Architecte>(e =>
        {
            e.Property(a => a.NumeroOrdre).IsRequired().HasMaxLength(50);
            e.HasIndex(a => a.NumeroOrdre).IsUnique();
            e.HasQueryFilter(a => !a.IsDeleted);
            e.Ignore(a => a.NomComplet);
        });

        b.Entity<CommissionExamen>(e =>
        {
            e.Property(c => c.StatutReunion).HasConversion<string>().HasMaxLength(50);
            e.HasQueryFilter(c => !c.IsDeleted);
        });

        b.Entity<DemandePermisBatir>(e =>
        {
            e.Property(d => d.NumeroDemande).IsRequired().HasMaxLength(50);
            e.Property(d => d.Statut).HasConversion<string>().HasMaxLength(50);
            e.Property(d => d.TypeConstruction).HasConversion<string>().HasMaxLength(100);
            e.Property(d => d.SuperficieTerrain).HasColumnType("decimal(18,2)");
            e.Property(d => d.SuperficieAConstruire).HasColumnType("decimal(18,2)");
            e.Property(d => d.CoutEstimatif).HasColumnType("decimal(18,2)");
            e.Property(d => d.Longitude).HasColumnType("decimal(10,8)");
            e.Property(d => d.Latitude).HasColumnType("decimal(10,8)");
            e.HasIndex(d => d.NumeroDemande).IsUnique();
            e.HasQueryFilter(d => !d.IsDeleted);
            e.HasOne(d => d.TypeDemande).WithMany()
             .HasForeignKey(d => d.TypeDemandeId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Demandeur).WithMany(dm => dm.Demandes)
             .HasForeignKey(d => d.DemandeurId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Architecte).WithMany()
             .HasForeignKey(d => d.ArchitecteId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.Zonage).WithMany()
             .HasForeignKey(d => d.ZonageId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.CommissionExamen).WithMany()
             .HasForeignKey(d => d.CommissionExamenId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.ServiceInstructeur).WithMany()
             .HasForeignKey(d => d.ServiceInstructeurId).OnDelete(DeleteBehavior.SetNull);
        });

        // ✅ FIX: HasQueryFilter sur DocumentPermis
        b.Entity<DocumentPermis>(e =>
        {
            e.Property(d => d.Statut).HasConversion<string>().HasMaxLength(50);
            e.HasOne(d => d.Demande).WithMany(dm => dm.Documents)
             .HasForeignKey(d => d.DemandeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(d => !d.Demande!.IsDeleted);
        });

        // ✅ FIX: HasQueryFilter sur PermisDelivre
        b.Entity<PermisDelivre>(e =>
        {
            e.Property(p => p.NumeroPermis).IsRequired().HasMaxLength(50);
            e.HasIndex(p => p.NumeroPermis).IsUnique();
            e.HasOne(p => p.Demande).WithOne(d => d.PermisDelivre)
             .HasForeignKey<PermisDelivre>(p => p.DemandeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(p => !p.Demande!.IsDeleted);
        });

        b.Entity<TypeTaxe>(e =>
        {
            e.Property(t => t.Code).IsRequired().HasMaxLength(50);
            e.Property(t => t.TauxCalcul).HasColumnType("decimal(18,2)");
            e.HasIndex(t => t.Code).IsUnique();
            e.HasQueryFilter(t => !t.IsDeleted);
        });

        // ✅ FIX: HasQueryFilter sur TaxePermis
        b.Entity<TaxePermis>(e =>
        {
            e.Property(t => t.Montant).HasColumnType("decimal(18,2)");
            e.Property(t => t.Statut).HasConversion<string>().HasMaxLength(50);
            e.HasOne(t => t.Demande).WithMany(d => d.Taxes)
             .HasForeignKey(t => t.DemandeId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(t => t.TypeTaxe).WithMany()
             .HasForeignKey(t => t.TypeTaxeId).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(t => !t.Demande!.IsDeleted);
        });

        // ✅ FIX: HasQueryFilter sur SuiviPermis
        b.Entity<SuiviPermis>(e =>
        {
            e.Property(s => s.StatutPrecedent).HasConversion<string>().HasMaxLength(50);
            e.Property(s => s.NouveauStatut).HasConversion<string>().HasMaxLength(50);
            e.HasOne(s => s.Demande).WithMany(d => d.Suivis)
             .HasForeignKey(s => s.DemandeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(s => !s.Demande!.IsDeleted);
        });

        // ✅ FIX: HasQueryFilter sur InspectionPermis
        b.Entity<InspectionPermis>(e =>
        {
            e.HasOne(i => i.Demande).WithMany(d => d.Inspections)
             .HasForeignKey(i => i.DemandeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(i => !i.Demande!.IsDeleted);
        });
    }

    // ── Notifications ─────────────────────────────────────────────────

    private static void ConfigureNotifications(ModelBuilder b)
    {
        b.Entity<Notification>(e =>
        {
            e.Property(n => n.Type).HasConversion<string>().HasMaxLength(60);
            e.Property(n => n.Cible).HasConversion<string>().HasMaxLength(20);
            e.HasQueryFilter(n => !n.IsDeleted);
            e.HasIndex(n => n.DestinataireAgentId);
            e.HasIndex(n => n.DestinataireCitoyenId);
        });
    }
}