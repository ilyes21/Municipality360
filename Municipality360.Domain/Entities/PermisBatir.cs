// ═══════════════════════════════════════════════════════════════════
//  PermisBatir.cs
//  Domain/Entities/PermisBatir.cs
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

// ─── ENUMS ──────────────────────────────────────────────────────────

public enum TypeDemandeur { Personne, SocieteConstruction }
public enum TypeConstruction { Residential, Commercial, Industriel, Mixte, Agricole }
public enum StatutDemande { Deposee, EnExamen, Approuvee, Rejetee, Suspendue }
public enum StatutDocument { Depose, Valide, AReviser }
public enum StatutTaxe { EnAttente, Payee, Exoneree }
public enum StatutCommission { Programmee, Tenue, Reportee, Annulee }

// ─── ZONAGE_URBANISME ───────────────────────────────────────────────

/// <summary>
/// Classifications des zones urbanistiques (COS, CUS, hauteur max).
/// Prêt pour intégration GIS / GEOGRAPHY SQL Server si nécessaire.
/// </summary>
public class ZonageUrbanisme : BaseEntity
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal? CoefficientOccupationSol { get; set; }   // COS
    public decimal? CoefficientUtilisationSol { get; set; }   // CUS
    public decimal? HauteurMaximale { get; set; }   // en mètres

    public bool IsActive { get; set; } = true;

    public ICollection<DemandePermisBatir> Demandes { get; set; } = new List<DemandePermisBatir>();
}

// ─── TYPE_DEMANDE_PERMIS ────────────────────────────────────────────

/// <summary>Types de permis : Construction, Démolition, Rénovation, Extension, Lotissement…</summary>
public class TypeDemandePermis : BaseEntity
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    public int DelaiTraitementJours { get; set; } = 30;
    public decimal? TarifBase { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<DemandePermisBatir> Demandes { get; set; } = new List<DemandePermisBatir>();
}

// ─── DEMANDEUR ──────────────────────────────────────────────────────

/// <summary>
/// Propriétaire / maître d'ouvrage — personne physique ou société de construction.
/// Lien optionnel vers Citoyen pour les particuliers enregistrés.
/// </summary>
public class Demandeur : BaseEntity
{
    public TypeDemandeur Type { get; set; } = TypeDemandeur.Personne;
    [MaxLength(20)] public string? CIN { get; set; }
    [Required, MaxLength(200)] public string Nom { get; set; } = string.Empty;
    [MaxLength(200)] public string? Prenom { get; set; }
    [MaxLength(300)] public string? RaisonSociale { get; set; }   // pour les sociétés

