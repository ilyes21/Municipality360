// ═══════════════════════════════════════════════════════════════════
//  NewRepositories.cs
//  Infrastructure/Repositories/NewRepositories.cs
//
//  يحتوي على جميع implementations للوحدات الجديدة:
//  SequenceRepository · Bureau d'Ordre · Réclamations · Permis · Notifications
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;
using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;

namespace Municipality360.Infrastructure.Repositories;
// ════════════════════════════════════════════════════════════════
//  Departement
// ════════════════════════════════════════════════════════════════
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
//
// ════════════════════════════════════════════════════════════════
//  Service
// ════════════════════════════════════════════════════════════════
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

// ════════════════════════════════════════════════════════════════
//  Employee
// ════════════════════════════════════════════════════════════════
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

    // ✅ FIXED: كانت تُلقي NotImplementedException
    public async Task<bool> CinExistsAsync(string cin, int? excludeId = null) =>
        await _dbSet.AnyAsync(e => e.Cin == cin && (excludeId == null || e.Id != excludeId));

    // ✅ FIXED: كانت تُلقي NotImplementedException
    public async Task<bool> IdentifiantExistsAsync(string identifiant, int? excludeId = null) =>
        await _dbSet.AnyAsync(e => e.Identifiant == identifiant && (excludeId == null || e.Id != excludeId));
}
// ════════════════════════════════════════════════════════════════
//  Poste
// ════════════════════════════════════════════════════════════════
public class PosteRepository : GenericRepository<Poste>, IPosteRepository
{
    public PosteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Poste>> GetActivePostesAsync() =>
        await _dbSet.Where(p => p.IsActive).OrderBy(p => p.Titre).ToListAsync();

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(p => p.Code == code && (excludeId == null || p.Id != excludeId));
}


// ════════════════════════════════════════════════════════════════
//  SEQUENCE — توليد الأرقام التسلسلية
// ════════════════════════════════════════════════════════════════

public class SequenceRepository : GenericRepository<NumeroSequence>, ISequenceRepository
{
    public SequenceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<string> GenererNumeroAsync(string prefixe)
    {
        var annee = DateTime.UtcNow.Year;

        var seq = await _dbSet.FirstOrDefaultAsync(s => s.Prefixe == prefixe && s.Annee == annee);
        if (seq == null)
        {
            seq = new NumeroSequence { Prefixe = prefixe, Annee = annee, DernierNumero = 0 };
            await _dbSet.AddAsync(seq);
        }

        seq.DernierNumero++;
        await _context.SaveChangesAsync();

        return $"{prefixe}-{annee}-{seq.DernierNumero:D5}";
    }
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — CONTACT
// ════════════════════════════════════════════════════════════════

public class BOContactRepository : GenericRepository<BOContact>, IBOContactRepository
{
    public BOContactRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<BOContact>> SearchAsync(string? term, bool activeOnly = true)
    {
        var query = _dbSet.AsQueryable();
        if (activeOnly) query = query.Where(c => c.IsActive);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.ToLower();
            query = query.Where(c =>
                (c.Nom != null && c.Nom.ToLower().Contains(t)) ||
                (c.Prenom != null && c.Prenom.ToLower().Contains(t)) ||
                (c.RaisonSociale != null && c.RaisonSociale.ToLower().Contains(t)) ||
                (c.Email != null && c.Email.ToLower().Contains(t)));
        }
        return await query.OrderBy(c => c.Nom ?? c.RaisonSociale).Take(50).ToListAsync();
    }
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — CATEGORIE COURRIER
// ════════════════════════════════════════════════════════════════

public class BOCategorieCourrierRepository : GenericRepository<BOCategorieCourrier>, IBOCategorieCourrierRepository
{
    public BOCategorieCourrierRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<BOCategorieCourrier>> GetAllActiveAsync() =>
        await _dbSet.Where(c => c.IsActive).OrderBy(c => c.Libelle).ToListAsync();

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(c => c.Code == code && (excludeId == null || c.Id != excludeId));
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — DOSSIER
// ════════════════════════════════════════════════════════════════

public class BODossierRepository : GenericRepository<BODossier>, IBODossierRepository
{
    public BODossierRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PagedResult<BODossierDto>> GetPagedAsync(int page, int size)
    {
        var query = _dbSet.Include(d => d.ServiceResponsable).AsQueryable();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<BODossierDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total, PageNumber = page, PageSize = size
        };
    }

