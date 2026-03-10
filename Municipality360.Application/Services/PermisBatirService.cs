// ═══════════════════════════════════════════════════════════════════
//  PermisBatirService.cs
//  Application/Services/PermisBatirService.cs
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.Common;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Application.Services;

public class DemandePermisBatirService : IDemandePermisBatirService
{
    private readonly IDemandePermisBatirRepository _repo;
    private readonly ISequenceRepository _seq;
    private readonly INotificationService _notif;

    public DemandePermisBatirService(
        IDemandePermisBatirRepository repo,
        ISequenceRepository seq,
        INotificationService notif)
    {
        _repo = repo; _seq = seq; _notif = notif;
    }

    // ── Lectures ────────────────────────────────────────────────────

    public Task<PagedResult<DemandePermisDto>> GetPagedAsync(DemandePermisFilterDto filter)
        => _repo.GetPagedAsync(filter);

    public async Task<DemandePermisDetailDto> GetByIdAsync(int id)
    {
        var d = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Demande permis #{id} introuvable.");
        return MapToDetail(d);
    }

    /// <summary>Suivi public Flutter — AllowAnonymous</summary>
    public async Task<DemandePermisSuiviPublicDto> GetByNumeroPublicAsync(string numero)
    {
        var d = await _repo.GetByNumeroAsync(numero)
            ?? throw new KeyNotFoundException($"Demande {numero} introuvable.");

        return new DemandePermisSuiviPublicDto
        {
            NumeroDemande = d.NumeroDemande,
            TypeDemande = d.TypeDemande?.Libelle ?? string.Empty,
            Statut = d.Statut.ToString(),
            AdresseProjet = d.AdresseProjet,
            DateDepot = d.DateDepot,
            DateDecision = d.DateDecision,
            NumeroPermis = d.PermisDelivre?.NumeroPermis,
            DateValiditePermis = d.PermisDelivre?.DateValidite,
            Historique = d.Suivis?
                .Where(s => s.VisibleCitoyen)
                .OrderByDescending(s => s.DateChangement)
                .Select(s => new SuiviPublicPermisDto
                {
                    Commentaire = s.Commentaire ?? string.Empty,
                    NouveauStatut = s.NouveauStatut?.ToString(),
                    DateChangement = s.DateChangement
                }).ToList() ?? new()
        };
    }

    public Task<List<DemandePermisDto>> GetByDemandeurAsync(int demandeurId)
        => _repo.GetByDemandeurAsync(demandeurId);

    public Task<List<DemandePermisDto>> GetEnRetardAsync()
        => _repo.GetEnRetardAsync();

    public Task<PermisStatsDto> GetStatsAsync(int? serviceId = null)
        => _repo.GetStatsAsync(serviceId);

    // ── Dépôt ───────────────────────────────────────────────────────

    public async Task<DemandePermisDetailDto> DeposerAsync(
        CreateDemandePermisDto dto, string? agentId)
    {
        var numero = await _seq.GenererNumeroAsync("PB");

        var demande = new DemandePermisBatir
        {
            NumeroDemande = numero,
            DateDepot = DateTime.UtcNow,
            TypeDemandeId = dto.TypeDemandeId,
            Statut = StatutDemande.Deposee,
            DemandeurId = dto.DemandeurId,
            ArchitecteId = dto.ArchitecteId,
            AdresseProjet = dto.AdresseProjet,
            NumeroParcelle = dto.NumeroParcelle,
            SuperficieTerrain = dto.SuperficieTerrain,
            SuperficieAConstruire = dto.SuperficieAConstruire,
            NombreNiveaux = dto.NombreNiveaux,
            TypeConstruction = dto.TypeConstruction != null
                ? Enum.Parse<TypeConstruction>(dto.TypeConstruction) : null,
            CoutEstimatif = dto.CoutEstimatif,
            ZonageId = dto.ZonageId,
            Longitude = dto.Longitude,
            Latitude = dto.Latitude,
            Observations = dto.Observations,
            EnregistreParId = agentId
        };

        await _repo.AddAsync(demande);

        // Suivi initial
        demande.Suivis.Add(new SuiviPermis
        {
            DemandeId = demande.Id,
            NouveauStatut = StatutDemande.Deposee,
            UtilisateurId = agentId ?? "citoyen",
            UtilisateurNom = agentId != null ? "Agent" : "Citoyen",
            Commentaire = "Demande de permis enregistrée.",
            VisibleCitoyen = true
        });

        await _repo.UpdateAsync(demande);

        await _notif.NotifierAgentAsync(
            string.Empty, TypeNotification.NouvelleDemandePermis,
            $"Nouvelle demande permis {numero} : {demande.AdresseProjet}",
            demande.Id.ToString(), "PermisBatir");

        return await GetByIdAsync(demande.Id);
    }

