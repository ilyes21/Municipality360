using Microsoft.EntityFrameworkCore;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;

namespace Municipality360.Infrastructure.Repositories;

public class PosteRepository : GenericRepository<Poste>, IPosteRepository
{
    public PosteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Poste>> GetActivePostesAsync() =>
        await _dbSet.Where(p => p.IsActive).OrderBy(p => p.Titre).ToListAsync();

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(p => p.Code == code && (excludeId == null || p.Id != excludeId));
}