    public async Task<BODossier?> GetByNumeroAsync(string numero) =>
        await _dbSet.Include(d => d.ServiceResponsable).FirstOrDefaultAsync(d => d.NumeroDossier == numero);

    private static BODossierDto MapToDto(BODossier d) => new()
    {
        Id = d.Id, NumeroDossier = d.NumeroDossier, Intitule = d.Intitule,
        Description = d.Description, ServiceResponsableId = d.ServiceResponsableId,
        ServiceResponsableNom = d.ServiceResponsable?.Nom, DateOuverture = d.DateOuverture,
        DateCloture = d.DateCloture, StatutDossier = d.StatutDossier.ToString(),
        NombreCourriersEntrants = d.CourriersEntrants?.Count ?? 0
    };
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER ENTRANT
// ════════════════════════════════════════════════════════════════

public class BOCourrierEntrantRepository : GenericRepository<BOCourrierEntrant>, IBOCourrierEntrantRepository
{
    public BOCourrierEntrantRepository(ApplicationDbContext context) : base(context) { }

    private IQueryable<BOCourrierEntrant> WithDetails() =>
        _dbSet
            .Include(c => c.Categorie)
            .Include(c => c.Dossier)
            .Include(c => c.ExpediteurContact)
            .Include(c => c.ServiceDestinataire)
            .Include(c => c.PiecesJointes)
            .Include(c => c.Circuit).ThenInclude(ct => ct.ServiceEmetteur)
            .Include(c => c.Circuit).ThenInclude(ct => ct.ServiceRecepteur)
            .Include(c => c.Reponses);

    public async Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter)
    {
        var q = _dbSet.AsQueryable();
        if (!string.IsNullOrEmpty(filter.Statut) && Enum.TryParse<StatutEntrant>(filter.Statut, out var st))
            q = q.Where(c => c.Statut == st);
        if (!string.IsNullOrEmpty(filter.Priorite) && Enum.TryParse<PrioriteCourrier>(filter.Priorite, out var pr))
            q = q.Where(c => c.Priorite == pr);
        if (filter.ServiceDestinataireId.HasValue)
            q = q.Where(c => c.ServiceDestinataireId == filter.ServiceDestinataireId);
        if (filter.DateDebut.HasValue)
            q = q.Where(c => c.DateReception >= filter.DateDebut.Value);
        if (filter.DateFin.HasValue)
            q = q.Where(c => c.DateReception <= filter.DateFin.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var t = filter.SearchTerm.ToLower();
            q = q.Where(c => c.ObjetCourrier.ToLower().Contains(t) || c.NumeroOrdre.ToLower().Contains(t));
        }

        var total = await q.CountAsync();
        var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var size = filter.PageSize < 1 ? 20 : filter.PageSize;
        var items = await q.OrderByDescending(c => c.DateReception)
            .Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<CourrierEntrantDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = total, PageNumber = page, PageSize = size
        };
    }

    public async Task<BOCourrierEntrant?> GetByIdWithDetailsAsync(int id) =>
        await WithDetails().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<BOCourrierEntrant?> GetByNumeroAsync(string numero) =>
        await WithDetails().FirstOrDefaultAsync(c => c.NumeroOrdre == numero);

