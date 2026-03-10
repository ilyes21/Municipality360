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

// ── Bureau d'Ordre ────────────────────────────────────────────────

public interface ICourrierEntrantService
{
    Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter);
    Task<CourrierEntrantDetailDto> GetByIdAsync(int id);
    Task<CourrierEntrantDetailDto> GetByNumeroAsync(string numero);
    Task<List<CourrierEntrantDto>> GetEnRetardAsync();
    Task<List<CourrierEntrantDto>> GetEnAttenteParServiceAsync(int serviceId);
    Task<BOStatsDto> GetStatsAsync(int? serviceId = null);
    Task<CourrierEntrantDetailDto> EnregistrerAsync(CreateCourrierEntrantDto dto, string agentId, string agentNom);
    Task<CourrierEntrantDetailDto> ModifierAsync(int id, UpdateCourrierEntrantDto dto, string agentId);
    Task ChangerStatutAsync(int id, ChangerStatutEntrantDto dto, string agentId, string agentNom);
    Task<BOCircuitTraitementDto> AcheminerAsync(int courrierEntrantId, AcheminerCourrierDto dto, string agentId);
    Task TraiterEtapeCircuitAsync(int circuitId, TraiterEtapeCircuitDto dto, string agentId, string agentNom);
    Task<List<BOCircuitTraitementDto>> GetCircuitAsync(int courrierEntrantId);
    Task<BOArchiveDto> ArchiverAsync(int id, ArchiversCourrierDto dto, string agentId);
}

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

public interface IBODossierService
{
    Task<PagedResult<BODossierDto>> GetPagedAsync(int page, int size);
    Task<BODossierDto> GetByIdAsync(int id);
    Task<BODossierDto> CreateAsync(CreateBODossierDto dto, string agentId);
    Task CloreAsync(int id, string agentId);
}

public interface IBOContactService
{
    Task<List<BOContactDto>> SearchAsync(string? term);
    Task<BOContactDto> GetByIdAsync(int id);
    Task<BOContactDto> CreateAsync(CreateBOContactDto dto);
    Task<BOContactDto> UpdateAsync(int id, CreateBOContactDto dto);
    Task DeleteAsync(int id);
}

public interface IBOArchiveService
{
    Task<BOArchiveDto> GetByCourrierEntrantAsync(int courrierEntrantId);
    Task<BOArchiveDto> GetByCourrierSortantAsync(int courrierSortantId);
}

// ── Réclamations ──────────────────────────────────────────────────

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
    Task<ReclamationPublicDto> GetByNumeroPublicAsync(string numero);
    Task<List<ReclamationDto>> GetByCitoyenAsync(int citoyenId);
    Task<List<ReclamationDto>> GetEnRetardAsync();
    Task<ReclamationStatsDto> GetStatsAsync(int? serviceId = null);
    Task<ReclamationDetailDto> DeposerAsync(CreateReclamationDto dto, string? agentId);
    Task AssignerAsync(int id, AssignerReclamationDto dto, string agentId, string agentNom);
    Task ChangerStatutAsync(int id, ChangerStatutReclamationDto dto, string agentId, string agentNom);
}

// ── Permis de Bâtir ──────────────────────────────────────────────

public interface IDemandeurService
{
    Task<List<DemandeurDto>> SearchAsync(string? term);
    Task<DemandeurDto> GetByIdAsync(int id);
    Task<DemandeurDto> CreateAsync(CreateDemandeurDto dto);
    Task<DemandeurDto> UpdateAsync(int id, CreateDemandeurDto dto);
}

public interface IArchitecteService
{
    Task<List<ArchitecteDto>> GetAllAsync();
    Task<ArchitecteDto> GetByIdAsync(int id);
    Task<ArchitecteDto> CreateAsync(CreateArchitecteDto dto);
    Task<ArchitecteDto> UpdateAsync(int id, CreateArchitecteDto dto);
}

public interface IDemandePermisBatirService
{
    Task<PagedResult<DemandePermisDto>> GetPagedAsync(DemandePermisFilterDto filter);
    Task<DemandePermisDetailDto> GetByIdAsync(int id);
    Task<DemandePermisSuiviPublicDto> GetByNumeroPublicAsync(string numero);
    Task<List<DemandePermisDto>> GetByDemandeurAsync(int demandeurId);
    Task<List<DemandePermisDto>> GetEnRetardAsync();
    Task<PermisStatsDto> GetStatsAsync(int? serviceId = null);
    Task<DemandePermisDetailDto> DeposerAsync(CreateDemandePermisDto dto, string? agentId);
    Task AssignerInstructeurAsync(int id, AssignerInstructeurDto dto, string agentId, string agentNom);
    Task AssignerCommissionAsync(int id, AssignerCommissionDto dto, string agentId, string agentNom);
    Task ChangerStatutAsync(int id, ChangerStatutPermisDto dto, string agentId, string agentNom);
    Task<PermisDelivreDto> DelivrerPermisAsync(int id, DelivrerPermisDto dto, string agentId);
    Task AjouterTaxeAsync(int id, AjouterTaxeDto dto);
    Task PayerTaxeAsync(int id, PayerTaxeDto dto, string agentId);
    Task AjouterInspectionAsync(int id, CreateInspectionDto dto, string agentId);
}

// ── Notifications ─────────────────────────────────────────────────

public interface INotificationService
{
    Task<List<NotificationDto>> GetByAgentAsync(string agentId, bool seulementNonLues = false);
    Task<List<NotificationDto>> GetByCitoyenAsync(string citoyenId, bool seulementNonLues = false);
    Task<int> GetNombreNonLuesAsync(string destinataireId, bool estAgent = true);
    Task MarquerLueAsync(int notificationId, string userId);
    Task MarquerToutesLuesAsync(string destinataireId, bool estAgent = true);
    Task NotifierAgentAsync(string agentId, TypeNotification type, string message, string? entiteId = null, string? entiteType = null);
    Task NotifierServiceAsync(int serviceId, TypeNotification type, string message, string? entiteId = null, string? entiteType = null);
    Task NotifierCitoyenAsync(string citoyenId, TypeNotification type, string message, string? entiteId = null, string? entiteType = null);
}
