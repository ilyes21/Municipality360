// ═══════════════════════════════════════════════════════════════════
//  BureauOrdreApiServices.cs
//  Municipality360.Web/Services/BureauOrdreApiServices.cs
//
//  يُضاف في Program.cs:
//    builder.Services.AddScoped<ICourrierEntrantApiService, CourrierEntrantApiService>();
//    builder.Services.AddScoped<ICourrierSortantApiService, CourrierSortantApiService>();
//    builder.Services.AddScoped<IBODossierApiService, BODossierApiService>();
//    builder.Services.AddScoped<IBOContactApiService, BOContactApiService>();
//    builder.Services.AddScoped<IBOStatsApiService, BOStatsApiService>();
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Components.Forms;
using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Structure;

namespace Municipality360.Web.Services;



// ── Courrier Entrant ──────────────────────────────────────────────

public interface ICourrierEntrantApiService
{
    // Lectures
    Task<PagedResult<CourrierEntrantDto>?> GetPagedAsync(CourrierEntrantFilterDto filter);
    Task<CourrierEntrantDetailDto?> GetByIdAsync(int id);
    Task<CourrierEntrantDetailDto?> GetByNumeroAsync(string numero);
    Task<List<CourrierEntrantDto>?> GetEnRetardAsync();
    Task<List<CourrierEntrantDto>?> GetEnAttenteParServiceAsync(int serviceId);
    Task<BOStatsDto?> GetStatsAsync(int? serviceId = null);

    // Écriture
    Task<CourrierEntrantDetailDto?> EnregistrerAsync(CreateCourrierEntrantDto dto);
    Task<CourrierEntrantDetailDto?> EnregistrerAvecFichiersAsync(CreateCourrierEntrantDto dto, List<IBrowserFile> fichiers);
    Task<CourrierEntrantDetailDto?> ModifierAsync(int id, UpdateCourrierEntrantDto dto);
    Task<bool> ChangerStatutAsync(int id, ChangerStatutEntrantDto dto);
    Task<bool> AffecterAsync(int id, AffecterCourrierEntrantDto dto);

    // Circuit
    Task<BOCircuitTraitementDto?> AcheminerAsync(int id, AcheminerCourrierDto dto);
    Task<bool> TraiterEtapeCircuitAsync(int circuitId, TraiterEtapeCircuitDto dto);
    Task<bool> RetournerEtapeCircuitAsync(int circuitId, RetournerEtapeCircuitDto dto);
    Task<List<BOCircuitTraitementDto>?> GetCircuitAsync(int id);

    // Pièces jointes
    Task<bool> UploadPiecesJointesAsync(int id, List<IBrowserFile> fichiers, string typePiece = "Annexe");

    // Archivage
    Task<BOArchiveDto?> ArchiverAsync(int id, ArchiversCourrierDto dto);

    // Services (pour les dropdowns)
    Task<List<ServiceItemDto>?> GetServicesAsync();
}

public class CourrierEntrantApiService : ICourrierEntrantApiService
{
    private readonly ApiService _api;

    public CourrierEntrantApiService(ApiService api)
    {
        _api = api;
    }

    // ── Lectures ──────────────────────────────────────────────────

    public async Task<PagedResult<CourrierEntrantDto>?> GetPagedAsync(CourrierEntrantFilterDto f)
    {
        var q = $"api/courriers-entrants?pageNumber={f.PageNumber}&pageSize={f.PageSize}";
        if (!string.IsNullOrEmpty(f.Statut)) q += $"&statut={f.Statut}";
        if (!string.IsNullOrEmpty(f.Priorite)) q += $"&priorite={f.Priorite}";
        if (!string.IsNullOrEmpty(f.TypeDocument)) q += $"&typeDocument={f.TypeDocument}";
        if (f.ServiceDestinataireId.HasValue) q += $"&serviceDestinataireId={f.ServiceDestinataireId}";
        if (f.CategorieId.HasValue) q += $"&categorieId={f.CategorieId}";
        if (f.DossierId.HasValue) q += $"&dossierId={f.DossierId}";
        if (f.NecessiteReponse.HasValue) q += $"&necessiteReponse={f.NecessiteReponse}";
        if (f.EnRetard.HasValue) q += $"&enRetard={f.EnRetard}";
        if (f.EstConfidentiel.HasValue) q += $"&estConfidentiel={f.EstConfidentiel}";
        if (f.DateDebut.HasValue) q += $"&dateDebut={f.DateDebut:yyyy-MM-dd}";
        if (f.DateFin.HasValue) q += $"&dateFin={f.DateFin:yyyy-MM-dd}";
        if (!string.IsNullOrEmpty(f.SearchTerm)) q += $"&searchTerm={Uri.EscapeDataString(f.SearchTerm)}";
        return await _api.GetAsync<PagedResult<CourrierEntrantDto>>(q);
    }

