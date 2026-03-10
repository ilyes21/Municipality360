// ═══════════════════════════════════════════════════════════════════
//  ReclamationsDtos.cs  ✅ FINAL
//  Application/DTOs/Reclamations/ReclamationsDtos.cs
//
//  يحتوي على جميع DTOs لوحدة الشكاوى:
//  Citoyen · TypeReclamation · CategorieReclamation
//  Reclamation · SuiviReclamation · PieceJointeReclamation
//  Stats
//
//  ⚠️ تنبيه التزامن — ReclamationStatsDto:
//    الحقول تطابق ReclamationRepository.GetStatsAsync():
//    Total · Nouvelles · EnCours · Traitees · Rejetees
//    Critiques · SatisfactionMoyenne
//
//  ⚠️ تنبيه التزامن — CitoyenFilterDto:
//    يحتوي على IsActive? مطلوب في CitoyenRepository.GetPagedAsync()
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations;

namespace Municipality360.Application.DTOs.Reclamations;

// ══════════════════════════════════════════════════════════════
//  CITOYEN
// ══════════════════════════════════════════════════════════════

public class CitoyenDto
{
    public int Id { get; set; }
    public string CIN { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public DateTime? DateNaissance { get; set; }
    public string? Sexe { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? CodePostal { get; set; }
    public string Telephone { get; set; } = string.Empty;
    public string? TelephoneMobile { get; set; }
    public string? Email { get; set; }
    public string? SituationFamiliale { get; set; }
    public bool IsActive { get; set; }
    public int NombreReclamations { get; set; }
}

public class CreateCitoyenDto
{
    [Required, MaxLength(20)]
    public string CIN { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Prenom { get; set; } = string.Empty;

    public DateTime? DateNaissance { get; set; }

    [MaxLength(200)]
    public string? LieuNaissance { get; set; }

    public string? Sexe { get; set; }

    [MaxLength(500)]
    public string? Adresse { get; set; }

    [MaxLength(100)]
    public string? Ville { get; set; }

    [MaxLength(20)]
    public string? CodePostal { get; set; }

    [Required, MaxLength(50)]
    public string Telephone { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TelephoneMobile { get; set; }

    [MaxLength(256), EmailAddress]
    public string? Email { get; set; }

    public string? SituationFamiliale { get; set; }
}

/// <summary>
/// Filtre liste citoyens.
/// ⚠️ IsActive requis par CitoyenRepository.GetPagedAsync()
/// </summary>
public class CitoyenFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Ville { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// ══════════════════════════════════════════════════════════════
//  TYPE RÉCLAMATION
// ══════════════════════════════════════════════════════════════

public class TypeReclamationDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DelaiTraitementJours { get; set; }
    public int? ServiceResponsableId { get; set; }
    public string? ServiceResponsableNom { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTypeReclamationDto
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    public int DelaiTraitementJours { get; set; } = 15;
    public int? ServiceResponsableId { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  CATÉGORIE RÉCLAMATION (hiérarchique — 3 niveaux)
// ══════════════════════════════════════════════════════════════

public class CategorieReclamationDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Icone { get; set; }
    public string? CouleurHex { get; set; }
    public int? ParentId { get; set; }
    public string? ParentLibelle { get; set; }
    public int Niveau { get; set; }
    public bool IsActive { get; set; }
    public List<CategorieReclamationDto> SousCategories { get; set; } = new();
}

public class CreateCategorieReclamationDto
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(10)] public string? Icone { get; set; }
    [MaxLength(7)] public string? CouleurHex { get; set; }
    public int? ParentId { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  RÉCLAMATION — DTO liste (court)
// ══════════════════════════════════════════════════════════════

public class ReclamationDto
{
    public int Id { get; set; }
    public string NumeroReclamation { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; }
    public DateTime? DateIncident { get; set; }
    // Classification
    public int TypeReclamationId { get; set; }
    public string TypeReclamationLibelle { get; set; } = string.Empty;
    public int CategorieId { get; set; }
    public string CategorieLibelle { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    // Réclamant
    public int CitoyenId { get; set; }
    public string CitoyenNomComplet { get; set; } = string.Empty;
    public string CitoyenTelephone { get; set; } = string.Empty;
    public bool EstAnonyme { get; set; }
    public string Canal { get; set; } = string.Empty;
    // Contenu
    public string Objet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Localisation { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    // Traitement
    public int? ServiceConcerneId { get; set; }
    public string? ServiceConcerneNom { get; set; }
    public string? AffecteAId { get; set; }
    public DateTime? DateAffectation { get; set; }
    public DateTime? DateCloture { get; set; }
    public int? SatisfactionCitoyen { get; set; }
    public int NombrePiecesJointes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>DTO détaillé — inclut suivis, pièces jointes et solution</summary>
public class ReclamationDetailDto : ReclamationDto
{
    public string? SolutionApportee { get; set; }
    public List<SuiviReclamationDto> Suivis { get; set; } = new();
    public List<PieceJointeReclamationDto> PiecesJointes { get; set; } = new();
}

/// <summary>Vue publique pour Flutter — aucune donnée sensible</summary>
public class ReclamationPublicDto
{
    public string NumeroReclamation { get; set; } = string.Empty;
    public string Objet { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public string TypeReclamation { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; }
    public DateTime? DateCloture { get; set; }
    public List<SuiviPublicDto> Historique { get; set; } = new();
}

public class SuiviPublicDto
{
    public string Commentaire { get; set; } = string.Empty;
    public string? NouveauStatut { get; set; }
    public DateTime DateChangement { get; set; }
}

public class CreateReclamationDto
{
    [Required]
    public int CitoyenId { get; set; }

    [Required]
    public int TypeReclamationId { get; set; }

    [Required]
    public int CategorieId { get; set; }

    public string Priorite { get; set; } = "Moyenne";
    public string Canal { get; set; } = "Guichet";

    [Required, MaxLength(500)]
    public string Objet { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Localisation { get; set; }

    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public DateTime? DateIncident { get; set; }
    public bool EstAnonyme { get; set; } = false;
}

public class AssignerReclamationDto
{
    public int? ServiceConcerneId { get; set; }
    public string? AffecteAId { get; set; }

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}

public class ChangerStatutReclamationDto
{
    [Required]
    public string NouveauStatut { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Commentaire { get; set; }

    [MaxLength(500)]
    public string? ActionEffectuee { get; set; }

    public bool VisibleCitoyen { get; set; } = false;

    [MaxLength(1000)]
    public string? SolutionApportee { get; set; }

    [Range(1, 5)]
    public int? SatisfactionCitoyen { get; set; }
}

public class ReclamationFilterDto
{
    public string? Statut { get; set; }
    public string? Priorite { get; set; }
    public int? TypeReclamationId { get; set; }
    public int? CategorieId { get; set; }
    public int? ServiceConcerneId { get; set; }
    public string? Canal { get; set; }
    public bool? EnRetard { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// ══════════════════════════════════════════════════════════════
//  SOUS-DTOs
// ══════════════════════════════════════════════════════════════

public class SuiviReclamationDto
{
    public int Id { get; set; }
    public string? StatutPrecedent { get; set; }
    public string? NouveauStatut { get; set; }
    public DateTime DateChangement { get; set; }
    public string UtilisateurNom { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
    public string? ActionEffectuee { get; set; }
    public bool VisibleCitoyen { get; set; }
}

public class PieceJointeReclamationDto
{
    public int Id { get; set; }
    public string? TypeDocument { get; set; }
    public string NomFichier { get; set; } = string.Empty;
    public long? TailleFichier { get; set; }
    public bool AjouteeParCitoyen { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  STATISTIQUES RÉCLAMATIONS
//  ⚠️ Synchronisé avec ReclamationRepository.GetStatsAsync()
// ══════════════════════════════════════════════════════════════

public class ReclamationStatsDto
{
    public int Total { get; set; }
    public int Nouvelles { get; set; }
    public int EnCours { get; set; }
    public int Traitees { get; set; }
    public int Rejetees { get; set; }
    public int Critiques { get; set; }
    public double SatisfactionMoyenne { get; set; }
}