    public async Task<List<CourrierEntrantDto>> GetEnRetardAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(c => c.NecessiteReponse && c.DelaiReponse.HasValue
                && c.DelaiReponse.Value.Date < today
                && c.Statut != StatutEntrant.Traite && c.Statut != StatutEntrant.Archive)
            .OrderBy(c => c.DelaiReponse)
            .Select(c => MapToListDto(c))
            .ToListAsync();
    }

    public async Task<List<CourrierEntrantDto>> GetNonTraitesParServiceAsync(int serviceId) =>
        await _dbSet
            .Where(c => c.ServiceDestinataireId == serviceId
                && c.Statut != StatutEntrant.Traite && c.Statut != StatutEntrant.Archive)
            .OrderBy(c => c.Priorite).ThenBy(c => c.DateReception)
            .Select(c => MapToListDto(c))
            .ToListAsync();

    public async Task<BOStatsDto> GetStatsAsync(int? serviceId = null)
    {
        var q = _dbSet.AsQueryable();
        if (serviceId.HasValue) q = q.Where(c => c.ServiceDestinataireId == serviceId);
        var today = DateTime.UtcNow.Date;

        return new BOStatsDto
        {
            TotalEntrants      = await q.CountAsync(),
            Enregistres        = await q.CountAsync(c => c.Statut == StatutEntrant.Enregistre),
            EnCours            = await q.CountAsync(c => c.Statut == StatutEntrant.EnCours),
            Traites            = await q.CountAsync(c => c.Statut == StatutEntrant.Traite),
            Archives           = await q.CountAsync(c => c.Statut == StatutEntrant.Archive),
            EnRetard           = await q.CountAsync(c => c.NecessiteReponse
                && c.DelaiReponse.HasValue && c.DelaiReponse.Value.Date < today
                && c.Statut != StatutEntrant.Traite && c.Statut != StatutEntrant.Archive),
            Urgents            = await q.CountAsync(c => c.Priorite == PrioriteCourrier.Urgent
                || c.Priorite == PrioriteCourrier.TresUrgent),
        };
    }

    private static CourrierEntrantDto MapToListDto(BOCourrierEntrant c) => new()
    {
        Id = c.Id, NumeroOrdre = c.NumeroOrdre, NumeroExterne = c.NumeroExterne,
        DateReception = c.DateReception, DateCourrier = c.DateCourrier,
        ObjetCourrier = c.ObjetCourrier, TypeDocument = c.TypeDocument.ToString(),
        ExpediteurNom = c.ExpediteurContact?.NomComplet ?? c.ExpediteurLibreNom ?? string.Empty,
        ModeReception = c.ModeReception.ToString(), Priorite = c.Priorite.ToString(),
        EstConfidentiel = c.EstConfidentiel, Statut = c.Statut.ToString(),
        DelaiReponse = c.DelaiReponse, NecessiteReponse = c.NecessiteReponse,
        ServiceDestinataireId = c.ServiceDestinataireId,
        ServiceDestinataireNom = c.ServiceDestinataire?.Nom,
        NombrePiecesJointes = c.PiecesJointes?.Count ?? 0,
        NombreEtapesCircuit = c.Circuit?.Count ?? 0,
        CreatedAt = c.CreatedAt
    };
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — CIRCUIT TRAITEMENT
// ════════════════════════════════════════════════════════════════

public class BOCircuitTraitementRepository : GenericRepository<BOCircuitTraitement>, IBOCircuitTraitementRepository
{
    public BOCircuitTraitementRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<BOCircuitTraitementDto>> GetByCourrierAsync(int courrierEntrantId)
    {
        var list = await _dbSet
            .Include(c => c.ServiceEmetteur)
            .Include(c => c.ServiceRecepteur)
            .Where(c => c.CourrierEntrantId == courrierEntrantId)
            .OrderBy(c => c.NumeroEtape)
            .ToListAsync();

        return list.Select(c => new BOCircuitTraitementDto
        {
            Id = c.Id, NumeroEtape = c.NumeroEtape,
            ServiceEmetteurId = c.ServiceEmetteurId, ServiceEmetteurNom = c.ServiceEmetteur?.Nom,
            ServiceRecepteurId = c.ServiceRecepteurId,
            ServiceRecepteurNom = c.ServiceRecepteur?.Nom ?? string.Empty,
            DateTransmission = c.DateTransmission, DateTraitement = c.DateTraitement,
            DelaiTraitement = c.DelaiTraitement, TypeAction = c.TypeAction.ToString(),
            InstructionTransmission = c.InstructionTransmission,
            CommentaireTraitement = c.CommentaireTraitement,
            ActionEffectuee = c.ActionEffectuee, StatutEtape = c.StatutEtape.ToString(),
            EstRetour = c.EstRetour, MotifRetour = c.MotifRetour
        }).ToList();
    }

    public async Task<BOCircuitTraitement?> GetEtapeActiveAsync(int courrierEntrantId) =>
        await _dbSet
            .Where(c => c.CourrierEntrantId == courrierEntrantId
                && (c.StatutEtape == StatutEtapeCircuit.EnAttente || c.StatutEtape == StatutEtapeCircuit.EnCours))
            .OrderByDescending(c => c.NumeroEtape)
            .FirstOrDefaultAsync();

    public async Task<short> GetProchaineNumeroEtapeAsync(int courrierEntrantId)
    {
        var max = await _dbSet
            .Where(c => c.CourrierEntrantId == courrierEntrantId)
            .MaxAsync(c => (short?)c.NumeroEtape) ?? 0;
        return (short)(max + 1);
    }
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER SORTANT
// ════════════════════════════════════════════════════════════════

public class BOCourrierSortantRepository : GenericRepository<BOCourrierSortant>, IBOCourrierSortantRepository
{
    public BOCourrierSortantRepository(ApplicationDbContext context) : base(context) { }

