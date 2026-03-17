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

    // ════════════════════════════════════════════════════════════
    //  LECTURES
    // ════════════════════════════════════════════════════════════

    public Task<PagedResult<ReclamationDto>> GetPagedAsync(ReclamationFilterDto filter)
        => _repo.GetPagedAsync(filter);

    public async Task<ReclamationDetailDto> GetByIdAsync(int id)
    {
        var r = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Réclamation #{id} introuvable.");
        return MapToDetail(r);
    }

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

    // ════════════════════════════════════════════════════════════
    //  DÉPÔT — ✅ SUIVI AMÉLIORÉ
    // ════════════════════════════════════════════════════════════

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

        // ✅ Suivi initial enrichi
        // Détermine qui a déposé : citoyen via app mobile/web ou agent au guichet
        bool estParAgent = !string.IsNullOrEmpty(agentId) && agentId != "citoyen";
        bool estMobile = dto.Canal == "Mobile" || dto.Canal == "Web";
        string auteurId = estParAgent ? agentId! : (dto.EstAnonyme ? "anonyme" : $"citoyen_{dto.CitoyenId}");
        string auteurNom = estParAgent ? "عون بلدي"
                                  : estMobile ? "مواطن (تطبيق الجوال)"
                                  : dto.Canal == "Email" ? "مواطن (بريد إلكتروني)"
                                  : "مواطن";
        string canalLabel = dto.Canal switch
        {
            "Guichet" => "الشباك",
            "Telephone" => "الهاتف",
            "Email" => "البريد الإلكتروني",
            "Web" => "الموقع الإلكتروني",
            "Mobile" => "تطبيق الجوال",
            _ => dto.Canal
        };

        rec.Suivis.Add(new SuiviReclamation
        {
            ReclamationId = rec.Id,
            NouveauStatut = StatutReclamation.Nouvelle,
            UtilisateurId = auteurId,
            UtilisateurNom = auteurNom,
            Commentaire = estParAgent
                ? $"تم تسجيل الشكوى عبر {canalLabel} بواسطة عون بلدي."
                : $"تم إيداع الشكوى عبر {canalLabel}.",
            ActionEffectuee = "تسجيل الشكوى",
            VisibleCitoyen = true
        });

        await _repo.UpdateAsync(rec);

        // Notification
        await _notif.NotifierAgentAsync(
            string.Empty, TypeNotification.NouvelleReclamation,
            $"شكوى جديدة {numero} : {dto.Objet}",
            rec.Id.ToString(), "Reclamation");

        return await GetByIdAsync(rec.Id);
    }

    // ════════════════════════════════════════════════════════════
    //  ASSIGNATION — ✅ SUIVI ENRICHI
    // ════════════════════════════════════════════════════════════

    public async Task AssignerAsync(
        int id, AssignerReclamationDto dto, string agentId, string agentNom)
    {
        var r = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Réclamation #{id} introuvable.");

        var ancienStatut = r.Statut;
        r.ServiceConcerneId = dto.ServiceConcerneId;
        r.AffecteAId = dto.AffecteAId;
        r.DateAffectation = DateTime.UtcNow;

        // Passer en "EnCours" si elle était "Nouvelle"
        if (r.Statut == StatutReclamation.Nouvelle)
            r.Statut = StatutReclamation.EnCours;

        r.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(r);

        // ✅ Suivi de la transmission avec détails
        string serviceNom = dto.ServiceConcerneId.HasValue
            ? $"المصلحة #{dto.ServiceConcerneId}"
            : "مصلحة غير محددة";

        string commentaireSuivi = dto.Commentaire ?? $"تم توجيه الشكوى إلى {serviceNom}.";
        if (!string.IsNullOrEmpty(dto.AffecteAId))
            commentaireSuivi += $" مكلّف: {dto.AffecteAId}.";

        r.Suivis.Add(new SuiviReclamation
        {
            ReclamationId = r.Id,
            StatutPrecedent = ancienStatut,
            NouveauStatut = r.Statut,
            UtilisateurId = agentId,
            UtilisateurNom = string.IsNullOrEmpty(agentNom) ? "عون بلدي" : agentNom,
            Commentaire = commentaireSuivi,
            ActionEffectuee = string.IsNullOrEmpty(dto.AffecteAId)
                ? $"توجيه الشكوى إلى {serviceNom}"
                : $"توجيه الشكوى إلى {serviceNom} وإسنادها للموظف: {dto.AffecteAId}",
            VisibleCitoyen = false   // le citoyen n'a pas besoin de voir les détails internes
        });

        await _repo.UpdateAsync(r);

        // Notifier l'agent affecté
        if (!string.IsNullOrEmpty(dto.AffecteAId))
            await _notif.NotifierAgentAsync(
                dto.AffecteAId,
                TypeNotification.ReclamationAssignee,
                $"شكوى {r.NumeroReclamation} أُسندت إليك: {r.Objet}",
                r.Id.ToString(), "Reclamation");

        // Notifier le service
        if (dto.ServiceConcerneId.HasValue)
            await _notif.NotifierServiceAsync(
                dto.ServiceConcerneId.Value,
                TypeNotification.ReclamationAssignee,
                $"شكوى جديدة موجَّهة لمصلحتكم: {r.NumeroReclamation}",
                r.Id.ToString(), "Reclamation");
    }

    // ════════════════════════════════════════════════════════════
    //  CHANGEMENT DE STATUT — ✅ SUIVI ENRICHI
    // ════════════════════════════════════════════════════════════

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

        // ✅ Libellés arabes pour les statuts
        string statutLabel = nouveauStatut switch
        {
            StatutReclamation.Nouvelle => "جديدة",
            StatutReclamation.EnCours => "قيد المعالجة",
            StatutReclamation.Traitee => "تمت المعالجة",
            StatutReclamation.Rejetee => "مرفوضة",
            StatutReclamation.Fermee => "مغلقة",
            _ => nouveauStatut.ToString()
        };

        string actionDefaut = nouveauStatut switch
        {
            StatutReclamation.EnCours => "استلام الشكوى وبدء المعالجة",
            StatutReclamation.Traitee => "إغلاق الملف — تمت المعالجة",
            StatutReclamation.Rejetee => "رفض الشكوى",
            StatutReclamation.Fermee => "إغلاق الملف",
            _ => $"تغيير الحالة إلى: {statutLabel}"
        };

        r.Suivis.Add(new SuiviReclamation
        {
            ReclamationId = r.Id,
            StatutPrecedent = ancienStatut,
            NouveauStatut = nouveauStatut,
            UtilisateurId = agentId,
            UtilisateurNom = string.IsNullOrEmpty(agentNom) ? "عون بلدي" : agentNom,
            Commentaire = dto.Commentaire,
            ActionEffectuee = string.IsNullOrEmpty(dto.ActionEffectuee)
                ? actionDefaut
                : dto.ActionEffectuee,
            VisibleCitoyen = dto.VisibleCitoyen
        });

        await _repo.UpdateAsync(r);

        // Notifications citoyen si résolution ou rejet
        if (nouveauStatut == StatutReclamation.Traitee)
            await _notif.NotifierCitoyenAsync(
                r.CitoyenId.ToString(),
                TypeNotification.ReclamationResolue,
                $"تم معالجة شكواكم {r.NumeroReclamation} بنجاح.",
                r.Id.ToString(), "Reclamation");

        else if (nouveauStatut == StatutReclamation.Rejetee)
            await _notif.NotifierCitoyenAsync(
                r.CitoyenId.ToString(),
                TypeNotification.ReclamationRejetee,
                $"تم رفض شكواكم {r.NumeroReclamation}.",
                r.Id.ToString(), "Reclamation");
    }

    // ════════════════════════════════════════════════════════════
    //  MAPPING
    // ════════════════════════════════════════════════════════════

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
            .OrderBy(s => s.DateChangement)      // chronologique — du plus ancien au plus récent
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
//  CITOYEN SERVICE (inchangé)
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
            throw new InvalidOperationException($"مواطن برقم هوية {dto.CIN} موجود مسبقاً.");

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
            throw new InvalidOperationException($"رقم الهوية {dto.CIN} مستخدم مسبقاً.");

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

