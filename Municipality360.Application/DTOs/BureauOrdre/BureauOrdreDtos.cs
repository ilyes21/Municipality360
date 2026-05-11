// ═══════════════════════════════════════════════════════════════════
//  BureauOrdreDtos.cs  ✅ COMPLET & MIS À JOUR
//  Application/DTOs/BureauOrdre/BureauOrdreDtos.cs
//
//  Changements vs version précédente:
//  ✅ CreateCourrierEntrantDto  — champs complets alignés avec l'entity
//  ✅ UpdateCourrierEntrantDto  — champs complets
//  ✅ CourrierEntrantDetailDto  — champs complets + NombrePages, NumeroRecommande
//  ✅ AffecterCourrierEntrantDto — nouveau (affectation directe sans circuit)
//  ✅ AjouterPieceJointeDto      — nouveau (upload serveur)
//  ✅ UploadPiecesJointesDto     — nouveau (multipart)
//  ✅ CreateCourrierEntrantAvecFichiersDto — nouveau (multipart tout-en-un)
//  ✅ RetournerEtapeCircuitDto   — nouveau
//  ✅ PieceJointeDetailDto       — avec CheminFichier pour téléchargement
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Municipality360.Application.DTOs.BureauOrdre;

// ══════════════════════════════════════════════════════════════
//  STATS
// ══════════════════════════════════════════════════════════════