    private IQueryable<BOCourrierSortant> WithDetails() =>
        _dbSet
            .Include(c => c.Categorie)
            .Include(c => c.Dossier)
            .Include(c => c.ServiceEmetteur)
            .Include(c => c.DestinataireContact)
            .Include(c => c.PiecesJointes)
            .Include(c => c.CourrierEntrantRef);

    public async Task<PagedResult<CourrierSortantDto>> GetPagedAsync(CourrierSortantFilterDto filter)
    {
        var q = _dbSet.AsQueryable();
        if (!string.IsNullOrEmpty(filter.Statut) && Enum.TryParse<StatutSortant>(filter.Statut, out var st))
            q = q.Where(c => c.Statut == st);
        if (filter.ServiceEmetteurId.HasValue)
            q = q.Where(c => c.ServiceEmetteurId == filter.ServiceEmetteurId);
        if (filter.DateDebut.HasValue) q = q.Where(c => c.DateRedaction >= filter.DateDebut.Value);
        if (filter.DateFin.HasValue) q = q.Where(c => c.DateRedaction <= filter.DateFin.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var t = filter.SearchTerm.ToLower();
            q = q.Where(c => c.ObjetCourrier.ToLower().Contains(t) || c.NumeroOrdre.ToLower().Contains(t));
        }

        var total = await q.CountAsync();
        var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var size = filter.PageSize < 1 ? 20 : filter.PageSize;
        var items = await q.OrderByDescending(c => c.DateRedaction)
            .Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<CourrierSortantDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = total, PageNumber = page, PageSize = size
        };
    }

    public async Task<BOCourrierSortant?> GetByIdWithDetailsAsync(int id) =>
        await WithDetails().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<BOCourrierSortant?> GetByNumeroAsync(string numero) =>
        await WithDetails().FirstOrDefaultAsync(c => c.NumeroOrdre == numero);

    public async Task<List<CourrierSortantDto>> GetBrouillonsAsync(string redacteurId) =>
        await _dbSet.Where(c => c.RedacteurId == redacteurId && c.Statut == StatutSortant.Brouillon)
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => MapToListDto(c)).ToListAsync();

    private static CourrierSortantDto MapToListDto(BOCourrierSortant c) => new()
    {
        Id = c.Id, NumeroOrdre = c.NumeroOrdre, ObjetCourrier = c.ObjetCourrier,
        TypeDocument = c.TypeDocument.ToString(), Statut = c.Statut.ToString(),
        Priorite = c.Priorite.ToString(), EstSigne = c.EstSigne,
        EstConfidentiel = c.EstConfidentiel, DateRedaction = c.DateRedaction,
        DateEnvoi = c.DateEnvoi, ServiceEmetteurId = c.ServiceEmetteurId,
        DestinataireNom = c.DestinataireContact?.NomComplet ?? c.DestinataireLibreNom,
        NombrePiecesJointes = c.PiecesJointes?.Count ?? 0, CreatedAt = c.CreatedAt
    };
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — ARCHIVE
// ════════════════════════════════════════════════════════════════

public class BOArchiveRepository : GenericRepository<BOArchive>, IBOArchiveRepository
{
    public BOArchiveRepository(ApplicationDbContext context) : base(context) { }

    public async Task<BOArchiveDto?> GetByCourrierEntrantIdAsync(int id)
    {
        var a = await _dbSet.FirstOrDefaultAsync(x => x.CourrierEntrantId == id);
        return a == null ? null : MapToDto(a);
    }

    public async Task<BOArchiveDto?> GetByCourrierSortantIdAsync(int id)
    {
        var a = await _dbSet.FirstOrDefaultAsync(x => x.CourrierSortantId == id);
        return a == null ? null : MapToDto(a);
    }

