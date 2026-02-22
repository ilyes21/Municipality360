using Microsoft.EntityFrameworkCore;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;

namespace Municipality360.Infrastructure.Repositories;

public class DepartementRepository : GenericRepository<Departement>, IDepartementRepository
{
    public DepartementRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Departement>> GetAllWithServicesAsync() =>
        await _dbSet.Include(d => d.Services).OrderBy(d => d.Nom).ToListAsync();

    public async Task<Departement?> GetByIdWithServicesAsync(int id) =>
        await _dbSet.Include(d => d.Services).FirstOrDefaultAsync(d => d.Id == id);

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(d => d.Code == code && (excludeId == null || d.Id != excludeId));
}
