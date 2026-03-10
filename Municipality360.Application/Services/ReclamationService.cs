// Application/Services/ReclamationService.cs
// ✅ FIXED: إزالة using Municipality360.Domain.Entities.Reclamations — namespace غير موجود

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

public class ReclamationService : IReclamationService
{
    private readonly IReclamationRepository _repo;
    private readonly ISequenceRepository _seq;
    private readonly INotificationService _notif;

    public ReclamationService(
        IReclamationRepository repo,
        ISequenceRepository seq,
        INotificationService notif)
    {
        _repo = repo; _seq = seq; _notif = notif;
    }

    // ── Lectures ────────────────────────────────────────────────────

    public Task<PagedResult<ReclamationDto>> GetPagedAsync(ReclamationFilterDto filter)
        => _repo.GetPagedAsync(filter);

    public async Task<ReclamationDetailDto> GetByIdAsync(int id)
    {
        var r = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Réclamation #{id} introuvable.");
        return MapToDetail(r);
    }

    /// <summary>Vue publique pour Flutter — sans authentification</summary>
    public async Task<ReclamationPublicDto> GetByNumeroPublicAsync(string numero)
    {
        var r = await _repo.GetByNumeroAsync(numero)
            ?? throw new KeyNotFoundException($"Réclamation {numero} introuvable.");

        return new ReclamationPublicDto
        {
            NumeroReclamation = r.NumeroReclamation,
            Objet = r.Objet,
            Statut = r.Statut.ToString(),
            TypeReclamation = r.TypeReclamation?.Libelle ?? string.Empty,
            Categorie = r.Categorie?.Libelle ?? string.Empty,
            DateDepot = r.DateDepot,
            DateCloture = r.DateCloture,
            Historique = r.Suivis?
                .Where(s => s.VisibleCitoyen)
                .OrderByDescending(s => s.DateChangement)
                .Select(s => new SuiviPublicDto
                {
                    Commentaire = s.Commentaire ?? string.Empty,
                    NouveauStatut = s.NouveauStatut?.ToString(),
                    DateChangement = s.DateChangement
                }).ToList() ?? new()
        };
    }

    public Task<List<ReclamationDto>> GetByCitoyenAsync(int citoyenId)
        => _repo.GetByCitoyenAsync(citoyenId);

    public Task<List<ReclamationDto>> GetEnRetardAsync()
        => _repo.GetEnRetardAsync();

    public Task<ReclamationStatsDto> GetStatsAsync(int? serviceId = null)
        => _repo.GetStatsAsync(serviceId);

    // ── Dépôt ───────────────────────────────────────────────────────

    public async Task<ReclamationDetailDto> DeposerAsync(
        CreateReclamationDto dto, string? agentId)
    {
        var numero = await _seq.GenererNumeroAsync("REC");

        var rec = new Reclamation
        {
            NumeroReclamation = numero,
            DateDepot = DateTime.UtcNow,
            DateIncident = dto.DateIncident,
            TypeReclamationId = dto.TypeReclamationId,
            CategorieId = dto.CategorieId,
            Priorite = Enum.Parse<PrioriteReclamation>(dto.Priorite),
            Statut = StatutReclamation.Nouvelle,
            CitoyenId = dto.CitoyenId,
            EstAnonyme = dto.EstAnonyme,
            Canal = Enum.Parse<CanalReclamation>(dto.Canal),
            Objet = dto.Objet,
            Description = dto.Description,
            Localisation = dto.Localisation,
            Longitude = dto.Longitude,
            Latitude = dto.Latitude,
            EnregistreParId = agentId
        };

        await _repo.AddAsync(rec);

        // Suivi initial
        rec.Suivis.Add(new SuiviReclamation
        {
            ReclamationId = rec.Id,
            NouveauStatut = StatutReclamation.Nouvelle,
            UtilisateurId = agentId ?? "citoyen",
            UtilisateurNom = agentId != null ? "Agent" : "Citoyen",
            Commentaire = "Réclamation enregistrée.",
            VisibleCitoyen = true
        });

        await _repo.UpdateAsync(rec);

        // Notification agents Réclamations
        await _notif.NotifierAgentAsync(
            string.Empty, TypeNotification.NouvelleReclamation,
            $"Nouvelle réclamation {numero} : {dto.Objet}",
            rec.Id.ToString(), "Reclamation");

        return await GetByIdAsync(rec.Id);
    }

    // ── Assignation ─────────────────────────────────────────────────