    private static BOArchiveDto MapToDto(BOArchive a) => new()
    {
        Id = a.Id, NumeroArchive = a.NumeroArchive, CodeBarre = a.CodeBarre,
        SalleArchive = a.SalleArchive, Rayon = a.Rayon, Boite = a.Boite,
        Classification = a.Classification.ToString(),
        DureeConservationAns = a.DureeConservationAns,
        DateDebutConservation = a.DateDebutConservation,
        DateFinConservation = a.DateFinConservation,
        CheminArchiveNumerique = a.CheminArchiveNumerique,
        EstDetruit = a.EstDetruit, Observation = a.Observation,
        DateArchivage = a.DateArchivage
    };
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS — CITOYEN
// ════════════════════════════════════════════════════════════════

public class CitoyenRepository : GenericRepository<Citoyen>, ICitoyenRepository
{
    public CitoyenRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PagedResult<CitoyenDto>> GetPagedAsync(CitoyenFilterDto filter)
    {
        var q = _dbSet.AsQueryable();
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var t = filter.SearchTerm.ToLower();
            q = q.Where(c => c.Nom.ToLower().Contains(t) || c.Prenom.ToLower().Contains(t)
                || c.CIN.ToLower().Contains(t) || (c.Telephone != null && c.Telephone.Contains(t)));
        }
        if (filter.IsActive.HasValue) q = q.Where(c => c.IsActive == filter.IsActive.Value);

        var total = await q.CountAsync();
        var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var size = filter.PageSize < 1 ? 20 : filter.PageSize;
        var items = await q.OrderBy(c => c.Nom).ThenBy(c => c.Prenom)
            .Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<CitoyenDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total, PageNumber = page, PageSize = size
        };
    }

    public async Task<Citoyen?> GetByCINAsync(string cin) =>
        await _dbSet.Include(c => c.Reclamations).FirstOrDefaultAsync(c => c.CIN == cin);

    public async Task<bool> CINExistsAsync(string cin, int? excludeId = null) =>
        await _dbSet.AnyAsync(c => c.CIN == cin && (excludeId == null || c.Id != excludeId));

    private static CitoyenDto MapToDto(Citoyen c) => new()
    {
        Id = c.Id, CIN = c.CIN, Nom = c.Nom, Prenom = c.Prenom, NomComplet = c.NomComplet,
        DateNaissance = c.DateNaissance, Sexe = c.Sexe?.ToString(),
        Adresse = c.Adresse, Ville = c.Ville, Telephone = c.Telephone,
        TelephoneMobile = c.TelephoneMobile, Email = c.Email,
        SituationFamiliale = c.SituationFamiliale?.ToString(), IsActive = c.IsActive,
        NombreReclamations = c.Reclamations?.Count ?? 0
    };
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS — TYPES & CATÉGORIES
// ════════════════════════════════════════════════════════════════

public class TypeReclamationRepository : GenericRepository<TypeReclamation>, ITypeReclamationRepository
{
    public TypeReclamationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<TypeReclamation>> GetAllActiveAsync() =>
        await _dbSet.Include(t => t.ServiceResponsable)
            .Where(t => t.IsActive).OrderBy(t => t.Libelle).ToListAsync();

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(t => t.Code == code && (excludeId == null || t.Id != excludeId));
}

public class CategorieReclamationRepository : GenericRepository<CategorieReclamation>, ICategorieReclamationRepository
{
    public CategorieReclamationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<CategorieReclamation>> GetHierarchieAsync() =>
        await _dbSet.Include(c => c.SousCategories)
            .Where(c => c.ParentId == null && c.IsActive).OrderBy(c => c.Libelle).ToListAsync();

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
        await _dbSet.AnyAsync(c => c.Code == code && (excludeId == null || c.Id != excludeId));
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS — RECLAMATION
// ════════════════════════════════════════════════════════════════

public class ReclamationRepository : GenericRepository<Reclamation>, IReclamationRepository
{
    public ReclamationRepository(ApplicationDbContext context) : base(context) { }

    private IQueryable<Reclamation> WithDetails() =>
        _dbSet
            .Include(r => r.TypeReclamation)
            .Include(r => r.Categorie)
            .Include(r => r.Citoyen)
            .Include(r => r.ServiceConcerne)
            .Include(r => r.Suivis)
            .Include(r => r.PiecesJointes);