public class BOStatsDto
{
    public int TotalEntrants { get; set; }
    public int Enregistres { get; set; }
    public int EnCours { get; set; }
    public int Traites { get; set; }
    public int Archives { get; set; }
    public int EnRetard { get; set; }
    public int Urgents { get; set; }
    // Sortants
    public int TotalSortants { get; set; }
    public int Brouillons { get; set; }
    public int Envoyes { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  PIÈCES JOINTES
// ══════════════════════════════════════════════════════════════

/// <summary>DTO lecture (liste/détail)</summary>
public class BOPieceJointeDto
{
    public int Id { get; set; }
    public string NomFichierOriginal { get; set; } = string.Empty;
    public string ExtensionFichier { get; set; } = string.Empty;
    public long TailleFichierOctets { get; set; }
    public string TypePiece { get; set; } = string.Empty;
    public short Ordre { get; set; }
    public string? Description { get; set; }
    public bool EstVersionFinale { get; set; }
    // URL de téléchargement calculée par le service
    public string? UrlTelechargement { get; set; }
}

/// <summary>DTO interne pour upload serveur (créé par le controller)</summary>
public class AjouterPieceJointeDto
{
    public string NomFichierOriginal { get; set; } = string.Empty;
    public string NomFichierStocke { get; set; } = string.Empty;
    public string CheminFichier { get; set; } = string.Empty;
    public string ExtensionFichier { get; set; } = string.Empty;
    public long TailleFichierOctets { get; set; }
    public string TypePiece { get; set; } = "Annexe";
    public string? Description { get; set; }
    public short Ordre { get; set; } = 1;
    public string UploadedById { get; set; } = string.Empty;
}

/// <summary>DTO avec CheminFichier pour téléchargement physique</summary>
public class PieceJointeDetailDto : BOPieceJointeDto
{
    public string CheminFichier { get; set; } = string.Empty;
}

/// <summary>Form multipart — upload de fichiers seuls vers un courrier existant</summary>
public class UploadPiecesJointesDto
{
    public List<IFormFile>? Fichiers { get; set; }
    public string? TypePiece { get; set; }
    public string? Description { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  COURRIER ENTRANT — CREATE / UPDATE
// ══════════════════════════════════════════════════════════════

public class CreateCourrierEntrantDto
{
    // ── Identification ────────────────────────────────────────
    /// <summary>Numéro externe de la correspondance si disponible</summary>
    [MaxLength(100)]
    public string? NumeroExterne { get; set; }

    // ── Dates ─────────────────────────────────────────────────
    /// <summary>Date figurant sur le document reçu</summary>
    [Required]
    public DateTime DateCourrier { get; set; }

    /// <summary>Date de réception physique au bureau d'ordre</summary>
    [Required]
    public DateTime DateReception { get; set; } = DateTime.Today;

    // ── Contenu ───────────────────────────────────────────────
    [Required, MaxLength(500)]
    public string ObjetCourrier { get; set; } = string.Empty;

    /// <summary>Lettre, Note, Circulaire, Rapport, Decision, Arrete, Instruction, Demande, Recours, Facture, Autre</summary>
    [Required]
    public string TypeDocument { get; set; } = "Lettre";

    /// <summary>Catégorie BOCategorieCourrier (facultatif)</summary>
    public int? CategorieId { get; set; }

    /// <summary>Rattacher à un dossier existant (facultatif)</summary>
    public int? DossierId { get; set; }

    // ── Expéditeur ────────────────────────────────────────────
    /// <summary>Expéditeur issu du référentiel BOContact (facultatif)</summary>
    public int? ExpediteurContactId { get; set; }

    /// <summary>Expéditeur libre (si non répertorié dans les contacts)</summary>
    [MaxLength(200)]
    public string? ExpediteurLibreNom { get; set; }

    // ── Réception ─────────────────────────────────────────────
    /// <summary>Guichet, Messagerie, Email, Fax, Recommande, Coursier, PSEM</summary>
    [Required]
    public string ModeReception { get; set; } = "Guichet";

    /// <summary>Numéro recommandé / AR si applicable</summary>
    [MaxLength(100)]
    public string? NumeroRecommande { get; set; }

    // ── Destination ───────────────────────────────────────────
    /// <summary>Service destinataire (peut être renseigné à la création ou plus tard)</summary>
    public int? ServiceDestinataireId { get; set; }

    /// <summary>Agent destinataire au sein du service (facultatif)</summary>
    [MaxLength(450)]
    public string? AgentDestinataireId { get; set; }

    // ── Caractéristiques physiques ────────────────────────────
    public short NombrePages { get; set; } = 1;
    public short NombrePiecesJointes { get; set; } = 0;

    // ── Priorité & confidentialité ────────────────────────────
    /// <summary>Normal, Urgent, TresUrgent, Confidentiel</summary>
    public string Priorite { get; set; } = "Normal";
    public bool EstConfidentiel { get; set; }

    // ── Traitement ────────────────────────────────────────────
    public bool NecessiteReponse { get; set; }
    public DateTime? DelaiReponse { get; set; }

    // ── Observations ──────────────────────────────────────────
    [MaxLength(1000)]
    public string? Observation { get; set; }
}

/// <summary>
/// Form multipart: tous les champs de CreateCourrierEntrantDto
/// + liste de fichiers optionnels (pdf/png/jpg/tiff).
/// </summary>
public class CreateCourrierEntrantAvecFichiersDto
{
    // ── mêmes champs que CreateCourrierEntrantDto ─────────────
    [MaxLength(100)]
    public string? NumeroExterne { get; set; }

    [Required]
    public DateTime DateCourrier { get; set; }

    [Required]
    public DateTime DateReception { get; set; } = DateTime.Today;

    [Required, MaxLength(500)]
    public string ObjetCourrier { get; set; } = string.Empty;

    [Required]
    public string TypeDocument { get; set; } = "Lettre";

    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public int? ExpediteurContactId { get; set; }

    [MaxLength(200)]
    public string? ExpediteurLibreNom { get; set; }

    [Required]
    public string ModeReception { get; set; } = "Guichet";

    [MaxLength(100)]
    public string? NumeroRecommande { get; set; }

    public int? ServiceDestinataireId { get; set; }

    [MaxLength(450)]
    public string? AgentDestinataireId { get; set; }

    public short NombrePages { get; set; } = 1;
    public string Priorite { get; set; } = "Normal";
    public bool EstConfidentiel { get; set; }
    public bool NecessiteReponse { get; set; }
    public DateTime? DelaiReponse { get; set; }

    [MaxLength(1000)]
    public string? Observation { get; set; }

    // ── Fichiers ──────────────────────────────────────────────
    public List<IFormFile>? Fichiers { get; set; }

    /// <summary>Convertir vers DTO standard sans fichiers</summary>
    public CreateCourrierEntrantDto ToCreateDto() => new()
    {
        NumeroExterne = NumeroExterne,
        DateCourrier = DateCourrier,
        DateReception = DateReception,
        ObjetCourrier = ObjetCourrier,
        TypeDocument = TypeDocument,
        CategorieId = CategorieId,
        DossierId = DossierId,
        ExpediteurContactId = ExpediteurContactId,
        ExpediteurLibreNom = ExpediteurLibreNom,
        ModeReception = ModeReception,
        NumeroRecommande = NumeroRecommande,
        ServiceDestinataireId = ServiceDestinataireId,
        AgentDestinataireId = AgentDestinataireId,
        NombrePages = NombrePages,
        NombrePiecesJointes = (short)(Fichiers?.Count ?? 0),
        Priorite = Priorite,
        EstConfidentiel = EstConfidentiel,
        NecessiteReponse = NecessiteReponse,
        DelaiReponse = DelaiReponse,
        Observation = Observation
    };
}

public class UpdateCourrierEntrantDto
{
    [Required, MaxLength(500)]
    public string ObjetCourrier { get; set; } = string.Empty;

    [Required]
    public string TypeDocument { get; set; } = "Lettre";

    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }

    // Expéditeur (modifiable en cas d'erreur)
    public int? ExpediteurContactId { get; set; }

    [MaxLength(200)]
    public string? ExpediteurLibreNom { get; set; }

    public int? ServiceDestinataireId { get; set; }

    [MaxLength(450)]
    public string? AgentDestinataireId { get; set; }

    public string Priorite { get; set; } = "Normal";
    public bool EstConfidentiel { get; set; }
    public bool NecessiteReponse { get; set; }
    public DateTime? DelaiReponse { get; set; }
    public short NombrePages { get; set; } = 1;

    [MaxLength(100)]
    public string? NumeroRecommande { get; set; }

    [MaxLength(1000)]
    public string? Observation { get; set; }
}

/// <summary>
/// Affectation directe (sans passer par le circuit de traitement).
/// Utilisé pour corriger / ré-affecter rapidement.
/// </summary>
public class AffecterCourrierEntrantDto
{
    [Required]
    public int ServiceDestinataireId { get; set; }

    [MaxLength(450)]
    public string? AgentDestinataireId { get; set; }

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}

public class ChangerStatutEntrantDto
{
    /// <summary>Recu | Enregistre | EnCours | Traite | Archive | Rejete</summary>
    [Required]
    public string NouveauStatut { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  COURRIER ENTRANT — LECTURE
// ══════════════════════════════════════════════════════════════

/// <summary>DTO liste (compact)</summary>
public class CourrierEntrantDto
{
    public int Id { get; set; }
    public string NumeroOrdre { get; set; } = string.Empty;
    public string? NumeroExterne { get; set; }
    public DateTime DateReception { get; set; }
    public DateTime DateCourrier { get; set; }
    public string ObjetCourrier { get; set; } = string.Empty;
    public string TypeDocument { get; set; } = string.Empty;
    public string ExpediteurNom { get; set; } = string.Empty;
    public string ModeReception { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public bool EstConfidentiel { get; set; }
    public string Statut { get; set; } = string.Empty;
    public bool NecessiteReponse { get; set; }
    public DateTime? DelaiReponse { get; set; }
    public int? ServiceDestinataireId { get; set; }
    public string? ServiceDestinataireNom { get; set; }
    public string? AgentDestinataireId { get; set; }
    public int NombrePiecesJointes { get; set; }
    public int NombreEtapesCircuit { get; set; }
    public bool EnRetard => NecessiteReponse
                                               && DelaiReponse.HasValue
                                               && DelaiReponse.Value.Date < DateTime.Today
                                               && Statut is not "Traite" and not "Archive";
    public DateTime CreatedAt { get; set; }
}

/// <summary>DTO détail complet (drawer / page détail)</summary>
public class CourrierEntrantDetailDto : CourrierEntrantDto
{
    public int? CategorieId { get; set; }
    public string? CategorieLibelle { get; set; }
    public int? DossierId { get; set; }
    public string? DossierIntitule { get; set; }
    public int? ExpediteurContactId { get; set; }
    public string? NumeroRecommande { get; set; }
    public short NombrePages { get; set; }
    public string? Observation { get; set; }
    public string? EnregistreParId { get; set; }
    public string? AgentDestinataireNom { get; set; }

    public List<BOPieceJointeDto> PiecesJointes { get; set; } = new();
    public List<BOCircuitTraitementDto> Circuit { get; set; } = new();
    public BOArchiveDto? Archive { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  FILTRES COURRIER ENTRANT
// ══════════════════════════════════════════════════════════════

public class CourrierEntrantFilterDto
{
    public string? Statut { get; set; }
    public string? Priorite { get; set; }
    public string? TypeDocument { get; set; }
    public int? ServiceDestinataireId { get; set; }
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public bool? NecessiteReponse { get; set; }
    public bool? EnRetard { get; set; }
    public bool? EstConfidentiel { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

// ══════════════════════════════════════════════════════════════
//  CIRCUIT DE TRAITEMENT
// ══════════════════════════════════════════════════════════════

public class AcheminerCourrierDto
{
    [Required]
    public int ServiceRecepteurId { get; set; }

    [MaxLength(450)]
    public string? AgentRecepteurId { get; set; }

    /// <summary>PourAction | PourDecision | PourInformation | PourSignature | PourSuite</summary>
    [Required]
    public string TypeAction { get; set; } = "PourAction";

    [MaxLength(500)]
    public string? InstructionTransmission { get; set; }

    public DateTime? DelaiTraitement { get; set; }
}

public class TraiterEtapeCircuitDto
{
    [Required]
    public string StatutEtape { get; set; } = "Traite";

    [MaxLength(500)]
    public string? CommentaireTraitement { get; set; }

    [MaxLength(500)]
    public string? ActionEffectuee { get; set; }
}

public class RetournerEtapeCircuitDto
{
    [Required, MaxLength(500)]
    public string MotifRetour { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}

public class BOCircuitTraitementDto
{
    public int Id { get; set; }
    public short NumeroEtape { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public string? ServiceEmetteurNom { get; set; }
    public int ServiceRecepteurId { get; set; }
    public string ServiceRecepteurNom { get; set; } = string.Empty;
    public string? AgentRecepteurId { get; set; }
    public DateTime DateTransmission { get; set; }
    public DateTime? DateTraitement { get; set; }
    public DateTime? DelaiTraitement { get; set; }
    public string TypeAction { get; set; } = string.Empty;
    public string? InstructionTransmission { get; set; }
    public string? CommentaireTraitement { get; set; }
    public string? ActionEffectuee { get; set; }
    public string StatutEtape { get; set; } = string.Empty;
    public bool EstRetour { get; set; }
    public string? MotifRetour { get; set; }
    public bool EnRetard => DelaiTraitement.HasValue
                                && DelaiTraitement.Value.Date < DateTime.Today
                                && StatutEtape is not "Traite" and not "Retourne";
}

// ══════════════════════════════════════════════════════════════
//  COURRIER SORTANT
// ══════════════════════════════════════════════════════════════

public class CourrierSortantDto
{
    public int Id { get; set; }
    public string NumeroOrdre { get; set; } = string.Empty;
    public string ObjetCourrier { get; set; } = string.Empty;
    public string TypeDocument { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public bool EstSigne { get; set; }
    public bool EstConfidentiel { get; set; }
    public DateTime DateRedaction { get; set; }
    public DateTime? DateEnvoi { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public string? DestinataireNom { get; set; }
    public int NombrePiecesJointes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourrierSortantDetailDto : CourrierSortantDto
{
    public string? NumeroReference { get; set; }
    public int? CourrierEntrantRefId { get; set; }
    public string? CourrierEntrantRefNumero { get; set; }
    public DateTime? DateSignature { get; set; }
    public int? CategorieId { get; set; }
    public string? CategorieLibelle { get; set; }
    public int? DossierId { get; set; }
    public string? ServiceEmetteurNom { get; set; }
    public string? FonctionSignataire { get; set; }
    public int? DestinataireContactId { get; set; }
    public string ModeEnvoi { get; set; } = string.Empty;
    public string? NumeroRecommande { get; set; }
    public bool AccuseReceptionRecu { get; set; }
    public string? Observation { get; set; }
    public List<BOPieceJointeDto> PiecesJointes { get; set; } = new();
}

public class CreateCourrierSortantDto
{
    [Required, MaxLength(500)]
    public string ObjetCourrier { get; set; } = string.Empty;

    [Required]
    public string TypeDocument { get; set; } = "Lettre";

    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public int? CourrierEntrantRefId { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public int? DestinataireContactId { get; set; }

    [MaxLength(200)]
    public string? DestinataireLibreNom { get; set; }

    public string ModeEnvoi { get; set; } = "Messagerie";

    [MaxLength(100)]
    public string? NumeroRecommande { get; set; }

    [MaxLength(200)]
    public string? FonctionSignataire { get; set; }

    public short NombrePages { get; set; } = 1;
    public bool EstConfidentiel { get; set; }
    public string Priorite { get; set; } = "Normal";

    [MaxLength(1000)]
    public string? Observation { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  ARCHIVE
// ══════════════════════════════════════════════════════════════

public class ArchiversCourrierDto
{
    [MaxLength(100)]
    public string? SalleArchive { get; set; }

    [MaxLength(100)]
    public string? Rayon { get; set; }

    [MaxLength(100)]
    public string? Boite { get; set; }

    /// <summary>Courant | Intermediaire | Definitif</summary>
    public string Classification { get; set; } = "Courant";

    public short DureeConservationAns { get; set; } = 10;

    [MaxLength(500)]
    public string? CheminArchiveNumerique { get; set; }

    [MaxLength(500)]
    public string? Observation { get; set; }
}

public class BOArchiveDto
{
    public int Id { get; set; }
    public int? CourrierEntrantId { get; set; }
    public int? CourrierSortantId { get; set; }
    public string NumeroArchive { get; set; } = string.Empty;
    public string? CodeBarre { get; set; }
    public string? SalleArchive { get; set; }
    public string? Rayon { get; set; }
    public string? Boite { get; set; }
    public string Classification { get; set; } = string.Empty;
    public short DureeConservationAns { get; set; }
    public DateTime DateDebutConservation { get; set; }
    public DateTime? DateFinConservation { get; set; }
    public string? CheminArchiveNumerique { get; set; }
    public string? ArchiveParId { get; set; }
    public string? Observation { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  DOSSIERS & CONTACTS (inchangés — pour référence)
// ══════════════════════════════════════════════════════════════

public class BOContactDto
{
    public int Id { get; set; }
    public string TypeContact { get; set; } = string.Empty;
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? CIN { get; set; }
    public string? RaisonSociale { get; set; }
    public string? Fonction { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? Wilaya { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateBOContactDto
{
    [Required]
    public string TypeContact { get; set; } = "Personne";

    [MaxLength(100)]
    public string? Nom { get; set; }

    [MaxLength(100)]
    public string? Prenom { get; set; }

    [MaxLength(20)]
    public string? CIN { get; set; }

    [MaxLength(200)]
    public string? RaisonSociale { get; set; }

    [MaxLength(100)]
    public string? Fonction { get; set; }

    [MaxLength(300)]
    public string? Adresse { get; set; }

    [MaxLength(100)]
    public string? Ville { get; set; }

    [MaxLength(100)]
    public string? Wilaya { get; set; }

    [MaxLength(50)]
    public string? Telephone { get; set; }

    [MaxLength(256), EmailAddress]
    public string? Email { get; set; }
}

public class BODossierDto
{
    public int Id { get; set; }
    public string NumeroDossier { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ServiceResponsableId { get; set; }
    public DateTime DateOuverture { get; set; }
    public DateTime? DateCloture { get; set; }
    public string StatutDossier { get; set; } = string.Empty;
}

public class CreateBODossierDto
{
    [Required, MaxLength(200)]
    public string Intitule { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? ServiceResponsableId { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  FILTER COURRIER SORTANT
// ══════════════════════════════════════════════════════════════

public class CourrierSortantFilterDto
{
    public string? Statut { get; set; }
    public string? Priorite { get; set; }
    public string? TypeDocument { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public class SignerCourrierDto
{
    [Required, MaxLength(200)]
    public string FonctionSignataire { get; set; } = string.Empty;
}

public class EnvoyerCourrierDto
{
    public DateTime? DateEnvoi { get; set; }
}

public class AnnulerCourrierDto
{
    [Required, MaxLength(500)]
    public string Motif { get; set; } = string.Empty;
}
public record ServiceItemDto(int Id, string Nom);

// ══════════════════════════════════════════════════════════════
//  CATÉGORIE COURRIER
// ══════════════════════════════════════════════════════════════

public class BOCategorieCourrierDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CouleurHex { get; set; }
    public bool EstConfidentiel { get; set; }
    public bool IsActive { get; set; }
}