    public async Task AssignerAsync(
        int id, AssignerReclamationDto dto, string agentId, string agentNom)
    {
        var r = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Réclamation #{id} introuvable.");

        var ancienStatut = r.Statut;
        r.ServiceConcerneId = dto.ServiceConcerneId;
        r.AffecteAId = dto.AffecteAId;
        r.DateAffectation = DateTime.UtcNow;
        if (r.Statut == StatutReclamation.Nouvelle)
            r.Statut = StatutReclamation.EnCours;
        r.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(r);

        r.Suivis.Add(new SuiviReclamation
        {
            ReclamationId = r.Id,
            StatutPrecedent = ancienStatut,
            NouveauStatut = r.Statut,
            UtilisateurId = agentId,
            UtilisateurNom = agentNom,
            Commentaire = dto.Commentaire ?? "Réclamation assignée.",
            VisibleCitoyen = false
        });

        await _repo.UpdateAsync(r);

        if (dto.AffecteAId != null)
            await _notif.NotifierAgentAsync(
                dto.AffecteAId, TypeNotification.ReclamationAssignee,
                $"Réclamation {r.NumeroReclamation} vous a été assignée.",
                r.Id.ToString(), "Reclamation");
    }

    // ── Changement de statut ────────────────────────────────────────

    public async Task ChangerStatutAsync(
        int id, ChangerStatutReclamationDto dto, string agentId, string agentNom)
    {
        var r = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Réclamation #{id} introuvable.");

        var ancienStatut = r.Statut;
        var nouveauStatut = Enum.Parse<StatutReclamation>(dto.NouveauStatut);

        r.Statut = nouveauStatut;
        r.UpdatedAt = DateTime.UtcNow;

        if (nouveauStatut == StatutReclamation.Traitee || nouveauStatut == StatutReclamation.Fermee)
        {
            r.DateCloture = DateTime.UtcNow;
            r.SolutionApportee = dto.SolutionApportee ?? r.SolutionApportee;
            if (dto.SatisfactionCitoyen.HasValue)
                r.SatisfactionCitoyen = dto.SatisfactionCitoyen;
        }

        await _repo.UpdateAsync(r);

        r.Suivis.Add(new SuiviReclamation
        {
            ReclamationId = r.Id,
            StatutPrecedent = ancienStatut,
            NouveauStatut = nouveauStatut,
            UtilisateurId = agentId,
            UtilisateurNom = agentNom,
            Commentaire = dto.Commentaire,
            ActionEffectuee = dto.ActionEffectuee,
            VisibleCitoyen = dto.VisibleCitoyen
        });

        await _repo.UpdateAsync(r);

        // Notification citoyen si résolution
        if (nouveauStatut == StatutReclamation.Traitee || nouveauStatut == StatutReclamation.Rejetee)
            await _notif.NotifierCitoyenAsync(
                r.CitoyenId.ToString(),
                nouveauStatut == StatutReclamation.Traitee
                    ? TypeNotification.ReclamationResolue
                    : TypeNotification.ReclamationRejetee,
                $"Votre réclamation {r.NumeroReclamation} a été {nouveauStatut.ToString().ToLower()}.",
                r.Id.ToString(), "Reclamation");
    }

    // ── Mapping privé ───────────────────────────────────────────────

    private static ReclamationDetailDto MapToDetail(Reclamation r) => new()
    {
        Id = r.Id,
        NumeroReclamation = r.NumeroReclamation,
        DateDepot = r.DateDepot,
        DateIncident = r.DateIncident,
        TypeReclamationId = r.TypeReclamationId,
        TypeReclamationLibelle = r.TypeReclamation?.Libelle ?? string.Empty,
        CategorieId = r.CategorieId,
        CategorieLibelle = r.Categorie?.Libelle ?? string.Empty,
        Priorite = r.Priorite.ToString(),
        Statut = r.Statut.ToString(),
        CitoyenId = r.CitoyenId,
        CitoyenNomComplet = r.Citoyen?.NomComplet ?? string.Empty,
        CitoyenTelephone = r.Citoyen?.Telephone ?? string.Empty,
        EstAnonyme = r.EstAnonyme,
        Canal = r.Canal.ToString(),
        Objet = r.Objet,
        Description = r.Description,
        SolutionApportee = r.SolutionApportee,
        Localisation = r.Localisation,
        Longitude = r.Longitude,
        Latitude = r.Latitude,
        ServiceConcerneId = r.ServiceConcerneId,
        ServiceConcerneNom = r.ServiceConcerne?.Nom,
        AffecteAId = r.AffecteAId,
        DateAffectation = r.DateAffectation,
        DateCloture = r.DateCloture,
        SatisfactionCitoyen = r.SatisfactionCitoyen,
        NombrePiecesJointes = r.PiecesJointes?.Count ?? 0,
        CreatedAt = r.CreatedAt,
        Suivis = r.Suivis?
            .OrderByDescending(s => s.DateChangement)
            .Select(s => new SuiviReclamationDto
            {
                Id = s.Id,
                StatutPrecedent = s.StatutPrecedent?.ToString(),
                NouveauStatut = s.NouveauStatut?.ToString(),
                DateChangement = s.DateChangement,
                UtilisateurNom = s.UtilisateurNom,
                Commentaire = s.Commentaire,
                ActionEffectuee = s.ActionEffectuee,
                VisibleCitoyen = s.VisibleCitoyen
            }).ToList() ?? new(),
        PiecesJointes = r.PiecesJointes?
            .Select(p => new PieceJointeReclamationDto
            {
                Id = p.Id,
                TypeDocument = p.TypeDocument,
                NomFichier = p.NomFichier,
                TailleFichier = p.TailleFichier,
                AjouteeParCitoyen = p.AjouteeParCitoyen
            }).ToList() ?? new()
    };
}

