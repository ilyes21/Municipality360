// ═══════════════════════════════════════════════════════════════════
//  Reclamations.cs
//  Domain/Entities/Reclamations.cs
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities.Reclamations;

// ─── ENUMS ──────────────────────────────────────────────────────────

public enum SexeCitoyen { M, F }
public enum SituationFamiliale { Celibataire, Marie, Divorce, Veuf }
public enum StatutReclamation { Nouvelle, EnCours, Traitee, Rejetee, Fermee }
public enum PrioriteReclamation { Basse, Moyenne, Haute, Critique }
public enum CanalReclamation { Guichet, Telephone, Email, Web, Mobile }

// ─── CITOYEN ────────────────────────────────────────────────────────

/// <summary>
/// Table des citoyens — identifiés par CIN.
/// Lien optionnel vers AspNetUsers (compte Flutter / portail citoyen).
/// </summary>
public class Citoyen : BaseEntity
{
    [Required, MaxLength(20)] public string CIN { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Nom { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Prenom { get; set; } = string.Empty;

    public DateTime? DateNaissance { get; set; }
    [MaxLength(200)] public string? LieuNaissance { get; set; }
    public SexeCitoyen? Sexe { get; set; }

    [MaxLength(500)] public string? Adresse { get; set; }
    [MaxLength(100)] public string? Ville { get; set; }
    [MaxLength(20)] public string? CodePostal { get; set; }

    [Required, MaxLength(50)] public string Telephone { get; set; } = string.Empty;
    [MaxLength(50)] public string? TelephoneMobile { get; set; }
    [MaxLength(256)] public string? Email { get; set; }

    public SituationFamiliale? SituationFamiliale { get; set; }

    /// <summary>UserId AspNetUsers (compte Flutter app / portail citoyen)</summary>
    [MaxLength(450)] public string? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    // Computed
    public string NomComplet => $"{Prenom} {Nom}".Trim();

    // Navigation
    public ICollection<Reclamation> Reclamations { get; set; } = new List<Reclamation>();
}

// ─── TYPE_RECLAMATION ───────────────────────────────────────────────

/// <summary>
/// Types de réclamation (Voirie, Éclairage, Déchets…).
/// Définit le service responsable par défaut et le délai réglementaire.
/// </summary>
public class TypeReclamation : BaseEntity
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }

    public int DelaiTraitementJours { get; set; } = 15;

    public int? ServiceResponsableId { get; set; }
    public Service? ServiceResponsable { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Reclamation> Reclamations { get; set; } = new List<Reclamation>();
}

// ─── CATEGORIE_RECLAMATION (hiérarchique) ───────────────────────────

/// <summary>
/// Catégories hiérarchiques à deux niveaux.
/// Niveau 1 : Voirie → Niveau 2 : Trottoir, Nids-de-poule…
/// </summary>
public class CategorieReclamation : BaseEntity
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(10)] public string? Icone { get; set; }
    [MaxLength(7)] public string? CouleurHex { get; set; }

    public int? ParentId { get; set; }
    public CategorieReclamation? Parent { get; set; }

    public int Niveau { get; set; } = 1;

    [MaxLength(500)] public string? CheminHierarchique { get; set; }   // "1/3/7"

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<CategorieReclamation> SousCategories { get; set; } = new List<CategorieReclamation>();
    public ICollection<Reclamation> Reclamations { get; set; } = new List<Reclamation>();
}

// ─── RECLAMATION ────────────────────────────────────────────────────

/// <summary>
/// Réclamation / شكوى — déposée par un citoyen.
/// Numérotation : REC-2025-00001
/// Endpoints publics (Flutter) : POST /api/reclamations + GET /api/reclamations/suivi/{numero}
/// </summary>
public class Reclamation : BaseEntity
{
    [Required, MaxLength(50)] public string NumeroReclamation { get; set; } = string.Empty;

    public DateTime DateDepot { get; set; } = DateTime.UtcNow;
    public DateTime? DateIncident { get; set; }

    // ── Classification ─────────────────────────────────────────────
    public int TypeReclamationId { get; set; }
    public TypeReclamation TypeReclamation { get; set; } = null!;

    public int CategorieId { get; set; }
    public CategorieReclamation Categorie { get; set; } = null!;

    public PrioriteReclamation Priorite { get; set; } = PrioriteReclamation.Moyenne;
    public StatutReclamation Statut { get; set; } = StatutReclamation.Nouvelle;

    // ── Réclamant ──────────────────────────────────────────────────
    public int CitoyenId { get; set; }
    public Citoyen Citoyen { get; set; } = null!;

    public bool EstAnonyme { get; set; } = false;
    public CanalReclamation Canal { get; set; } = CanalReclamation.Guichet;

    // ── Contenu ────────────────────────────────────────────────────
    [Required, MaxLength(500)] public string Objet { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;

    [MaxLength(500)] public string? Localisation { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    // ── Traitement interne ─────────────────────────────────────────
    public int? ServiceConcerneId { get; set; }
    public Service? ServiceConcerne { get; set; }

    [MaxLength(450)] public string? AffecteAId { get; set; }
    public DateTime? DateAffectation { get; set; }
    public DateTime? DateCloture { get; set; }

    public string? SolutionApportee { get; set; }
    public int? SatisfactionCitoyen { get; set; }   // 1..5

    [MaxLength(450)] public string? EnregistreParId { get; set; }

    // ── Navigation ─────────────────────────────────────────────────
    public ICollection<SuiviReclamation> Suivis { get; set; } = new List<SuiviReclamation>();
    public ICollection<PieceJointeReclamation> PiecesJointes { get; set; } = new List<PieceJointeReclamation>();
}

// ─── SUIVI_RECLAMATION ──────────────────────────────────────────────

public class SuiviReclamation : BaseEntity
{
    public int ReclamationId { get; set; }
    public Reclamation Reclamation { get; set; } = null!;

    public StatutReclamation? StatutPrecedent { get; set; }
    public StatutReclamation? NouveauStatut { get; set; }

    public DateTime DateChangement { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(450)] public string UtilisateurId { get; set; } = string.Empty;
    [MaxLength(150)] public string UtilisateurNom { get; set; } = string.Empty;

    public string? Commentaire { get; set; }
    [MaxLength(500)] public string? ActionEffectuee { get; set; }

    /// <summary>Visible par le citoyen dans l'app Flutter</summary>
    public bool VisibleCitoyen { get; set; } = false;
}

// ─── PIECE_JOINTE_RECLAMATION ───────────────────────────────────────

public class PieceJointeReclamation : BaseEntity
{
    public int ReclamationId { get; set; }
    public Reclamation Reclamation { get; set; } = null!;

    [MaxLength(100)] public string? TypeDocument { get; set; }
    [Required, MaxLength(500)] public string NomFichier { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string CheminFichier { get; set; } = string.Empty;
    public long? TailleFichier { get; set; }

    public bool AjouteeParCitoyen { get; set; } = false;
    [Required, MaxLength(450)] public string UploadedById { get; set; } = string.Empty;
}