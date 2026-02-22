using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

public class PosteService : IPosteService
{
    private readonly IPosteRepository _repository;

    public PosteService(IPosteRepository repository) => _repository = repository;

    public async Task<Result<IEnumerable<PosteDto>>> GetAllAsync()
    {
        var postes = await _repository.GetActivePostesAsync();
        return Result<IEnumerable<PosteDto>>.Success(postes.Select(MapToDto));
    }

    public async Task<Result<PosteDto>> GetByIdAsync(int id)
    {
        var poste = await _repository.GetByIdAsync(id);
        if (poste == null) return Result<PosteDto>.Failure($"Poste {id} introuvable.");
        return Result<PosteDto>.Success(MapToDto(poste));
    }

    public async Task<Result<PosteDto>> CreateAsync(CreatePosteDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Code) && await _repository.CodeExistsAsync(dto.Code))
            return Result<PosteDto>.Failure("Code deja utilise.");

        var poste = new Poste
        {
            Titre = dto.Titre,
            Description = dto.Description,
            Code = dto.Code,
            SalaireMin = dto.SalaireMin,
            SalaireMax = dto.SalaireMax
        };

        await _repository.AddAsync(poste);
        return Result<PosteDto>.Success(MapToDto(poste), "Poste cree.");
    }

    public async Task<Result<PosteDto>> UpdateAsync(int id, UpdatePosteDto dto)
    {
        var poste = await _repository.GetByIdAsync(id);
        if (poste == null) return Result<PosteDto>.Failure($"Poste {id} introuvable.");

        poste.Titre = dto.Titre;
        poste.Description = dto.Description;
        poste.Code = dto.Code;
        poste.SalaireMin = dto.SalaireMin;
        poste.SalaireMax = dto.SalaireMax;
        poste.IsActive = dto.IsActive;
        poste.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(poste);
        return Result<PosteDto>.Success(MapToDto(poste), "Poste mis a jour.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var poste = await _repository.GetByIdAsync(id);
        if (poste == null) return Result.Failure($"Poste {id} introuvable.");
        await _repository.DeleteAsync(poste);
        return Result.Success("Poste supprime.");
    }

    private static PosteDto MapToDto(Poste p) => new()
    {
        Id = p.Id,
        Titre = p.Titre,
        Description = p.Description,
        Code = p.Code,
        SalaireMin = p.SalaireMin,
        SalaireMax = p.SalaireMax,
        IsActive = p.IsActive,
        NombreEmployes = p.Employes?.Count ?? 0,
        CreatedAt = p.CreatedAt
    };
}
