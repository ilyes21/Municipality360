// Application/Services/CourrierService.cs
// ✅ FIXED: إزالة using Municipality360.Domain.Entities.BureauOrdre — namespace غير موجود
// جميع الـ entities في Municipality360.Domain.Entities

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;


// ════════════════════════════════════════════════════════════════
//  COURRIER ENTRANT SERVICE
// ════════════════════════════════════════════════════════════════

public class CourrierEntrantService : ICourrierEntrantService
{
    private readonly IBOCourrierEntrantRepository _repo;
    private readonly IBOCircuitTraitementRepository _circuitRepo;
    private readonly IBOArchiveRepository _archiveRepo;
    private readonly ISequenceRepository _seq;
    private readonly INotificationService _notif;

    public CourrierEntrantService(
        IBOCourrierEntrantRepository repo,
        IBOCircuitTraitementRepository circuitRepo,
        IBOArchiveRepository archiveRepo,
        ISequenceRepository seq,
        INotificationService notif)
    {
        _repo = repo;
        _circuitRepo = circuitRepo;
        _archiveRepo = archiveRepo;
        _seq = seq;
        _notif = notif;
    }

    // ══════════════════════════════════════════════════════════════
    //  LECTURES
    // ══════════════════════════════════════════════════════════════

    public Task<PagedResult<CourrierEntrantDto>> GetPagedAsync(CourrierEntrantFilterDto filter)
        => _repo.GetPagedAsync(filter);

