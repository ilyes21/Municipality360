// ═══════════════════════════════════════════════════════════════════
//  AdditionalServices.cs
//  Application/Services/AdditionalServices.cs
//  يحتوي: BODossierService · BOContactService · BOArchiveService
//          DemandeurService · ArchitecteService
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Application.Common;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

// ── BODossierService ──────────────────────────────────────────────

public class BODossierService : IBODossierService
{
    private readonly IBODossierRepository _repo;
    private readonly ISequenceRepository _seq;
    public BODossierService(IBODossierRepository repo, ISequenceRepository seq)
    { _repo = repo; _seq = seq; }

    public Task<PagedResult<BODossierDto>> GetPagedAsync(int page, int size)
        => _repo.GetPagedAsync(page, size);

    public async Task<BODossierDto> GetByIdAsync(int id)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Dossier #{id} introuvable.");
        return new BODossierDto
        {
            Id = d.Id, NumeroDossier = d.NumeroDossier, Intitule = d.Intitule,
            Description = d.Description, ServiceResponsableId = d.ServiceResponsableId,
            DateOuverture = d.DateOuverture, DateCloture = d.DateCloture,
            StatutDossier = d.StatutDossier.ToString()
        };
    }

    public async Task<BODossierDto> CreateAsync(CreateBODossierDto dto, string agentId)
    {
        var numero = await _seq.GenererNumeroAsync("DOS");
        var d = new BODossier
        {
            NumeroDossier = numero, Intitule = dto.Intitule,
            Description = dto.Description, ServiceResponsableId = dto.ServiceResponsableId,
            DateOuverture = DateTime.UtcNow, StatutDossier = StatutDossierBO.Ouvert,
            IsActive = true, CreatedById = agentId
        };
        await _repo.AddAsync(d);
        return await GetByIdAsync(d.Id);
    }

    public async Task CloreAsync(int id, string agentId)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Dossier #{id} introuvable.");
        d.StatutDossier = StatutDossierBO.Cloture;
        d.DateCloture = DateTime.UtcNow;
        d.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(d);
    }
}

// ── BOContactService ──────────────────────────────────────────────

public class BOContactService : IBOContactService
{
    private readonly IBOContactRepository _repo;
    public BOContactService(IBOContactRepository repo) => _repo = repo;

    public async Task<List<BOContactDto>> SearchAsync(string? term)
    {
        var list = await _repo.SearchAsync(term);
        return list.Select(MapToDto).ToList();
    }

    public async Task<BOContactDto> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Contact #{id} introuvable.");
        return MapToDto(c);
    }

    public async Task<BOContactDto> CreateAsync(CreateBOContactDto dto)
    {
        var c = new BOContact
        {
            TypeContact = Enum.Parse<TypeContact>(dto.TypeContact),
            Nom = dto.Nom, Prenom = dto.Prenom, CIN = dto.CIN, RaisonSociale = dto.RaisonSociale,
            Fonction = dto.Fonction, Adresse = dto.Adresse, Ville = dto.Ville,
            Wilaya = dto.Wilaya, Telephone = dto.Telephone, Email = dto.Email,
            IsActive = true
        };
        await _repo.AddAsync(c);
        return MapToDto(c);
    }

    public async Task<BOContactDto> UpdateAsync(int id, CreateBOContactDto dto)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Contact #{id} introuvable.");
        c.TypeContact = Enum.Parse<TypeContact>(dto.TypeContact);
        c.Nom = dto.Nom; c.Prenom = dto.Prenom; c.CIN = dto.CIN; c.RaisonSociale = dto.RaisonSociale;
        c.Fonction = dto.Fonction; c.Adresse = dto.Adresse;
        c.Ville = dto.Ville; c.Wilaya = dto.Wilaya;
        c.Telephone = dto.Telephone; c.Email = dto.Email;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
        return MapToDto(c);
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Contact #{id} introuvable.");
        await _repo.DeleteAsync(c);
    }

    private static BOContactDto MapToDto(BOContact c) => new()
    {
        Id = c.Id, TypeContact = c.TypeContact.ToString(), Nom = c.Nom, Prenom = c.Prenom,
        CIN = c.CIN, RaisonSociale = c.RaisonSociale, Fonction = c.Fonction, Adresse = c.Adresse,
        Ville = c.Ville, Wilaya = c.Wilaya, Telephone = c.Telephone, Email = c.Email,
        NomComplet = c.NomComplet, IsActive = c.IsActive
    };
}

// ── BOArchiveService ──────────────────────────────────────────────

public class BOArchiveService : IBOArchiveService
{
    private readonly IBOArchiveRepository _repo;
    public BOArchiveService(IBOArchiveRepository repo) => _repo = repo;