    public async Task<PagedResult<ReclamationDto>> GetPagedAsync(ReclamationFilterDto filter)
    {
        var q = _dbSet.Include(r => r.TypeReclamation).Include(r => r.Citoyen)
            .Include(r => r.ServiceConcerne).AsQueryable();

        if (!string.IsNullOrEmpty(filter.Statut) && Enum.TryParse<StatutReclamation>(filter.Statut, out var st))
            q = q.Where(r => r.Statut == st);
        if (!string.IsNullOrEmpty(filter.Priorite) && Enum.TryParse<PrioriteReclamation>(filter.Priorite, out var pr))
            q = q.Where(r => r.Priorite == pr);
        if (filter.ServiceConcerneId.HasValue) q = q.Where(r => r.ServiceConcerneId == filter.ServiceConcerneId);
        if (filter.TypeReclamationId.HasValue) q = q.Where(r => r.TypeReclamationId == filter.TypeReclamationId);
        if (filter.DateDebut.HasValue) q = q.Where(r => r.DateDepot >= filter.DateDebut.Value);
        if (filter.DateFin.HasValue) q = q.Where(r => r.DateDepot <= filter.DateFin.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var t = filter.SearchTerm.ToLower();
            q = q.Where(r => r.Objet.ToLower().Contains(t) || r.NumeroReclamation.ToLower().Contains(t));
        }

        var total = await q.CountAsync();
        var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var size = filter.PageSize < 1 ? 20 : filter.PageSize;
        var items = await q.OrderByDescending(r => r.DateDepot)
            .Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<ReclamationDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = total, PageNumber = page, PageSize = size
        };
    }

    public async Task<Reclamation?> GetByIdWithDetailsAsync(int id) =>
        await WithDetails().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Reclamation?> GetByNumeroAsync(string numero) =>
        await WithDetails().FirstOrDefaultAsync(r => r.NumeroReclamation == numero);

    public async Task<List<ReclamationDto>> GetByCitoyenAsync(int citoyenId) =>
        await _dbSet.Where(r => r.CitoyenId == citoyenId)
            .OrderByDescending(r => r.DateDepot)
            .Select(r => MapToListDto(r)).ToListAsync();

    public async Task<List<ReclamationDto>> GetEnRetardAsync()
    {
        var limites = await _dbSet
            .Include(r => r.TypeReclamation)
            .Where(r => r.Statut != StatutReclamation.Traitee
                && r.Statut != StatutReclamation.Rejetee
                && r.Statut != StatutReclamation.Fermee)
            .ToListAsync();

        var today = DateTime.UtcNow;
        return limites
            .Where(r => r.TypeReclamation != null
                && r.DateDepot.AddDays(r.TypeReclamation.DelaiTraitementJours) < today)
            .Select(MapToListDto).ToList();
    }

    public async Task<ReclamationStatsDto> GetStatsAsync(int? serviceId = null)
    {
        var q = _dbSet.AsQueryable();
        if (serviceId.HasValue) q = q.Where(r => r.ServiceConcerneId == serviceId);

        return new ReclamationStatsDto
        {
            Total      = await q.CountAsync(),
            Nouvelles  = await q.CountAsync(r => r.Statut == StatutReclamation.Nouvelle),
            EnCours    = await q.CountAsync(r => r.Statut == StatutReclamation.EnCours),
            Traitees   = await q.CountAsync(r => r.Statut == StatutReclamation.Traitee),
            Rejetees   = await q.CountAsync(r => r.Statut == StatutReclamation.Rejetee),
            Critiques  = await q.CountAsync(r => r.Priorite == PrioriteReclamation.Critique),
            SatisfactionMoyenne = await q.Where(r => r.SatisfactionCitoyen.HasValue)
                .AverageAsync(r => (double?)r.SatisfactionCitoyen) ?? 0
        };
    }

    private static ReclamationDto MapToListDto(Reclamation r) => new()
    {
        Id = r.Id, NumeroReclamation = r.NumeroReclamation, DateDepot = r.DateDepot,
        TypeReclamationLibelle = r.TypeReclamation?.Libelle ?? string.Empty,
        CategorieLibelle = r.Categorie?.Libelle ?? string.Empty,
        Priorite = r.Priorite.ToString(), Statut = r.Statut.ToString(),
        CitoyenNomComplet = r.EstAnonyme ? "Anonyme" : r.Citoyen?.NomComplet ?? string.Empty,
        Canal = r.Canal.ToString(), Objet = r.Objet,
        ServiceConcerneNom = r.ServiceConcerne?.Nom, DateCloture = r.DateCloture,
        CreatedAt = r.CreatedAt
    };
}

// ════════════════════════════════════════════════════════════════
//  PERMIS DE BÂTIR — DEMANDEUR & ARCHITECTE
// ════════════════════════════════════════════════════════════════

public class DemandeurRepository : GenericRepository<Demandeur>, IDemandeurRepository
{
    public DemandeurRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<Demandeur>> SearchAsync(string? term)
    {
        var q = _dbSet.Where(d => d.IsActive);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.ToLower();
            q = q.Where(d => d.Nom.ToLower().Contains(t)
                || (d.Prenom != null && d.Prenom.ToLower().Contains(t))
                || (d.CIN != null && d.CIN.ToLower().Contains(t))
                || (d.RaisonSociale != null && d.RaisonSociale.ToLower().Contains(t)));
        }
        return await q.OrderBy(d => d.Nom).Take(50).ToListAsync();
    }
}

