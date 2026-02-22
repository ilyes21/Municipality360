using Microsoft.EntityFrameworkCore;
using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;

namespace Municipality360.Infrastructure.Repositories;

public class EmployeRepository : GenericRepository<Employe>, IEmployeRepository
{
    public EmployeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PagedResult<Employe>> GetPagedAsync(EmployeFilterDto filter)
    {
        var query = _dbSet
            .Include(e => e.Service).ThenInclude(s => s.Departement)
            .Include(e => e.Poste)
            .AsQueryable();

        if (filter.ServiceId.HasValue)
            query = query.Where(e => e.ServiceId == filter.ServiceId);

        if (filter.DepartementId.HasValue)
            query = query.Where(e => e.Service.DepartementId == filter.DepartementId);

        if (filter.PosteId.HasValue)
            query = query.Where(e => e.PosteId == filter.PosteId);

        if (!string.IsNullOrEmpty(filter.Statut) && Enum.TryParse<StatutEmploye>(filter.Statut, true, out var statut))
            query = query.Where(e => e.Statut == statut);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(e =>
                e.Nom.ToLower().Contains(term) ||
                e.Prenom.ToLower().Contains(term) ||
                e.Identifiant.ToLower().Contains(term) ||
                e.Cin.ToLower().Contains(term) ||
                (e.Email != null && e.Email.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.Nom).ThenBy(e => e.Prenom)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Employe>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<Employe?> GetByIdWithDetailsAsync(int id) =>
        await _dbSet
            .Include(e => e.Service).ThenInclude(s => s.Departement)
            .Include(e => e.Poste)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<Employe>> GetByServiceAsync(int serviceId) =>
        await _dbSet.Include(e => e.Poste)
            .Where(e => e.ServiceId == serviceId)
            .OrderBy(e => e.Nom).ToListAsync();

    public async Task<bool> MatriculeExistsAsync(string Identifiant, int? excludeId = null) =>
        await _dbSet.AnyAsync(e => e.Identifiant == Identifiant && (excludeId == null || e.Id != excludeId));

    public Task<bool> CinExistsAsync(string Cin, int? excludeId = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IdentifiantExistsAsync(string Identifiant, int? excludeId = null)
    {
        throw new NotImplementedException();
    }
}
