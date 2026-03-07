// ═══════════════════════════════════════════════════════════════════
//  BureauOrdre.cs
//  Domain/Entities/BureauOrdre.cs
//  Toutes les entités du module Bureau d'Ordre (v2 — basé sur bd.sql)
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities.BureauOrdre;

// ─── ENUMS ──────────────────────────────────────────────────────────

public enum TypeContact { Personne, Organisme, Administration }

public enum TypeDocumentBO
{
    Lettre, Note, Circulaire, Rapport, Decision,
    Arrete, Instruction, Demande, Recours, Facture,
    Notification, Convocation, Reponse, Autre
}

public enum ModeReception { Guichet, Messagerie, Email, Fax, Recommande, Coursier, PSEM }
public enum ModeEnvoi { Guichet, PostaleSimple, Recommande, Coursier, Email, Fax, PSEM, Autre }
public enum PrioriteCourrier { Normal, Urgent, TresUrgent, Confidentiel }
public enum StatutEntrant { Recu, Enregistre, EnCours, Traite, Archive, Rejete }
public enum StatutSortant { Brouillon, EnValidation, Signe, Envoye, Archive, Annule }
public enum StatutDossierBO { Ouvert, EnCours, Cloture, Suspendu }
public enum TypeActionCircuit { PourAction, PourDecision, PourInformation, PourSignature, PourSuite }
public enum StatutEtapeCircuit { EnAttente, EnCours, Traite, Retourne }
public enum TypePieceJointeBO { DocumentPrincipal, Annexe, PieceIdentite, VersionFinale, Autre }
public enum ClassificationArchive { Courant, Intermediaire, Definitif }

// ─── BO_CONTACTS ────────────────────────────────────────────────────

/// <summary>
/// Répertoire des expéditeurs / destinataires externes (personnes, organismes, administrations).
/// Réutilisé par entrants et sortants.
/// </summary>
public class BOContact : BaseEntity
{
    public TypeContact TypeContact { get; set; }

    [MaxLength(100)] public string? Nom { get; set; }
    [MaxLength(100)] public string? Prenom { get; set; }
    [MaxLength(300)] public string? RaisonSociale { get; set; }
    [MaxLength(200)] public string? Fonction { get; set; }
    [MaxLength(500)] public string? Adresse { get; set; }
    [MaxLength(100)] public string? Ville { get; set; }
    [MaxLength(100)] public string? Wilaya { get; set; }
    [MaxLength(30)] public string? Telephone { get; set; }
    [MaxLength(30)] public string? Fax { get; set; }
    [MaxLength(256)] public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    // Computed — non mappé
    public string NomComplet => (TypeContact == TypeContact.Organisme || TypeContact == TypeContact.Administration)
        ? (RaisonSociale ?? string.Empty)
        : $"{Prenom} {Nom}".Trim();
}

// ─── BO_CATEGORIES_COURRIER ─────────────────────────────────────────