// ════════════════════════════════════════════════════════════════
//  TYPE RÉCLAMATION SERVICE
// ════════════════════════════════════════════════════════════════

public class TypeReclamationService : ITypeReclamationService
{
    private readonly ITypeReclamationRepository _repo;
    public TypeReclamationService(ITypeReclamationRepository repo) => _repo = repo;

    public async Task<List<TypeReclamationDto>> GetAllActiveAsync()
    {
        var list = await _repo.GetAllActiveAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<TypeReclamationDto> GetByIdAsync(int id)
    {
        var t = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Type réclamation #{id} introuvable.");
        return MapToDto(t);
    }

    public async Task<TypeReclamationDto> CreateAsync(CreateTypeReclamationDto dto)
    {
        if (await _repo.CodeExistsAsync(dto.Code))
            throw new InvalidOperationException($"Le code '{dto.Code}' existe déjà.");

        var t = new TypeReclamation
        {
            Code = dto.Code,
            Libelle = dto.Libelle,
            Description = dto.Description,
            DelaiTraitementJours = dto.DelaiTraitementJours,
            ServiceResponsableId = dto.ServiceResponsableId,
            IsActive = true
        };
        await _repo.AddAsync(t);
        return MapToDto(t);
    }

    public async Task<TypeReclamationDto> UpdateAsync(int id, CreateTypeReclamationDto dto)
    {
        var t = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Type réclamation #{id} introuvable.");

        if (await _repo.CodeExistsAsync(dto.Code, id))
            throw new InvalidOperationException($"Le code '{dto.Code}' est déjà utilisé.");

        t.Code = dto.Code;
        t.Libelle = dto.Libelle;
        t.Description = dto.Description;
        t.DelaiTraitementJours = dto.DelaiTraitementJours;
        t.ServiceResponsableId = dto.ServiceResponsableId;
        t.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(t);
        return MapToDto(t);
    }

    public async Task DeleteAsync(int id)
    {
        var t = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Type réclamation #{id} introuvable.");
        await _repo.DeleteAsync(t);
    }

    private static TypeReclamationDto MapToDto(TypeReclamation t) => new()
    {
        Id = t.Id,
        Code = t.Code,
        Libelle = t.Libelle,
        Description = t.Description,
        DelaiTraitementJours = t.DelaiTraitementJours,
        ServiceResponsableId = t.ServiceResponsableId,
        ServiceResponsableNom = t.ServiceResponsable?.Nom,
        IsActive = t.IsActive
    };
}

// ════════════════════════════════════════════════════════════════
//  CATÉGORIE RÉCLAMATION SERVICE
// ════════════════════════════════════════════════════════════════

public class CategorieReclamationService : ICategorieReclamationService
{
    private readonly ICategorieReclamationRepository _repo;
    public CategorieReclamationService(ICategorieReclamationRepository repo) => _repo = repo;