    public Task<CourrierEntrantDetailDto?> GetByIdAsync(int id) =>
        _api.GetAsync<CourrierEntrantDetailDto>($"api/courriers-entrants/{id}");

    public Task<CourrierEntrantDetailDto?> GetByNumeroAsync(string numero) =>
        _api.GetAsync<CourrierEntrantDetailDto>($"api/courriers-entrants/numero/{numero}");

    public Task<List<CourrierEntrantDto>?> GetEnRetardAsync() =>
        _api.GetAsync<List<CourrierEntrantDto>>("api/courriers-entrants/en-retard");

    public Task<List<CourrierEntrantDto>?> GetEnAttenteParServiceAsync(int serviceId) =>
        _api.GetAsync<List<CourrierEntrantDto>>($"api/courriers-entrants/en-attente/service/{serviceId}");

    public async Task<BOStatsDto?> GetStatsAsync(int? serviceId = null)
    {
        var q = "api/courriers-entrants/stats";
        if (serviceId.HasValue) q += $"?serviceId={serviceId}";
        return await _api.GetAsync<BOStatsDto>(q);
    }

    // ── Enregistrement JSON ───────────────────────────────────────

    public Task<CourrierEntrantDetailDto?> EnregistrerAsync(CreateCourrierEntrantDto dto) =>
        _api.PostAsync<CreateCourrierEntrantDto, CourrierEntrantDetailDto>("api/courriers-entrants", dto);

    // ── Enregistrement avec fichiers (multipart) ──────────────────

    public async Task<CourrierEntrantDetailDto?> EnregistrerAvecFichiersAsync(
        CreateCourrierEntrantDto dto, List<IBrowserFile> fichiers)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            // Champs texte
            void Add(string key, string? val)
            { if (val != null) content.Add(new StringContent(val), key); }

            Add("NumeroExterne", dto.NumeroExterne);
            Add("DateCourrier", dto.DateCourrier.ToString("yyyy-MM-dd"));
            Add("DateReception", dto.DateReception.ToString("yyyy-MM-dd"));
            Add("ObjetCourrier", dto.ObjetCourrier);
            Add("TypeDocument", dto.TypeDocument);
            Add("ModeReception", dto.ModeReception);
            Add("NumeroRecommande", dto.NumeroRecommande);
            Add("Priorite", dto.Priorite);
            Add("Observation", dto.Observation);
            Add("ExpediteurLibreNom", dto.ExpediteurLibreNom);
            content.Add(new StringContent(dto.NombrePages.ToString()), "NombrePages");
            content.Add(new StringContent(dto.EstConfidentiel.ToString()), "EstConfidentiel");
            content.Add(new StringContent(dto.NecessiteReponse.ToString()), "NecessiteReponse");
            if (dto.ServiceDestinataireId.HasValue)
                content.Add(new StringContent(dto.ServiceDestinataireId.Value.ToString()), "ServiceDestinataireId");
            if (dto.CategorieId.HasValue)
                content.Add(new StringContent(dto.CategorieId.Value.ToString()), "CategorieId");
            if (dto.DossierId.HasValue)
                content.Add(new StringContent(dto.DossierId.Value.ToString()), "DossierId");
            if (dto.DelaiReponse.HasValue)
                content.Add(new StringContent(dto.DelaiReponse.Value.ToString("yyyy-MM-dd")), "DelaiReponse");
            if (dto.ExpediteurContactId.HasValue)
                content.Add(new StringContent(dto.ExpediteurContactId.Value.ToString()), "ExpediteurContactId");
            if (!string.IsNullOrEmpty(dto.AgentDestinataireId))
                content.Add(new StringContent(dto.AgentDestinataireId), "AgentDestinataireId");

