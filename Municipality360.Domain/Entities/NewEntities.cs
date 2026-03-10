// ═══════════════════════════════════════════════════════════════════
//  NewEntities.cs
//  Domain/Entities/NewEntities.cs
//  يحتوي على جميع entities للوحدات الجديدة:
//  Bureau d'Ordre · Réclamations · Permis de Bâtir · Notifications
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

// ════════════════════════════════════════════════════════════════
//  ENUMS COMMUNS
// ════════════════════════════════════════════════════════════════

public enum TypeDocumentBO    { Lettre, Note, Circulaire, Rapport, Decision, Arrete, Instruction, Demande, Recours, Facture, Autre }
public enum ModeReception     { Guichet, Messagerie, Email, Fax, Recommande, Coursier, PSEM }
public enum ModeEnvoi         { Guichet, Messagerie, Email, Fax, Recommande, Coursier }
public enum PrioriteCourrier  { Normal, Urgent, TresUrgent, Confidentiel }
public enum StatutEntrant     { Recu, Enregistre, EnCours, Traite, Archive, Rejete }
public enum StatutSortant     { Brouillon, EnValidation, Signe, Envoye, Archive, Annule }
public enum StatutDossierBO   { Ouvert, EnCours, Cloture, Suspendu }
public enum TypeActionCircuit { PourAction, PourDecision, PourInformation, PourSignature, PourSuite }
public enum StatutEtapeCircuit{ EnAttente, EnCours, Traite, Retourne }
public enum TypePieceJointeBO { DocumentPrincipal, Annexe, PieceIdentite, Autre }
public enum ClassificationArchive { Courant, Intermediaire, Definitif }
public enum TypeContact       { Personne, Organisme, Administration }

public enum SexeCitoyen       { Masculin, Feminin }
public enum SituationFamiliale{ Celibataire, Marie, Divorce, Veuf }
public enum StatutReclamation { Nouvelle, EnCours, Traitee, Rejetee, Fermee }
public enum PrioriteReclamation{ Basse, Moyenne, Haute, Critique }
public enum CanalReclamation  { Guichet, Telephone, Email, Web, Mobile }

public enum TypeDemandeur     { Personne, SocieteConstruction }
public enum TypeConstruction  { Residential, Commercial, Industriel, Mixte, Agricole }
public enum StatutDemande     { Deposee, EnExamen, Approuvee, Rejetee, Suspendue }
public enum StatutDocument    { Depose, Valide, AReviser }
public enum StatutTaxe        { EnAttente, Payee, Exoneree }
public enum StatutCommission  { Programmee, Tenue, Reportee, Annulee }

public enum TypeNotification  {
    NouveauCourrier, CourrierAssigne, CourrierTraite,
    NouvelleReclamation, ReclamationAssignee, ReclamationResolue, ReclamationRejetee,
    NouvelleDemandePermis, DemandePermisEnInstruction, DemandePermisApprouvee, DemandePermisRefusee,
    PermisDelivre, InspectionProgrammee
}
public enum CibleNotification { AgentInterne, Citoyen }

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE
// ════════════════════════════════════════════════════════════════

