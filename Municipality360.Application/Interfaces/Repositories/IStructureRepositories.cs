using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Interfaces.Repositories;

public interface IDepartementRepository : IGenericRepository<Departement>
{
    Task<IEnumerable<Departement>> GetAllWithServicesAsync();
    Task<Departement?> GetByIdWithServicesAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IServiceRepository : IGenericRepository<Service>
{
    Task<IEnumerable<Service>> GetByDepartementAsync(int departementId);
    Task<Service?> GetByIdWithDetailsAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IPosteRepository : IGenericRepository<Poste>
{
    Task<IEnumerable<Poste>> GetActivePostesAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

public interface IEmployeRepository : IGenericRepository<Employe>
{
    Task<PagedResult<Employe>> GetPagedAsync(EmployeFilterDto filter);
    Task<Employe?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Employe>> GetByServiceAsync(int serviceId);
    Task<bool> CinExistsAsync(string Cin, int? excludeId = null);
    Task<bool> IdentifiantExistsAsync(string Identifiant, int? excludeId = null);
}
