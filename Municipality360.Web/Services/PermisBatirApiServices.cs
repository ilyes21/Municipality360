// ═══════════════════════════════════════════════════════════════════
//  PermisBatirApiService.cs
//  Municipality360.Web/Services/PermisBatirApiService.cs
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.PermisBatir;

namespace Municipality360.Web.Services;

// ── Interface principale ──────────────────────────────────────────
public interface IPermisBatirApiService
{
    Task<PagedResult<DemandePermisDto>?> GetPagedAsync(DemandePermisFilterDto filter);
    Task<DemandePermisDetailDto?> GetByIdAsync(int id);
    Task<DemandePermisSuiviPublicDto?> GetByNumeroPublicAsync(string numero);
    Task<List<DemandePermisDto>?> GetEnRetardAsync();
    Task<PermisStatsDto?> GetStatsAsync(int? serviceId = null);
    Task<DemandePermisDetailDto?> DeposerAsync(CreateDemandePermisDto dto);
    Task<bool> AssignerInstructeurAsync(int id, AssignerInstructeurDto dto);
    Task<bool> AssignerCommissionAsync(int id, AssignerCommissionDto dto);
    Task<bool> ChangerStatutAsync(int id, ChangerStatutPermisDto dto);
    Task<PermisDelivreDto?> DelivrerPermisAsync(int id, DelivrerPermisDto dto);
    Task<bool> AjouterTaxeAsync(int id, AjouterTaxeDto dto);
    Task<bool> PayerTaxeAsync(int id, PayerTaxeDto dto);
    Task<bool> AjouterInspectionAsync(int id, CreateInspectionDto dto);
}

public class PermisBatirApiService : IPermisBatirApiService
{
    private readonly ApiService _api;
    public PermisBatirApiService(ApiService api) => _api = api;

    public async Task<PagedResult<DemandePermisDto>?> GetPagedAsync(DemandePermisFilterDto f)
    {
        var q = $"api/permis-batir?pageNumber={f.PageNumber}&pageSize={f.PageSize}";
        if (!string.IsNullOrEmpty(f.Statut)) q += $"&statut={f.Statut}";
        if (f.TypeDemandeId.HasValue) q += $"&typeDemandeId={f.TypeDemandeId}";
        if (!string.IsNullOrEmpty(f.TypeConstruction)) q += $"&typeConstruction={f.TypeConstruction}";
        if (f.ServiceInstructeurId.HasValue) q += $"&serviceInstructeurId={f.ServiceInstructeurId}";
        if (f.TaxesPayees.HasValue) q += $"&taxesPayees={f.TaxesPayees}";
        if (f.DateDebut.HasValue) q += $"&dateDebut={f.DateDebut.Value:yyyy-MM-dd}";
        if (f.DateFin.HasValue) q += $"&dateFin={f.DateFin.Value:yyyy-MM-dd}";
        if (!string.IsNullOrEmpty(f.SearchTerm)) q += $"&searchTerm={Uri.EscapeDataString(f.SearchTerm)}";
        return await _api.GetAsync<PagedResult<DemandePermisDto>>(q);
    }

    public Task<DemandePermisDetailDto?> GetByIdAsync(int id) =>
        _api.GetAsync<DemandePermisDetailDto>($"api/permis-batir/{id}");

    public Task<DemandePermisSuiviPublicDto?> GetByNumeroPublicAsync(string numero) =>
        _api.GetAsync<DemandePermisSuiviPublicDto>($"api/permis-batir/suivi-public/{numero}");

    public Task<List<DemandePermisDto>?> GetEnRetardAsync() =>
        _api.GetAsync<List<DemandePermisDto>>("api/permis-batir/en-retard");

    public async Task<PermisStatsDto?> GetStatsAsync(int? serviceId = null)
    {
        var q = "api/permis-batir/stats";
        if (serviceId.HasValue) q += $"?serviceId={serviceId}";
        return await _api.GetAsync<PermisStatsDto>(q);
    }

    public Task<DemandePermisDetailDto?> DeposerAsync(CreateDemandePermisDto dto) =>
        _api.PostAsync<CreateDemandePermisDto, DemandePermisDetailDto>("api/permis-batir", dto);

    public async Task<bool> AssignerInstructeurAsync(int id, AssignerInstructeurDto dto)
    {
        var r = await _api.PatchAsync<AssignerInstructeurDto, object>($"api/permis-batir/{id}/assigner-instructeur", dto);
        return r != null;
    }

    public async Task<bool> AssignerCommissionAsync(int id, AssignerCommissionDto dto)
    {
        var r = await _api.PatchAsync<AssignerCommissionDto, object>($"api/permis-batir/{id}/assigner-commission", dto);
        return r != null;
    }

    public async Task<bool> ChangerStatutAsync(int id, ChangerStatutPermisDto dto)
    {
        var r = await _api.PatchAsync<ChangerStatutPermisDto, object>($"api/permis-batir/{id}/statut", dto);
        return r != null;
    }

    public Task<PermisDelivreDto?> DelivrerPermisAsync(int id, DelivrerPermisDto dto) =>
        _api.PostAsync<DelivrerPermisDto, PermisDelivreDto>($"api/permis-batir/{id}/delivrer-permis", dto);

    public async Task<bool> AjouterTaxeAsync(int id, AjouterTaxeDto dto)
    {
        var r = await _api.PostAsync<AjouterTaxeDto, object>($"api/permis-batir/{id}/taxes", dto);
        return r != null;
    }

    public async Task<bool> PayerTaxeAsync(int id, PayerTaxeDto dto)
    {
        var r = await _api.PatchAsync<PayerTaxeDto, object>($"api/permis-batir/{id}/taxes/payer", dto);
        return r != null;
    }