public class ArchitecteRepository : GenericRepository<Architecte>, IArchitecteRepository
{
    public ArchitecteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Architecte?> GetByNumeroOrdreAsync(string numero) =>
        await _dbSet.FirstOrDefaultAsync(a => a.NumeroOrdre == numero);

    public async Task<bool> NumeroOrdreExistsAsync(string numero, int? excludeId = null) =>
        await _dbSet.AnyAsync(a => a.NumeroOrdre == numero && (excludeId == null || a.Id != excludeId));
}

public class CommissionExamenRepository : GenericRepository<CommissionExamen>, ICommissionExamenRepository
{
    public CommissionExamenRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<CommissionExamen>> GetActiveAsync() =>
        await _dbSet
            .Where(c => c.StatutReunion == StatutCommission.Programmee || c.StatutReunion == StatutCommission.Tenue)
            .OrderByDescending(c => c.DateReunion).ToListAsync();
}

// ════════════════════════════════════════════════════════════════
//  PERMIS DE BÂTIR — DEMANDE PERMIS
// ════════════════════════════════════════════════════════════════

public class DemandePermisBatirRepository : GenericRepository<DemandePermisBatir>, IDemandePermisBatirRepository
{
    public DemandePermisBatirRepository(ApplicationDbContext context) : base(context) { }

    private IQueryable<DemandePermisBatir> WithDetails() =>
        _dbSet
            .Include(d => d.TypeDemande)
            .Include(d => d.Demandeur)
            .Include(d => d.Architecte)
            .Include(d => d.Zonage)
            .Include(d => d.CommissionExamen)
            .Include(d => d.ServiceInstructeur)
            .Include(d => d.PermisDelivre)
            .Include(d => d.Documents)
            .Include(d => d.Taxes).ThenInclude(t => t.TypeTaxe)
            .Include(d => d.Suivis)
            .Include(d => d.Inspections);

    public async Task<PagedResult<DemandePermisDto>> GetPagedAsync(DemandePermisFilterDto filter)
    {
        var q = _dbSet.Include(d => d.TypeDemande).Include(d => d.Demandeur).AsQueryable();

        if (!string.IsNullOrEmpty(filter.Statut) && Enum.TryParse<StatutDemande>(filter.Statut, out var st))
            q = q.Where(d => d.Statut == st);
        if (filter.ServiceInstructeurId.HasValue) q = q.Where(d => d.ServiceInstructeurId == filter.ServiceInstructeurId);
        if (filter.TypeDemandeId.HasValue) q = q.Where(d => d.TypeDemandeId == filter.TypeDemandeId);
        if (filter.DateDebut.HasValue) q = q.Where(d => d.DateDepot >= filter.DateDebut.Value);
        if (filter.DateFin.HasValue) q = q.Where(d => d.DateDepot <= filter.DateFin.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var t = filter.SearchTerm.ToLower();
            q = q.Where(d => d.NumeroDemande.ToLower().Contains(t)
                || d.AdresseProjet.ToLower().Contains(t));
        }

        var total = await q.CountAsync();
        var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var size = filter.PageSize < 1 ? 20 : filter.PageSize;
        var items = await q.OrderByDescending(d => d.DateDepot)
            .Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<DemandePermisDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = total, PageNumber = page, PageSize = size
        };
    }

    public async Task<DemandePermisBatir?> GetByIdWithDetailsAsync(int id) =>
        await WithDetails().FirstOrDefaultAsync(d => d.Id == id);

    public async Task<DemandePermisBatir?> GetByNumeroAsync(string numero) =>
        await WithDetails().FirstOrDefaultAsync(d => d.NumeroDemande == numero);

    public async Task<List<DemandePermisDto>> GetByDemandeurAsync(int demandeurId) =>
        await _dbSet.Include(d => d.TypeDemande)
            .Where(d => d.DemandeurId == demandeurId)
            .OrderByDescending(d => d.DateDepot)
            .Select(d => MapToListDto(d)).ToListAsync();

    public async Task<List<DemandePermisDto>> GetEnRetardAsync()
    {
        var list = await _dbSet.Include(d => d.TypeDemande)
            .Where(d => d.Statut != StatutDemande.Approuvee
                && d.Statut != StatutDemande.Rejetee)
            .ToListAsync();

        var today = DateTime.UtcNow;
        return list
            .Where(d => d.TypeDemande != null
                && d.DateDepot.AddDays(d.TypeDemande.DelaiTraitementJours) < today)
            .Select(MapToListDto).ToList();
    }

    public async Task<PermisStatsDto> GetStatsAsync(int? serviceId = null)
    {
        var q = _dbSet.AsQueryable();
        if (serviceId.HasValue) q = q.Where(d => d.ServiceInstructeurId == serviceId);

        return new PermisStatsDto
        {
            TotalDemandes     = await q.CountAsync(),
            Deposees          = await q.CountAsync(d => d.Statut == StatutDemande.Deposee),
            EnExamen          = await q.CountAsync(d => d.Statut == StatutDemande.EnExamen),
            Approuvees        = await q.CountAsync(d => d.Statut == StatutDemande.Approuvee),
            Rejetees          = await q.CountAsync(d => d.Statut == StatutDemande.Rejetee),
            TotalPermisDelivres = await _context.Set<PermisDelivre>().CountAsync()
        };
    }

    private static DemandePermisDto MapToListDto(DemandePermisBatir d) => new()
    {
        Id = d.Id, NumeroDemande = d.NumeroDemande, DateDepot = d.DateDepot,
        TypeDemandeLibelle = d.TypeDemande?.Libelle ?? string.Empty,
        Statut = d.Statut.ToString(), DemandeurNomComplet = d.Demandeur?.NomComplet ?? string.Empty,
        AdresseProjet = d.AdresseProjet, TypeConstruction = d.TypeConstruction?.ToString(),
        ServiceInstructeurId = d.ServiceInstructeurId, DateDecision = d.DateDecision,
        NumeroPermis = d.PermisDelivre?.NumeroPermis, CreatedAt = d.CreatedAt
    };
}