    [Required, MaxLength(500)] public string Adresse { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string Telephone { get; set; } = string.Empty;
    [MaxLength(256)] public string? Email { get; set; }

    public int? CitoyenId { get; set; }   // lien optionnel si particulier enregistré
    public bool IsActive { get; set; } = true;

    public string NomComplet => Type == TypeDemandeur.SocieteConstruction
        ? (RaisonSociale ?? Nom)
        : $"{Prenom} {Nom}".Trim();

    public ICollection<DemandePermisBatir> Demandes { get; set; } = new List<DemandePermisBatir>();
}

// ─── ARCHITECTE ─────────────────────────────────────────────────────

public class Architecte : BaseEntity
{
    [Required, MaxLength(50)] public string NumeroOrdre { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string CIN { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Nom { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Prenom { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string Telephone { get; set; } = string.Empty;
    [MaxLength(256)] public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public string NomComplet => $"{Prenom} {Nom}".Trim();

    public ICollection<DemandePermisBatir> Demandes { get; set; } = new List<DemandePermisBatir>();
}

// ─── COMMISSION_EXAMEN ──────────────────────────────────────────────

/// <summary>Commission d'examen des dossiers permis de bâtir.</summary>
public class CommissionExamen : BaseEntity
{
    [Required, MaxLength(300)] public string Libelle { get; set; } = string.Empty;
    public DateTime? DateReunion { get; set; }
    [Required, MaxLength(450)] public string PresidentId { get; set; } = string.Empty;
    [MaxLength(450)] public string? SecretaireId { get; set; }
    public StatutCommission StatutReunion { get; set; } = StatutCommission.Programmee;
    public string? ProcesVerbal { get; set; }

    public ICollection<DemandePermisBatir> Demandes { get; set; } = new List<DemandePermisBatir>();
}

// ─── DEMANDE_PERMIS_BATIR ────────────────────────────────────────────

/// <summary>
/// Demande de permis de bâtir.
/// Numérotation : PB-2025-00001
/// Workflow : Deposee → EnExamen → Approuvee/Rejetee/Suspendue
/// Endpoints publics (Flutter) : POST + GET /suivi/{numero}
/// </summary>
public class DemandePermisBatir : BaseEntity
{
    [Required, MaxLength(50)] public string NumeroDemande { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; } = DateTime.UtcNow;

    // ── Classification ─────────────────────────────────────────────
    public int TypeDemandeId { get; set; }
    public TypeDemandePermis TypeDemande { get; set; } = null!;

    public StatutDemande Statut { get; set; } = StatutDemande.Deposee;

    // ── Acteurs ────────────────────────────────────────────────────
    public int DemandeurId { get; set; }
    public Demandeur Demandeur { get; set; } = null!;

    public int? ArchitecteId { get; set; }
    public Architecte? Architecte { get; set; }

    // ── Projet ─────────────────────────────────────────────────────
    [Required, MaxLength(500)] public string AdresseProjet { get; set; } = string.Empty;
    [MaxLength(100)] public string? NumeroParcelle { get; set; }

    public decimal? SuperficieTerrain { get; set; }
    public decimal? SuperficieAConstruire { get; set; }
    public int? NombreNiveaux { get; set; }
    public TypeConstruction? TypeConstruction { get; set; }
    public decimal? CoutEstimatif { get; set; }

    public int? ZonageId { get; set; }
    public ZonageUrbanisme? Zonage { get; set; }

    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    // ── Instruction ────────────────────────────────────────────────
    public int? ServiceInstructeurId { get; set; }
    public Service? ServiceInstructeur { get; set; }
    [MaxLength(450)] public string? AgentInstructeurId { get; set; }
    public DateTime? DateDebutInstruction { get; set; }

    // ── Commission ─────────────────────────────────────────────────
    public int? CommissionExamenId { get; set; }
    public CommissionExamen? CommissionExamen { get; set; }

    // ── Décision ───────────────────────────────────────────────────
    public DateTime? DateDecision { get; set; }
    public string? MotifRejet { get; set; }
    [MaxLength(1000)] public string? ConditionsSpeciales { get; set; }
    public string? Observations { get; set; }

    [MaxLength(450)] public string? EnregistreParId { get; set; }

    // ── Navigation ─────────────────────────────────────────────────
    public ICollection<DocumentPermis> Documents { get; set; } = new List<DocumentPermis>();
    public ICollection<TaxePermis> Taxes { get; set; } = new List<TaxePermis>();
    public ICollection<SuiviPermis> Suivis { get; set; } = new List<SuiviPermis>();
    public ICollection<InspectionPermis> Inspections { get; set; } = new List<InspectionPermis>();
    public PermisDelivre? PermisDelivre { get; set; }
}

// ─── DOCUMENT_PERMIS ────────────────────────────────────────────────

public class DocumentPermis : BaseEntity
{
    public int DemandeId { get; set; }
    public DemandePermisBatir Demande { get; set; } = null!;

    [Required, MaxLength(100)] public string TypeDocument { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string NomFichier { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string CheminFichier { get; set; } = string.Empty;
    public long? TailleFichier { get; set; }

    public StatutDocument Statut { get; set; } = StatutDocument.Depose;
    [MaxLength(300)] public string? Observations { get; set; }
    public bool EstObligatoire { get; set; } = false;
    public bool AjouteeParCitoyen { get; set; } = false;

    [Required, MaxLength(450)] public string UploadedById { get; set; } = string.Empty;
}

// ─── PERMIS_DELIVRE ─────────────────────────────────────────────────

/// <summary>Permis officiel délivré après décision favorable.</summary>
public class PermisDelivre : BaseEntity
{
    public int DemandeId { get; set; }
    public DemandePermisBatir Demande { get; set; } = null!;

    [Required, MaxLength(50)] public string NumeroPermis { get; set; } = string.Empty;
    public DateTime DateDelivrance { get; set; }
    public DateTime DateValidite { get; set; }       // généralement +24 mois
    public string? Conditions { get; set; }
    [MaxLength(1000)] public string? FichierPermis { get; set; }

    [Required, MaxLength(450)] public string DelivreParId { get; set; } = string.Empty;
    public bool EstRevoque { get; set; } = false;
    public string? MotifRevocation { get; set; }
}

// ─── TAXES ──────────────────────────────────────────────────────────

public class TypeTaxe : BaseEntity
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    public decimal? TauxCalcul { get; set; }
    [MaxLength(50)] public string? UniteCalcul { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TaxePermis : BaseEntity
{
    public int DemandeId { get; set; }
    public DemandePermisBatir Demande { get; set; } = null!;

    public int TypeTaxeId { get; set; }
    public TypeTaxe TypeTaxe { get; set; } = null!;

    public decimal Montant { get; set; }
    public DateTime DateCalcul { get; set; } = DateTime.UtcNow;
    public StatutTaxe Statut { get; set; } = StatutTaxe.EnAttente;
    public DateTime? DatePaiement { get; set; }
    [MaxLength(100)] public string? NumeroRecu { get; set; }
}

// ─── SUIVI_PERMIS ───────────────────────────────────────────────────

public class SuiviPermis : BaseEntity
{
    public int DemandeId { get; set; }
    public DemandePermisBatir Demande { get; set; } = null!;

    public StatutDemande? StatutPrecedent { get; set; }
    public StatutDemande? NouveauStatut { get; set; }

    public DateTime DateChangement { get; set; } = DateTime.UtcNow;
    [Required, MaxLength(450)] public string UtilisateurId { get; set; } = string.Empty;
    [MaxLength(150)] public string UtilisateurNom { get; set; } = string.Empty;

    public string? Commentaire { get; set; }
    public bool VisibleCitoyen { get; set; } = false;
}

// ─── INSPECTION_PERMIS ──────────────────────────────────────────────

public class InspectionPermis : BaseEntity
{
    public int DemandeId { get; set; }
    public DemandePermisBatir Demande { get; set; } = null!;

    [Required, MaxLength(200)] public string Objet { get; set; } = string.Empty;
    public DateTime DateInspection { get; set; }

    [MaxLength(450)] public string? InspecteurId { get; set; }
    [MaxLength(150)] public string? NomInspecteur { get; set; }
    public string? Observations { get; set; }
    [MaxLength(500)] public string? ReservesEmises { get; set; }
    public bool Conforme { get; set; } = false;
}