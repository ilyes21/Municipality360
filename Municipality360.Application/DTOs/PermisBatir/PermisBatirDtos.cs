// ═══════════════════════════════════════════════════════════════════
//  PermisBatirDtos.cs  ✅ FINAL
//  Application/DTOs/PermisBatir/PermisBatirDtos.cs
//
//  يحتوي على جميع DTOs لوحدة رخص البناء:
//  ZonageUrbanisme · TypeDemandePermis · TypeTaxe
//  Demandeur · Architecte · CommissionExamen
//  DemandePermisBatir · DocumentPermis · TaxePermis
//  PermisDelivre · SuiviPermis · InspectionPermis
//  Stats
//
//  ⚠️ تنبيه التزامن — PermisStatsDto:
//    الحقول تطابق DemandePermisBatirRepository.GetStatsAsync():
//    TotalDemandes · Deposees · EnExamen · Approuvees
//    Rejetees · TotalPermisDelivres
//
//  ⚠️ تنبيه — CreateInspectionDto:
//    الملف الأصلي يحتوي على حقول خاطئة (نسخ من FilterDto).
//    تم الإصلاح: الحقول الصحيحة لعملية الإضافة.
// ═══════════════════════════════════════════════════════════════════

using System.ComponentModel.DataAnnotations;

namespace Municipality360.Application.DTOs.PermisBatir;

// ══════════════════════════════════════════════════════════════
//  ZONAGE URBANISME
// ══════════════════════════════════════════════════════════════

public class ZonageUrbanismeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? CoefficientOccupationSol { get; set; }
    public decimal? CoefficientUtilisationSol { get; set; }
    public decimal? HauteurMaximale { get; set; }
    public bool IsActive { get; set; }
}