    public async Task<List<CategorieReclamationDto>> GetHierarchieAsync()
    {
        var list = await _repo.GetHierarchieAsync();
        // ✅ Utiliser x => MapToDto(x) pour lever l'ambiguïté
        return list.Select(x => MapToDto(x)).ToList();
    }

    /// <summary>Retourne toutes les catégories à plat (parents + enfants) pour les dropdowns</summary>
    public async Task<List<CategorieReclamationDto>> GetFlatAsync()
    {
        var parents = await _repo.GetHierarchieAsync();
        var flat = new List<CategorieReclamationDto>();

        foreach (var p in parents)
        {
            flat.Add(MapToDto(p, includeSubs: false));
            foreach (var sub in p.SousCategories.Where(s => s.IsActive))
                flat.Add(MapToDto(sub, includeSubs: false));
        }
        return flat;
    }

    public async Task<CategorieReclamationDto> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Catégorie #{id} introuvable.");
        return MapToDto(c);
    }

    public async Task<CategorieReclamationDto> CreateAsync(CreateCategorieReclamationDto dto)
    {
        if (await _repo.CodeExistsAsync(dto.Code))
            throw new InvalidOperationException($"Le code '{dto.Code}' existe déjà.");

        var c = new CategorieReclamation
        {
            Code = dto.Code,
            Libelle = dto.Libelle,
            Icone = dto.Icone,
            CouleurHex = dto.CouleurHex,
            ParentId = dto.ParentId,
            Niveau = dto.ParentId.HasValue ? 2 : 1,
            IsActive = true
        };
        await _repo.AddAsync(c);
        return MapToDto(c);
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Catégorie #{id} introuvable.");
        await _repo.DeleteAsync(c);
    }

    private static CategorieReclamationDto MapToDto(CategorieReclamation c, bool includeSubs = true) => new()
    {
        Id = c.Id,
        Code = c.Code,
        Libelle = c.Libelle,
        Icone = c.Icone,
        CouleurHex = c.CouleurHex,
        ParentId = c.ParentId,
        ParentLibelle = c.Parent?.Libelle,
        Niveau = c.Niveau,
        IsActive = c.IsActive,

        // ✅ SOLUTION : Utiliser une lambda (s => MapToDto(s, false)) 
        // au lieu de la syntaxe de groupe de méthodes
        SousCategories = includeSubs
        ? c.SousCategories
            .Where(s => s.IsActive)
            .Select(s => MapToDto(s, false)) // Modification ici
            .ToList()
        : new List<CategorieReclamationDto>()
    };
}
