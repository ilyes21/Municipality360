using System.ComponentModel.DataAnnotations;

namespace Municipality360.Application.DTOs.BureauOrdre;

// ══════════════════════════════════════════════════════════════
//  CONTACT
// ══════════════════════════════════════════════════════════════

public class BOContactDto
{
    public int Id { get; set; }
    public string TypeContact { get; set; } = string.Empty;
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
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
    public string TypeContact { get; set; } = string.Empty;
    [MaxLength(100)] public string? Nom { get; set; }
    [MaxLength(100)] public string? Prenom { get; set; }
    [MaxLength(300)] public string? RaisonSociale { get; set; }
    [MaxLength(200)] public string? Fonction { get; set; }
    [MaxLength(500)] public string? Adresse { get; set; }
    [MaxLength(100)] public string? Ville { get; set; }
    [MaxLength(100)] public string? Wilaya { get; set; }
    [MaxLength(30)] public string? Telephone { get; set; }
    [MaxLength(256), EmailAddress] public string? Email { get; set; }
}

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

public class CreateBOCategorieDto
{
    [Required, MaxLength(30)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(7)] public string? CouleurHex { get; set; }
    public bool EstConfidentiel { get; set; } = false;
}

// ══════════════════════════════════════════════════════════════
//  DOSSIER
// ══════════════════════════════════════════════════════════════

