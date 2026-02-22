using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

public class DepartementService : IDepartementService
{
    private readonly IDepartementRepository _repository;

    public DepartementService(IDepartementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<DepartementDto>>> GetAllAsync()
    {
        var departements = await _repository.GetAllWithServicesAsync();
        var dtos = departements.Select(d => MapToDto(d));
        return Result<IEnumerable<DepartementDto>>.Success(dtos);
    }

    public async Task<Result<DepartementDto>> GetByIdAsync(int id)
    {
        var departement = await _repository.GetByIdWithServicesAsync(id);
        if (departement == null)
            return Result<DepartementDto>.Failure($"Departement avec l'id {id} introuvable.");
        return Result<DepartementDto>.Success(MapToDto(departement));
    }

    public async Task<Result<DepartementDto>> CreateAsync(CreateDepartementDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Code) && await _repository.CodeExistsAsync(dto.Code))
            return Result<DepartementDto>.Failure("Un departement avec ce code existe deja.");

        var departement = new Departement
        {
            Nom = dto.Nom,
            Description = dto.Description,
            Code = dto.Code
        };

        await _repository.AddAsync(departement);
        return Result<DepartementDto>.Success(MapToDto(departement), "Departement cree avec succes.");
    }

    public async Task<Result<DepartementDto>> UpdateAsync(int id, UpdateDepartementDto dto)
    {
        var departement = await _repository.GetByIdAsync(id);
        if (departement == null)
            return Result<DepartementDto>.Failure($"Departement avec l'id {id} introuvable.");

        if (!string.IsNullOrEmpty(dto.Code) && await _repository.CodeExistsAsync(dto.Code, id))
            return Result<DepartementDto>.Failure("Un departement avec ce code existe deja.");

        departement.Nom = dto.Nom;
        departement.Description = dto.Description;
        departement.Code = dto.Code;
        departement.IsActive = dto.IsActive;
        departement.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(departement);
        return Result<DepartementDto>.Success(MapToDto(departement), "Departement mis a jour.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var departement = await _repository.GetByIdWithServicesAsync(id);
        if (departement == null)
            return Result.Failure($"Departement avec l'id {id} introuvable.");

        if (departement.Services.Any())
            return Result.Failure("Impossible de supprimer: le departement contient des services.");

        await _repository.DeleteAsync(departement);
        return Result.Success("Departement supprime.");
    }

    private static DepartementDto MapToDto(Departement d) => new()
    {
        Id = d.Id,
        Nom = d.Nom,
        Description = d.Description,
        Code = d.Code,
        IsActive = d.IsActive,
        NombreServices = d.Services?.Count ?? 0,
        CreatedAt = d.CreatedAt
    };
}
