// ═══════════════════════════════════════════════════════════════════
//  ServiceRepository.cs  ✅ FIXED
//  Infrastructure/Repositories/ServiceRepository.cs
//
//  الإصلاح: إضافة GetEmployesAsync — مطلوب بواسطة NotificationService
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;

namespace Municipality360.Infrastructure.Repositories;

public class ServiceRepository : GenericRepository<Domain.Entities.Service>, IServiceRepository
{
    public ServiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Domain.Entities.Service>> GetByDepartementAsync(int departementId)
    {
        var query = _dbSet.Include(s => s.Departement).Include(s => s.Employes);
        return departementId == 0
            ? await query.OrderBy(s => s.Nom).ToListAsync()
            : await query.Where(s => s.DepartementId == departementId).OrderBy(s => s.Nom).ToListAsync();
    }

    public async Task<Domain.Entities.Service?> GetByIdWithDetailsAsync(int id) =>
        await _dbSet.Include(s => s.Departement).Include(s => s.Employes)
                    .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(s => s.Code == code && (excludeId == null || s.Id != excludeId));

    // ✅ FIXED: مطلوب بواسطة NotificationService.NotifierServiceAsync
    public async Task<List<Employe>> GetEmployesAsync(int serviceId) =>
        await _context.Set<Employe>()
            .Where(e => e.ServiceId == serviceId && !e.IsDeleted)
            .ToListAsync();
}
