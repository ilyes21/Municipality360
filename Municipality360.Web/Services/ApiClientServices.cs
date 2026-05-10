using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.DTOs.Structure;

namespace Municipality360.Web.Services;

// ── DTO محلي للربط (يطابق LinkUserDto في الـ Application) ─────────
file record LinkUserDto(string UserId);

// ======================== AUTH ========================
public interface IAuthApiService
{
    Task<Result<AuthResponseDto>?> LoginAsync(LoginDto dto);
    Task<Result<AuthResponseDto>?> RegisterAsync(RegisterDto dto);
    Task LogoutAsync();
}

public class AuthApiService : IAuthApiService
{
    private readonly ApiService _api;
    private readonly CustomAuthStateProvider _authProvider;

    public AuthApiService(ApiService api, CustomAuthStateProvider authProvider)
    {
        _api = api;
        _authProvider = authProvider;
    }

    public async Task<Result<AuthResponseDto>?> LoginAsync(LoginDto dto)
    {
        try
        {
            var result = await _api.PostAsync<LoginDto, Result<AuthResponseDto>>("api/auth/login", dto);
            if (result?.Succeeded == true && result.Data != null)
                await _authProvider.NotifyUserLoggedIn(result.Data.Token);
            return result;
        }
        catch { return null; }
    }

    public async Task<Result<AuthResponseDto>?> RegisterAsync(RegisterDto dto)
    {
        try { return await _api.PostAsync<RegisterDto, Result<AuthResponseDto>>("api/auth/users", dto); }
        catch { return null; }
    }

    public async Task LogoutAsync() => await _authProvider.NotifyUserLoggedOut();
}

// ======================== DEPARTEMENTS ========================
public interface IDepartementApiService
{
    Task<Result<IEnumerable<DepartementDto>?>?> GetAllAsync();
    Task<Result<DepartementDto>?> GetByIdAsync(int id);
    Task<Result<DepartementDto>?> CreateAsync(CreateDepartementDto dto);
    Task<Result<DepartementDto>?> UpdateAsync(int id, UpdateDepartementDto dto);
    Task<bool> DeleteAsync(int id);
}

public class DepartementApiService : IDepartementApiService
{
    private readonly ApiService _api;
    public DepartementApiService(ApiService api) => _api = api;

    public async Task<Result<IEnumerable<DepartementDto>?>?> GetAllAsync() =>
        await _api.GetAsync<Result<IEnumerable<DepartementDto>>>("api/departements");

    public async Task<Result<DepartementDto>?> GetByIdAsync(int id) =>
        await _api.GetAsync<Result<DepartementDto>>($"api/departements/{id}");

    public async Task<Result<DepartementDto>?> CreateAsync(CreateDepartementDto dto) =>
        await _api.PostAsync<CreateDepartementDto, Result<DepartementDto>>("api/departements", dto);

    public async Task<Result<DepartementDto>?> UpdateAsync(int id, UpdateDepartementDto dto) =>
        await _api.PutAsync<UpdateDepartementDto, Result<DepartementDto>>($"api/departements/{id}", dto);

    public async Task<bool> DeleteAsync(int id) => await _api.DeleteAsync($"api/departements/{id}");
}

// ======================== SERVICES ========================
public interface IServiceApiService
{
    Task<Result<IEnumerable<ServiceDto>?>?> GetAllAsync();
    Task<Result<IEnumerable<ServiceDto>?>?> GetByDepartementAsync(int departementId);
    Task<Result<ServiceDto>?> CreateAsync(CreateServiceDto dto);
    Task<Result<ServiceDto>?> UpdateAsync(int id, UpdateServiceDto dto);
    Task<bool> DeleteAsync(int id);
}

public class ServiceApiService : IServiceApiService
{
    private readonly ApiService _api;
    public ServiceApiService(ApiService api) => _api = api;

    public async Task<Result<IEnumerable<ServiceDto>?>?> GetAllAsync() =>
        await _api.GetAsync<Result<IEnumerable<ServiceDto>>>("api/services");

    public async Task<Result<IEnumerable<ServiceDto>?>?> GetByDepartementAsync(int departementId) =>
        await _api.GetAsync<Result<IEnumerable<ServiceDto>>>($"api/services/departement/{departementId}");

    public async Task<Result<ServiceDto>?> CreateAsync(CreateServiceDto dto) =>
        await _api.PostAsync<CreateServiceDto, Result<ServiceDto>>("api/services", dto);

    public async Task<Result<ServiceDto>?> UpdateAsync(int id, UpdateServiceDto dto) =>
        await _api.PutAsync<UpdateServiceDto, Result<ServiceDto>>($"api/services/{id}", dto);

