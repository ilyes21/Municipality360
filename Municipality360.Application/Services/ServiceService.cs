using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _repository;
    private readonly IDepartementRepository _deptRepository;

    public ServiceService(IServiceRepository repository, IDepartementRepository deptRepository)
    {
        _repository = repository;
        _deptRepository = deptRepository;
    }

    public async Task<Result<IEnumerable<ServiceDto>>> GetAllAsync()
    {
        var services = await _repository.GetByDepartementAsync(0);
        return Result<IEnumerable<ServiceDto>>.Success(services.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<ServiceDto>>> GetByDepartementAsync(int departementId)
    {
        var services = await _repository.GetByDepartementAsync(departementId);
        return Result<IEnumerable<ServiceDto>>.Success(services.Select(MapToDto));
    }

    public async Task<Result<ServiceDto>> GetByIdAsync(int id)
    {
        var service = await _repository.GetByIdWithDetailsAsync(id);
        if (service == null) return Result<ServiceDto>.Failure($"Service {id} introuvable.");
        return Result<ServiceDto>.Success(MapToDto(service));
    }

    public async Task<Result<ServiceDto>> CreateAsync(CreateServiceDto dto)
    {
        if (!await _deptRepository.ExistsAsync(dto.DepartementId))
            return Result<ServiceDto>.Failure("Departement introuvable.");

        if (!string.IsNullOrEmpty(dto.Code) && await _repository.CodeExistsAsync(dto.Code))
            return Result<ServiceDto>.Failure("Code deja utilise.");

        var service = new Domain.Entities.Service
        {
            Nom = dto.Nom,
            Description = dto.Description,
            Code = dto.Code,
            DepartementId = dto.DepartementId
        };

        await _repository.AddAsync(service);
        var created = await _repository.GetByIdWithDetailsAsync(service.Id);
        return Result<ServiceDto>.Success(MapToDto(created!), "Service cree.");
    }

    public async Task<Result<ServiceDto>> UpdateAsync(int id, UpdateServiceDto dto)
    {
        var service = await _repository.GetByIdAsync(id);
        if (service == null) return Result<ServiceDto>.Failure($"Service {id} introuvable.");

        service.Nom = dto.Nom;
        service.Description = dto.Description;
        service.Code = dto.Code;
        service.DepartementId = dto.DepartementId;
        service.IsActive = dto.IsActive;
        service.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(service);
        var updated = await _repository.GetByIdWithDetailsAsync(id);
        return Result<ServiceDto>.Success(MapToDto(updated!), "Service mis a jour.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var service = await _repository.GetByIdAsync(id);
        if (service == null) return Result.Failure($"Service {id} introuvable.");
        await _repository.DeleteAsync(service);
        return Result.Success("Service supprime.");
    }

    private static ServiceDto MapToDto(Domain.Entities.Service s) => new()
    {
        Id = s.Id,
        Nom = s.Nom,
        Description = s.Description,
        Code = s.Code,
        IsActive = s.IsActive,
        DepartementId = s.DepartementId,
        DepartementNom = s.Departement?.Nom ?? string.Empty,
        NombreEmployes = s.Employes?.Count ?? 0,
        CreatedAt = s.CreatedAt
    };
}