    public async Task<bool> AjouterInspectionAsync(int id, CreateInspectionDto dto)
    {
        var r = await _api.PostAsync<CreateInspectionDto, object>($"api/permis-batir/{id}/inspections", dto);
        return r != null;
    }
}

// ── Demandeurs ────────────────────────────────────────────────────
public interface IDemandeurApiService
{
    Task<List<DemandeurDto>?> SearchAsync(string? term = null);
    Task<DemandeurDto?> GetByIdAsync(int id);
    Task<DemandeurDto?> CreateAsync(CreateDemandeurDto dto);
    Task<DemandeurDto?> UpdateAsync(int id, CreateDemandeurDto dto);
}

public class DemandeurApiService : IDemandeurApiService
{
    private readonly ApiService _api;
    public DemandeurApiService(ApiService api) => _api = api;

    public Task<List<DemandeurDto>?> SearchAsync(string? term = null)
    {
        var q = "api/demandeurs";
        if (!string.IsNullOrEmpty(term)) q += $"?term={Uri.EscapeDataString(term)}";
        return _api.GetAsync<List<DemandeurDto>>(q);
    }
    public Task<DemandeurDto?> GetByIdAsync(int id) =>
        _api.GetAsync<DemandeurDto>($"api/demandeurs/{id}");
    public Task<DemandeurDto?> CreateAsync(CreateDemandeurDto dto) =>
        _api.PostAsync<CreateDemandeurDto, DemandeurDto>("api/demandeurs", dto);
    public Task<DemandeurDto?> UpdateAsync(int id, CreateDemandeurDto dto) =>
        _api.PutAsync<CreateDemandeurDto, DemandeurDto>($"api/demandeurs/{id}", dto);
}

// ── Architectes ───────────────────────────────────────────────────
public interface IArchitecteApiService
{
    Task<List<ArchitecteDto>?> GetAllAsync();
    Task<ArchitecteDto?> CreateAsync(CreateArchitecteDto dto);
    Task<ArchitecteDto?> UpdateAsync(int id, CreateArchitecteDto dto);
}

public class ArchitecteApiService : IArchitecteApiService
{
    private readonly ApiService _api;
    public ArchitecteApiService(ApiService api) => _api = api;

    public Task<List<ArchitecteDto>?> GetAllAsync() =>
        _api.GetAsync<List<ArchitecteDto>>("api/architectes");
    public Task<ArchitecteDto?> CreateAsync(CreateArchitecteDto dto) =>
        _api.PostAsync<CreateArchitecteDto, ArchitecteDto>("api/architectes", dto);
    public Task<ArchitecteDto?> UpdateAsync(int id, CreateArchitecteDto dto) =>
        _api.PutAsync<CreateArchitecteDto, ArchitecteDto>($"api/architectes/{id}", dto);
}


// ── Zonage + TypeDemande + Commission (référentiels) ─────────────

public interface IPermisReferentielsApiService
{
    Task<List<ZonageUrbanismeDto>?> GetZonagesAsync();
    Task<List<TypeDemandePermisDto>?> GetTypesDemandeAsync();
    Task<List<CommissionExamenDto>?> GetCommissionsAsync();
    Task<List<TypeTaxeDto>?> GetTypesTaxeAsync();
    Task<ZonageUrbanismeDto?> CreateZonageAsync(CreateZonageDto dto);
    Task<TypeDemandePermisDto?> CreateTypeDemandeAsync(CreateTypeDemandePermisDto dto);
    Task<CommissionExamenDto?> CreateCommissionAsync(CreateCommissionExamenDto dto);
    Task<TypeTaxeDto?> CreateTypeTaxeAsync(CreateTypeTaxeDto dto);
}

public class PermisReferentielsApiService : IPermisReferentielsApiService
{
    private readonly ApiService _api;
    public PermisReferentielsApiService(ApiService api) => _api = api;

    public Task<List<ZonageUrbanismeDto>?> GetZonagesAsync() =>
        _api.GetAsync<List<ZonageUrbanismeDto>>("api/zonages-urbanisme");

    public Task<List<TypeDemandePermisDto>?> GetTypesDemandeAsync() =>
        _api.GetAsync<List<TypeDemandePermisDto>>("api/types-demande-permis");

    public Task<List<CommissionExamenDto>?> GetCommissionsAsync() =>
        _api.GetAsync<List<CommissionExamenDto>>("api/commissions-examen");

    public Task<List<TypeTaxeDto>?> GetTypesTaxeAsync() =>
        _api.GetAsync<List<TypeTaxeDto>>("api/types-taxe");

    public Task<ZonageUrbanismeDto?> CreateZonageAsync(CreateZonageDto dto) =>
        _api.PostAsync<CreateZonageDto, ZonageUrbanismeDto>("api/zonages-urbanisme", dto);

    public Task<TypeDemandePermisDto?> CreateTypeDemandeAsync(CreateTypeDemandePermisDto dto) =>
        _api.PostAsync<CreateTypeDemandePermisDto, TypeDemandePermisDto>("api/types-demande-permis", dto);

    public Task<CommissionExamenDto?> CreateCommissionAsync(CreateCommissionExamenDto dto) =>
        _api.PostAsync<CreateCommissionExamenDto, CommissionExamenDto>("api/commissions-examen", dto);

    public Task<TypeTaxeDto?> CreateTypeTaxeAsync(CreateTypeTaxeDto dto) =>
        _api.PostAsync<CreateTypeTaxeDto, TypeTaxeDto>("api/types-taxe", dto);
}