    public async Task<CourrierEntrantDetailDto> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Courrier entrant #{id} introuvable.");
        return MapToDetail(e);
    }

    public async Task<CourrierEntrantDetailDto> GetByNumeroAsync(string numero)
    {
        var e = await _repo.GetByNumeroAsync(numero)
            ?? throw new KeyNotFoundException($"Courrier {numero} introuvable.");
        return MapToDetail(e);
    }

    public Task<List<CourrierEntrantDto>> GetEnRetardAsync()
        => _repo.GetEnRetardAsync();

    public Task<List<CourrierEntrantDto>> GetEnAttenteParServiceAsync(int serviceId)
        => _repo.GetNonTraitesParServiceAsync(serviceId);

    public Task<BOStatsDto> GetStatsAsync(int? serviceId = null)
        => _repo.GetStatsAsync(serviceId);

    // ══════════════════════════════════════════════════════════════
    //  ENREGISTREMENT
    // ══════════════════════════════════════════════════════════════

    public async Task<CourrierEntrantDetailDto> EnregistrerAsync(
        CreateCourrierEntrantDto dto, string agentId, string agentNom)
    {
        var numero = await _seq.GenererNumeroAsync("ENT");

        var courrier = new BOCourrierEntrant
        {
            NumeroOrdre = numero,
            NumeroExterne = dto.NumeroExterne,
            DateCourrier = dto.DateCourrier,
            DateReception = dto.DateReception,
            ObjetCourrier = dto.ObjetCourrier,
            TypeDocument = Enum.Parse<TypeDocumentBO>(dto.TypeDocument),
            CategorieId = dto.CategorieId,
            DossierId = dto.DossierId,
            ExpediteurContactId = dto.ExpediteurContactId,
            ExpediteurLibreNom = dto.ExpediteurLibreNom,
            ModeReception = Enum.Parse<ModeReception>(dto.ModeReception),
            NumeroRecommande = dto.NumeroRecommande,
            ServiceDestinataireId = dto.ServiceDestinataireId,
            AgentDestinataireId = dto.AgentDestinataireId,
            NombrePages = dto.NombrePages,
            Priorite = Enum.Parse<PrioriteCourrier>(dto.Priorite),
            EstConfidentiel = dto.EstConfidentiel,
            Statut = StatutEntrant.Enregistre,
            DelaiReponse = dto.DelaiReponse,
            NecessiteReponse = dto.NecessiteReponse,
            Observation = dto.Observation,
            EnregistreParId = agentId
        };

        await _repo.AddAsync(courrier);

        // Notification au service destinataire si défini
        if (dto.ServiceDestinataireId.HasValue)
            await _notif.NotifierServiceAsync(
                dto.ServiceDestinataireId.Value,
                TypeNotification.NouveauCourrier,
                $"Nouveau courrier entrant {numero} : {dto.ObjetCourrier}",
                courrier.Id.ToString(), "CourrierEntrant");

        // Notification à l'agent destinataire si défini
        if (!string.IsNullOrEmpty(dto.AgentDestinataireId))
            await _notif.NotifierAgentAsync(
                dto.AgentDestinataireId,
                TypeNotification.CourrierAssigne,
                $"Courrier {numero} vous a été assigné : {dto.ObjetCourrier}",
                courrier.Id.ToString(), "CourrierEntrant");

        return await GetByIdAsync(courrier.Id);
    }

    // ══════════════════════════════════════════════════════════════
    //  MODIFICATION
    // ══════════════════════════════════════════════════════════════

    public async Task<CourrierEntrantDetailDto> ModifierAsync(
        int id, UpdateCourrierEntrantDto dto, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Courrier entrant #{id} introuvable.");

        if (c.Statut == StatutEntrant.Archive)
            throw new InvalidOperationException("Impossible de modifier un courrier archivé.");

        c.ObjetCourrier = dto.ObjetCourrier;
        c.TypeDocument = Enum.Parse<TypeDocumentBO>(dto.TypeDocument);
        c.CategorieId = dto.CategorieId;
        c.DossierId = dto.DossierId;
        c.ExpediteurContactId = dto.ExpediteurContactId;
        c.ExpediteurLibreNom = dto.ExpediteurLibreNom;
        c.ServiceDestinataireId = dto.ServiceDestinataireId;
        c.AgentDestinataireId = dto.AgentDestinataireId;
        c.Priorite = Enum.Parse<PrioriteCourrier>(dto.Priorite);
        c.EstConfidentiel = dto.EstConfidentiel;
        c.NecessiteReponse = dto.NecessiteReponse;
        c.DelaiReponse = dto.DelaiReponse;
        c.NombrePages = dto.NombrePages;
        c.NumeroRecommande = dto.NumeroRecommande;
        c.Observation = dto.Observation;
        c.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(c);
        return await GetByIdAsync(id);
    }

    // ══════════════════════════════════════════════════════════════
    //  STATUT
    // ══════════════════════════════════════════════════════════════

    public async Task ChangerStatutAsync(
        int id, ChangerStatutEntrantDto dto, string agentId, string agentNom)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Courrier entrant #{id} introuvable.");

        c.Statut = Enum.Parse<StatutEntrant>(dto.NouveauStatut);
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);

        if (c.Statut == StatutEntrant.Traite)
            await _notif.NotifierAgentAsync(
                agentId, TypeNotification.CourrierTraite,
                $"Courrier {c.NumeroOrdre} marqué comme traité.",
                c.Id.ToString(), "CourrierEntrant");
    }

    // ══════════════════════════════════════════════════════════════
    //  AFFECTATION DIRECTE (sans circuit)
    // ══════════════════════════════════════════════════════════════

    public async Task AffecterAsync(
        int id, AffecterCourrierEntrantDto dto, string agentId, string agentNom)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Courrier entrant #{id} introuvable.");

        if (c.Statut == StatutEntrant.Archive)
            throw new InvalidOperationException("Impossible d'affecter un courrier archivé.");

        c.ServiceDestinataireId = dto.ServiceDestinataireId;
        c.AgentDestinataireId = dto.AgentDestinataireId;
        // Passer en EnCours si c'était Enregistre/Recu
        if (c.Statut is StatutEntrant.Recu or StatutEntrant.Enregistre)
            c.Statut = StatutEntrant.EnCours;
        c.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(c);

        // Notification service
        await _notif.NotifierServiceAsync(
            dto.ServiceDestinataireId,
            TypeNotification.CourrierAssigne,
            $"Courrier {c.NumeroOrdre} affecté à votre service : {c.ObjetCourrier}",
            c.Id.ToString(), "CourrierEntrant");

        // Notification agent
        if (!string.IsNullOrEmpty(dto.AgentDestinataireId))
            await _notif.NotifierAgentAsync(
                dto.AgentDestinataireId,
                TypeNotification.CourrierAssigne,
                $"Courrier {c.NumeroOrdre} vous a été affecté : {c.ObjetCourrier}",
                c.Id.ToString(), "CourrierEntrant");
    }

    // ══════════════════════════════════════════════════════════════
    //  CIRCUIT DE TRAITEMENT
    // ══════════════════════════════════════════════════════════════

    public async Task<BOCircuitTraitementDto> AcheminerAsync(
        int courrierEntrantId, AcheminerCourrierDto dto, string agentId)
    {
        var c = await _repo.GetByIdAsync(courrierEntrantId)
            ?? throw new KeyNotFoundException($"Courrier entrant #{courrierEntrantId} introuvable.");

        if (c.Statut == StatutEntrant.Archive)
            throw new InvalidOperationException("Impossible d'acheminer un courrier archivé.");

        // Clôturer l'étape active précédente
        var etapeActive = await _circuitRepo.GetEtapeActiveAsync(courrierEntrantId);
        if (etapeActive != null)
        {
            etapeActive.StatutEtape = StatutEtapeCircuit.Traite;
            etapeActive.DateTraitement = DateTime.UtcNow;
            etapeActive.UpdatedAt = DateTime.UtcNow;
            await _circuitRepo.UpdateAsync(etapeActive);
        }

        // Mettre à jour le service destinataire principal du courrier
        c.ServiceDestinataireId = dto.ServiceRecepteurId;
        c.AgentDestinataireId = dto.AgentRecepteurId;
        if (c.Statut is StatutEntrant.Recu or StatutEntrant.Enregistre)
            c.Statut = StatutEntrant.EnCours;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);

        var etape = new BOCircuitTraitement
        {
            CourrierEntrantId = courrierEntrantId,
            NumeroEtape = await _circuitRepo.GetProchaineNumeroEtapeAsync(courrierEntrantId),
            ServiceEmetteurId = c.ServiceDestinataireId,
            AgentEmetteurId = agentId,
            ServiceRecepteurId = dto.ServiceRecepteurId,
            AgentRecepteurId = dto.AgentRecepteurId,
            TypeAction = Enum.Parse<TypeActionCircuit>(dto.TypeAction),
            InstructionTransmission = dto.InstructionTransmission,
            DelaiTraitement = dto.DelaiTraitement,
            StatutEtape = StatutEtapeCircuit.EnAttente,
            DateTransmission = DateTime.UtcNow,
            CreatedById = agentId
        };

        await _circuitRepo.AddAsync(etape);

        await _notif.NotifierServiceAsync(
            dto.ServiceRecepteurId,
            TypeNotification.NouveauCourrier,
            $"Courrier {c.NumeroOrdre} acheminé vers votre service ({dto.TypeAction}).",
            c.Id.ToString(), "CourrierEntrant");

        if (!string.IsNullOrEmpty(dto.AgentRecepteurId))
            await _notif.NotifierAgentAsync(
                dto.AgentRecepteurId,
                TypeNotification.CourrierAssigne,
                $"Courrier {c.NumeroOrdre} vous a été acheminé : {c.ObjetCourrier}",
                c.Id.ToString(), "CourrierEntrant");

        var liste = await _circuitRepo.GetByCourrierAsync(courrierEntrantId);
        return liste.Last();
    }

    public async Task TraiterEtapeCircuitAsync(
        int circuitId, TraiterEtapeCircuitDto dto, string agentId, string agentNom)
    {
        var etape = await _circuitRepo.GetByIdAsync(circuitId)
            ?? throw new KeyNotFoundException($"Étape circuit #{circuitId} introuvable.");

        if (etape.StatutEtape == StatutEtapeCircuit.Traite)
            throw new InvalidOperationException("Étape déjà traitée.");

        etape.StatutEtape = Enum.Parse<StatutEtapeCircuit>(dto.StatutEtape);
        etape.CommentaireTraitement = dto.CommentaireTraitement;
        etape.ActionEffectuee = dto.ActionEffectuee;
        etape.DateTraitement = DateTime.UtcNow;
        etape.UpdatedAt = DateTime.UtcNow;

        await _circuitRepo.UpdateAsync(etape);
    }

    public async Task RetournerEtapeCircuitAsync(
        int circuitId, RetournerEtapeCircuitDto dto, string agentId, string agentNom)
    {
        var etape = await _circuitRepo.GetByIdAsync(circuitId)
            ?? throw new KeyNotFoundException($"Étape circuit #{circuitId} introuvable.");

        etape.StatutEtape = StatutEtapeCircuit.Retourne;
        etape.EstRetour = true;
        etape.MotifRetour = dto.MotifRetour;
        etape.CommentaireTraitement = dto.Commentaire;
        etape.DateTraitement = DateTime.UtcNow;
        etape.UpdatedAt = DateTime.UtcNow;

        await _circuitRepo.UpdateAsync(etape);

        // Notifier l'émetteur
        if (!string.IsNullOrEmpty(etape.AgentEmetteurId))
            await _notif.NotifierAgentAsync(
                etape.AgentEmetteurId,
                TypeNotification.NouveauCourrier,
                $"Courrier #{etape.CourrierEntrantId} retourné: {dto.MotifRetour}",
                etape.CourrierEntrantId.ToString(), "CourrierEntrant");
    }

    public Task<List<BOCircuitTraitementDto>> GetCircuitAsync(int courrierEntrantId)
        => _circuitRepo.GetByCourrierAsync(courrierEntrantId);

    // ══════════════════════════════════════════════════════════════
    //  PIÈCES JOINTES
    // ══════════════════════════════════════════════════════════════

    public async Task<BOPieceJointeDto> AjouterPieceJointeAsync(
        int courrierEntrantId, AjouterPieceJointeDto dto)
    {
        var c = await _repo.GetByIdAsync(courrierEntrantId)
            ?? throw new KeyNotFoundException($"Courrier entrant #{courrierEntrantId} introuvable.");

        if (c.Statut == StatutEntrant.Archive)
            throw new InvalidOperationException("Impossible d'ajouter une pièce jointe à un courrier archivé.");

        var pj = new BOPieceJointeEntrant
        {
            CourrierEntrantId = courrierEntrantId,
            NomFichierOriginal = dto.NomFichierOriginal,
            NomFichierStocke = dto.NomFichierStocke,
            CheminFichier = dto.CheminFichier,
            ExtensionFichier = dto.ExtensionFichier,
            TailleFichierOctets = dto.TailleFichierOctets,
            TypePiece = Enum.Parse<TypePieceJointeBO>(dto.TypePiece),
            Description = dto.Description,
            Ordre = dto.Ordre,
            UploadedById = dto.UploadedById
        };

        // Utiliser le repository générique via le contexte
        // (nécessite d'exposer un IGenericRepository<BOPieceJointeEntrant> ou une méthode dédiée)
        await _repo.AddPieceJointeAsync(pj);

        return new BOPieceJointeDto
        {
            Id = pj.Id,
            NomFichierOriginal = pj.NomFichierOriginal,
            ExtensionFichier = pj.ExtensionFichier,
            TailleFichierOctets = pj.TailleFichierOctets,
            TypePiece = pj.TypePiece.ToString(),
            Ordre = pj.Ordre,
            Description = pj.Description,
            UrlTelechargement = $"/api/courriers-entrants/{courrierEntrantId}/pieces-jointes/{pj.Id}/telecharger"
        };
    }

    public async Task<PieceJointeDetailDto> GetPieceJointeAsync(int courrierEntrantId, int pjId)
    {
        var pj = await _repo.GetPieceJointeAsync(courrierEntrantId, pjId)
            ?? throw new KeyNotFoundException($"Pièce jointe #{pjId} introuvable pour courrier #{courrierEntrantId}.");

        return new PieceJointeDetailDto
        {
            Id = pj.Id,
            NomFichierOriginal = pj.NomFichierOriginal,
            ExtensionFichier = pj.ExtensionFichier,
            TailleFichierOctets = pj.TailleFichierOctets,
            TypePiece = pj.TypePiece.ToString(),
            Ordre = pj.Ordre,
            Description = pj.Description,
            CheminFichier = pj.CheminFichier,
            UrlTelechargement = $"/api/courriers-entrants/{courrierEntrantId}/pieces-jointes/{pj.Id}/telecharger"
        };
    }

    public async Task SupprimerPieceJointeAsync(int courrierEntrantId, int pjId, string agentId)
    {
        var pj = await _repo.GetPieceJointeAsync(courrierEntrantId, pjId)
            ?? throw new KeyNotFoundException($"Pièce jointe #{pjId} introuvable.");

        await _repo.SupprimerPieceJointeAsync(pj);
    }

    // ══════════════════════════════════════════════════════════════
    //  ARCHIVAGE
    // ══════════════════════════════════════════════════════════════

    public async Task<BOArchiveDto> ArchiverAsync(
        int id, ArchiversCourrierDto dto, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Courrier entrant #{id} introuvable.");

        if (c.Statut == StatutEntrant.Archive)
            throw new InvalidOperationException("Courrier déjà archivé.");

        var numeroArchive = await _seq.GenererNumeroAsync("ARC");
        var dateDebut = DateTime.UtcNow;

        var archive = new BOArchive
        {
            CourrierEntrantId = id,
            NumeroArchive = numeroArchive,
            SalleArchive = dto.SalleArchive,
            Rayon = dto.Rayon,
            Boite = dto.Boite,
            Classification = Enum.Parse<ClassificationArchive>(dto.Classification),
            DureeConservationAns = dto.DureeConservationAns,
            DateDebutConservation = dateDebut,
            DateFinConservation = dateDebut.AddYears(dto.DureeConservationAns),
            CheminArchiveNumerique = dto.CheminArchiveNumerique,
            ArchiveParId = agentId,
            Observation = dto.Observation
        };

        await _archiveRepo.AddAsync(archive);

        c.Statut = StatutEntrant.Archive;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);

        return (await _archiveRepo.GetByCourrierEntrantIdAsync(id))!;
    }

    // ══════════════════════════════════════════════════════════════
    //  SUPPRESSION (soft delete)
    // ══════════════════════════════════════════════════════════════

    public async Task SupprimerAsync(int id, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Courrier entrant #{id} introuvable.");

        if (c.Statut == StatutEntrant.Archive)
            throw new InvalidOperationException("Impossible de supprimer un courrier archivé.");

        await _repo.DeleteAsync(c); // soft-delete via BaseEntity.IsDeleted
    }

    // ══════════════════════════════════════════════════════════════
    //  MAPPING
    // ══════════════════════════════════════════════════════════════

    private static CourrierEntrantDetailDto MapToDetail(BOCourrierEntrant c) => new()
    {
        Id = c.Id,
        NumeroOrdre = c.NumeroOrdre,
        NumeroExterne = c.NumeroExterne,
        DateReception = c.DateReception,
        DateCourrier = c.DateCourrier,
        ObjetCourrier = c.ObjetCourrier,
        TypeDocument = c.TypeDocument.ToString(),
        CategorieId = c.CategorieId,
        CategorieLibelle = c.Categorie?.Libelle,
        DossierId = c.DossierId,
        DossierIntitule = c.Dossier?.Intitule,
        ExpediteurContactId = c.ExpediteurContactId,
        ExpediteurNom = c.ExpediteurContact?.NomComplet ?? c.ExpediteurLibreNom ?? string.Empty,
        ModeReception = c.ModeReception.ToString(),
        NumeroRecommande = c.NumeroRecommande,
        ServiceDestinataireId = c.ServiceDestinataireId,
        ServiceDestinataireNom = c.ServiceDestinataire?.Nom,
        AgentDestinataireId = c.AgentDestinataireId,
        NombrePages = c.NombrePages,
        Priorite = c.Priorite.ToString(),
        EstConfidentiel = c.EstConfidentiel,
        Statut = c.Statut.ToString(),
        NecessiteReponse = c.NecessiteReponse,
        DelaiReponse = c.DelaiReponse,
        Observation = c.Observation,
        EnregistreParId = c.EnregistreParId,
        NombrePiecesJointes = c.PiecesJointes?.Count ?? 0,
        NombreEtapesCircuit = c.Circuit?.Count ?? 0,
        CreatedAt = c.CreatedAt,

        PiecesJointes = c.PiecesJointes?
            .OrderBy(p => p.Ordre)
            .Select(p => new BOPieceJointeDto
            {
                Id = p.Id,
                NomFichierOriginal = p.NomFichierOriginal,
                ExtensionFichier = p.ExtensionFichier,
                TailleFichierOctets = p.TailleFichierOctets,
                TypePiece = p.TypePiece.ToString(),
                Ordre = p.Ordre,
                Description = p.Description,
                UrlTelechargement = $"/api/courriers-entrants/{c.Id}/pieces-jointes/{p.Id}/telecharger"
            }).ToList() ?? new(),

        Circuit = c.Circuit?
            .OrderBy(ct => ct.NumeroEtape)
            .Select(ct => new BOCircuitTraitementDto
            {
                Id = ct.Id,
                NumeroEtape = ct.NumeroEtape,
                ServiceEmetteurId = ct.ServiceEmetteurId,
                ServiceEmetteurNom = ct.ServiceEmetteur?.Nom,
                ServiceRecepteurId = ct.ServiceRecepteurId,
                ServiceRecepteurNom = ct.ServiceRecepteur?.Nom ?? string.Empty,
                AgentRecepteurId = ct.AgentRecepteurId,
                DateTransmission = ct.DateTransmission,
                DateTraitement = ct.DateTraitement,
                DelaiTraitement = ct.DelaiTraitement,
                TypeAction = ct.TypeAction.ToString(),
                InstructionTransmission = ct.InstructionTransmission,
                CommentaireTraitement = ct.CommentaireTraitement,
                ActionEffectuee = ct.ActionEffectuee,
                StatutEtape = ct.StatutEtape.ToString(),
                EstRetour = ct.EstRetour,
                MotifRetour = ct.MotifRetour
            }).ToList() ?? new()
    };
}