// ════════════════════════════════════════════════════════════════
//  NOTIFICATIONS
// ════════════════════════════════════════════════════════════════

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<NotificationDto>> GetByAgentAsync(string agentId, bool seulementNonLues = false)
    {
        var q = _dbSet.Where(n => n.DestinataireAgentId == agentId);
        if (seulementNonLues) q = q.Where(n => !n.EstLue);
        return await q.OrderByDescending(n => n.DateCreation)
            .Take(100)
            .Select(n => MapToDto(n)).ToListAsync();
    }

    public async Task<List<NotificationDto>> GetByCitoyenAsync(string citoyenId, bool seulementNonLues = false)
    {
        var q = _dbSet.Where(n => n.DestinataireCitoyenId == citoyenId);
        if (seulementNonLues) q = q.Where(n => !n.EstLue);
        return await q.OrderByDescending(n => n.DateCreation)
            .Take(100)
            .Select(n => MapToDto(n)).ToListAsync();
    }

    public async Task<int> GetNombreNonLuesAsync(string destinataireId, bool estAgent = true) =>
        estAgent
            ? await _dbSet.CountAsync(n => n.DestinataireAgentId == destinataireId && !n.EstLue)
            : await _dbSet.CountAsync(n => n.DestinataireCitoyenId == destinataireId && !n.EstLue);

    public async Task MarquerLueAsync(int notificationId)
    {
        var n = await _dbSet.FindAsync(notificationId);
        if (n == null) return;
        n.EstLue = true;
        n.DateLecture = DateTime.UtcNow;
        n.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task MarquerToutesLuesAsync(string destinataireId, bool estAgent = true)
    {
        var notifs = estAgent
            ? await _dbSet.Where(n => n.DestinataireAgentId == destinataireId && !n.EstLue).ToListAsync()
            : await _dbSet.Where(n => n.DestinataireCitoyenId == destinataireId && !n.EstLue).ToListAsync();

        foreach (var n in notifs) { n.EstLue = true; n.DateLecture = DateTime.UtcNow; }
        await _context.SaveChangesAsync();
    }

    private static NotificationDto MapToDto(Notification n) => new()
    {
        Id = n.Id, Type = n.Type.ToString(), Titre = n.Titre, Message = n.Message,
        EntiteId = n.EntiteId, EntiteType = n.EntiteType, LienNavigation = n.LienNavigation,
        DestinataireAgentId = n.DestinataireAgentId, DestinataireCitoyenId = n.DestinataireCitoyenId,
        EstLue = n.EstLue, DateLecture = n.DateLecture, DateCreation = n.DateCreation
    };
}
