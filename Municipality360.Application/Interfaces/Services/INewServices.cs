// ═══════════════════════════════════════════════════════════════════
//  INewServices.cs
//  Application/Interfaces/Services/INewServices.cs
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Interfaces.Services;

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER ENTRANT
// ════════════════════════════════════════════════════════════════

public interface ICourrierEntrantService
{
    Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter);
    Task<CourrierEntrantDetailDto> GetByIdAsync(int id);
    Task<CourrierEntrantDetailDto> GetByNumeroAsync(string numero);
    Task<List<CourrierEntrantDto>> GetEnRetardAsync();
    Task<List<CourrierEntrantDto>> GetEnAttenteParServiceAsync(int serviceId);
    Task<BOStatsDto> GetStatsAsync(int? serviceId = null);

    /// <summary>Enregistrement + génération numéro ENT/YYYY/XXXXXX</summary>
    Task<CourrierEntrantDetailDto> EnregistrerAsync(CreateCourrierEntrantDto dto, string agentId, string agentNom);
    Task<CourrierEntrantDetailDto> ModifierAsync(int id, UpdateCourrierEntrantDto dto, string agentId);
    Task ChangerStatutAsync(int id, ChangerStatutEntrantDto dto, string agentId, string agentNom);

    /// <summary>Acheminer vers un service → crée une étape dans le circuit</summary>
    Task<BOCircuitTraitementDto> AcheminerAsync(int courrierEntrantId, AcheminerCourrierDto dto, string agentId);

    /// <summary>L'agent récepteur traite / retourne son étape</summary>
    Task TraiterEtapeCircuitAsync(int circuitId, TraiterEtapeCircuitDto dto, string agentId, string agentNom);

    Task<List<BOCircuitTraitementDto>> GetCircuitAsync(int courrierEntrantId);
    Task<BOArchiveDto> ArchiverAsync(int id, ArchiversCourrierDto dto, string agentId);
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER SORTANT
// ════════════════════════════════════════════════════════════════

public interface ICourrierSortantService
{
    Task<PagedResult<CourrierSortantDto>> GetPagedAsync(CourrierSortantFilterDto filter);
    Task<CourrierSortantDetailDto> GetByIdAsync(int id);
    Task<CourrierSortantDetailDto> GetByNumeroAsync(string numero);
    Task<List<CourrierSortantDto>> GetBrouillonsAsync(string redacteurId);

    Task<CourrierSortantDetailDto> CreerBrouillonAsync(CreateCourrierSortantDto dto, string agentId);
    Task<CourrierSortantDetailDto> ModifierAsync(int id, CreateCourrierSortantDto dto, string agentId);
    Task SoumettreEnValidationAsync(int id, string agentId);
    Task MarquerSigneAsync(int id, string signatairId, string fonctionSignataire);
    Task MarquerEnvoyeAsync(int id, string agentId, DateTime? dateEnvoi = null);
    Task AccuserReceptionAsync(int id, DateTime dateAccuse, string agentId);
    Task<BOArchiveDto> ArchiverAsync(int id, ArchiversCourrierDto dto, string agentId);
    Task AnnulerAsync(int id, string motif, string agentId);
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — DOSSIERS / CONTACTS / ARCHIVE
// ════════════════════════════════════════════════════════════════

public interface IBODossierService
{
    Task<PagedResult<BODossierDto>> GetPagedAsync(string? statut, int? serviceId, string? searchTerm, int page, int pageSize);
    Task<BODossierDto> GetByIdAsync(int id);
    Task<BODossierDto> CreateAsync(CreateBODossierDto dto, string agentId);
    Task<BODossierDto> UpdateAsync(int id, CreateBODossierDto dto);
    Task CloturerAsync(int id, string agentId);
}

public interface IBOContactService
{
    Task<PagedResult<BOContactDto>> GetPagedAsync(string? searchTerm, string? typeContact, int page, int pageSize);
    Task<List<BOContactDto>> SearchAsync(string term);
    Task<BOContactDto> GetByIdAsync(int id);
    Task<BOContactDto> CreateAsync(CreateBOContactDto dto);
    Task<BOContactDto> UpdateAsync(int id, CreateBOContactDto dto);
    Task DeactivateAsync(int id);
}

public interface IBOArchiveService
{
    Task<PagedResult<BOArchiveDto>> GetPagedAsync(string? classification, string? searchTerm, int page, int pageSize);
    Task<BOArchiveDto> GetByIdAsync(int id);
    Task<List<BOArchiveDto>> GetExpirantAsync(int joursAvantEcheance = 30);
    Task MarquerDetruitAsync(int id, string agentId);
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS
// ════════════════════════════════════════════════════════════════

public interface ICitoyenService
{
    Task<PagedResult<CitoyenDto>> GetPagedAsync(CitoyenFilterDto filter);
    Task<CitoyenDto> GetByIdAsync(int id);
    Task<CitoyenDto> GetByCINAsync(string cin);
    Task<CitoyenDto> CreateAsync(CreateCitoyenDto dto);
    Task<CitoyenDto> UpdateAsync(int id, CreateCitoyenDto dto);
    Task<bool> CINExistsAsync(string cin, int? excludeId = null);
}

public interface IReclamationService
{
    Task<PagedResult<ReclamationDto>> GetPagedAsync(ReclamationFilterDto filter);
    Task<ReclamationDetailDto> GetByIdAsync(int id);
    Task<ReclamationPublicDto> GetByNumeroPublicAsync(string numero);  // Flutter — sans auth
    Task<List<ReclamationDto>> GetByCitoyenAsync(int citoyenId);
    Task<List<ReclamationDto>> GetEnRetardAsync();
    Task<ReclamationStatsDto> GetStatsAsync(int? serviceId = null);

