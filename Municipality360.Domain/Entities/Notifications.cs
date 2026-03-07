using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

// ══════════════════════════════════════════════
//  ENUMS
// ══════════════════════════════════════════════

public enum TypeNotification
{
    // Bureau d'Ordre
    NouveauCourrier,
    CourrierAssigne,
    CourrierTraite,

    // Réclamations
    NouvelleReclamation,
    ReclamationAssignee,
    ReclamationMiseAJour,
    ReclamationResolue,
    ReclamationRejetee,

    // Permis de Bâtir
    NouvelleDemandePermis,
    DemandePermisEnInstruction,
    DemandePermisApprouvee,
    DemandePermisRefusee,
    PermisDelivre,
    PermisDocumentManquant,
    InspectionProgrammee,

    // Système
    Alerte,
    Information
}

public enum CibleNotification { AgentInterne, Citoyen, Both }

// ══════════════════════════════════════════════
//  NOTIFICATION
// ══════════════════════════════════════════════

/// <summary>
/// Notifications pour agents internes (SignalR vers Blazor/Web)
/// ET pour citoyens (SignalR vers Flutter app)
/// </summary>
public class Notification : BaseEntity
{
    public TypeNotification Type { get; set; }
    public CibleNotification Cible { get; set; }

    [Required, MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>ID de l'entité concernée (CourrierId, ReclamationId, PermisId)</summary>
    public int? EntiteId { get; set; }

    /// <summary>Nom de l'entité: "Courrier", "Reclamation", "PermisBatir"</summary>
    [MaxLength(50)]
    public string? EntiteType { get; set; }

    /// <summary>URL de navigation dans l'app</summary>
    [MaxLength(300)]
    public string? LienNavigation { get; set; }

    // ── Destinataire ──────────────────────────
    /// <summary>UserId de l'agent interne (AspNetUsers.Id)</summary>
    [MaxLength(450)]
    public string? DestinataireAgentId { get; set; }

    /// <summary>CitoyenId (peut être AppUser ou identifiant Flutter)</summary>
    [MaxLength(450)]
    public string? DestinataireCitoyenId { get; set; }

    public bool EstLue { get; set; } = false;
    public DateTime? DateLecture { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}

// ══════════════════════════════════════════════
//  NUMÉROTATION AUTOMATIQUE
// ══════════════════════════════════════════════

/// <summary>
/// Séquences de numérotation pour Courrier, Réclamation, Permis
/// Permet de générer des numéros uniques par type et par année
/// </summary>
public class Sequence : BaseEntity
{
    [Required, MaxLength(50)]
    public string Prefixe { get; set; } = string.Empty;

    public int Annee { get; set; }

    public int DernierNumero { get; set; } = 0;
}