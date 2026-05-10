using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.DTOs.Intelligence;
using Municipality360.Application.DTOs.Mobile;
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Interfaces.Services;

// ════════════════════════════════════════════════════
//  AUTH SERVICE
// ════════════════════════════════════════════════════
public interface IAuthService
{
    // ── Authentification ──────────────────────────────
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);

    // ── Gestion des utilisateurs ──────────────────────
    Task<Result<IEnumerable<UserDto>>> GetUsersAsync();
    Task<Result<IEnumerable<UserDetailDto>>> GetUsersDetailAsync();
    Task<Result<UserDetailDto>> GetUserByIdAsync(string userId);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> ToggleUserActiveAsync(ToggleUserDto dto);
    Task<Result> ResetPasswordAsync(ResetPasswordDto dto);

    // ── Gestion des rôles ─────────────────────────────
    Task<Result<IEnumerable<string>>> GetAllRolesAsync();
    Task<Result> AssignRoleAsync(AssignRoleDto dto);
    Task<Result> RemoveRoleAsync(AssignRoleDto dto);
    // ── Self-update (كل مستخدم — بياناته الشخصية فقط) ─
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto);

}

// ════════════════════════════════════════════════════
//  STRUCTURE SERVICES (inchangés)
// ════════════════════════════════════════════════════
public interface IDepartementService
{
    Task<Result<IEnumerable<DepartementDto>>> GetAllAsync();
    Task<Result<DepartementDto>> GetByIdAsync(int id);
    Task<Result<DepartementDto>> CreateAsync(CreateDepartementDto dto);
    Task<Result<DepartementDto>> UpdateAsync(int id, UpdateDepartementDto dto);
    Task<Result> DeleteAsync(int id);
}

public interface IServiceService
{
    Task<Result<IEnumerable<ServiceDto>>> GetAllAsync();
    Task<Result<IEnumerable<ServiceDto>>> GetByDepartementAsync(int departementId);
    Task<Result<ServiceDto>> GetByIdAsync(int id);
    Task<Result<ServiceDto>> CreateAsync(CreateServiceDto dto);
    Task<Result<ServiceDto>> UpdateAsync(int id, UpdateServiceDto dto);
    Task<Result> DeleteAsync(int id);
}

public interface IPosteService
{
    Task<Result<IEnumerable<PosteDto>>> GetAllAsync();
    Task<Result<PosteDto>> GetByIdAsync(int id);
    Task<Result<PosteDto>> CreateAsync(CreatePosteDto dto);
    Task<Result<PosteDto>> UpdateAsync(int id, UpdatePosteDto dto);
    Task<Result> DeleteAsync(int id);
}

public interface IEmployeService
{
    Task<Result<PagedResult<EmployeDto>>> GetPagedAsync(EmployeFilterDto filter);
    Task<Result<EmployeDto>> GetByIdAsync(int id);
    Task<Result<EmployeDto>> CreateAsync(CreateEmployeDto dto);
    Task<Result<EmployeDto>> UpdateAsync(int id, UpdateEmployeDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<EmployeDto>> LinkUserAsync(int employeId, string userId);
    Task<Result<EmployeDto>> UnlinkUserAsync(int employeId);
}

// ── Bureau d'Ordre ────────────────────────────────────────────────

public interface ICourrierEntrantService
{
    // ── Lectures ──────────────────────────────────────────────────
    Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter);
    Task<CourrierEntrantDetailDto> GetByIdAsync(int id);
    Task<CourrierEntrantDetailDto> GetByNumeroAsync(string numero);
    Task<List<CourrierEntrantDto>> GetEnRetardAsync();
    Task<List<CourrierEntrantDto>> GetEnAttenteParServiceAsync(int serviceId);
    Task<BOStatsDto> GetStatsAsync(int? serviceId = null);

    // ── Écriture principale ───────────────────────────────────────
    Task<CourrierEntrantDetailDto> EnregistrerAsync(CreateCourrierEntrantDto dto, string agentId, string agentNom);
    Task<CourrierEntrantDetailDto> ModifierAsync(int id, UpdateCourrierEntrantDto dto, string agentId);
    Task ChangerStatutAsync(int id, ChangerStatutEntrantDto dto, string agentId, string agentNom);

    // ── Affectation directe ───────────────────────────────────────
    Task AffecterAsync(int id, AffecterCourrierEntrantDto dto, string agentId, string agentNom);

