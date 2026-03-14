// ═══════════════════════════════════════════════════════════════════
//  IStructureRepositories.cs  ✅ FIXED
//  Application/Interfaces/Repositories/IStructureRepositories.cs
//
//  الإصلاح: إضافة GetEmployesAsync لـ IServiceRepository
//           يستخدمها NotificationService.NotifierServiceAsync
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Interfaces.Repositories;

public interface IDepartementRepository : IGenericRepository<Departement>
{
    Task<IEnumerable<Departement>> GetAllWithServicesAsync();
    Task<Departement?> GetByIdWithServicesAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IServiceRepository : IGenericRepository<Service>
{
    Task<IEnumerable<Service>> GetByDepartementAsync(int departementId);
    Task<Service?> GetByIdWithDetailsAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    // ✅ FIXED: مطلوب بواسطة NotificationService.NotifierServiceAsync
    Task<List<Employe>> GetEmployesAsync(int serviceId);
}

public interface IPosteRepository : IGenericRepository<Poste>
{
    Task<IEnumerable<Poste>> GetActivePostesAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IEmployeRepository : IGenericRepository<Employe>
{
    Task<PagedResult<Employe>> GetPagedAsync(EmployeFilterDto filter);
    Task<Employe?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Employe>> GetByServiceAsync(int serviceId);
    Task<bool> CinExistsAsync(string cin, int? excludeId = null);
    Task<bool> IdentifiantExistsAsync(string identifiant, int? excludeId = null);
}
public interface ISequenceRepository
{
    Task<string> GenererNumeroAsync(string prefixe);
}

// ── Bureau d'Ordre ────────────────────────────────────────────────

public interface IBOContactRepository : IGenericRepository<BOContact>
{
    Task<List<BOContact>> SearchAsync(string? term, bool activeOnly = true);
}

public interface IBOCategorieCourrierRepository : IGenericRepository<BOCategorieCourrier>
{
    Task<List<BOCategorieCourrier>> GetAllActiveAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IBODossierRepository : IGenericRepository<BODossier>
{
    Task<PagedResult<BODossierDto>> GetPagedAsync(int page, int size);
    Task<BODossier?> GetByNumeroAsync(string numero);
}

public interface IBOCourrierEntrantRepository : IGenericRepository<BOCourrierEntrant>
{
    Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter);
    Task<BOCourrierEntrant?> GetByIdWithDetailsAsync(int id);
    Task<BOCourrierEntrant?> GetByNumeroAsync(string numero);
    Task<List<CourrierEntrantDto>> GetEnRetardAsync();
    Task<List<CourrierEntrantDto>> GetNonTraitesParServiceAsync(int serviceId);
    Task<BOStatsDto> GetStatsAsync(int? serviceId = null);

    // ── Pièces jointes ────────────────────────────────────────────
    Task AddPieceJointeAsync(BOPieceJointeEntrant pj);
    Task<BOPieceJointeEntrant?> GetPieceJointeAsync(int courrierEntrantId, int pjId);
    Task SupprimerPieceJointeAsync(BOPieceJointeEntrant pj);
}

public interface IBOCircuitTraitementRepository : IGenericRepository<BOCircuitTraitement>
{
    Task<List<BOCircuitTraitementDto>> GetByCourrierAsync(int courrierEntrantId);
    Task<BOCircuitTraitement?> GetEtapeActiveAsync(int courrierEntrantId);
    Task<short> GetProchaineNumeroEtapeAsync(int courrierEntrantId);
}

public interface IBOCourrierSortantRepository : IGenericRepository<BOCourrierSortant>
{
    Task<PagedResult<CourrierSortantDto>> GetPagedAsync(CourrierSortantFilterDto filter);
    Task<BOCourrierSortant?> GetByIdWithDetailsAsync(int id);
    Task<BOCourrierSortant?> GetByNumeroAsync(string numero);
    Task<List<CourrierSortantDto>> GetBrouillonsAsync(string redacteurId);
}

public interface IBOArchiveRepository : IGenericRepository<BOArchive>
{
    Task<BOArchiveDto?> GetByCourrierEntrantIdAsync(int id);
    Task<BOArchiveDto?> GetByCourrierSortantIdAsync(int id);
}

// ── Réclamations ──────────────────────────────────────────────────

public interface ICitoyenRepository : IGenericRepository<Citoyen>
{
    Task<PagedResult<CitoyenDto>> GetPagedAsync(CitoyenFilterDto filter);
    Task<Citoyen?> GetByCINAsync(string cin);
    Task<bool> CINExistsAsync(string cin, int? excludeId = null);
}

public interface ITypeReclamationRepository : IGenericRepository<TypeReclamation>
{
    Task<List<TypeReclamation>> GetAllActiveAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface ICategorieReclamationRepository : IGenericRepository<CategorieReclamation>
{
    Task<List<CategorieReclamation>> GetHierarchieAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IReclamationRepository : IGenericRepository<Reclamation>
{
    Task<PagedResult<ReclamationDto>> GetPagedAsync(ReclamationFilterDto filter);
    Task<Reclamation?> GetByIdWithDetailsAsync(int id);
    Task<Reclamation?> GetByNumeroAsync(string numero);
    Task<List<ReclamationDto>> GetByCitoyenAsync(int citoyenId);
    Task<List<ReclamationDto>> GetEnRetardAsync();
    Task<ReclamationStatsDto> GetStatsAsync(int? serviceId = null);
}

// ── Permis de Bâtir ──────────────────────────────────────────────

public interface IDemandeurRepository : IGenericRepository<Demandeur>
{
    Task<List<Demandeur>> SearchAsync(string? term);
}

public interface IArchitecteRepository : IGenericRepository<Architecte>
{
    Task<Architecte?> GetByNumeroOrdreAsync(string numero);
    Task<bool> NumeroOrdreExistsAsync(string numero, int? excludeId = null);
}

public interface ICommissionExamenRepository : IGenericRepository<CommissionExamen>
{
    Task<List<CommissionExamen>> GetActiveAsync();
}

public interface IDemandePermisBatirRepository : IGenericRepository<DemandePermisBatir>
{
    Task<PagedResult<DemandePermisDto>> GetPagedAsync(DemandePermisFilterDto filter);
    Task<DemandePermisBatir?> GetByIdWithDetailsAsync(int id);
    Task<DemandePermisBatir?> GetByNumeroAsync(string numero);
    Task<List<DemandePermisDto>> GetByDemandeurAsync(int demandeurId);
    Task<List<DemandePermisDto>> GetEnRetardAsync();
    Task<PermisStatsDto> GetStatsAsync(int? serviceId = null);
}

// ── Notifications ─────────────────────────────────────────────────

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<NotificationDto>> GetByAgentAsync(string agentId, bool seulementNonLues = false);
    Task<List<NotificationDto>> GetByCitoyenAsync(string citoyenId, bool seulementNonLues = false);
    Task<int> GetNombreNonLuesAsync(string destinataireId, bool estAgent = true);
    Task MarquerLueAsync(int notificationId);
    Task MarquerToutesLuesAsync(string destinataireId, bool estAgent = true);
}