    public async Task<bool> DeleteAsync(int id) => await _api.DeleteAsync($"api/services/{id}");
}

// ======================== POSTES ========================
public interface IPosteApiService
{
    Task<Result<IEnumerable<PosteDto>?>?> GetAllAsync();
    Task<Result<PosteDto>?> CreateAsync(CreatePosteDto dto);
    Task<Result<PosteDto>?> UpdateAsync(int id, UpdatePosteDto dto);
    Task<bool> DeleteAsync(int id);
}

public class PosteApiService : IPosteApiService
{
    private readonly ApiService _api;
    public PosteApiService(ApiService api) => _api = api;

    public async Task<Result<IEnumerable<PosteDto>?>?> GetAllAsync() =>
        await _api.GetAsync<Result<IEnumerable<PosteDto>>>("api/postes");

    public async Task<Result<PosteDto>?> CreateAsync(CreatePosteDto dto) =>
        await _api.PostAsync<CreatePosteDto, Result<PosteDto>>("api/postes", dto);

    public async Task<Result<PosteDto>?> UpdateAsync(int id, UpdatePosteDto dto) =>
        await _api.PutAsync<UpdatePosteDto, Result<PosteDto>>($"api/postes/{id}", dto);

    public async Task<bool> DeleteAsync(int id) => await _api.DeleteAsync($"api/postes/{id}");
}

// ======================== EMPLOYES ========================
public interface IEmployeApiService
{
    Task<Result<PagedResult<EmployeDto>>?> GetPagedAsync(EmployeFilterDto filter);
    Task<Result<EmployeDto>?> GetByIdAsync(int id);
    Task<Result<EmployeDto>?> CreateAsync(CreateEmployeDto dto);
    Task<Result<EmployeDto>?> UpdateAsync(int id, UpdateEmployeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<Result<EmployeDto>?> LinkUserAsync(int employeId, string userId);
    Task<Result<EmployeDto>?> UnlinkUserAsync(int employeId);
}

public class EmployeApiService : IEmployeApiService
{
    private readonly ApiService _api;
    public EmployeApiService(ApiService api) => _api = api;

    public async Task<Result<PagedResult<EmployeDto>>?> GetPagedAsync(EmployeFilterDto filter)
    {
        var query = $"api/employes?pageNumber={filter.PageNumber}&pageSize={filter.PageSize}";
        if (filter.ServiceId.HasValue) query += $"&serviceId={filter.ServiceId}";
        if (filter.DepartementId.HasValue) query += $"&departementId={filter.DepartementId}";
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query += $"&searchTerm={Uri.EscapeDataString(filter.SearchTerm)}";
        return await _api.GetAsync<Result<PagedResult<EmployeDto>>>(query);
    }

    public async Task<Result<EmployeDto>?> GetByIdAsync(int id) =>
        await _api.GetAsync<Result<EmployeDto>>($"api/employes/{id}");

    public async Task<Result<EmployeDto>?> CreateAsync(CreateEmployeDto dto) =>
        await _api.PostAsync<CreateEmployeDto, Result<EmployeDto>>("api/employes", dto);

    public async Task<Result<EmployeDto>?> UpdateAsync(int id, UpdateEmployeDto dto) =>
        await _api.PutAsync<UpdateEmployeDto, Result<EmployeDto>>($"api/employes/{id}", dto);

    public async Task<bool> DeleteAsync(int id) => await _api.DeleteAsync($"api/employes/{id}");

    public async Task<Result<EmployeDto>?> LinkUserAsync(int employeId, string userId) =>
        await _api.PatchAsync<LinkUserDto, Result<EmployeDto>>(
            $"api/employes/{employeId}/link-user",
            new LinkUserDto(userId));

    public async Task<Result<EmployeDto>?> UnlinkUserAsync(int employeId) =>
        await _api.DeleteAsync($"api/employes/{employeId}/link-user")
            ? await _api.GetAsync<Result<EmployeDto>>($"api/employes/{employeId}")
            : null;
}

// ======================== USERS – إدارة كاملة ========================

/// <summary>
/// واجهة كاملة تغطي جميع Endpoints في AuthController:
///   GET    api/auth/users              → GetAllDetailAsync
///   GET    api/auth/users/simple       → GetAllSimpleAsync
///   GET    api/auth/users/{id}         → GetByIdAsync
///   POST   api/auth/users              → RegisterAsync
///   DELETE api/auth/users/{id}         → DeleteAsync
///   PATCH  api/auth/users/toggle       → ToggleActiveAsync
///   POST   api/auth/users/reset-pwd    → ResetPasswordAsync
///   GET    api/auth/roles              → GetRolesAsync
///   POST   api/auth/roles/assign       → AssignRoleAsync
///   POST   api/auth/roles/remove       → RemoveRoleAsync
/// </summary>
public interface IUserApiService
{
    // ── قراءة ──────────────────────────────────────────────────
    Task<Result<IEnumerable<UserDetailDto>>?> GetAllDetailAsync();
    Task<Result<IEnumerable<UserDto>>?> GetAllAsync();
    Task<Result<UserDetailDto>?> GetByIdAsync(string userId);