/// <summary>Catégories / classifications des courriers (avec code couleur).</summary>
public class BOCategorieCourrier : BaseEntity
{
    [Required, MaxLength(30)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(7)] public string? CouleurHex { get; set; }   // ex: #FF5733
    public bool EstConfidentiel { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

// ─── BO_DOSSIERS ────────────────────────────────────────────────────

/// <summary>
/// Dossiers thématiques regroupant plusieurs courriers entrants/sortants.
/// Numérotation : DOS/2025/000001
/// </summary>
public class BODossier : BaseEntity
{
    [Required, MaxLength(50)] public string NumeroDossier { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string Intitule { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int? ServiceResponsableId { get; set; }
    public Service? ServiceResponsable { get; set; }

    public DateTime DateOuverture { get; set; } = DateTime.UtcNow;
    public DateTime? DateCloture { get; set; }

    public StatutDossierBO StatutDossier { get; set; } = StatutDossierBO.Ouvert;

    [MaxLength(450)] public string? CreatedById { get; set; }

    // Navigation
    public ICollection<BOCourrierEntrant> CourriersEntrants { get; set; } = new List<BOCourrierEntrant>();
    public ICollection<BOCourrierSortant> CourriersSortants { get; set; } = new List<BOCourrierSortant>();
}

// ─── BO_COURRIER_ENTRANT ────────────────────────────────────────────

/// <summary>
/// Courrier entrant — البريد الوارد.
/// Numérotation : ENT/2025/000001
/// Circuit de traitement inter-services via BOCircuitTraitement.
/// </summary>
public class BOCourrierEntrant : BaseEntity
{
    [Required, MaxLength(60)] public string NumeroOrdre { get; set; } = string.Empty;
    [MaxLength(100)] public string? NumeroExterne { get; set; }

    // ── Dates ──────────────────────────────────────────────────────
    public DateTime DateReception { get; set; } = DateTime.UtcNow;
    public DateTime DateCourrier { get; set; }             // date figurant sur le doc

    // ── Contenu ────────────────────────────────────────────────────
    [Required, MaxLength(500)] public string ObjetCourrier { get; set; } = string.Empty;
    public TypeDocumentBO TypeDocument { get; set; } = TypeDocumentBO.Lettre;

    public int? CategorieId { get; set; }
    public BOCategorieCourrier? Categorie { get; set; }

    public int? DossierId { get; set; }
    public BODossier? Dossier { get; set; }

    // ── Expéditeur ─────────────────────────────────────────────────
    public int? ExpediteurContactId { get; set; }
    public BOContact? ExpediteurContact { get; set; }
    [MaxLength(300)] public string? ExpediteurLibreNom { get; set; }   // si non enregistré

    public ModeReception ModeReception { get; set; } = ModeReception.Guichet;
    [MaxLength(60)] public string? NumeroRecommande { get; set; }

    // ── Destinataire interne ───────────────────────────────────────
    public int? ServiceDestinataireId { get; set; }
    public Service? ServiceDestinataire { get; set; }
    [MaxLength(450)] public string? AgentDestinataireId { get; set; }

    // ── Caractéristiques ───────────────────────────────────────────
    public short NombrePages { get; set; } = 1;
    public PrioriteCourrier Priorite { get; set; } = PrioriteCourrier.Normal;
    public bool EstConfidentiel { get; set; } = false;

    // ── Statut & délai ─────────────────────────────────────────────
    public StatutEntrant Statut { get; set; } = StatutEntrant.Enregistre;
    public DateTime? DelaiReponse { get; set; }
    public bool NecessiteReponse { get; set; } = false;
    public string? Observation { get; set; }

    [Required, MaxLength(450)] public string EnregistreParId { get; set; } = string.Empty;

    // ── Navigation ─────────────────────────────────────────────────
    public ICollection<BOPieceJointeEntrant> PiecesJointes { get; set; } = new List<BOPieceJointeEntrant>();
    public ICollection<BOCircuitTraitement> Circuit { get; set; } = new List<BOCircuitTraitement>();
    public ICollection<BOCourrierSortant> Reponses { get; set; } = new List<BOCourrierSortant>();
    public BOArchive? Archive { get; set; }
}

// ─── BO_PIECES_JOINTES_ENTRANT ──────────────────────────────────────

/// <summary>Fichiers PDF attachés à un courrier entrant.</summary>
public class BOPieceJointeEntrant : BaseEntity
{
    public int CourrierEntrantId { get; set; }
    public BOCourrierEntrant CourrierEntrant { get; set; } = null!;

    [Required, MaxLength(500)] public string NomFichierOriginal { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string NomFichierStocke { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string CheminFichier { get; set; } = string.Empty;
    [MaxLength(10)] public string ExtensionFichier { get; set; } = ".pdf";

    public long TailleFichierOctets { get; set; }
    public TypePieceJointeBO TypePiece { get; set; } = TypePieceJointeBO.Annexe;
    public short Ordre { get; set; } = 1;
    [MaxLength(300)] public string? Description { get; set; }

    [Required, MaxLength(450)] public string UploadedById { get; set; } = string.Empty;
}

// ─── BO_CIRCUIT_TRAITEMENT ──────────────────────────────────────────

/// <summary>
/// Étapes du circuit de traitement interne d'un courrier entrant.
/// Chaque acheminement vers un service crée une étape numérotée.
/// </summary>
public class BOCircuitTraitement : BaseEntity
{
    public int CourrierEntrantId { get; set; }
    public BOCourrierEntrant CourrierEntrant { get; set; } = null!;

    public short NumeroEtape { get; set; }                  // 1, 2, 3 ...

    // ── Acteurs ────────────────────────────────────────────────────
    public int? ServiceEmetteurId { get; set; }
    public Service? ServiceEmetteur { get; set; }
    [MaxLength(450)] public string? AgentEmetteurId { get; set; }

    public int ServiceRecepteurId { get; set; }
    public Service ServiceRecepteur { get; set; } = null!;
    [MaxLength(450)] public string? AgentRecepteurId { get; set; }

    // ── Dates ──────────────────────────────────────────────────────
    public DateTime DateTransmission { get; set; } = DateTime.UtcNow;
    public DateTime? DateTraitement { get; set; }
    public DateTime? DelaiTraitement { get; set; }

    // ── Action ─────────────────────────────────────────────────────
    public TypeActionCircuit TypeAction { get; set; } = TypeActionCircuit.PourAction;
    public string? InstructionTransmission { get; set; }
    public string? CommentaireTraitement { get; set; }
    [MaxLength(500)] public string? ActionEffectuee { get; set; }

    // ── Statut ─────────────────────────────────────────────────────
    public StatutEtapeCircuit StatutEtape { get; set; } = StatutEtapeCircuit.EnAttente;
    public bool EstRetour { get; set; } = false;
    [MaxLength(300)] public string? MotifRetour { get; set; }

    [Required, MaxLength(450)] public string CreatedById { get; set; } = string.Empty;
}

// ─── BO_COURRIER_SORTANT ────────────────────────────────────────────

/// <summary>
/// Courrier sortant — البريد الصادر.
/// Numérotation : SRT/2025/000001
/// Workflow : Brouillon → EnValidation → Signe → Envoye → Archive
/// </summary>
public class BOCourrierSortant : BaseEntity
{
    [Required, MaxLength(60)] public string NumeroOrdre { get; set; } = string.Empty;
    [MaxLength(100)] public string? NumeroReference { get; set; }

    // ── Lien vers courrier entrant (réponse) ───────────────────────
    public int? CourrierEntrantRefId { get; set; }
    public BOCourrierEntrant? CourrierEntrantRef { get; set; }

    // ── Dates ──────────────────────────────────────────────────────
    public DateTime DateRedaction { get; set; } = DateTime.UtcNow;
    public DateTime? DateSignature { get; set; }
    public DateTime? DateEnvoi { get; set; }

    // ── Contenu ────────────────────────────────────────────────────
    [Required, MaxLength(500)] public string ObjetCourrier { get; set; } = string.Empty;
    public TypeDocumentBO TypeDocument { get; set; } = TypeDocumentBO.Lettre;

    public int? CategorieId { get; set; }
    public BOCategorieCourrier? Categorie { get; set; }

    public int? DossierId { get; set; }
    public BODossier? Dossier { get; set; }

    // ── Émetteur ───────────────────────────────────────────────────
    public int? ServiceEmetteurId { get; set; }
    public Service? ServiceEmetteur { get; set; }
    [MaxLength(450)] public string? RedacteurId { get; set; }

    // ── Signataire ─────────────────────────────────────────────────
    [MaxLength(450)] public string? SignataireId { get; set; }
    [MaxLength(200)] public string? FonctionSignataire { get; set; }
    public bool EstSigne { get; set; } = false;

    // ── Destinataire ───────────────────────────────────────────────
    public int? DestinataireContactId { get; set; }
    public BOContact? DestinataireContact { get; set; }
    [MaxLength(300)] public string? DestinataireLibreNom { get; set; }

    // ── Envoi ──────────────────────────────────────────────────────
    public ModeEnvoi ModeEnvoi { get; set; } = ModeEnvoi.Guichet;
    [MaxLength(60)] public string? NumeroRecommande { get; set; }

    // ── Caractéristiques ───────────────────────────────────────────
    public short NombrePages { get; set; } = 1;
    public bool EstConfidentiel { get; set; } = false;
    public PrioriteCourrier Priorite { get; set; } = PrioriteCourrier.Normal;

    // ── Statut ─────────────────────────────────────────────────────
    public StatutSortant Statut { get; set; } = StatutSortant.Brouillon;
    public bool AccuseReceptionRecu { get; set; } = false;
    public DateTime? DateAccuseExtRecu { get; set; }

    public string? Observation { get; set; }
    [Required, MaxLength(450)] public string CreatedById { get; set; } = string.Empty;

    // ── Navigation ─────────────────────────────────────────────────
    public ICollection<BOPieceJointeSortant> PiecesJointes { get; set; } = new List<BOPieceJointeSortant>();
    public BOArchive? Archive { get; set; }
}

// ─── BO_PIECES_JOINTES_SORTANT ──────────────────────────────────────

public class BOPieceJointeSortant : BaseEntity
{
    public int CourrierSortantId { get; set; }
    public BOCourrierSortant CourrierSortant { get; set; } = null!;

    [Required, MaxLength(500)] public string NomFichierOriginal { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string NomFichierStocke { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string CheminFichier { get; set; } = string.Empty;
    [MaxLength(10)] public string ExtensionFichier { get; set; } = ".pdf";

    public long TailleFichierOctets { get; set; }
    public TypePieceJointeBO TypePiece { get; set; } = TypePieceJointeBO.Annexe;
    public bool EstVersionFinale { get; set; } = false;
    public short Ordre { get; set; } = 1;
    [MaxLength(300)] public string? Description { get; set; }

    [Required, MaxLength(450)] public string UploadedById { get; set; } = string.Empty;
}

// ─── BO_ARCHIVE ─────────────────────────────────────────────────────

/// <summary>
/// Archive partagée — couvre à la fois les entrants et les sortants.
/// Une seule des deux FK (CourrierEntrantId / CourrierSortantId) doit être renseignée.
/// Numérotation : ARC/2025/000001
/// </summary>
public class BOArchive : BaseEntity
{
    public int? CourrierEntrantId { get; set; }
    public BOCourrierEntrant? CourrierEntrant { get; set; }

    public int? CourrierSortantId { get; set; }
    public BOCourrierSortant? CourrierSortant { get; set; }

    [Required, MaxLength(100)] public string NumeroArchive { get; set; } = string.Empty;
    [MaxLength(100)] public string? CodeBarre { get; set; }

    // ── Emplacement physique ───────────────────────────────────────
    [MaxLength(100)] public string? SalleArchive { get; set; }
    [MaxLength(50)] public string? Rayon { get; set; }
    [MaxLength(50)] public string? Boite { get; set; }

    // ── Archivage numérique ────────────────────────────────────────
    [MaxLength(1000)] public string? CheminArchiveNumerique { get; set; }

    // ── Conservation ──────────────────────────────────────────────
    public ClassificationArchive Classification { get; set; } = ClassificationArchive.Courant;
    public short DureeConservationAns { get; set; } = 10;
    public DateTime DateDebutConservation { get; set; } = DateTime.UtcNow;
    public DateTime? DateFinConservation { get; set; }
    public DateTime? DateDestructionPrevue { get; set; }
    public bool EstDetruit { get; set; } = false;

    [Required, MaxLength(450)] public string ArchiveParId { get; set; } = string.Empty;
    public DateTime DateArchivage { get; set; } = DateTime.UtcNow;
    public string? Observation { get; set; }
}