            // Fichiers
            foreach (var file in fichiers)
            {
                var stream = file.OpenReadStream(maxAllowedSize: 10_485_760);
                var sc = new StreamContent(stream);
                sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : "image/jpeg");
                content.Add(sc, "Fichiers", file.Name);
            }

            return await _api.PostMultipartAsync<CourrierEntrantDetailDto>("api/courriers-entrants/avec-fichiers", content);
        }
        catch { return null; }
    }

    // ── Modification ──────────────────────────────────────────────

    public Task<CourrierEntrantDetailDto?> ModifierAsync(int id, UpdateCourrierEntrantDto dto) =>
        _api.PutAsync<UpdateCourrierEntrantDto, CourrierEntrantDetailDto>($"api/courriers-entrants/{id}", dto);

    // ── Statut ────────────────────────────────────────────────────

    public async Task<bool> ChangerStatutAsync(int id, ChangerStatutEntrantDto dto)
    {
        var result = await _api.PatchAsync<ChangerStatutEntrantDto, object>(
            $"api/courriers-entrants/{id}/statut", dto);
        return result != null;
    }

    // ── Affectation directe ───────────────────────────────────────

    public async Task<bool> AffecterAsync(int id, AffecterCourrierEntrantDto dto)
    {
        var result = await _api.PatchAsync<AffecterCourrierEntrantDto, object>(
            $"api/courriers-entrants/{id}/affecter", dto);
        return result != null;
    }

    // ── Circuit ───────────────────────────────────────────────────

    public Task<BOCircuitTraitementDto?> AcheminerAsync(int id, AcheminerCourrierDto dto) =>
        _api.PostAsync<AcheminerCourrierDto, BOCircuitTraitementDto>(
            $"api/courriers-entrants/{id}/acheminer", dto);

    public async Task<bool> TraiterEtapeCircuitAsync(int circuitId, TraiterEtapeCircuitDto dto)
    {
        var result = await _api.PatchAsync<TraiterEtapeCircuitDto, object>(
            $"api/courriers-entrants/circuit/{circuitId}/traiter", dto);
        return result != null;
    }

    public async Task<bool> RetournerEtapeCircuitAsync(int circuitId, RetournerEtapeCircuitDto dto)
    {
        var result = await _api.PatchAsync<RetournerEtapeCircuitDto, object>(
            $"api/courriers-entrants/circuit/{circuitId}/retourner", dto);
        return result != null;
    }

    public Task<List<BOCircuitTraitementDto>?> GetCircuitAsync(int id) =>
        _api.GetAsync<List<BOCircuitTraitementDto>>($"api/courriers-entrants/{id}/circuit");

    // ── Pièces jointes multipart ──────────────────────────────────

    public async Task<bool> UploadPiecesJointesAsync(
        int id, List<IBrowserFile> fichiers, string typePiece = "Annexe")
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(typePiece), "TypePiece");

            foreach (var file in fichiers)
            {
                var stream = file.OpenReadStream(maxAllowedSize: 10_485_760);
                var sc = new StreamContent(stream);
                sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : "image/jpeg");
                content.Add(sc, "Fichiers", file.Name);
            }

            var result = await _api.PostMultipartAsync<object>($"api/courriers-entrants/{id}/pieces-jointes", content);
            return result != null;
        }
        catch { return false; }
    }

    // ── Archivage ─────────────────────────────────────────────────

    public Task<BOArchiveDto?> ArchiverAsync(int id, ArchiversCourrierDto dto) =>
        _api.PostAsync<ArchiversCourrierDto, BOArchiveDto>(
            $"api/courriers-entrants/{id}/archiver", dto);

    // ── Services (dropdown) ───────────────────────────────────────

    public async Task<List<ServiceItemDto>?> GetServicesAsync()
    {
        try
        {
            var result = await _api.GetAsync<Result<IEnumerable<ServiceDto>>>("api/services");
            return result?.Data?
                .Where(s => s.IsActive)
                .Select(s => new ServiceItemDto(s.Id, s.Nom))
                .ToList();
        }
        catch { return new(); }
    }
}


// ── Courrier Sortant ──────────────────────────────────────────────

public interface ICourrierSortantApiService
{
    Task<PagedResult<CourrierSortantDto>?> GetPagedAsync(CourrierSortantFilterDto filter);
    Task<CourrierSortantDetailDto?> GetByIdAsync(int id);
    Task<CourrierSortantDetailDto?> GetByNumeroAsync(string numero);
    Task<List<CourrierSortantDto>?> GetBrouillonsAsync();
    Task<CourrierSortantDetailDto?> CreerBrouillonAsync(CreateCourrierSortantDto dto);
    Task<CourrierSortantDetailDto?> ModifierAsync(int id, CreateCourrierSortantDto dto);
    Task<bool> SoumettreEnValidationAsync(int id);
    Task<bool> MarquerSigneAsync(int id, string fonctionSignataire);
    Task<bool> MarquerEnvoyeAsync(int id, DateTime? dateEnvoi);
    Task<bool> AccuserReceptionAsync(int id, DateTime dateAccuse);
    Task<BOArchiveDto?> ArchiverAsync(int id, ArchiversCourrierDto dto);
    Task<bool> AnnulerAsync(int id, string motif);
    Task<bool> UploadPiecesJointesAsync(int id, List<IBrowserFile> fichiers, string typePiece = "Annexe");
}

