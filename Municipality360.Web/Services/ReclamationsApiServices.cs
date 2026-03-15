// ═══════════════════════════════════════════════════════════════════
//  ReclamationsApiServices.cs
//  Municipality360.Web/Services/ReclamationsApiServices.cs
//
//  Client API Blazor pour le module Réclamations :
//  IReclamationApiService · IReclamationApiService
//  ICitoyenApiService      · ITypeReclamationApiService
//  ICategorieReclamationApiService
//
//  Enregistrement dans Program.cs (Web) :
//    builder.Services.AddScoped<IReclamationApiService, ReclamationApiService>();
//    builder.Services.AddScoped<ICitoyenApiService, CitoyenApiService>();
//    builder.Services.AddScoped<ITypeReclamationApiService, TypeReclamationApiService>();
//    builder.Services.AddScoped<ICategorieReclamationApiService, CategorieReclamationApiService>();
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Reclamations;

namespace Municipality360.Web.Services;

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS
// ════════════════════════════════════════════════════════════════

public interface IReclamationApiService
{
    Task<PagedResult<ReclamationDto>?>     GetPagedAsync(ReclamationFilterDto filter);
    Task<ReclamationDetailDto?>            GetByIdAsync(int id);
    Task<ReclamationPublicDto?>            GetByNumeroPublicAsync(string numero);
    Task<List<ReclamationDto>?>            GetByCitoyenAsync(int citoyenId);
    Task<List<ReclamationDto>?>            GetEnRetardAsync();
    Task<ReclamationStatsDto?>             GetStatsAsync(int? serviceId = null);
    Task<ReclamationDetailDto?>            DeposerAsync(CreateReclamationDto dto);
    Task<bool>                             AssignerAsync(int id, AssignerReclamationDto dto);
    Task<bool>                             ChangerStatutAsync(int id, ChangerStatutReclamationDto dto);
}

public class ReclamationApiService : IReclamationApiService
{
    private readonly ApiService _api;
    public ReclamationApiService(ApiService api) => _api = api;

    public async Task<PagedResult<ReclamationDto>?> GetPagedAsync(ReclamationFilterDto f)
    {
        var q = $"api/reclamations?pageNumber={f.PageNumber}&pageSize={f.PageSize}";
        if (!string.IsNullOrEmpty(f.Statut))            q += $"&statut={f.Statut}";
        if (!string.IsNullOrEmpty(f.Priorite))          q += $"&priorite={f.Priorite}";
        if (f.TypeReclamationId.HasValue)                q += $"&typeReclamationId={f.TypeReclamationId}";
        if (f.CategorieId.HasValue)                      q += $"&categorieId={f.CategorieId}";
        if (f.ServiceConcerneId.HasValue)                q += $"&serviceConcerneId={f.ServiceConcerneId}";
        if (!string.IsNullOrEmpty(f.Canal))              q += $"&canal={f.Canal}";
        if (f.EnRetard.HasValue)                         q += $"&enRetard={f.EnRetard}";
        if (f.DateDebut.HasValue)                        q += $"&dateDebut={f.DateDebut.Value:yyyy-MM-dd}";
        if (f.DateFin.HasValue)                          q += $"&dateFin={f.DateFin.Value:yyyy-MM-dd}";
        if (!string.IsNullOrEmpty(f.SearchTerm))         q += $"&searchTerm={Uri.EscapeDataString(f.SearchTerm)}";
        return await _api.GetAsync<PagedResult<ReclamationDto>>(q);
    }

    public async Task<ReclamationDetailDto?> GetByIdAsync(int id) =>
        await _api.GetAsync<ReclamationDetailDto>($"api/reclamations/{id}");

    public async Task<ReclamationPublicDto?> GetByNumeroPublicAsync(string numero) =>
        await _api.GetAsync<ReclamationPublicDto>($"api/reclamations/suivi-public/{numero}");

    public async Task<List<ReclamationDto>?> GetByCitoyenAsync(int citoyenId) =>
        await _api.GetAsync<List<ReclamationDto>>($"api/reclamations/citoyen/{citoyenId}");

    public async Task<List<ReclamationDto>?> GetEnRetardAsync() =>
        await _api.GetAsync<List<ReclamationDto>>("api/reclamations/en-retard");

    public async Task<ReclamationStatsDto?> GetStatsAsync(int? serviceId = null)
    {
        var q = "api/reclamations/stats";
        if (serviceId.HasValue) q += $"?serviceId={serviceId}";
        return await _api.GetAsync<ReclamationStatsDto>(q);
    }

    public async Task<ReclamationDetailDto?> DeposerAsync(CreateReclamationDto dto) =>
        await _api.PostAsync<CreateReclamationDto, ReclamationDetailDto>("api/reclamations", dto);

    public async Task<bool> AssignerAsync(int id, AssignerReclamationDto dto)
    {
        var result = await _api.PatchAsync<AssignerReclamationDto, object>($"api/reclamations/{id}/assigner", dto);
        return result != null;
    }

    public async Task<bool> ChangerStatutAsync(int id, ChangerStatutReclamationDto dto)
    {
        var result = await _api.PatchAsync<ChangerStatutReclamationDto, object>($"api/reclamations/{id}/statut", dto);
        return result != null;
    }
}

