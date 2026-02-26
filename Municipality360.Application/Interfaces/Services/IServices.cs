using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.DTOs.Structure;

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
}