    // ── Circuit ───────────────────────────────────────────────────
    Task<BOCircuitTraitementDto> AcheminerAsync(int courrierEntrantId, AcheminerCourrierDto dto, string agentId);
    Task TraiterEtapeCircuitAsync(int circuitId, TraiterEtapeCircuitDto dto, string agentId, string agentNom);
    Task RetournerEtapeCircuitAsync(int circuitId, RetournerEtapeCircuitDto dto, string agentId, string agentNom);
    Task<List<BOCircuitTraitementDto>> GetCircuitAsync(int courrierEntrantId);

    // ── Pièces jointes ────────────────────────────────────────────
    Task<BOPieceJointeDto> AjouterPieceJointeAsync(int courrierEntrantId, AjouterPieceJointeDto dto);
    Task<PieceJointeDetailDto> GetPieceJointeAsync(int courrierEntrantId, int pjId);
    Task SupprimerPieceJointeAsync(int courrierEntrantId, int pjId, string agentId);

    // ── Archivage & suppression ───────────────────────────────────
    Task<BOArchiveDto> ArchiverAsync(int id, ArchiversCourrierDto dto, string agentId);
    Task SupprimerAsync(int id, string agentId);
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
public interface ITypeReclamationService
{
    Task<List<TypeReclamationDto>> GetAllActiveAsync();
    Task<TypeReclamationDto> GetByIdAsync(int id);
    Task<TypeReclamationDto> CreateAsync(CreateTypeReclamationDto dto);
    Task<TypeReclamationDto> UpdateAsync(int id, CreateTypeReclamationDto dto);
    Task DeleteAsync(int id);
}

public interface ICategorieReclamationService
{
    Task<List<CategorieReclamationDto>> GetHierarchieAsync();
    Task<List<CategorieReclamationDto>> GetFlatAsync();
    Task<CategorieReclamationDto> GetByIdAsync(int id);
    Task<CategorieReclamationDto> CreateAsync(CreateCategorieReclamationDto dto);
    Task DeleteAsync(int id);
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

// ── Citoyen Service ─────────────────────────────────────────────────
public interface ICitoyenAuthService
{
    Task<Result<CitoyenAuthResponseDto>> RegisterAsync(CitoyenRegisterMobileDto dto);
    Task<Result<CitoyenAuthResponseDto>> LoginAsync(CitoyenLoginMobileDto dto);
    Task<Result<CitoyenProfileMobileDto>> GetProfileAsync(int citoyenId);
    Task<Result> UpdateFcmTokenAsync(int citoyenId, string fcmToken);
    Task<Result<CitoyenDashboardStatsDto>> GetDashboardStatsAsync(int citoyenId);
}
// ════════════════════════════════════════════════════
//  التنفيذ في Infrastructure باستخدام ML.NET + OpenAI/Gemini.
// ════════════════════════════════════════════════════
public interface IComplaintIntelligenceService
{
    // ── التصنيف ───────────────────────────────────────────────────

    /// <summary>
    /// يُصنّف شكوى واحدة باستخدام نموذج ML.NET المُحمَّل مسبقاً.
    /// لا يُعدّل قاعدة البيانات — مجرد تحليل.
    /// </summary>
    Task<ClassificationResultDto> ClassifyAsync(ClassificationRequestDto request);

    /// <summary>
    /// يُعيد تدريب النموذج من بيانات الشكاوى المُؤرشفة.
    /// يُستدعى من CronJob ليلي أو من لوحة التحكم.
    /// </summary>
    Task RetrainModelAsync(CancellationToken cancellationToken = default);

    // ── الرد الآلي ───────────────────────────────────────────────

    /// <summary>
    /// يولّد رداً رسمياً بالعربية الفصحى بأسلوب بلدية "العين".
    /// يُرسل طلباً إلى OpenAI / Gemini ويعيد النص المُولَّد.
    /// </summary>
    Task<AutoResponseResultDto> GenerateAutoResponseAsync(AutoResponseRequestDto request);

    // ── الدُّفعة ─────────────────────────────────────────────────

    /// <summary>
    /// يُصنّف ويولّد رداً في خطوة واحدة ذرية.
    /// الاستخدام المُفضَّل من ReclamationService.
    /// </summary>
    Task<(ClassificationResultDto Classification, AutoResponseResultDto Response)>
        ProcessNewComplaintAsync(ClassificationRequestDto classRequest, AutoResponseRequestDto responseRequest);
}