public class CourrierSortantApiService : ICourrierSortantApiService
{
    private readonly ApiService _api;
    public CourrierSortantApiService(ApiService api) { _api = api; }

    public async Task<PagedResult<CourrierSortantDto>?> GetPagedAsync(CourrierSortantFilterDto f)
    {
        var q = $"api/courriers-sortants?pageNumber={f.PageNumber}&pageSize={f.PageSize}";
        if (!string.IsNullOrEmpty(f.Statut)) q += $"&statut={f.Statut}";
        if (!string.IsNullOrEmpty(f.Priorite)) q += $"&priorite={f.Priorite}";
        if (!string.IsNullOrEmpty(f.TypeDocument)) q += $"&typeDocument={f.TypeDocument}";
        if (f.ServiceEmetteurId.HasValue) q += $"&serviceEmetteurId={f.ServiceEmetteurId}";
        if (f.CategorieId.HasValue) q += $"&categorieId={f.CategorieId}";
        if (f.DossierId.HasValue) q += $"&dossierId={f.DossierId}";
        if (f.DateDebut.HasValue) q += $"&dateDebut={f.DateDebut:yyyy-MM-dd}";
        if (f.DateFin.HasValue) q += $"&dateFin={f.DateFin:yyyy-MM-dd}";
        if (!string.IsNullOrEmpty(f.SearchTerm)) q += $"&searchTerm={Uri.EscapeDataString(f.SearchTerm)}";
        return await _api.GetAsync<PagedResult<CourrierSortantDto>>(q);
    }

    public async Task<CourrierSortantDetailDto?> GetByIdAsync(int id) =>
        await _api.GetAsync<CourrierSortantDetailDto>($"api/courriers-sortants/{id}");

    public async Task<CourrierSortantDetailDto?> GetByNumeroAsync(string numero) =>
        await _api.GetAsync<CourrierSortantDetailDto>($"api/courriers-sortants/numero/{numero}");

    public async Task<List<CourrierSortantDto>?> GetBrouillonsAsync() =>
        await _api.GetAsync<List<CourrierSortantDto>>("api/courriers-sortants/brouillons");

    public async Task<CourrierSortantDetailDto?> CreerBrouillonAsync(CreateCourrierSortantDto dto) =>
        await _api.PostAsync<CreateCourrierSortantDto, CourrierSortantDetailDto>("api/courriers-sortants", dto);

    public async Task<CourrierSortantDetailDto?> ModifierAsync(int id, CreateCourrierSortantDto dto) =>
        await _api.PutAsync<CreateCourrierSortantDto, CourrierSortantDetailDto>($"api/courriers-sortants/{id}", dto);

    public async Task<bool> SoumettreEnValidationAsync(int id)
    {
        var result = await _api.PatchAsync<object, object>($"api/courriers-sortants/{id}/soumettre", new { });
        return result != null;
    }

    public async Task<bool> MarquerSigneAsync(int id, string fonctionSignataire)
    {
        var dto = new SignerCourrierDto { FonctionSignataire = fonctionSignataire };
        var result = await _api.PatchAsync<SignerCourrierDto, object>($"api/courriers-sortants/{id}/signer", dto);
        return result != null;
    }

    public async Task<bool> MarquerEnvoyeAsync(int id, DateTime? dateEnvoi)
    {
        var dto = new EnvoyerCourrierDto { DateEnvoi = dateEnvoi };
        var result = await _api.PatchAsync<EnvoyerCourrierDto, object>($"api/courriers-sortants/{id}/envoyer", dto);
        return result != null;
    }

    public async Task<bool> AccuserReceptionAsync(int id, DateTime dateAccuse)
    {
        var result = await _api.PatchAsync<object, object>($"api/courriers-sortants/{id}/accuser-reception", new { dateAccuse });
        return result != null;
    }

    public async Task<BOArchiveDto?> ArchiverAsync(int id, ArchiversCourrierDto dto) =>
        await _api.PostAsync<ArchiversCourrierDto, BOArchiveDto>($"api/courriers-sortants/{id}/archiver", dto);

    public async Task<bool> AnnulerAsync(int id, string motif)
    {
        var dto = new AnnulerCourrierDto { Motif = motif };
        var result = await _api.PatchAsync<AnnulerCourrierDto, object>($"api/courriers-sortants/{id}/annuler", dto);
        return result != null;
    }