    // ── إنشاء / حذف ────────────────────────────────────────────
    Task<Result<AuthResponseDto>?> RegisterAsync(RegisterDto dto);
    Task<bool> DeleteAsync(string userId);

    // ── تفعيل / إيقاف ──────────────────────────────────────────
    Task<bool> ToggleActiveAsync(string userId, bool activate);

    // ── كلمة المرور ────────────────────────────────────────────
    Task<bool> ResetPasswordAsync(string userId, string newPassword);

    // ── الملف الشخصي ──────────────────────────────────────────────
    Task<bool> UpdateProfileAsync(UpdateProfileDto dto);
    Task<bool> ChangePasswordAsync(ChangePasswordDto dto);

    // ── الأدوار ────────────────────────────────────────────────
    Task<Result<IEnumerable<string>>?> GetRolesAsync();
    Task<bool> AssignRoleAsync(string userId, string role);
    Task<bool> RemoveRoleAsync(string userId, string role);
}

public class UserApiService : IUserApiService
{
    private readonly ApiService _api;
    public UserApiService(ApiService api) => _api = api;

    // GET api/auth/users  → UserDetailDto (isActive, emailConfirmed, roles)
    public async Task<Result<IEnumerable<UserDetailDto>>?> GetAllDetailAsync() =>
        await _api.GetAsync<Result<IEnumerable<UserDetailDto>>>("api/auth/users");

    // GET api/auth/users/simple → UserDto (مبسّط)
    public async Task<Result<IEnumerable<UserDto>>?> GetAllAsync() =>
        await _api.GetAsync<Result<IEnumerable<UserDto>>>("api/auth/users/simple");

    // GET api/auth/users/{id}
    public async Task<Result<UserDetailDto>?> GetByIdAsync(string userId) =>
        await _api.GetAsync<Result<UserDetailDto>>($"api/auth/users/{userId}");

    // POST api/auth/users
    public async Task<Result<AuthResponseDto>?> RegisterAsync(RegisterDto dto) =>
        await _api.PostAsync<RegisterDto, Result<AuthResponseDto>>("api/auth/users", dto);

    // DELETE api/auth/users/{id}
    public async Task<bool> DeleteAsync(string userId) =>
        await _api.DeleteAsync($"api/auth/users/{userId}");

    // PATCH api/auth/users/toggle
    public async Task<bool> ToggleActiveAsync(string userId, bool activate)
    {
        var dto = new ToggleUserDto { UserId = userId, IsActive = activate };
        var result = await _api.PatchAsync<ToggleUserDto, Result>("api/auth/users/toggle", dto);
        return result?.Succeeded == true;
    }

    // POST api/auth/users/reset-password
    public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
    {
        var dto = new ResetPasswordDto { UserId = userId, NewPassword = newPassword };
        var result = await _api.PostAsync<ResetPasswordDto, Result>("api/auth/users/reset-password", dto);
        return result?.Succeeded == true;
    }

    // GET api/auth/roles
    public async Task<Result<IEnumerable<string>>?> GetRolesAsync() =>
        await _api.GetAsync<Result<IEnumerable<string>>>("api/auth/roles");

    // POST api/auth/roles/assign
    public async Task<bool> AssignRoleAsync(string userId, string role)
    {
        var dto = new AssignRoleDto { UserId = userId, Role = role };
        var result = await _api.PostAsync<AssignRoleDto, Result>("api/auth/roles/assign", dto);
        return result?.Succeeded == true;
    }

    // PUT api/auth/me/profile
    public async Task<bool> UpdateProfileAsync(UpdateProfileDto dto)
    {
        var result = await _api.PutAsync<UpdateProfileDto, Result>("api/auth/me/profile", dto);
        return result?.Succeeded == true;
    }

    // PUT api/auth/me/password
    public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var result = await _api.PutAsync<ChangePasswordDto, Result>("api/auth/me/password", dto);
        return result?.Succeeded == true;
    }

    // POST api/auth/roles/remove
    public async Task<bool> RemoveRoleAsync(string userId, string role)
    {
        var dto = new AssignRoleDto { UserId = userId, Role = role };
        var result = await _api.PostAsync<AssignRoleDto, Result>("api/auth/roles/remove", dto);
        return result?.Succeeded == true;
    }
}