    /// <summary>Déposer une réclamation (guichet ou Flutter — AllowAnonymous)</summary>
    Task<ReclamationDetailDto> DeposerAsync(CreateReclamationDto dto, string? agentId);
    Task AssignerAsync(int id, AssignerReclamationDto dto, string agentId, string agentNom);
    Task ChangerStatutAsync(int id, ChangerStatutReclamationDto dto, string agentId, string agentNom);
}

// ════════════════════════════════════════════════════════════════
//  PERMIS DE BÂTIR
// ════════════════════════════════════════════════════════════════

public interface IDemandeurService
{
    Task<PagedResult<DemandeurDto>> GetPagedAsync(string? searchTerm, string? type, int page, int pageSize);
    Task<DemandeurDto> GetByIdAsync(int id);
    Task<DemandeurDto> CreateAsync(CreateDemandeurDto dto);
    Task<DemandeurDto> UpdateAsync(int id, CreateDemandeurDto dto);
}

public interface IArchitecteService
{
    Task<PagedResult<ArchitecteDto>> GetPagedAsync(string? searchTerm, int page, int pageSize);
    Task<ArchitecteDto> GetByIdAsync(int id);
    Task<ArchitecteDto> CreateAsync(CreateArchitecteDto dto);
    Task<ArchitecteDto> UpdateAsync(int id, CreateArchitecteDto dto);
}

public interface IDemandePermisBatirService
{
    Task<PagedResult<DemandePermisDto>> GetPagedAsync(DemandePermisFilterDto filter);
    Task<DemandePermisDetailDto> GetByIdAsync(int id);
    Task<DemandePermisSuiviPublicDto> GetByNumeroPublicAsync(string numero);  // Flutter — AllowAnonymous
    Task<List<DemandePermisDto>> GetByDemandeurAsync(int demandeurId);
    Task<List<DemandePermisDto>> GetEnRetardAsync();
    Task<PermisStatsDto> GetStatsAsync(int? serviceId = null);

    /// <summary>Déposer une demande (guichet ou Flutter)</summary>
    Task<DemandePermisDetailDto> DeposerAsync(CreateDemandePermisDto dto, string? agentId);
    Task AssignerInstructeurAsync(int id, AssignerInstructeurDto dto, string agentId, string agentNom);
    Task AssignerCommissionAsync(int id, AssignerCommissionDto dto, string agentId, string agentNom);
    Task ChangerStatutAsync(int id, ChangerStatutPermisDto dto, string agentId, string agentNom);
    Task<PermisDelivreDto> DelivrerPermisAsync(int id, DelivrerPermisDto dto, string agentId);
    Task AjouterTaxeAsync(int id, AjouterTaxeDto dto);
    Task PayerTaxeAsync(int id, PayerTaxeDto dto, string agentId);
    Task AjouterInspectionAsync(int id, CreateInspectionDto dto, string agentId); //problem 
}

// ════════════════════════════════════════════════════════════════
//  NOTIFICATIONS
// ════════════════════════════════════════════════════════════════

public interface INotificationService
{
    Task<List<NotificationDto>> GetByAgentAsync(string agentId, bool seulementNonLues = false);
    Task<List<NotificationDto>> GetByCitoyenAsync(string citoyenId, bool seulementNonLues = false);
    Task<int> GetNombreNonLuesAsync(string destinataireId, bool estAgent = true);
    Task MarquerLueAsync(int notificationId, string userId);
    Task MarquerToutesLuesAsync(string destinataireId, bool estAgent = true);

    // ── Helpers appelés par les autres services ─────────────────
    Task NotifierAgentAsync(string agentId, TypeNotification type, string message, string? entiteId = null, string? entiteType = null);
    Task NotifierServiceAsync(int serviceId, TypeNotification type, string message, string? entiteId = null, string v = null);
    Task NotifierCitoyenAsync(string citoyenId, TypeNotification type, string message, string? entiteId = null, string? entiteType = null);
}