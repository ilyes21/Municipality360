using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.DTOs.Structure;

namespace Municipality360.Web.Services;

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
        try { return await _api.PostAsync<RegisterDto, Result<AuthResponseDto>>("api/auth/register", dto); }
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
        if (!string.IsNullOrEmpty(filter.SearchTerm)) query += $"&searchTerm={Uri.EscapeDataString(filter.SearchTerm)}";
        return await _api.GetAsync<Result<PagedResult<EmployeDto>>>(query);
    }

    public async Task<Result<EmployeDto>?> GetByIdAsync(int id) =>
        await _api.GetAsync<Result<EmployeDto>>($"api/employes/{id}");

    public async Task<Result<EmployeDto>?> CreateAsync(CreateEmployeDto dto) =>
        await _api.PostAsync<CreateEmployeDto, Result<EmployeDto>>("api/employes", dto);

    public async Task<Result<EmployeDto>?> UpdateAsync(int id, UpdateEmployeDto dto) =>
        await _api.PutAsync<UpdateEmployeDto, Result<EmployeDto>>($"api/employes/{id}", dto);

    public async Task<bool> DeleteAsync(int id) => await _api.DeleteAsync($"api/employes/{id}");
}