    // ── Assignation instructeur ─────────────────────────────────────

    public async Task AssignerInstructeurAsync(
        int id, AssignerInstructeurDto dto, string agentId, string agentNom)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        d.ServiceInstructeurId = dto.ServiceInstructeurId;
        d.AgentInstructeurId = dto.AgentInstructeurId;
        d.DateDebutInstruction = DateTime.UtcNow;
        if (d.Statut == StatutDemande.Deposee)
            d.Statut = StatutDemande.EnExamen;
        d.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(d);

        d.Suivis.Add(new SuiviPermis
        {
            DemandeId = d.Id,
            StatutPrecedent = StatutDemande.Deposee,
            NouveauStatut = d.Statut,
            UtilisateurId = agentId,
            UtilisateurNom = agentNom,
            Commentaire = dto.Commentaire ?? "Dossier mis en instruction.",
            VisibleCitoyen = true
        });
        await _repo.UpdateAsync(d);

        if (dto.AgentInstructeurId != null)
            await _notif.NotifierAgentAsync(
                dto.AgentInstructeurId, TypeNotification.DemandePermisEnInstruction,
                $"Demande {d.NumeroDemande} vous a été assignée pour instruction.",
                d.Id.ToString(), "PermisBatir");
    }

    // ── Assignation commission ──────────────────────────────────────

    public async Task AssignerCommissionAsync(
        int id, AssignerCommissionDto dto, string agentId, string agentNom)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        d.CommissionExamenId = dto.CommissionExamenId;
        d.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(d);

        d.Suivis.Add(new SuiviPermis
        {
            DemandeId = d.Id,
            UtilisateurId = agentId,
            UtilisateurNom = agentNom,
            Commentaire = dto.Commentaire ?? "Dossier soumis à la commission d'examen.",
            VisibleCitoyen = true
        });
        await _repo.UpdateAsync(d);
    }

    // ── Changement de statut ────────────────────────────────────────

    public async Task ChangerStatutAsync(
        int id, ChangerStatutPermisDto dto, string agentId, string agentNom)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        var ancienStatut = d.Statut;
        var nouveauStatut = Enum.Parse<StatutDemande>(dto.NouveauStatut);

        d.Statut = nouveauStatut;
        d.UpdatedAt = DateTime.UtcNow;

        if (nouveauStatut == StatutDemande.Approuvee || nouveauStatut == StatutDemande.Rejetee)
        {
            d.DateDecision = DateTime.UtcNow;
            d.MotifRejet = dto.MotifRejet ?? d.MotifRejet;
            d.ConditionsSpeciales = dto.ConditionsSpeciales ?? d.ConditionsSpeciales;
        }

        await _repo.UpdateAsync(d);

        d.Suivis.Add(new SuiviPermis
        {
            DemandeId = d.Id,
            StatutPrecedent = ancienStatut,
            NouveauStatut = nouveauStatut,
            UtilisateurId = agentId,
            UtilisateurNom = agentNom,
            Commentaire = dto.Commentaire,
            VisibleCitoyen = dto.VisibleCitoyen
        });
        await _repo.UpdateAsync(d);

        // Notification citoyen
        if (nouveauStatut == StatutDemande.Approuvee || nouveauStatut == StatutDemande.Rejetee)
            await _notif.NotifierCitoyenAsync(
                d.DemandeurId.ToString(),
                nouveauStatut == StatutDemande.Approuvee
                    ? TypeNotification.DemandePermisApprouvee
                    : TypeNotification.DemandePermisRefusee,
                $"Votre demande {d.NumeroDemande} a été {(nouveauStatut == StatutDemande.Approuvee ? "approuvée" : "refusée")}.",
                d.Id.ToString(), "PermisBatir");
    }

    // ── Délivrance du permis ────────────────────────────────────────

    public async Task<PermisDelivreDto> DelivrerPermisAsync(
        int id, DelivrerPermisDto dto, string agentId)
    {
        var d = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        if (d.Statut != StatutDemande.Approuvee)
            throw new InvalidOperationException("Seules les demandes approuvées peuvent recevoir un permis.");

        if (d.PermisDelivre != null)
            throw new InvalidOperationException("Un permis a déjà été délivré pour cette demande.");

        var permis = new PermisDelivre
        {
            DemandeId = id,
            NumeroPermis = dto.NumeroPermis,
            DateDelivrance = dto.DateDelivrance,
            DateValidite = dto.DateValidite,
            Conditions = dto.Conditions,
            FichierPermis = dto.FichierPermis,
            DelivreParId = agentId
        };

        d.Suivis.Add(new SuiviPermis
        {
            DemandeId = d.Id,
            UtilisateurId = agentId,
            Commentaire = $"Permis N° {dto.NumeroPermis} délivré. Valide jusqu'au {dto.DateValidite:dd/MM/yyyy}.",
            VisibleCitoyen = true
        });

        d.PermisDelivre = permis;
        await _repo.UpdateAsync(d);

        await _notif.NotifierCitoyenAsync(
            d.DemandeurId.ToString(), TypeNotification.PermisDelivre,
            $"Votre permis {dto.NumeroPermis} est disponible.",
            d.Id.ToString(), "PermisBatir");

        return new PermisDelivreDto
        {
            Id = permis.Id,
            NumeroPermis = permis.NumeroPermis,
            DateDelivrance = permis.DateDelivrance,
            DateValidite = permis.DateValidite,
            Conditions = permis.Conditions,
            FichierPermis = permis.FichierPermis,
            EstRevoque = permis.EstRevoque
        };
    }

    // ── Taxes ───────────────────────────────────────────────────────

    public async Task AjouterTaxeAsync(int id, AjouterTaxeDto dto)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        d.Taxes.Add(new TaxePermis
        {
            DemandeId = id,
            TypeTaxeId = dto.TypeTaxeId,
            Montant = dto.Montant,
            Statut = StatutTaxe.EnAttente
        });
        await _repo.UpdateAsync(d);
    }

    public async Task PayerTaxeAsync(int id, PayerTaxeDto dto, string agentId)
    {
        var d = await _repo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        var taxe = d.Taxes.FirstOrDefault(t => t.Id == dto.TaxeId)
            ?? throw new KeyNotFoundException($"Taxe #{dto.TaxeId} introuvable.");

        taxe.Statut = StatutTaxe.Payee;
        taxe.DatePaiement = DateTime.UtcNow;
        taxe.NumeroRecu = dto.NumeroRecu;
        await _repo.UpdateAsync(d);
    }

    // ── Inspection ──────────────────────────────────────────────────

    public async Task AjouterInspectionAsync(int id, CreateInspectionDto dto, string agentId)
    {
        var d = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Demande #{id} introuvable.");

        d.Inspections.Add(new InspectionPermis
        {
            DemandeId = id,
            Objet = dto.Objet,
            DateInspection = dto.DateInspection,
            InspecteurId = agentId,
            NomInspecteur = dto.NomInspecteur,
            Observations = dto.Observations,
            ReservesEmises = dto.ReservesEmises,
            Conforme = dto.Conforme
        });

        await _repo.UpdateAsync(d);

        await _notif.NotifierCitoyenAsync(
            d.DemandeurId.ToString(), TypeNotification.InspectionProgrammee,
            $"Une inspection est programmée le {dto.DateInspection:dd/MM/yyyy} pour votre demande {d.NumeroDemande}.",
            d.Id.ToString(), "PermisBatir");
    }

    // ── Mapping privé ───────────────────────────────────────────────

    private static DemandePermisDetailDto MapToDetail(DemandePermisBatir d) => new()
    {
        Id = d.Id,
        NumeroDemande = d.NumeroDemande,
        DateDepot = d.DateDepot,
        TypeDemandeId = d.TypeDemandeId,
        TypeDemandeLibelle = d.TypeDemande?.Libelle ?? string.Empty,
        Statut = d.Statut.ToString(),
        DemandeurId = d.DemandeurId,
        DemandeurNomComplet = d.Demandeur?.NomComplet ?? string.Empty,
        DemandeurTelephone = d.Demandeur?.Telephone ?? string.Empty,
        ArchitecteId = d.ArchitecteId,
        ArchitecteNomComplet = d.Architecte?.NomComplet,
        ArchitecteNumeroOrdre = d.Architecte?.NumeroOrdre,
        AdresseProjet = d.AdresseProjet,
        NumeroParcelle = d.NumeroParcelle,
        SuperficieTerrain = d.SuperficieTerrain,
        SuperficieAConstruire = d.SuperficieAConstruire,
        NombreNiveaux = d.NombreNiveaux,
        TypeConstruction = d.TypeConstruction?.ToString(),
        CoutEstimatif = d.CoutEstimatif,
        ZonageId = d.ZonageId,
        ZonageLibelle = d.Zonage?.Libelle,
        Longitude = d.Longitude,
        Latitude = d.Latitude,
        ServiceInstructeurId = d.ServiceInstructeurId,
        ServiceInstructeurNom = d.ServiceInstructeur?.Nom,
        DateDebutInstruction = d.DateDebutInstruction,
        CommissionExamenId = d.CommissionExamenId,
        CommissionLibelle = d.CommissionExamen?.Libelle,
        DateDecision = d.DateDecision,
        MotifRejet = d.MotifRejet,
        ConditionsSpeciales = d.ConditionsSpeciales,
        Observations = d.Observations,
        TotalTaxes = d.Taxes?.Sum(t => t.Montant) ?? 0,
        ToutesLTaxesPayees = d.Taxes?.All(t => t.Statut != StatutTaxe.EnAttente) ?? true,
        NombreDocuments = d.Documents?.Count ?? 0,
        NumeroPermis = d.PermisDelivre?.NumeroPermis,
        DateDelivrance = d.PermisDelivre?.DateDelivrance,
        DateValiditePermis = d.PermisDelivre?.DateValidite,
        CreatedAt = d.CreatedAt,
        Documents = d.Documents?
            .Select(doc => new DocumentPermisDto
            {
                Id = doc.Id,
                TypeDocument = doc.TypeDocument,
                NomFichier = doc.NomFichier,
                TailleFichier = doc.TailleFichier,
                Statut = doc.Statut.ToString(),
                Observations = doc.Observations,
                EstObligatoire = doc.EstObligatoire,
                AjouteeParCitoyen = doc.AjouteeParCitoyen
            }).ToList() ?? new(),
        Taxes = d.Taxes?
            .Select(t => new TaxePermisDto
            {
                Id = t.Id,
                TypeTaxeLibelle = t.TypeTaxe?.Libelle ?? string.Empty,
                Montant = t.Montant,
                Statut = t.Statut.ToString(),
                DatePaiement = t.DatePaiement,
                NumeroRecu = t.NumeroRecu
            }).ToList() ?? new(),
        Suivis = d.Suivis?
            .OrderByDescending(s => s.DateChangement)
            .Select(s => new SuiviPermisDto
            {
                Id = s.Id,
                StatutPrecedent = s.StatutPrecedent?.ToString(),
                NouveauStatut = s.NouveauStatut?.ToString(),
                DateChangement = s.DateChangement,
                UtilisateurNom = s.UtilisateurNom,
                Commentaire = s.Commentaire,
                VisibleCitoyen = s.VisibleCitoyen
            }).ToList() ?? new(),
        Inspections = d.Inspections?
            .OrderByDescending(i => i.DateInspection)
            .Select(i => new InspectionPermisDto
            {
                Id = i.Id,
                Objet = i.Objet,
                DateInspection = i.DateInspection,
                NomInspecteur = i.NomInspecteur,
                Observations = i.Observations,
                ReservesEmises = i.ReservesEmises,
                Conforme = i.Conforme
            }).ToList() ?? new(),
        PermisDelivre = d.PermisDelivre != null ? new PermisDelivreDto
        {
            Id = d.PermisDelivre.Id,
            NumeroPermis = d.PermisDelivre.NumeroPermis,
            DateDelivrance = d.PermisDelivre.DateDelivrance,
            DateValidite = d.PermisDelivre.DateValidite,
            Conditions = d.PermisDelivre.Conditions,
            FichierPermis = d.PermisDelivre.FichierPermis,
            EstRevoque = d.PermisDelivre.EstRevoque,
            MotifRevocation = d.PermisDelivre.MotifRevocation
        } : null
    };
}