public class BODossierDto
{
    public int Id { get; set; }
    public string NumeroDossier { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ServiceResponsableId { get; set; }
    public string? ServiceResponsableNom { get; set; }
    public DateTime DateOuverture { get; set; }
    public DateTime? DateCloture { get; set; }
    public string StatutDossier { get; set; } = string.Empty;
    public int NombreCourriersEntrants { get; set; }
    public int NombreCourriersSortants { get; set; }
}

public class CreateBODossierDto
{
    [Required, MaxLength(300)] public string Intitule { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ServiceResponsableId { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  COURRIER ENTRANT
// ══════════════════════════════════════════════════════════════

public class CourrierEntrantDto
{
    public int Id { get; set; }
    public string NumeroOrdre { get; set; } = string.Empty;
    public string? NumeroExterne { get; set; }
    public DateTime DateReception { get; set; }
    public DateTime DateCourrier { get; set; }
    public string ObjetCourrier { get; set; } = string.Empty;
    public string TypeDocument { get; set; } = string.Empty;
    public int? CategorieId { get; set; }
    public string? CategorieLibelle { get; set; }
    public int? DossierId { get; set; }
    public string? DossierIntitule { get; set; }
    // Expéditeur
    public int? ExpediteurContactId { get; set; }
    public string? ExpediteurNom { get; set; }
    public string ModeReception { get; set; } = string.Empty;
    public string? NumeroRecommande { get; set; }
    // Destinataire
    public int? ServiceDestinataireId { get; set; }
    public string? ServiceDestinataireNom { get; set; }
    public string? AgentDestinataireId { get; set; }
    // Caractéristiques
    public short NombrePages { get; set; }
    public string Priorite { get; set; } = string.Empty;
    public bool EstConfidentiel { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime? DelaiReponse { get; set; }
    public bool NecessiteReponse { get; set; }
    public string? Observation { get; set; }
    public int NombrePiecesJointes { get; set; }
    public int NombreEtapesCircuit { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourrierEntrantDetailDto : CourrierEntrantDto
{
    public List<BOCircuitTraitementDto> Circuit { get; set; } = new();
    public List<BOPieceJointeDto> PiecesJointes { get; set; } = new();
    public List<CourrierSortantDto> Reponses { get; set; } = new();
}

public class CreateCourrierEntrantDto
{
    [Required]
    public DateTime DateCourrier { get; set; }

    public DateTime DateReception { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(500)]
    public string ObjetCourrier { get; set; } = string.Empty;

    [Required]
    public string TypeDocument { get; set; } = "Lettre";

    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }

    // Expéditeur (contact enregistré OU nom libre)
    public int? ExpediteurContactId { get; set; }

    [MaxLength(300)]
    public string? ExpediteurLibreNom { get; set; }

    [Required]
    public string ModeReception { get; set; } = "Guichet";

    [MaxLength(60)]
    public string? NumeroRecommande { get; set; }

    public int? ServiceDestinataireId { get; set; }
    public string? AgentDestinataireId { get; set; }

    public short NombrePages { get; set; } = 1;
    public string Priorite { get; set; } = "Normal";
    public bool EstConfidentiel { get; set; } = false;
    public DateTime? DelaiReponse { get; set; }
    public bool NecessiteReponse { get; set; } = false;
    public string? Observation { get; set; }

    [MaxLength(100)]
    public string? NumeroExterne { get; set; }
}

public class UpdateCourrierEntrantDto
{
    [Required, MaxLength(500)] public string ObjetCourrier { get; set; } = string.Empty;
    public string TypeDocument { get; set; } = "Lettre";
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public int? ServiceDestinataireId { get; set; }
    public string? AgentDestinataireId { get; set; }
    public string Priorite { get; set; } = "Normal";
    public DateTime? DelaiReponse { get; set; }
    public bool NecessiteReponse { get; set; }
    public string? Observation { get; set; }
}

public class CourrierEntrantFilterDto
{
    public string? TypeDocument { get; set; }
    public string? Statut { get; set; }
    public string? Priorite { get; set; }
    public int? ServiceDestinataireId { get; set; }
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public bool? NecessiteReponse { get; set; }
    public bool? EnRetard { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// ── Circuit de traitement ─────────────────────────────────────

public class BOCircuitTraitementDto
{
    public int Id { get; set; }
    public short NumeroEtape { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public string? ServiceEmetteurNom { get; set; }
    public int ServiceRecepteurId { get; set; }
    public string ServiceRecepteurNom { get; set; } = string.Empty;
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
}

public class AcheminerCourrierDto
{
    [Required]
    public int ServiceRecepteurId { get; set; }

    public string? AgentRecepteurId { get; set; }

    [Required]
    public string TypeAction { get; set; } = "PourAction";

    public string? InstructionTransmission { get; set; }

    public DateTime? DelaiTraitement { get; set; }
}

public class TraiterEtapeCircuitDto
{
    [Required, MaxLength(500)]
    public string ActionEffectuee { get; set; } = string.Empty;

    public string? CommentaireTraitement { get; set; }

    public bool EstRetour { get; set; } = false;

    [MaxLength(300)]
    public string? MotifRetour { get; set; }
}

public class ChangerStatutEntrantDto
{
    [Required]
    public string NouveauStatut { get; set; } = string.Empty;

    public string? Commentaire { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  COURRIER SORTANT
// ══════════════════════════════════════════════════════════════

public class CourrierSortantDto
{
    public int Id { get; set; }
    public string NumeroOrdre { get; set; } = string.Empty;
    public string? NumeroReference { get; set; }
    public int? CourrierEntrantRefId { get; set; }
    public string? CourrierEntrantRefNumero { get; set; }
    public DateTime DateRedaction { get; set; }
    public DateTime? DateSignature { get; set; }
    public DateTime? DateEnvoi { get; set; }
    public string ObjetCourrier { get; set; } = string.Empty;
    public string TypeDocument { get; set; } = string.Empty;
    public int? CategorieId { get; set; }
    public string? CategorieLibelle { get; set; }
    public int? DossierId { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public string? ServiceEmetteurNom { get; set; }
    public string? FonctionSignataire { get; set; }
    public bool EstSigne { get; set; }
    public int? DestinataireContactId { get; set; }
    public string? DestinataireNom { get; set; }
    public string ModeEnvoi { get; set; } = string.Empty;
    public string? NumeroRecommande { get; set; }
    public string Priorite { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public bool AccuseReceptionRecu { get; set; }
    public int NombrePiecesJointes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourrierSortantDetailDto : CourrierSortantDto
{
    public List<BOPieceJointeDto> PiecesJointes { get; set; } = new();
    public string? Observation { get; set; }
}

public class CreateCourrierSortantDto
{
    [Required, MaxLength(500)]
    public string ObjetCourrier { get; set; } = string.Empty;

    [Required]
    public string TypeDocument { get; set; } = "Lettre";

    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }

    // Lien réponse (optionnel)
    public int? CourrierEntrantRefId { get; set; }

    // Émetteur
    public int? ServiceEmetteurId { get; set; }

    // Destinataire (contact enregistré OU nom libre)
    public int? DestinataireContactId { get; set; }

    [MaxLength(300)]
    public string? DestinataireLibreNom { get; set; }

    [Required]
    public string ModeEnvoi { get; set; } = "Guichet";

    [MaxLength(60)]
    public string? NumeroRecommande { get; set; }

    [MaxLength(200)]
    public string? FonctionSignataire { get; set; }

    public short NombrePages { get; set; } = 1;
    public bool EstConfidentiel { get; set; } = false;
    public string Priorite { get; set; } = "Normal";
    public string? Observation { get; set; }
}

public class CourrierSortantFilterDto
{
    public string? TypeDocument { get; set; }
    public string? Statut { get; set; }
    public string? Priorite { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// ── Pièce jointe commune ──────────────────────────────────────

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
}

// ── Archive ───────────────────────────────────────────────────

public class BOArchiveDto
{
    public int Id { get; set; }
    public string NumeroArchive { get; set; } = string.Empty;
    public string? CodeBarre { get; set; }
    public string? SalleArchive { get; set; }
    public string? Rayon { get; set; }
    public string? Boite { get; set; }
    public string Classification { get; set; } = string.Empty;
    public short DureeConservationAns { get; set; }
    public DateTime DateDebutConservation { get; set; }
    public DateTime? DateFinConservation { get; set; }
    public bool EstDetruit { get; set; }
    public DateTime DateArchivage { get; set; }
}

public class ArchiversCourrierDto
{
    [MaxLength(100)] public string? SalleArchive { get; set; }
    [MaxLength(50)] public string? Rayon { get; set; }
    [MaxLength(50)] public string? Boite { get; set; }
    public string Classification { get; set; } = "Courant";
    public short DureeConservationAns { get; set; } = 10;
    [MaxLength(1000)] public string? CheminArchiveNumerique { get; set; }
    public string? Observation { get; set; }
}

// ── Statistiques ──────────────────────────────────────────────

public class BOStatsDto
{
    public int TotalEntrantsAujourdhui { get; set; }
    public int TotalSortantsAujourdhui { get; set; }
    public int EnAttente { get; set; }
    public int EnRetard { get; set; }
    public int Urgents { get; set; }
    public int NonTraites { get; set; }
}