// ════════════════════════════════════════════════════════════════
//  CITOYEN SERVICE
// ════════════════════════════════════════════════════════════════

public class CitoyenService : ICitoyenService
{
    private readonly ICitoyenRepository _repo;

    public CitoyenService(ICitoyenRepository repo) => _repo = repo;

    public Task<PagedResult<CitoyenDto>> GetPagedAsync(CitoyenFilterDto filter)
        => _repo.GetPagedAsync(filter);

    public async Task<CitoyenDto> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Citoyen #{id} introuvable.");
        return MapToDto(c);
    }

    public async Task<CitoyenDto> GetByCINAsync(string cin)
    {
        var c = await _repo.GetByCINAsync(cin)
            ?? throw new KeyNotFoundException($"Citoyen CIN={cin} introuvable.");
        return MapToDto(c);
    }

    public async Task<CitoyenDto> CreateAsync(CreateCitoyenDto dto)
    {
        if (await _repo.CINExistsAsync(dto.CIN))
            throw new InvalidOperationException($"Un citoyen avec CIN={dto.CIN} existe déjà.");

        var c = new Citoyen
        {
            CIN = dto.CIN,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            DateNaissance = dto.DateNaissance,
            LieuNaissance = dto.LieuNaissance,
            Sexe = dto.Sexe != null ? Enum.Parse<SexeCitoyen>(dto.Sexe) : null,
            Adresse = dto.Adresse,
            Ville = dto.Ville,
            CodePostal = dto.CodePostal,
            Telephone = dto.Telephone,
            TelephoneMobile = dto.TelephoneMobile,
            Email = dto.Email,
            SituationFamiliale = dto.SituationFamiliale != null
                ? Enum.Parse<SituationFamiliale>(dto.SituationFamiliale) : null,
            IsActive = true
        };
        await _repo.AddAsync(c);
        return MapToDto(c);
    }

    public async Task<CitoyenDto> UpdateAsync(int id, CreateCitoyenDto dto)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Citoyen #{id} introuvable.");

        if (await _repo.CINExistsAsync(dto.CIN, id))
            throw new InvalidOperationException($"CIN={dto.CIN} déjà utilisé.");

        c.CIN = dto.CIN; c.Nom = dto.Nom; c.Prenom = dto.Prenom;
        c.DateNaissance = dto.DateNaissance; c.LieuNaissance = dto.LieuNaissance;
        c.Sexe = dto.Sexe != null ? Enum.Parse<SexeCitoyen>(dto.Sexe) : null;
        c.Adresse = dto.Adresse; c.Ville = dto.Ville; c.CodePostal = dto.CodePostal;
        c.Telephone = dto.Telephone; c.TelephoneMobile = dto.TelephoneMobile;
        c.Email = dto.Email; c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
        return MapToDto(c);
    }

    public Task<bool> CINExistsAsync(string cin, int? excludeId = null)
        => _repo.CINExistsAsync(cin, excludeId);

    private static CitoyenDto MapToDto(Citoyen c) => new()
    {
        Id = c.Id,
        CIN = c.CIN,
        Nom = c.Nom,
        Prenom = c.Prenom,
        NomComplet = c.NomComplet,
        DateNaissance = c.DateNaissance,
        Sexe = c.Sexe?.ToString(),
        Adresse = c.Adresse,
        Ville = c.Ville,
        Telephone = c.Telephone,
        TelephoneMobile = c.TelephoneMobile,
        Email = c.Email,
        SituationFamiliale = c.SituationFamiliale?.ToString(),
        IsActive = c.IsActive,
        NombreReclamations = c.Reclamations?.Count ?? 0
    };
}