// ════════════════════════════════════════════════════════════════
//  COURRIER SORTANT SERVICE
// ════════════════════════════════════════════════════════════════

public class CourrierSortantService : ICourrierSortantService
{
    private readonly IBOCourrierSortantRepository _repo;
    private readonly IBOArchiveRepository _archiveRepo;
    private readonly ISequenceRepository _seq;
    private readonly INotificationService _notif;

    public CourrierSortantService(
        IBOCourrierSortantRepository repo,
        IBOArchiveRepository archiveRepo,
        ISequenceRepository seq,
        INotificationService notif)
    {
        _repo = repo; _archiveRepo = archiveRepo; _seq = seq; _notif = notif;
    }

    public Task<PagedResult<CourrierSortantDto>> GetPagedAsync(CourrierSortantFilterDto filter)
        => _repo.GetPagedAsync(filter);

    public async Task<CourrierSortantDetailDto> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Courrier sortant #{id} introuvable.");
        return MapToDetail(c);
    }

    public async Task<CourrierSortantDetailDto> GetByNumeroAsync(string numero)
    {
        var c = await _repo.GetByNumeroAsync(numero)
            ?? throw new KeyNotFoundException($"Courrier sortant {numero} introuvable.");
        return MapToDetail(c);
    }

    public Task<List<CourrierSortantDto>> GetBrouillonsAsync(string redacteurId)
        => _repo.GetBrouillonsAsync(redacteurId);

    public async Task<CourrierSortantDetailDto> CreerBrouillonAsync(
        CreateCourrierSortantDto dto, string agentId)
    {
        var numero = await _seq.GenererNumeroAsync("SRT");

        var c = new BOCourrierSortant
        {
            NumeroOrdre = numero,
            CourrierEntrantRefId = dto.CourrierEntrantRefId,
            DateRedaction = DateTime.UtcNow,
            ObjetCourrier = dto.ObjetCourrier,
            TypeDocument = Enum.Parse<TypeDocumentBO>(dto.TypeDocument),
            CategorieId = dto.CategorieId,
            DossierId = dto.DossierId,
            ServiceEmetteurId = dto.ServiceEmetteurId,
            RedacteurId = agentId,
            DestinataireContactId = dto.DestinataireContactId,
            DestinataireLibreNom = dto.DestinataireLibreNom,
            ModeEnvoi = Enum.Parse<ModeEnvoi>(dto.ModeEnvoi),
            NumeroRecommande = dto.NumeroRecommande,
            FonctionSignataire = dto.FonctionSignataire,
            NombrePages = dto.NombrePages,
            EstConfidentiel = dto.EstConfidentiel,
            Priorite = Enum.Parse<PrioriteCourrier>(dto.Priorite),
            Statut = StatutSortant.Brouillon,
            Observation = dto.Observation,
            CreatedById = agentId
        };

        await _repo.AddAsync(c);
        return await GetByIdAsync(c.Id);
    }

    public async Task<CourrierSortantDetailDto> ModifierAsync(
        int id, CreateCourrierSortantDto dto, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Courrier sortant #{id} introuvable.");
        if (c.Statut != StatutSortant.Brouillon)
            throw new InvalidOperationException("Seuls les brouillons peuvent être modifiés.");

        c.ObjetCourrier = dto.ObjetCourrier;
        c.TypeDocument = Enum.Parse<TypeDocumentBO>(dto.TypeDocument);
        c.CategorieId = dto.CategorieId;
        c.DossierId = dto.DossierId;
        c.ServiceEmetteurId = dto.ServiceEmetteurId;
        c.DestinataireContactId = dto.DestinataireContactId;
        c.DestinataireLibreNom = dto.DestinataireLibreNom;
        c.ModeEnvoi = Enum.Parse<ModeEnvoi>(dto.ModeEnvoi);
        c.FonctionSignataire = dto.FonctionSignataire;
        c.NombrePages = dto.NombrePages;
        c.EstConfidentiel = dto.EstConfidentiel;
        c.Priorite = Enum.Parse<PrioriteCourrier>(dto.Priorite);
        c.Observation = dto.Observation;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
        return await GetByIdAsync(id);
    }

    public async Task SoumettreEnValidationAsync(int id, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"#{id} introuvable.");
        if (c.Statut != StatutSortant.Brouillon)
            throw new InvalidOperationException("Seuls les brouillons peuvent être soumis.");
        c.Statut = StatutSortant.EnValidation;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
    }

    public async Task MarquerSigneAsync(int id, string signatairId, string fonctionSignataire)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"#{id} introuvable.");
        c.Statut = StatutSortant.Signe;
        c.SignataireId = signatairId;
        c.FonctionSignataire = fonctionSignataire;
        c.EstSigne = true;
        c.DateSignature = DateTime.UtcNow;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
    }

    public async Task MarquerEnvoyeAsync(int id, string agentId, DateTime? dateEnvoi = null)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"#{id} introuvable.");
        if (c.Statut != StatutSortant.Signe)
            throw new InvalidOperationException("Le courrier doit être signé avant envoi.");
        c.Statut = StatutSortant.Envoye;
        c.DateEnvoi = dateEnvoi ?? DateTime.UtcNow;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
    }

    public async Task AccuserReceptionAsync(int id, DateTime dateAccuse, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"#{id} introuvable.");
        c.AccuseReceptionRecu = true;
        c.DateAccuseExtRecu = dateAccuse;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
    }

    public async Task<BOArchiveDto> ArchiverAsync(int id, ArchiversCourrierDto dto, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"#{id} introuvable.");
        if (c.Statut != StatutSortant.Envoye)
            throw new InvalidOperationException("Seuls les courriers envoyés peuvent être archivés.");

        var numeroArchive = await _seq.GenererNumeroAsync("ARC");
        var dateDebut = DateTime.UtcNow;
        var archive = new BOArchive
        {
            CourrierSortantId = id,
            NumeroArchive = numeroArchive,
            SalleArchive = dto.SalleArchive,
            Rayon = dto.Rayon,
            Boite = dto.Boite,
            Classification = Enum.Parse<ClassificationArchive>(dto.Classification),
            DureeConservationAns = dto.DureeConservationAns,
            DateDebutConservation = dateDebut,
            DateFinConservation = dateDebut.AddYears(dto.DureeConservationAns),
            CheminArchiveNumerique = dto.CheminArchiveNumerique,
            ArchiveParId = agentId,
            Observation = dto.Observation
        };
        await _archiveRepo.AddAsync(archive);
        c.Statut = StatutSortant.Archive; c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
        return (await _archiveRepo.GetByCourrierSortantIdAsync(id))!;
    }

    public async Task AnnulerAsync(int id, string motif, string agentId)
    {
        var c = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"#{id} introuvable.");
        if (c.Statut == StatutSortant.Envoye || c.Statut == StatutSortant.Archive)
            throw new InvalidOperationException("Impossible d'annuler un courrier déjà envoyé/archivé.");
        c.Statut = StatutSortant.Annule;
        c.Observation = $"[ANNULÉ: {motif}] {c.Observation}".Trim();
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
    }

    public async Task<BOPieceJointeDto> AjouterPieceJointeAsync(int courrierSortantId, AjouterPieceJointeDto dto)
    {
        var c = await _repo.GetByIdAsync(courrierSortantId)
            ?? throw new KeyNotFoundException($"Courrier sortant #{courrierSortantId} introuvable.");

        if (c.Statut == StatutSortant.Archive)
            throw new InvalidOperationException("Impossible d'ajouter une pièce jointe à un courrier archivé.");

        var pj = new BOPieceJointeSortant
        {
            CourrierSortantId = courrierSortantId,
            NomFichierOriginal = dto.NomFichierOriginal,
            NomFichierStocke = dto.NomFichierStocke,
            CheminFichier = dto.CheminFichier,
            ExtensionFichier = dto.ExtensionFichier,
            TailleFichierOctets = dto.TailleFichierOctets,
            TypePiece = Enum.Parse<TypePieceJointeBO>(dto.TypePiece),
            Description = dto.Description,
            Ordre = dto.Ordre,
            UploadedById = dto.UploadedById
        };

        await _repo.AddPieceJointeAsync(pj);

        return new BOPieceJointeDto
        {
            Id = pj.Id,
            NomFichierOriginal = pj.NomFichierOriginal,
            ExtensionFichier = pj.ExtensionFichier,
            TailleFichierOctets = pj.TailleFichierOctets,
            TypePiece = pj.TypePiece.ToString(),
            Ordre = pj.Ordre,
            Description = pj.Description,
            UrlTelechargement = $"/api/courriers-sortants/{courrierSortantId}/pieces-jointes/{pj.Id}/telecharger"
        };
    }

    private static CourrierSortantDetailDto MapToDetail(BOCourrierSortant c) => new()
    {
        Id = c.Id,
        NumeroOrdre = c.NumeroOrdre,
        NumeroReference = c.NumeroReference,
        CourrierEntrantRefId = c.CourrierEntrantRefId,
        CourrierEntrantRefNumero = c.CourrierEntrantRef?.NumeroOrdre,
        DateRedaction = c.DateRedaction,
        DateSignature = c.DateSignature,
        DateEnvoi = c.DateEnvoi,
        ObjetCourrier = c.ObjetCourrier,
        TypeDocument = c.TypeDocument.ToString(),
        CategorieId = c.CategorieId,
        CategorieLibelle = c.Categorie?.Libelle,
        DossierId = c.DossierId,
        ServiceEmetteurId = c.ServiceEmetteurId,
        ServiceEmetteurNom = c.ServiceEmetteur?.Nom,
        FonctionSignataire = c.FonctionSignataire,
        EstSigne = c.EstSigne,
        DestinataireContactId = c.DestinataireContactId,
        DestinataireNom = c.DestinataireContact?.NomComplet ?? c.DestinataireLibreNom,
        ModeEnvoi = c.ModeEnvoi.ToString(),
        NumeroRecommande = c.NumeroRecommande,
        Priorite = c.Priorite.ToString(),
        Statut = c.Statut.ToString(),
        AccuseReceptionRecu = c.AccuseReceptionRecu,
        Observation = c.Observation,
        NombrePiecesJointes = c.PiecesJointes?.Count ?? 0,
        CreatedAt = c.CreatedAt,
        PiecesJointes = c.PiecesJointes?
            .Select(p => new BOPieceJointeDto
            {
                Id = p.Id,
                NomFichierOriginal = p.NomFichierOriginal,
                ExtensionFichier = p.ExtensionFichier,
                TailleFichierOctets = p.TailleFichierOctets,
                TypePiece = p.TypePiece.ToString(),
                Ordre = p.Ordre,
                Description = p.Description,
                EstVersionFinale = p.EstVersionFinale
            }).ToList() ?? new()
    };
}