    public async Task<bool> UploadPiecesJointesAsync(int id, List<IBrowserFile> fichiers, string typePiece = "Annexe")
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(typePiece), "typePiece");
            foreach (var file in fichiers)
            {
                var stream = file.OpenReadStream(maxAllowedSize: 10_485_760);
                var sc = new StreamContent(stream);
                sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : "image/jpeg");
                content.Add(sc, "fichiers", file.Name);
            }
            var result = await _api.PostMultipartAsync<object>($"api/courriers-sortants/{id}/pieces-jointes", content);
            return result != null;
        }
        catch { return false; }
    }
}

// ── Dossiers ──────────────────────────────────────────────────────

public interface IBODossierApiService
{
    Task<PagedResult<BODossierDto>?> GetPagedAsync(int page = 1, int size = 20);
    Task<List<BODossierDto>?> SearchAsync(string? term = null);
    Task<BODossierDto?> GetByIdAsync(int id);
    Task<BODossierDto?> CreateAsync(CreateBODossierDto dto);
    Task<bool> CloreAsync(int id);
}

public class BODossierApiService : IBODossierApiService
{
    private readonly ApiService _api;
    public BODossierApiService(ApiService api) => _api = api;

    public async Task<PagedResult<BODossierDto>?> GetPagedAsync(int page = 1, int size = 20) =>
        await _api.GetAsync<PagedResult<BODossierDto>>($"api/bo-dossiers?page={page}&size={size}");

    public async Task<List<BODossierDto>?> SearchAsync(string? term = null)
    {
        var q = "api/bo-dossiers/search";
        if (!string.IsNullOrEmpty(term)) q += $"?term={Uri.EscapeDataString(term)}";
        return await _api.GetAsync<List<BODossierDto>>(q);
    }

    public async Task<BODossierDto?> GetByIdAsync(int id) =>
        await _api.GetAsync<BODossierDto>($"api/bo-dossiers/{id}");

    public async Task<BODossierDto?> CreateAsync(CreateBODossierDto dto) =>
        await _api.PostAsync<CreateBODossierDto, BODossierDto>("api/bo-dossiers", dto);

    public async Task<bool> CloreAsync(int id)
    {
        var result = await _api.PatchAsync<object, object>($"api/bo-dossiers/{id}/clore", new { });
        return result != null;
    }
}

// ── Catégories Courrier ───────────────────────────────────────────

public interface IBOCategorieCourrierApiService
{
    Task<List<BOCategorieCourrierDto>?> GetAllAsync();
}

public class BOCategorieCourrierApiService : IBOCategorieCourrierApiService
{
    private readonly ApiService _api;
    public BOCategorieCourrierApiService(ApiService api) => _api = api;

    public async Task<List<BOCategorieCourrierDto>?> GetAllAsync() =>
        await _api.GetAsync<List<BOCategorieCourrierDto>>("api/bo-categories");
}

// ── Contacts ──────────────────────────────────────────────────────

public interface IBOContactApiService
{
    Task<List<BOContactDto>?> SearchAsync(string? term = null);
    Task<BOContactDto?> GetByIdAsync(int id);
    Task<BOContactDto?> CreateAsync(CreateBOContactDto dto);
    Task<BOContactDto?> UpdateAsync(int id, CreateBOContactDto dto);
    Task<bool> DeleteAsync(int id);
}

public class BOContactApiService : IBOContactApiService
{
    private readonly ApiService _api;
    public BOContactApiService(ApiService api) => _api = api;

    public async Task<List<BOContactDto>?> SearchAsync(string? term = null)
    {
        var q = "api/bo-contacts";
        if (!string.IsNullOrEmpty(term)) q += $"?term={Uri.EscapeDataString(term)}";
        return await _api.GetAsync<List<BOContactDto>>(q);
    }

    public async Task<BOContactDto?> GetByIdAsync(int id) =>
        await _api.GetAsync<BOContactDto>($"api/bo-contacts/{id}");

    public async Task<BOContactDto?> CreateAsync(CreateBOContactDto dto) =>
        await _api.PostAsync<CreateBOContactDto, BOContactDto>("api/bo-contacts", dto);

    public async Task<BOContactDto?> UpdateAsync(int id, CreateBOContactDto dto) =>
        await _api.PutAsync<CreateBOContactDto, BOContactDto>($"api/bo-contacts/{id}", dto);

    public async Task<bool> DeleteAsync(int id) => await _api.DeleteAsync($"api/bo-contacts/{id}");
}