    public async Task<BOArchiveDto> GetByCourrierEntrantAsync(int courrierEntrantId)
        => await _repo.GetByCourrierEntrantIdAsync(courrierEntrantId)
            ?? throw new KeyNotFoundException($"Archive pour courrier entrant #{courrierEntrantId} introuvable.");

    public async Task<BOArchiveDto> GetByCourrierSortantAsync(int courrierSortantId)
        => await _repo.GetByCourrierSortantIdAsync(courrierSortantId)
            ?? throw new KeyNotFoundException($"Archive pour courrier sortant #{courrierSortantId} introuvable.");
}

// ── DemandeurService ──────────────────────────────────────────────

public class DemandeurService : IDemandeurService
{
    private readonly IDemandeurRepository _repo;
    public DemandeurService(IDemandeurRepository repo) => _repo = repo;

    public async Task<List<DemandeurDto>> SearchAsync(string? term)
        => (await _repo.SearchAsync(term)).Select(MapToDto).ToList();

    public async Task<DemandeurDto> GetByIdAsync(int id)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demandeur #{id} introuvable.");
        return MapToDto(d);
    }

    public async Task<DemandeurDto> CreateAsync(CreateDemandeurDto dto)
    {
        var d = new Demandeur
        {
            Type = Enum.Parse<TypeDemandeur>(dto.Type), CIN = dto.CIN,
            Nom = dto.Nom, Prenom = dto.Prenom, RaisonSociale = dto.RaisonSociale,
            Adresse = dto.Adresse, Telephone = dto.Telephone, Email = dto.Email, IsActive = true
        };
        await _repo.AddAsync(d);
        return MapToDto(d);
    }

    public async Task<DemandeurDto> UpdateAsync(int id, CreateDemandeurDto dto)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demandeur #{id} introuvable.");
        d.Type = Enum.Parse<TypeDemandeur>(dto.Type); d.CIN = dto.CIN;
        d.Nom = dto.Nom; d.Prenom = dto.Prenom; d.RaisonSociale = dto.RaisonSociale;
        d.Adresse = dto.Adresse; d.Telephone = dto.Telephone;
        d.Email = dto.Email; d.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(d);
        return MapToDto(d);
    }

    private static DemandeurDto MapToDto(Demandeur d) => new()
    {
        Id = d.Id, Type = d.Type.ToString(), CIN = d.CIN, Nom = d.Nom,
        Prenom = d.Prenom, RaisonSociale = d.RaisonSociale, NomComplet = d.NomComplet,
        Adresse = d.Adresse, Telephone = d.Telephone, Email = d.Email,
        NombreDemandes = d.Demandes?.Count ?? 0
    };
}

// ── ArchitecteService ─────────────────────────────────────────────

public class ArchitecteService : IArchitecteService
{
    private readonly IArchitecteRepository _repo;
    public ArchitecteService(IArchitecteRepository repo) => _repo = repo;

    public async Task<List<ArchitecteDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(MapToDto).ToList();

    public async Task<ArchitecteDto> GetByIdAsync(int id)
    {
        var a = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Architecte #{id} introuvable.");
        return MapToDto(a);
    }

    public async Task<ArchitecteDto> CreateAsync(CreateArchitecteDto dto)
    {
        if (await _repo.NumeroOrdreExistsAsync(dto.NumeroOrdre))
            throw new InvalidOperationException($"Numéro d'ordre {dto.NumeroOrdre} déjà utilisé.");
        var a = new Architecte
        {
            NumeroOrdre = dto.NumeroOrdre, CIN = dto.CIN, Nom = dto.Nom,
            Prenom = dto.Prenom, Telephone = dto.Telephone, Email = dto.Email, IsActive = true
        };
        await _repo.AddAsync(a);
        return MapToDto(a);
    }

    public async Task<ArchitecteDto> UpdateAsync(int id, CreateArchitecteDto dto)
    {
        var a = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Architecte #{id} introuvable.");
        if (await _repo.NumeroOrdreExistsAsync(dto.NumeroOrdre, id))
            throw new InvalidOperationException($"Numéro d'ordre {dto.NumeroOrdre} déjà utilisé.");
        a.NumeroOrdre = dto.NumeroOrdre; a.CIN = dto.CIN; a.Nom = dto.Nom;
        a.Prenom = dto.Prenom; a.Telephone = dto.Telephone; a.Email = dto.Email;
        a.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(a);
        return MapToDto(a);
    }

    private static ArchitecteDto MapToDto(Architecte a) => new()
    {
        Id = a.Id, NumeroOrdre = a.NumeroOrdre, CIN = a.CIN, Nom = a.Nom,
        Prenom = a.Prenom, NomComplet = a.NomComplet, Telephone = a.Telephone,
        Email = a.Email, IsActive = a.IsActive
    };
}