// ════════════════════════════════════════════════════════════════
//  CITOYENS
// ════════════════════════════════════════════════════════════════

public interface ICitoyenApiService
{
    Task<PagedResult<CitoyenDto>?> GetPagedAsync(CitoyenFilterDto filter);
    Task<CitoyenDto?>              GetByIdAsync(int id);
    Task<CitoyenDto?>              GetByCINAsync(string cin);
    Task<bool>                     CINExistsAsync(string cin, int? excludeId = null);
    Task<CitoyenDto?>              CreateAsync(CreateCitoyenDto dto);
    Task<CitoyenDto?>              UpdateAsync(int id, CreateCitoyenDto dto);
    Task<List<ReclamationDto>?>    GetReclamationsAsync(int citoyenId);
}

public class CitoyenApiService : ICitoyenApiService
{
    private readonly ApiService _api;
    public CitoyenApiService(ApiService api) => _api = api;

    public async Task<PagedResult<CitoyenDto>?> GetPagedAsync(CitoyenFilterDto f)
    {
        var q = $"api/citoyens?pageNumber={f.PageNumber}&pageSize={f.PageSize}";
        if (!string.IsNullOrEmpty(f.SearchTerm)) q += $"&searchTerm={Uri.EscapeDataString(f.SearchTerm)}";
        if (f.IsActive.HasValue)                 q += $"&isActive={f.IsActive}";
        if (!string.IsNullOrEmpty(f.Ville))      q += $"&ville={Uri.EscapeDataString(f.Ville)}";
        return await _api.GetAsync<PagedResult<CitoyenDto>>(q);
    }

    public async Task<CitoyenDto?> GetByIdAsync(int id) =>
        await _api.GetAsync<CitoyenDto>($"api/citoyens/{id}");

    public async Task<CitoyenDto?> GetByCINAsync(string cin) =>
        await _api.GetAsync<CitoyenDto>($"api/citoyens/cin/{cin}");

    public async Task<bool> CINExistsAsync(string cin, int? excludeId = null)
    {
        var q = $"api/citoyens/cin-exists/{cin}";
        if (excludeId.HasValue) q += $"?excludeId={excludeId}";
        return await _api.GetAsync<bool>(q);
    }

    public async Task<CitoyenDto?> CreateAsync(CreateCitoyenDto dto) =>
        await _api.PostAsync<CreateCitoyenDto, CitoyenDto>("api/citoyens", dto);

    public async Task<CitoyenDto?> UpdateAsync(int id, CreateCitoyenDto dto) =>
        await _api.PutAsync<CreateCitoyenDto, CitoyenDto>($"api/citoyens/{id}", dto);

    public async Task<List<ReclamationDto>?> GetReclamationsAsync(int citoyenId) =>
        await _api.GetAsync<List<ReclamationDto>>($"api/reclamations/citoyen/{citoyenId}");
}

// ════════════════════════════════════════════════════════════════
//  TYPES & CATÉGORIES RÉCLAMATION
// ════════════════════════════════════════════════════════════════

public interface ITypeReclamationApiService
{
    Task<List<TypeReclamationDto>?> GetAllActiveAsync();
    Task<TypeReclamationDto?>       CreateAsync(CreateTypeReclamationDto dto);
    Task<TypeReclamationDto?>       UpdateAsync(int id, CreateTypeReclamationDto dto);
    Task<bool>                      DeleteAsync(int id);
}

public class TypeReclamationApiService : ITypeReclamationApiService
{
    private readonly ApiService _api;
    public TypeReclamationApiService(ApiService api) => _api = api;

    public async Task<List<TypeReclamationDto>?> GetAllActiveAsync() =>
        await _api.GetAsync<List<TypeReclamationDto>>("api/types-reclamation");

    public async Task<TypeReclamationDto?> CreateAsync(CreateTypeReclamationDto dto) =>
        await _api.PostAsync<CreateTypeReclamationDto, TypeReclamationDto>("api/types-reclamation", dto);

    public async Task<TypeReclamationDto?> UpdateAsync(int id, CreateTypeReclamationDto dto) =>
        await _api.PutAsync<CreateTypeReclamationDto, TypeReclamationDto>($"api/types-reclamation/{id}", dto);

    public async Task<bool> DeleteAsync(int id) =>
        await _api.DeleteAsync($"api/types-reclamation/{id}");
}

public interface ICategorieReclamationApiService
{
    Task<List<CategorieReclamationDto>?> GetHierarchieAsync();
    Task<CategorieReclamationDto?>       CreateAsync(CreateCategorieReclamationDto dto);
    Task<bool>                           DeleteAsync(int id);
}

public class CategorieReclamationApiService : ICategorieReclamationApiService
{
    private readonly ApiService _api;
    public CategorieReclamationApiService(ApiService api) => _api = api;

    public async Task<List<CategorieReclamationDto>?> GetHierarchieAsync() =>
        await _api.GetAsync<List<CategorieReclamationDto>>("api/categories-reclamation");

    public async Task<CategorieReclamationDto?> CreateAsync(CreateCategorieReclamationDto dto) =>
        await _api.PostAsync<CreateCategorieReclamationDto, CategorieReclamationDto>("api/categories-reclamation", dto);

    public async Task<bool> DeleteAsync(int id) =>
        await _api.DeleteAsync($"api/categories-reclamation/{id}");
}
