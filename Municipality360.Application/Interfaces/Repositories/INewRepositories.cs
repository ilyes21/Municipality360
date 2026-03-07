// ═══════════════════════════════════════════════════════════════════
//  INewRepositories.cs
//  Application/Interfaces/Repositories/INewRepositories.cs
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Domain.Entities;
using Municipality360.Domain.Entities.BureauOrdre;
using Municipality360.Domain.Entities.Reclamations;
using System.Runtime.InteropServices;

namespace Municipality360.Application.Interfaces.Repositories;

// ════════════════════════════════════════════════════════════════
//  SÉQUENCES
// ════════════════════════════════════════════════════════════════

public interface ISequenceRepository : IGenericRepository<Sequence>
{
    /// <summary>Génère le prochain numéro : ex. ENT-2025-00001</summary>
    Task<string> GenererNumeroAsync(string prefixe);
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE
// ════════════════════════════════════════════════════════════════

public interface IBOContactRepository : IGenericRepository<BOContact>
{
    Task<PagedResult<BOContactDto>> GetPagedAsync(string? searchTerm, string? typeContact, int page, int pageSize);
    Task<List<BOContactDto>> SearchAsync(string term, int limit = 10);
}

public interface IBOCategorieCourrierRepository : IGenericRepository<BOCategorieCourrier>
{
    Task<List<BOCategorieCourrierDto>> GetAllActiveAsync();
    Task<BOCategorieCourrier?> GetByCodeAsync(string code);
}

public interface IBODossierRepository : IGenericRepository<BODossier>
{
    Task<BODossier?> GetByIdWithDetailsAsync(int id);
    Task<PagedResult<BODossierDto>> GetPagedAsync(string? statut, int? serviceId, string? searchTerm, int page, int pageSize);
}

public interface IBOCourrierEntrantRepository : IGenericRepository<BOCourrierEntrant>
{
    Task<BOCourrierEntrant?> GetByIdWithDetailsAsync(int id);
    Task<BOCourrierEntrant?> GetByNumeroAsync(string numero);
    Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter);
    Task<List<CourrierEntrantDto>> GetEnRetardAsync();
    Task<List<CourrierEntrantDto>> GetNonTraitesParServiceAsync(int serviceId);
    Task<BOStatsDto> GetStatsAsync(int? serviceId = null);
}

public interface IBOCircuitTraitementRepository : IGenericRepository<BOCircuitTraitement>
{
    Task<List<BOCircuitTraitementDto>> GetByCourrierAsync(int courrierEntrantId);
    Task<BOCircuitTraitement?> GetEtapeActiveAsync(int courrierEntrantId);
    Task<short> GetProchaineNumeroEtapeAsync(int courrierEntrantId);
    Task<List<BOCircuitTraitementDto>> GetEnAttenteParServiceAsync(int serviceId);
}

public interface IBOCourrierSortantRepository : IGenericRepository<BOCourrierSortant>
{
    Task<BOCourrierSortant?> GetByIdWithDetailsAsync(int id);
    Task<BOCourrierSortant?> GetByNumeroAsync(string numero);
    Task<PagedResult<CourrierSortantDto>> GetPagedAsync(CourrierSortantFilterDto filter);
    Task<List<CourrierSortantDto>> GetBrouillonsAsync(string redacteurId);
}

public interface IBOArchiveRepository : IGenericRepository<BOArchive>
{
    Task<BOArchiveDto?> GetByCourrierEntrantIdAsync(int courrierEntrantId);
    Task<BOArchiveDto?> GetByCourrierSortantIdAsync(int courrierSortantId);
    Task<PagedResult<BOArchiveDto>> GetPagedAsync(string? classification, string? searchTerm, int page, int pageSize);
    Task<List<BOArchiveDto>> GetExpirantAsync(int joursAvantEcheance = 30);
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS
// ════════════════════════════════════════════════════════════════

public interface ICitoyenRepository : IGenericRepository<Citoyen>
{
    Task<Citoyen?> GetByCINAsync(string cin);
    Task<Citoyen?> GetByUserIdAsync(string userId);
    Task<PagedResult<CitoyenDto>> GetPagedAsync(CitoyenFilterDto filter);
    Task<bool> CINExistsAsync(string cin, int? excludeId = null);
}

public interface ITypeReclamationRepository : IGenericRepository<TypeReclamation>
{
    Task<List<TypeReclamationDto>> GetAllActiveAsync();
    Task<TypeReclamation?> GetByCodeAsync(string code);
}

public interface ICategorieReclamationRepository : IGenericRepository<CategorieReclamation>
{
    Task<List<CategorieReclamationDto>> GetAllActiveWithHierarchyAsync();
    Task<List<CategorieReclamationDto>> GetByNiveauAsync(int niveau);
}

public interface IReclamationRepository : IGenericRepository<Reclamation>
{
    Task<Reclamation?> GetByIdWithDetailsAsync(int id);
    Task<Reclamation?> GetByNumeroAsync(string numero);
    Task<PagedResult<ReclamationDto>> GetPagedAsync(ReclamationFilterDto filter);
    Task<List<ReclamationDto>> GetByCitoyenAsync(int citoyenId);
    Task<List<ReclamationDto>> GetEnRetardAsync();
    Task<ReclamationStatsDto> GetStatsAsync(int? serviceId = null);
}

// ════════════════════════════════════════════════════════════════
//  PERMIS DE BÂTIR
// ════════════════════════════════════════════════════════════════

public interface IDemandeurRepository : IGenericRepository<Demandeur>
{
    Task<PagedResult<DemandeurDto>> GetPagedAsync(string? searchTerm, string? type, int page, int pageSize);
    Task<Demandeur?> GetByCINAsync(string cin);
}

public interface IArchitecteRepository : IGenericRepository<Architecte>
{
    Task<PagedResult<ArchitecteDto>> GetPagedAsync(string? searchTerm, int page, int pageSize);
    Task<Architecte?> GetByNumeroOrdreAsync(string numeroOrdre);
    Task<bool> NumeroOrdreExistsAsync(string numeroOrdre, int? excludeId = null);
}

public interface ICommissionExamenRepository : IGenericRepository<CommissionExamen>
{
    Task<PagedResult<CommissionExamenDto>> GetPagedAsync(string? statut, int page, int pageSize);
    Task<CommissionExamen?> GetByIdWithDemandesAsync(int id);
}

public interface IDemandePermisBatirRepository : IGenericRepository<DemandePermisBatir>
{
    Task<DemandePermisBatir?> GetByIdWithDetailsAsync(int id);
    Task<DemandePermisBatir?> GetByNumeroAsync(string numero);
    Task<PagedResult<DemandePermisDto>> GetPagedAsync(DemandePermisFilterDto filter);
    Task<List<DemandePermisDto>> GetByDemandeurAsync(int demandeurId);
    Task<List<DemandePermisDto>> GetEnRetardAsync();
    Task<PermisStatsDto> GetStatsAsync(int? serviceId = null);
}

// ════════════════════════════════════════════════════════════════
//  NOTIFICATIONS
// ════════════════════════════════════════════════════════════════

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<NotificationDto>> GetByAgentAsync(string agentId, bool seulementNonLues = false);
    Task<List<NotificationDto>> GetByCitoyenAsync(string citoyenId, bool seulementNonLues = false);
    Task<int> GetNombreNonLuesAsync(string destinataireId, bool estAgent = true);
    Task MarquerLueAsync(int notificationId);
    Task MarquerToutesLuesAsync(string destinataireId, bool estAgent = true);
}