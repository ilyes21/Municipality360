using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

public class EmployeService : IEmployeService
{
    private readonly IEmployeRepository _employeRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly IPosteRepository _posteRepo;

    public EmployeService(IEmployeRepository employeRepo, IServiceRepository serviceRepo, IPosteRepository posteRepo)
    {
        _employeRepo = employeRepo;
        _serviceRepo = serviceRepo;
        _posteRepo = posteRepo;
    }

    public async Task<Result<PagedResult<EmployeDto>>> GetPagedAsync(EmployeFilterDto filter)
    {
        var result = await _employeRepo.GetPagedAsync(filter);
        var dtos = new PagedResult<EmployeDto>
        {
            Items = result.Items.Select(MapToDto),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        return Result<PagedResult<EmployeDto>>.Success(dtos);
    }

    public async Task<Result<EmployeDto>> GetByIdAsync(int id)
    {
        var employe = await _employeRepo.GetByIdWithDetailsAsync(id);
        if (employe == null)
            return Result<EmployeDto>.Failure($"Employe avec l'id {id} introuvable.");
        return Result<EmployeDto>.Success(MapToDto(employe));
    }

    public async Task<Result<EmployeDto>> CreateAsync(CreateEmployeDto dto)
    {
        if (await _employeRepo.IdentifiantExistsAsync(dto.Cin))
            return Result<EmployeDto>.Failure($"Un employe avec le Identifiant {dto.Identifiant} existe deja.");

        if (await _employeRepo.CinExistsAsync(dto.Cin))
            return Result<EmployeDto>.Failure($"Un employe avec le Cin {dto.Cin} existe deja.");

        if (!await _serviceRepo.ExistsAsync(dto.ServiceId))
            return Result<EmployeDto>.Failure("Service introuvable.");

        if (!await _posteRepo.ExistsAsync(dto.PosteId))
            return Result<EmployeDto>.Failure("Poste introuvable.");

        if (!Enum.TryParse<Genre>(dto.Genre, true, out var genre))
            return Result<EmployeDto>.Failure("Genre invalide. Valeurs: Masculin, Feminin.");

        var employe = new Employe
        {
            Identifiant = dto.Identifiant,
            Cin = dto.Cin,
            Prenom = dto.Prenom,
            Nom = dto.Nom,
            Email = dto.Email,
            Telephone = dto.Telephone,
            DateNaissance = dto.DateNaissance,
            DateEmbauche = dto.DateEmbauche,
            Genre = genre,
            Salaire = dto.Salaire,
            Adresse = dto.Adresse,
            ServiceId = dto.ServiceId,
            PosteId = dto.PosteId
        };

        await _employeRepo.AddAsync(employe);
        var created = await _employeRepo.GetByIdWithDetailsAsync(employe.Id);
        return Result<EmployeDto>.Success(MapToDto(created!), "Employe cree avec succes.");
    }

    public async Task<Result<EmployeDto>> UpdateAsync(int id, UpdateEmployeDto dto)
    {
        var employe = await _employeRepo.GetByIdAsync(id);
        if (employe == null)
            return Result<EmployeDto>.Failure($"Employe avec l'id {id} introuvable.");

        if (!Enum.TryParse<Genre>(dto.Genre, true, out var genre))
            return Result<EmployeDto>.Failure("Genre invalide.");

        if (!Enum.TryParse<StatutEmploye>(dto.Statut, true, out var statut))
            return Result<EmployeDto>.Failure("Statut invalide.");

        employe.Prenom = dto.Prenom;
        employe.Nom = dto.Nom;
        employe.Email = dto.Email;
        employe.Telephone = dto.Telephone;
        employe.DateNaissance = dto.DateNaissance;
        employe.DateEmbauche = dto.DateEmbauche;
        employe.DateDepart = dto.DateDepart;
        employe.Genre = genre;
        employe.Statut = statut;
        employe.Salaire = dto.Salaire;
        employe.Adresse = dto.Adresse;
        employe.ServiceId = dto.ServiceId;
        employe.PosteId = dto.PosteId;
        employe.UpdatedAt = DateTime.UtcNow;

        await _employeRepo.UpdateAsync(employe);
        var updated = await _employeRepo.GetByIdWithDetailsAsync(id);
        return Result<EmployeDto>.Success(MapToDto(updated!), "Employe mis a jour.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var employe = await _employeRepo.GetByIdAsync(id);
        if (employe == null)
            return Result.Failure($"Employe avec l'id {id} introuvable.");

        await _employeRepo.DeleteAsync(employe);
        return Result.Success("Employe supprime.");
    }

    private static EmployeDto MapToDto(Employe e) => new()
    {
        Id = e.Id,
        Identifiant = e.Identifiant,
        Cin = e.Cin,
        Prenom = e.Prenom,
        Nom = e.Nom,
        NomComplet = e.NomComplet,
        Email = e.Email,
        Telephone = e.Telephone,
        DateNaissance = e.DateNaissance,
        DateEmbauche = e.DateEmbauche,
        DateDepart = e.DateDepart,
        Genre = e.Genre.ToString(),
        Statut = e.Statut.ToString(),
        Salaire = e.Salaire,
        Adresse = e.Adresse,
        ServiceId = e.ServiceId,
        ServiceNom = e.Service?.Nom ?? string.Empty,
        DepartementNom = e.Service?.Departement?.Nom ?? string.Empty,
        PosteId = e.PosteId,
        PosteTitre = e.Poste?.Titre ?? string.Empty,
        CreatedAt = e.CreatedAt
    };
}