public class BOContact : BaseEntity
{
    public TypeContact TypeContact { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? RaisonSociale { get; set; }
    public string? Fonction { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? Wilaya { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public string NomComplet => TypeContact == TypeContact.Personne
        ? $"{Prenom} {Nom}".Trim()
        : RaisonSociale ?? Nom ?? string.Empty;
}

public class BOCategorieCourrier : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CouleurHex { get; set; }
    public bool EstConfidentiel { get; set; }
    public bool IsActive { get; set; } = true;
}

public class BODossier : BaseEntity
{
    public string NumeroDossier { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ServiceResponsableId { get; set; }
    public DateTime DateOuverture { get; set; } = DateTime.UtcNow;
    public DateTime? DateCloture { get; set; }
    public StatutDossierBO StatutDossier { get; set; } = StatutDossierBO.Ouvert;
    public bool IsActive { get; set; } = true;
    public string? CreatedById { get; set; }

    public Service? ServiceResponsable { get; set; }
    public ICollection<BOCourrierEntrant> CourriersEntrants { get; set; } = new List<BOCourrierEntrant>();
    public ICollection<BOCourrierSortant> CourriersSortants { get; set; } = new List<BOCourrierSortant>();
}

public class BOCourrierEntrant : BaseEntity
{
    public string NumeroOrdre { get; set; } = string.Empty;
    public string? NumeroExterne { get; set; }
    public DateTime DateReception { get; set; }
    public DateTime DateCourrier { get; set; }
    public string ObjetCourrier { get; set; } = string.Empty;
    public TypeDocumentBO TypeDocument { get; set; }
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public int? ExpediteurContactId { get; set; }
    public string? ExpediteurLibreNom { get; set; }
    public ModeReception ModeReception { get; set; }
    public string? NumeroRecommande { get; set; }
    public int? ServiceDestinataireId { get; set; }
    public string? AgentDestinataireId { get; set; }
    public short NombrePages { get; set; } = 1;
    public PrioriteCourrier Priorite { get; set; } = PrioriteCourrier.Normal;
    public bool EstConfidentiel { get; set; }
    public StatutEntrant Statut { get; set; } = StatutEntrant.Enregistre;
    public DateTime? DelaiReponse { get; set; }
    public bool NecessiteReponse { get; set; }
    public string? Observation { get; set; }
    public string? EnregistreParId { get; set; }

    public BOCategorieCourrier? Categorie { get; set; }
    public BODossier? Dossier { get; set; }
    public BOContact? ExpediteurContact { get; set; }
    public Service? ServiceDestinataire { get; set; }
    public ICollection<BOPieceJointeEntrant> PiecesJointes { get; set; } = new List<BOPieceJointeEntrant>();
    public ICollection<BOCircuitTraitement> Circuit { get; set; } = new List<BOCircuitTraitement>();
    public ICollection<BOCourrierSortant> Reponses { get; set; } = new List<BOCourrierSortant>();
    public BOArchive? Archive { get; set; }
}

public class BOPieceJointeEntrant : BaseEntity
{
    public int CourrierEntrantId { get; set; }
    public string NomFichierOriginal { get; set; } = string.Empty;
    public string NomFichierStocke { get; set; } = string.Empty;
    public string CheminFichier { get; set; } = string.Empty;
    public string ExtensionFichier { get; set; } = string.Empty;
    public long TailleFichierOctets { get; set; }
    public TypePieceJointeBO TypePiece { get; set; } = TypePieceJointeBO.Annexe;
    public short Ordre { get; set; } = 1;
    public string? Description { get; set; }
    public string UploadedById { get; set; } = string.Empty;

    public BOCourrierEntrant CourrierEntrant { get; set; } = null!;
}

public class BOCircuitTraitement : BaseEntity
{
    public int CourrierEntrantId { get; set; }
    public short NumeroEtape { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public string? AgentEmetteurId { get; set; }
    public int ServiceRecepteurId { get; set; }
    public string? AgentRecepteurId { get; set; }
    public DateTime DateTransmission { get; set; } = DateTime.UtcNow;
    public DateTime? DateTraitement { get; set; }
    public DateTime? DelaiTraitement { get; set; }
    public TypeActionCircuit TypeAction { get; set; }
    public string? InstructionTransmission { get; set; }
    public string? CommentaireTraitement { get; set; }
    public string? ActionEffectuee { get; set; }
    public StatutEtapeCircuit StatutEtape { get; set; } = StatutEtapeCircuit.EnAttente;
    public bool EstRetour { get; set; }
    public string? MotifRetour { get; set; }
    public string? CreatedById { get; set; }

    public BOCourrierEntrant CourrierEntrant { get; set; } = null!;
    public Service? ServiceEmetteur { get; set; }
    public Service ServiceRecepteur { get; set; } = null!;
}

public class BOCourrierSortant : BaseEntity
{
    public string NumeroOrdre { get; set; } = string.Empty;
    public string? NumeroReference { get; set; }
    public int? CourrierEntrantRefId { get; set; }
    public DateTime DateRedaction { get; set; } = DateTime.UtcNow;
    public DateTime? DateSignature { get; set; }
    public DateTime? DateEnvoi { get; set; }
    public string ObjetCourrier { get; set; } = string.Empty;
    public TypeDocumentBO TypeDocument { get; set; }
    public int? CategorieId { get; set; }
    public int? DossierId { get; set; }
    public int? ServiceEmetteurId { get; set; }
    public string? RedacteurId { get; set; }
    public string? SignataireId { get; set; }
    public string? FonctionSignataire { get; set; }
    public bool EstSigne { get; set; }
    public int? DestinataireContactId { get; set; }
    public string? DestinataireLibreNom { get; set; }
    public ModeEnvoi ModeEnvoi { get; set; }
    public string? NumeroRecommande { get; set; }
    public short NombrePages { get; set; } = 1;
    public bool EstConfidentiel { get; set; }
    public PrioriteCourrier Priorite { get; set; } = PrioriteCourrier.Normal;
    public StatutSortant Statut { get; set; } = StatutSortant.Brouillon;
    public bool AccuseReceptionRecu { get; set; }
    public DateTime? DateAccuseExtRecu { get; set; }
    public string? Observation { get; set; }
    public string? CreatedById { get; set; }

    public BOCourrierEntrant? CourrierEntrantRef { get; set; }
    public BOCategorieCourrier? Categorie { get; set; }
    public BODossier? Dossier { get; set; }
    public Service? ServiceEmetteur { get; set; }
    public BOContact? DestinataireContact { get; set; }
    public ICollection<BOPieceJointeSortant> PiecesJointes { get; set; } = new List<BOPieceJointeSortant>();
    public BOArchive? Archive { get; set; }
}

public class BOPieceJointeSortant : BaseEntity
{
    public int CourrierSortantId { get; set; }
    public string NomFichierOriginal { get; set; } = string.Empty;
    public string NomFichierStocke { get; set; } = string.Empty;
    public string CheminFichier { get; set; } = string.Empty;
    public string ExtensionFichier { get; set; } = string.Empty;
    public long TailleFichierOctets { get; set; }
    public TypePieceJointeBO TypePiece { get; set; } = TypePieceJointeBO.Annexe;
    public short Ordre { get; set; } = 1;
    public string? Description { get; set; }
    public bool EstVersionFinale { get; set; }
    public string UploadedById { get; set; } = string.Empty;

    public BOCourrierSortant CourrierSortant { get; set; } = null!;
}

public class BOArchive : BaseEntity
{
    public int? CourrierEntrantId { get; set; }
    public int? CourrierSortantId { get; set; }
    public string NumeroArchive { get; set; } = string.Empty;
    public string? CodeBarre { get; set; }
    public string? SalleArchive { get; set; }
    public string? Rayon { get; set; }
    public string? Boite { get; set; }
    public ClassificationArchive Classification { get; set; } = ClassificationArchive.Courant;
    public short DureeConservationAns { get; set; } = 10;
    public DateTime DateDebutConservation { get; set; }
    public DateTime? DateFinConservation { get; set; }
    public string? CheminArchiveNumerique { get; set; }
    public bool EstDetruit { get; set; }
    public string? ArchiveParId { get; set; }
    public string? Observation { get; set; }
    public DateTime DateArchivage { get; set; } = DateTime.UtcNow;

    public BOCourrierEntrant? CourrierEntrant { get; set; }
    public BOCourrierSortant? CourrierSortant { get; set; }
}

public class NumeroSequence : BaseEntity
{
    public string Prefixe { get; set; } = string.Empty;
    public int Annee { get; set; }
    public int DernierNumero { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS
// ════════════════════════════════════════════════════════════════

public class Citoyen : BaseEntity
{
    public string CIN { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public DateTime? DateNaissance { get; set; }
    public string? LieuNaissance { get; set; }
    public SexeCitoyen? Sexe { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? CodePostal { get; set; }
    public string Telephone { get; set; } = string.Empty;
    public string? TelephoneMobile { get; set; }
    public string? Email { get; set; }
    public SituationFamiliale? SituationFamiliale { get; set; }
    public bool IsActive { get; set; } = true;

    public string NomComplet => $"{Prenom} {Nom}".Trim();
    public ICollection<Reclamation> Reclamations { get; set; } = new List<Reclamation>();
}

public class TypeReclamation : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DelaiTraitementJours { get; set; } = 15;
    public int? ServiceResponsableId { get; set; }
    public bool IsActive { get; set; } = true;

    public Service? ServiceResponsable { get; set; }
}

public class CategorieReclamation : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Icone { get; set; }
    public string? CouleurHex { get; set; }
    public int? ParentId { get; set; }
    public int Niveau { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public CategorieReclamation? Parent { get; set; }
    public ICollection<CategorieReclamation> SousCategories { get; set; } = new List<CategorieReclamation>();
}

public class Reclamation : BaseEntity
{
    public string NumeroReclamation { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; } = DateTime.UtcNow;
    public DateTime? DateIncident { get; set; }
    public int TypeReclamationId { get; set; }
    public int CategorieId { get; set; }
    public PrioriteReclamation Priorite { get; set; } = PrioriteReclamation.Moyenne;
    public StatutReclamation Statut { get; set; } = StatutReclamation.Nouvelle;
    public int CitoyenId { get; set; }
    public bool EstAnonyme { get; set; }
    public CanalReclamation Canal { get; set; } = CanalReclamation.Guichet;
    public string Objet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Localisation { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int? ServiceConcerneId { get; set; }
    public string? AffecteAId { get; set; }
    public DateTime? DateAffectation { get; set; }
    public DateTime? DateCloture { get; set; }
    public string? SolutionApportee { get; set; }
    public int? SatisfactionCitoyen { get; set; }
    public string? EnregistreParId { get; set; }

    public TypeReclamation? TypeReclamation { get; set; }
    public CategorieReclamation? Categorie { get; set; }
    public Citoyen? Citoyen { get; set; }
    public Service? ServiceConcerne { get; set; }
    public ICollection<SuiviReclamation> Suivis { get; set; } = new List<SuiviReclamation>();
    public ICollection<PieceJointeReclamation> PiecesJointes { get; set; } = new List<PieceJointeReclamation>();
}

public class SuiviReclamation : BaseEntity
{
    public int ReclamationId { get; set; }
    public StatutReclamation? StatutPrecedent { get; set; }
    public StatutReclamation? NouveauStatut { get; set; }
    public DateTime DateChangement { get; set; } = DateTime.UtcNow;
    public string UtilisateurId { get; set; } = string.Empty;
    public string UtilisateurNom { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
    public string? ActionEffectuee { get; set; }
    public bool VisibleCitoyen { get; set; }

    public Reclamation Reclamation { get; set; } = null!;
}

public class PieceJointeReclamation : BaseEntity
{
    public int ReclamationId { get; set; }
    public string? TypeDocument { get; set; }
    public string NomFichier { get; set; } = string.Empty;
    public string CheminFichier { get; set; } = string.Empty;
    public long? TailleFichier { get; set; }
    public bool AjouteeParCitoyen { get; set; }
    public string UploadedById { get; set; } = string.Empty;

    public Reclamation Reclamation { get; set; } = null!;
}

// ════════════════════════════════════════════════════════════════
//  PERMIS DE BÂTIR
// ════════════════════════════════════════════════════════════════

public class ZonageUrbanisme : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? CoefficientOccupationSol { get; set; }
    public decimal? CoefficientUtilisationSol { get; set; }
    public decimal? HauteurMaximale { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TypeDemandePermis : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public int DelaiTraitementJours { get; set; } = 30;
    public decimal? TarifBase { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Demandeur : BaseEntity
{
    public TypeDemandeur Type { get; set; } = TypeDemandeur.Personne;
    public string? CIN { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Prenom { get; set; }
    public string? RaisonSociale { get; set; }
    public string Adresse { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public string NomComplet => Type == TypeDemandeur.Personne
        ? $"{Prenom} {Nom}".Trim()
        : RaisonSociale ?? Nom;

    public ICollection<DemandePermisBatir> Demandes { get; set; } = new List<DemandePermisBatir>();
}

public class Architecte : BaseEntity
{
    public string NumeroOrdre { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public string NomComplet => $"{Prenom} {Nom}".Trim();
}

public class CommissionExamen : BaseEntity
{
    public string Libelle { get; set; } = string.Empty;
    public DateTime? DateReunion { get; set; }
    public string PresidentId { get; set; } = string.Empty;
    public string? SecretaireId { get; set; }
    public StatutCommission StatutReunion { get; set; } = StatutCommission.Programmee;
    public string? ProcesVerbal { get; set; }
}

public class DemandePermisBatir : BaseEntity
{
    public string NumeroDemande { get; set; } = string.Empty;
    public DateTime DateDepot { get; set; } = DateTime.UtcNow;
    public int TypeDemandeId { get; set; }
    public int DemandeurId { get; set; }
    public StatutDemande Statut { get; set; } = StatutDemande.Deposee;
    public int? ArchitecteId { get; set; }
    public string AdresseProjet { get; set; } = string.Empty;
    public string? NumeroParcelle { get; set; }
    public decimal? SuperficieTerrain { get; set; }
    public decimal? SuperficieAConstruire { get; set; }
    public int? NombreNiveaux { get; set; }
    public TypeConstruction? TypeConstruction { get; set; }
    public decimal? CoutEstimatif { get; set; }
    public int? ZonageId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int? ServiceInstructeurId { get; set; }
    public string? AgentInstructeurId { get; set; }
    public DateTime? DateDebutInstruction { get; set; }
    public int? CommissionExamenId { get; set; }
    public DateTime? DateDecision { get; set; }
    public string? MotifRejet { get; set; }
    public string? ConditionsSpeciales { get; set; }
    public string? Observations { get; set; }
    public string? EnregistreParId { get; set; }

    public TypeDemandePermis? TypeDemande { get; set; }
    public Demandeur? Demandeur { get; set; }
    public Architecte? Architecte { get; set; }
    public ZonageUrbanisme? Zonage { get; set; }
    public CommissionExamen? CommissionExamen { get; set; }
    public Service? ServiceInstructeur { get; set; }
    public PermisDelivre? PermisDelivre { get; set; }
    public ICollection<DocumentPermis> Documents { get; set; } = new List<DocumentPermis>();
    public ICollection<TaxePermis> Taxes { get; set; } = new List<TaxePermis>();
    public ICollection<SuiviPermis> Suivis { get; set; } = new List<SuiviPermis>();
    public ICollection<InspectionPermis> Inspections { get; set; } = new List<InspectionPermis>();
}

public class DocumentPermis : BaseEntity
{
    public int DemandeId { get; set; }
    public string TypeDocument { get; set; } = string.Empty;
    public string NomFichier { get; set; } = string.Empty;
    public string CheminFichier { get; set; } = string.Empty;
    public long? TailleFichier { get; set; }
    public StatutDocument Statut { get; set; } = StatutDocument.Depose;
    public string? Observations { get; set; }
    public bool EstObligatoire { get; set; }
    public bool AjouteeParCitoyen { get; set; }
    public string UploadedById { get; set; } = string.Empty;

    public DemandePermisBatir Demande { get; set; } = null!;
}

public class PermisDelivre : BaseEntity
{
    public int DemandeId { get; set; }
    public string NumeroPermis { get; set; } = string.Empty;
    public DateTime DateDelivrance { get; set; }
    public DateTime DateValidite { get; set; }
    public string? Conditions { get; set; }
    public string? FichierPermis { get; set; }
    public string DelivreParId { get; set; } = string.Empty;
    public bool EstRevoque { get; set; }
    public string? MotifRevocation { get; set; }

    public DemandePermisBatir Demande { get; set; } = null!;
}

public class TypeTaxe : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public decimal? TauxCalcul { get; set; }
    public string? UniteCalcul { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TaxePermis : BaseEntity
{
    public int DemandeId { get; set; }
    public int TypeTaxeId { get; set; }
    public decimal Montant { get; set; }
    public StatutTaxe Statut { get; set; } = StatutTaxe.EnAttente;
    public DateTime? DatePaiement { get; set; }
    public string? NumeroRecu { get; set; }

    public DemandePermisBatir Demande { get; set; } = null!;
    public TypeTaxe? TypeTaxe { get; set; }
}

public class SuiviPermis : BaseEntity
{
    public int DemandeId { get; set; }
    public StatutDemande? StatutPrecedent { get; set; }
    public StatutDemande? NouveauStatut { get; set; }
    public DateTime DateChangement { get; set; } = DateTime.UtcNow;
    public string UtilisateurId { get; set; } = string.Empty;
    public string UtilisateurNom { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
    public bool VisibleCitoyen { get; set; }

    public DemandePermisBatir Demande { get; set; } = null!;
}

public class InspectionPermis : BaseEntity
{
    public int DemandeId { get; set; }
    public string Objet { get; set; } = string.Empty;
    public DateTime DateInspection { get; set; }
    public string? InspecteurId { get; set; }
    public string? NomInspecteur { get; set; }
    public string? Observations { get; set; }
    public string? ReservesEmises { get; set; }
    public bool Conforme { get; set; }

    public DemandePermisBatir Demande { get; set; } = null!;
}

// ════════════════════════════════════════════════════════════════
//  NOTIFICATIONS
// ════════════════════════════════════════════════════════════════

public class Notification : BaseEntity
{
    public TypeNotification Type { get; set; }
    public CibleNotification Cible { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? EntiteId { get; set; }
    public string? EntiteType { get; set; }
    public string? LienNavigation { get; set; }
    public string? DestinataireAgentId { get; set; }
    public string? DestinataireCitoyenId { get; set; }
    public bool EstLue { get; set; }
    public DateTime? DateLecture { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}