public class CreateZonageDto
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Description { get; set; }
    public decimal? CoefficientOccupationSol { get; set; }
    public decimal? CoefficientUtilisationSol { get; set; }
    public decimal? HauteurMaximale { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  TYPE DEMANDE PERMIS
// ══════════════════════════════════════════════════════════════

public class TypeDemandePermisDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public int DelaiTraitementJours { get; set; }
    public decimal? TarifBase { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTypeDemandePermisDto
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    public int DelaiTraitementJours { get; set; } = 30;
    public decimal? TarifBase { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  TYPE TAXE
// ══════════════════════════════════════════════════════════════

public class TypeTaxeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public decimal? TauxCalcul { get; set; }
    public string? UniteCalcul { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTypeTaxeDto
{
    [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Libelle { get; set; } = string.Empty;
    public decimal? TauxCalcul { get; set; }
    [MaxLength(50)] public string? UniteCalcul { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  DEMANDEUR
// ══════════════════════════════════════════════════════════════

public class DemandeurDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? CIN { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Prenom { get; set; }
    public string? RaisonSociale { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public int NombreDemandes { get; set; }
}

public class CreateDemandeurDto
{
    [Required]
    public string Type { get; set; } = "Personne";

    [MaxLength(20)]
    public string? CIN { get; set; }

    [Required, MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Prenom { get; set; }

    [MaxLength(300)]
    public string? RaisonSociale { get; set; }

    [Required, MaxLength(500)]
    public string Adresse { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Telephone { get; set; } = string.Empty;

    [MaxLength(256), EmailAddress]
    public string? Email { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  ARCHITECTE
// ══════════════════════════════════════════════════════════════

public class ArchitecteDto
{
    public int Id { get; set; }
    public string NumeroOrdre { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class CreateArchitecteDto
{
    [Required, MaxLength(50)] public string NumeroOrdre { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string CIN { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Nom { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Prenom { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string Telephone { get; set; } = string.Empty;
    [MaxLength(256), EmailAddress] public string? Email { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  COMMISSION D'EXAMEN
// ══════════════════════════════════════════════════════════════

public class CommissionExamenDto
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public DateTime? DateReunion { get; set; }
    public string PresidentId { get; set; } = string.Empty;
    public string? PresidentNom { get; set; }
    public string StatutReunion { get; set; } = string.Empty;
    public string? ProcesVerbal { get; set; }
    public int NombreDemandes { get; set; }
}

public class CreateCommissionExamenDto
{
    [Required, MaxLength(300)] public string Libelle { get; set; } = string.Empty;
    public DateTime? DateReunion { get; set; }
    [Required] public string PresidentId { get; set; } = string.Empty;
    public string? SecretaireId { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  DEMANDE PERMIS DE BÂTIR — DTO liste (court)
// ══════════════════════════════════════════════════════════════

public class DemandePermisDto
{
    public int Id { get; set; }
    public string NumeroDemande { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; }
    // Classification
    public int TypeDemandeId { get; set; }
    public string TypeDemandeLibelle { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    // Demandeur
    public int DemandeurId { get; set; }
    public string DemandeurNomComplet { get; set; } = string.Empty;
    public string DemandeurTelephone { get; set; } = string.Empty;
    // Architecte
    public int? ArchitecteId { get; set; }
    public string? ArchitecteNomComplet { get; set; }
    public string? ArchitecteNumeroOrdre { get; set; }
    // Projet
    public string AdresseProjet { get; set; } = string.Empty;
    public string? NumeroParcelle { get; set; }
    public decimal? SuperficieTerrain { get; set; }
    public decimal? SuperficieAConstruire { get; set; }
    public int? NombreNiveaux { get; set; }
    public string? TypeConstruction { get; set; }
    public decimal? CoutEstimatif { get; set; }
    public int? ZonageId { get; set; }
    public string? ZonageLibelle { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    // Instruction
    public int? ServiceInstructeurId { get; set; }
    public string? ServiceInstructeurNom { get; set; }
    public DateTime? DateDebutInstruction { get; set; }
    // Commission
    public int? CommissionExamenId { get; set; }
    public string? CommissionLibelle { get; set; }
    // Décision
    public DateTime? DateDecision { get; set; }
    // Permis délivré
    public string? NumeroPermis { get; set; }
    public DateTime? DateDelivrance { get; set; }
    public DateTime? DateValiditePermis { get; set; }
    // Taxes
    public decimal TotalTaxes { get; set; }
    public bool ToutesLTaxesPayees { get; set; }
    public int NombreDocuments { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>DTO détaillé — inclut documents, taxes, suivis, inspections et permis délivré</summary>
public class DemandePermisDetailDto : DemandePermisDto
{
    public string? MotifRejet { get; set; }
    public string? ConditionsSpeciales { get; set; }
    public string? Observations { get; set; }
    public List<DocumentPermisDto> Documents { get; set; } = new();
    public List<TaxePermisDto> Taxes { get; set; } = new();
    public List<SuiviPermisDto> Suivis { get; set; } = new();
    public List<InspectionPermisDto> Inspections { get; set; } = new();
    public PermisDelivreDto? PermisDelivre { get; set; }
}

/// <summary>Vue publique pour Flutter — citoyen sans données sensibles</summary>
public class DemandePermisSuiviPublicDto
{
    public string NumeroDemande { get; set; } = string.Empty;
    public string TypeDemande { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public string AdresseProjet { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; }
    public DateTime? DateDecision { get; set; }
    public string? NumeroPermis { get; set; }
    public DateTime? DateValiditePermis { get; set; }
    public List<SuiviPublicPermisDto> Historique { get; set; } = new();
}

public class SuiviPublicPermisDto
{
    public string Commentaire { get; set; } = string.Empty;
    public string? NouveauStatut { get; set; }
    public DateTime DateChangement { get; set; }
}

public class CreateDemandePermisDto
{
    [Required] public int TypeDemandeId { get; set; }
    [Required] public int DemandeurId { get; set; }
    public int? ArchitecteId { get; set; }

    [Required, MaxLength(500)]
    public string AdresseProjet { get; set; } = string.Empty;

    [MaxLength(100)] public string? NumeroParcelle { get; set; }
    public decimal? SuperficieTerrain { get; set; }
    public decimal? SuperficieAConstruire { get; set; }
    public int? NombreNiveaux { get; set; }
    public string? TypeConstruction { get; set; }
    public decimal? CoutEstimatif { get; set; }
    public int? ZonageId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    [MaxLength(2000)]
    public string? Observations { get; set; }
}

public class AssignerInstructeurDto
{
    public int? ServiceInstructeurId { get; set; }
    public string? AgentInstructeurId { get; set; }

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}

public class AssignerCommissionDto
{
    [Required] public int CommissionExamenId { get; set; }

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}

public class ChangerStatutPermisDto
{
    [Required] public string NouveauStatut { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Commentaire { get; set; }

    public bool VisibleCitoyen { get; set; } = false;

    [MaxLength(1000)]
    public string? MotifRejet { get; set; }

    [MaxLength(1000)]
    public string? ConditionsSpeciales { get; set; }
}

public class DelivrerPermisDto
{
    [Required, MaxLength(50)]
    public string NumeroPermis { get; set; } = string.Empty;

    [Required]
    public DateTime DateDelivrance { get; set; }

    [Required]
    public DateTime DateValidite { get; set; }

    [MaxLength(2000)]
    public string? Conditions { get; set; }

    [MaxLength(1000)]
    public string? FichierPermis { get; set; }
}

public class AjouterTaxeDto
{
    [Required] public int TypeTaxeId { get; set; }
    [Required, Range(0.01, double.MaxValue)] public decimal Montant { get; set; }
}

public class PayerTaxeDto
{
    [Required] public int TaxeId { get; set; }

    [MaxLength(100)]
    public string? NumeroRecu { get; set; }
}

public class DemandePermisFilterDto
{
    public string? Statut { get; set; }
    public int? TypeDemandeId { get; set; }
    public string? TypeConstruction { get; set; }
    public int? ServiceInstructeurId { get; set; }
    public int? CommissionExamenId { get; set; }
    public bool? TaxesPayees { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// ✅ FIXED — Le fichier original contenait les champs de DemandePermisFilterDto (copier/coller erroné).
/// Voici les vrais champs utilisés par PermisBatirService.AjouterInspectionAsync()
/// et DemandePermisBatirRepository.
/// </summary>
public class CreateInspectionDto
{
    [Required, MaxLength(300)]
    public string Objet { get; set; } = string.Empty;

    [Required]
    public DateTime DateInspection { get; set; }

    [MaxLength(200)]
    public string? NomInspecteur { get; set; }

    [MaxLength(2000)]
    public string? Observations { get; set; }

    [MaxLength(1000)]
    public string? ReservesEmises { get; set; }

    public bool Conforme { get; set; } = true;
}

// ══════════════════════════════════════════════════════════════
//  SOUS-DTOs
// ══════════════════════════════════════════════════════════════

public class DocumentPermisDto
{
    public int Id { get; set; }
    public string TypeDocument { get; set; } = string.Empty;
    public string NomFichier { get; set; } = string.Empty;
    public long? TailleFichier { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public bool EstObligatoire { get; set; }
    public bool AjouteeParCitoyen { get; set; }
}

public class TaxePermisDto
{
    public int Id { get; set; }
    public string TypeTaxeLibelle { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime? DatePaiement { get; set; }
    public string? NumeroRecu { get; set; }
}

public class PermisDelivreDto
{
    public int Id { get; set; }
    public string NumeroPermis { get; set; } = string.Empty;
    public DateTime DateDelivrance { get; set; }
    public DateTime DateValidite { get; set; }
    public string? Conditions { get; set; }
    public string? FichierPermis { get; set; }
    public bool EstRevoque { get; set; }
    public string? MotifRevocation { get; set; }
}

public class SuiviPermisDto
{
    public int Id { get; set; }
    public string? StatutPrecedent { get; set; }
    public string? NouveauStatut { get; set; }
    public DateTime DateChangement { get; set; }
    public string UtilisateurNom { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
    public bool VisibleCitoyen { get; set; }
}

public class InspectionPermisDto
{
    public int Id { get; set; }
    public string Objet { get; set; } = string.Empty;
    public DateTime DateInspection { get; set; }
    public string? NomInspecteur { get; set; }
    public string? Observations { get; set; }
    public string? ReservesEmises { get; set; }
    public bool Conforme { get; set; }
}

// ══════════════════════════════════════════════════════════════
//  STATISTIQUES PERMIS DE BÂTIR
//  ⚠️ Synchronisé avec DemandePermisBatirRepository.GetStatsAsync()
// ══════════════════════════════════════════════════════════════

public class PermisStatsDto
{
    public int TotalDemandes { get; set; }
    public int Deposees { get; set; }
    public int EnExamen { get; set; }
    public int Approuvees { get; set; }
    public int Rejetees { get; set; }
    public int TotalPermisDelivres { get; set; }
    // Indicateurs supplémentaires (calculés côté service si nécessaire)
    public int EnRetard { get; set; }
    public double DelaiMoyenJours { get; set; }
    public decimal MontantTaxesEnAttente { get; set; }
}
/// <summary>نتيجة البحث عن الرخص برقم بطاقة التعريف</summary>
public class PermisByCINResultDto
{
    public int DemandeurId { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public int NombreDemandes { get; set; }
    public List<PermisPublicItemDto> Demandes { get; set; } = new();
}

/// <summary>نتيجة GET api/mobile/mes-permis</summary>
public class MesPermisDto
{
    public string NomComplet { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;
    public int NombreDemandes { get; set; }
    public List<PermisPublicItemDto> Demandes { get; set; } = new();
}

/// <summary>رخصة مبسّطة للمواطن</summary>
public class PermisPublicItemDto
{
    public int Id { get; set; }
    public string NumeroDemande { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public string TypeDemande { get; set; } = string.Empty;
    public string AdresseProjet { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; }
    public DateTime? DateDecision { get; set; }
    public string? NumeroPermis { get; set; }
    public DateTime? DateDelivrance { get; set; }
    public DateTime? DateValidite { get; set; }
    public bool TaxesPayees { get; set; }
    public decimal TotalTaxes { get; set; }
}