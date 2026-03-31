// ═══════════════════════════════════════════════════════════════════
//  DbSeeder.cs  ✅ COMPLET
//  Infrastructure/Services/DbSeeder.cs
//
//  Données de test couvrant tous les modules:
//  ✅ Identity: 13 Roles + 6 utilisateurs
//  ✅ Structure: 5 Départements, 10 Services, 4 Postes, 10 Employés
//  ✅ Bureau d'Ordre: 8 Contacts, 5 Catégories, 4 Dossiers,
//                    6 Courriers Entrants, 3 Sortants, Circuit, Archives
//  ✅ Réclamations: 6 Types, 8 Catégories, 7 Citoyens, 7 Réclamations + Suivis
//  ✅ Permis de Bâtir: 5 Zonages, 5 Types, 4 Taxe Types, 3 Architectes,
//                      5 Demandeurs, 3 Commissions, 5 Demandes + Permis + Taxes + Inspections
//  ✅ Séquences numérotation (6 préfixes)
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;
using Municipality360.Infrastructure.Data.Training;
using Municipality360.Infrastructure.Identity;

namespace Municipality360.Infrastructure.Services;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        //await context.Database.EnsureCreatedAsync();
        await context.Database.MigrateAsync(); // ✅ هذا سيطبق الـ Migrations تلقائياً

        // ── IA training ────────────────────────────────────────────
        if (!File.Exists("Data/Training/training_complaints.csv"))
            await TrainingSamples.ExportToCsvAsync();
        // ── Roles ────────────────────────────────────────────────────
        string[] roles =
        {
            "SuperAdmin", "Admin", "Manager", "Employee", "Finances",
            "Urbanisme", "EtatCivil", "SIG", "BureauOrdre",
            "Reclamations", "Nettoyage", "Stock", "RessourcesHumaines"
        };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        // ── Utilisateurs ─────────────────────────────────────────────
        var usersData = new[]
        {
            ("ilyescherif21@gmail.com", "Ilyes21",   "Ilyes",   "Cherif",   "Admin@123456", "SuperAdmin"),
            ("zekrimourad1@gmail.com",  "MouradZ",  "Mourad", "Zekri", "Admin@123456", "Admin"),
            ("thouraya@gmail.com",      "thouraya",  "thouraya",  "kalabi",    "Agent@123456", "BureauOrdre"),
            ("mouhamedZgal@gmail.com", "Mouhamed", "Mouhamed",   "Zgal",  "Agent@123456", "Urbanisme"),
            ("Nadiahammemi@gmail.com",  "Nadia", "Nadia",   "Hammemi",     "Agent@123456", "Reclamations"),
            ("Hatem2019@gmail.com",   "Hatem", "Hatem",  "Hachicha",   "Agent@123456", "Finances"),
        };

        var createdUsers = new Dictionary<string, ApplicationUser>();
        foreach (var (email, username, prenom, nom, pwd, role) in usersData)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = prenom,
                    LastName = nom,
                    Email = email,
                    UserName = username,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, pwd);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    createdUsers[email] = user;
                }
            }
            else createdUsers[email] = existing;
        }

        var adminId = createdUsers.GetValueOrDefault("ilyescherif21@gmail.com")?.Id ?? "";
        var boId = createdUsers.GetValueOrDefault("bo@mairie.dz")?.Id ?? "";
        var urbId = createdUsers.GetValueOrDefault("urb@mairie.dz")?.Id ?? "";
        var recId = createdUsers.GetValueOrDefault("rec@mairie.dz")?.Id ?? "";

        // ════════════════════════════════════════════════════════════
        //  STRUCTURE ORGANISATIONNELLE
        // ════════════════════════════════════════════════════════════

        if (!context.Departements.IgnoreQueryFilters().Any())
        {
            var depts = new[]
            {
                new Departement { Nom = "الكاتب العام",   Code = "ADM", Description = "الإدارة العامة والتنسيق"   },
                new Departement { Nom = "إدارة الشؤون المالية",        Code = "FIN", Description = "الإدارة المالية والميزانية"     },
                new Departement { Nom = "الإدارة الفنية", Code = "URB", Description = "الأشغال وتراخيص البناء والتطوير"  },
                new Departement { Nom = "إدارة الشؤون الادارية",                Code = "ERH",  Description = "إدارة الشؤون الادارية"          },
            };
            await context.Departements.AddRangeAsync(depts);
            await context.SaveChangesAsync();
        }

        if (!context.Postes.IgnoreQueryFilters().Any())
        {
            await context.Postes.AddRangeAsync(
                new Poste { Titre = "مدير", Code = "DG", SalaireMin = 90000, SalaireMax = 130000 },
                new Poste { Titre = "رئيس مصلحة", Code = "CS", SalaireMin = 65000, SalaireMax = 95000 },
                new Poste { Titre = "الإطار الإداري", Code = "CA", SalaireMin = 45000, SalaireMax = 70000 },
                new Poste { Titre = "العامل الإداري", Code = "AA", SalaireMin = 28000, SalaireMax = 48000 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Services.IgnoreQueryFilters().Any())
        {
            var adm = context.Departements.IgnoreQueryFilters().FirstOrDefault(d => d.Code == "ADM")!;
            var fin = context.Departements.IgnoreQueryFilters().FirstOrDefault(d => d.Code == "FIN")!;
            var urb = context.Departements.IgnoreQueryFilters().FirstOrDefault(d => d.Code == "URB")!;
            var ERH = context.Departements.IgnoreQueryFilters().FirstOrDefault(d => d.Code == "ERH")!;

            await context.Services.AddRangeAsync(
                new Service { Nom = "الكاتب العام", Code = "ADM-SEC", DepartementId = adm.Id },
                new Service { Nom = "مكتب الضبط", Code = "ADM-BO", DepartementId = adm.Id },
                new Service { Nom = "الإعلامية والأرشيف", Code = "ADM-INF", DepartementId = adm.Id },
                new Service { Nom = "الحسابية", Code = "FIN-CPT", DepartementId = fin.Id },
                new Service { Nom = "المزانية", Code = "FIN-BDG", DepartementId = fin.Id },
                new Service { Nom = "النزاعات والملك البلديوالأسواق", Code = "FIN-SM", DepartementId = fin.Id },
                new Service { Nom = "الصفقات", Code = "FIN-CON", DepartementId = fin.Id },
                new Service { Nom = "التأجير", Code = "FIN-PY", DepartementId = fin.Id },
                new Service { Nom = "رخصة بناء", Code = "URB-PC", DepartementId = urb.Id },
                new Service { Nom = "الأشغال", Code = "URB-CAD", DepartementId = urb.Id },
                new Service { Nom = "النظافة العامة والصحة", Code = "ENV-PRO", DepartementId = urb.Id },
                new Service { Nom = "الموارد البشرية", Code = "ADM-RH", DepartementId = ERH.Id },
                new Service { Nom = "الحالة المدنية", Code = "EC-NM", DepartementId = ERH.Id }
                
            );
            await context.SaveChangesAsync();
        }

        if (!context.Employes.IgnoreQueryFilters().Any())
        {
            var sec = context.Services.IgnoreQueryFilters().First(s => s.Code == "ADM-SEC");
            var rh = context.Services.IgnoreQueryFilters().First(s => s.Code == "ADM-RH");
            var bo = context.Services.IgnoreQueryFilters().First(s => s.Code == "ADM-BO");
            var cpt = context.Services.IgnoreQueryFilters().First(s => s.Code == "FIN-CPT");
            var pc = context.Services.IgnoreQueryFilters().First(s => s.Code == "URB-PC");

            var dg = context.Postes.IgnoreQueryFilters().First(p => p.Code == "DG");
            var cs = context.Postes.IgnoreQueryFilters().First(p => p.Code == "CS");
            var ca = context.Postes.IgnoreQueryFilters().First(p => p.Code == "CA");
            var aa = context.Postes.IgnoreQueryFilters().First(p => p.Code == "AA");

            await context.Employes.AddRangeAsync(
                new Employe { Identifiant = "EMP001", Cin = "18273645", Nom = "Bensalem", Prenom = "Mohamed", ServiceId = sec.Id, PosteId = dg.Id, Salaire = 120000, Genre = Genre.Masculin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2015, 3, 1), Email = "m.bensalem@mairie.dz", Telephone = "0551234567" },
                new Employe { Identifiant = "EMP002", Cin = "19384756", Nom = "Ouali", Prenom = "Fatima", ServiceId = bo.Id, PosteId = cs.Id, Salaire = 78000, Genre = Genre.Feminin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2018, 6, 15), Email = "f.ouali@mairie.dz", Telephone = "0662345678" },
                new Employe { Identifiant = "EMP003", Cin = "20495867", Nom = "Meziane", Prenom = "Karim", ServiceId = pc.Id, PosteId = cs.Id, Salaire = 82000, Genre = Genre.Masculin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2016, 9, 1), Email = "k.meziane@mairie.dz", Telephone = "0773456789" },
                new Employe { Identifiant = "EMP004", Cin = "17162738", Nom = "Hadj", Prenom = "Amina", ServiceId = sec.Id, PosteId = ca.Id, Salaire = 55000, Genre = Genre.Feminin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2020, 1, 10), Email = "a.hadj@mairie.dz", Telephone = "0554567890" },
                new Employe { Identifiant = "EMP005", Cin = "16051627", Nom = "Belaid", Prenom = "Youcef", ServiceId = cpt.Id, PosteId = ca.Id, Salaire = 60000, Genre = Genre.Masculin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2019, 4, 5), Email = "y.belaid@mairie.dz", Telephone = "0665678901" },
                new Employe { Identifiant = "EMP006", Cin = "21506172", Nom = "Rahmani", Prenom = "Nadia", ServiceId = rh.Id, PosteId = ca.Id, Salaire = 52000, Genre = Genre.Feminin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2021, 8, 20), Email = "n.rahmani@mairie.dz", Telephone = "0776789012" },
                new Employe { Identifiant = "EMP007", Cin = "15394857", Nom = "Cherif", Prenom = "Sofiane", ServiceId = bo.Id, PosteId = aa.Id, Salaire = 38000, Genre = Genre.Masculin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2022, 2, 14), Email = "s.cherif@mairie.dz", Telephone = "0557890123" },
                new Employe { Identifiant = "EMP008", Cin = "14283746", Nom = "Bouazza", Prenom = "Lynda", ServiceId = pc.Id, PosteId = aa.Id, Salaire = 36000, Genre = Genre.Feminin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2023, 3, 1), Email = "l.bouazza@mairie.dz", Telephone = "0668901234" },
                new Employe { Identifiant = "EMP009", Cin = "22617384", Nom = "Khelil", Prenom = "Rachid", ServiceId = cpt.Id, PosteId = aa.Id, Salaire = 34000, Genre = Genre.Masculin, Statut = StatutEmploye.Suspendu, DateEmbauche = new DateTime(2022, 7, 1), Email = "r.khelil@mairie.dz", Telephone = "0779012345" },
                new Employe { Identifiant = "EMP010", Cin = "23728495", Nom = "Mazouz", Prenom = "Sara", ServiceId = sec.Id, PosteId = aa.Id, Salaire = 32000, Genre = Genre.Feminin, Statut = StatutEmploye.Actif, DateEmbauche = new DateTime(2023, 9, 15), Email = "s.mazouz@mairie.dz", Telephone = "0550123456" }
            );
            await context.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════
        //  BUREAU D'ORDRE
        // ════════════════════════════════════════════════════════════

        if (!context.BOCategoriesCourrier.IgnoreQueryFilters().Any())
        {
            await context.BOCategoriesCourrier.AddRangeAsync(
                new BOCategorieCourrier { Code = "ADMIN", Libelle = "Courrier Administratif", CouleurHex = "#3B82F6", EstConfidentiel = false },
                new BOCategorieCourrier { Code = "LEGAL", Libelle = "Courrier Juridique", CouleurHex = "#EF4444", EstConfidentiel = true },
                new BOCategorieCourrier { Code = "CITOYEN", Libelle = "Demande Citoyenne", CouleurHex = "#10B981", EstConfidentiel = false },
                new BOCategorieCourrier { Code = "FINANCE", Libelle = "Financier et Comptable", CouleurHex = "#F59E0B", EstConfidentiel = true },
                new BOCategorieCourrier { Code = "TECH", Libelle = "Technique et Travaux", CouleurHex = "#8B5CF6", EstConfidentiel = false }
            );
            await context.SaveChangesAsync();
        }

        if (!context.BOContacts.IgnoreQueryFilters().Any())
        {
            await context.BOContacts.AddRangeAsync(
                new BOContact { TypeContact = TypeContact.Administration, RaisonSociale = "Wilaya d'Alger", Ville = "Alger", Telephone = "023211000", Email = "courrier@wilaya-alger.dz" },
                new BOContact { TypeContact = TypeContact.Administration, RaisonSociale = "Ministère de l'Intérieur", Ville = "Alger", Telephone = "023591000", Email = "info@interieur.gov.dz" },
                new BOContact { TypeContact = TypeContact.Organisme, RaisonSociale = "Trésor Public", Ville = "Alger", Telephone = "023441000", Email = "tresor@finances.gov.dz" },
                new BOContact { TypeContact = TypeContact.Organisme, RaisonSociale = "Caisse Nationale Retraites", Ville = "Alger", Telephone = "023881000", Email = "cnr@cnr.dz" },
                new BOContact { TypeContact = TypeContact.Personne, Nom = "Benali", Prenom = "Omar", Fonction = "Avocat", Telephone = "0661234567", Email = "o.benali@avocat.dz" },
                new BOContact { TypeContact = TypeContact.Personne, Nom = "Mebarki", Prenom = "Samira", Fonction = "Notaire", Telephone = "0772345678", Email = "s.mebarki@notaire.dz" },
                new BOContact { TypeContact = TypeContact.Organisme, RaisonSociale = "SONELGAZ", Ville = "Alger", Telephone = "023771000", Email = "relation@sonelgaz.dz" },
                new BOContact { TypeContact = TypeContact.Organisme, RaisonSociale = "ADE - Algérienne Des Eaux", Ville = "Alger", Telephone = "023661000", Email = "ade@ade.dz" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.BODossiers.IgnoreQueryFilters().Any())
        {
            var boSvcId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ADM-BO").Select(x => x.Id).FirstOrDefault();
            await context.BODossiers.AddRangeAsync(
                new BODossier { NumeroDossier = "DOS-2025-001", Intitule = "Correspondances Wilaya 2025", ServiceResponsableId = boSvcId, StatutDossier = StatutDossierBO.EnCours, CreatedById = boId },
                new BODossier { NumeroDossier = "DOS-2025-002", Intitule = "Dossiers Contentieux Juridiques", ServiceResponsableId = boSvcId, StatutDossier = StatutDossierBO.Ouvert, CreatedById = adminId },
                new BODossier { NumeroDossier = "DOS-2025-003", Intitule = "Budget Supplémentaire 2025", ServiceResponsableId = boSvcId, StatutDossier = StatutDossierBO.EnCours, CreatedById = boId },
                new BODossier { NumeroDossier = "DOS-2024-012", Intitule = "Travaux Voirie 2024", ServiceResponsableId = boSvcId, StatutDossier = StatutDossierBO.Cloture, CreatedById = boId }
            );
            await context.SaveChangesAsync();
        }

        if (!context.NumeroSequences.IgnoreQueryFilters().Any())
        {
            var y = DateTime.UtcNow.Year;
            await context.NumeroSequences.AddRangeAsync(
                new NumeroSequence { Prefixe = "ENT", Annee = y, DernierNumero = 42 },
                new NumeroSequence { Prefixe = "SRT", Annee = y, DernierNumero = 28 },
                new NumeroSequence { Prefixe = "ARC", Annee = y, DernierNumero = 15 },
                new NumeroSequence { Prefixe = "REC", Annee = y, DernierNumero = 67 },
                new NumeroSequence { Prefixe = "PB", Annee = y, DernierNumero = 18 },
                new NumeroSequence { Prefixe = "PC", Annee = y, DernierNumero = 12 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.BOCourriersEntrants.IgnoreQueryFilters().Any())
        {
            var y = DateTime.UtcNow.Year;
            var cat1 = context.BOCategoriesCourrier.IgnoreQueryFilters().FirstOrDefault(c => c.Code == "ADMIN")!;
            var cat2 = context.BOCategoriesCourrier.IgnoreQueryFilters().FirstOrDefault(c => c.Code == "CITOYEN")!;
            var cat3 = context.BOCategoriesCourrier.IgnoreQueryFilters().FirstOrDefault(c => c.Code == "LEGAL")!;
            var ct1 = context.BOContacts.IgnoreQueryFilters().FirstOrDefault(c => c.RaisonSociale == "Wilaya d'Alger")!;
            var ct2 = context.BOContacts.IgnoreQueryFilters().FirstOrDefault(c => c.RaisonSociale == "Ministère de l'Intérieur")!;
            var dos1 = context.BODossiers.IgnoreQueryFilters().FirstOrDefault(d => d.NumeroDossier == "DOS-2025-001")!;
            var secId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ADM-SEC").Select(x => x.Id).FirstOrDefault();
            var boSvcId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ADM-BO").Select(x => x.Id).FirstOrDefault();
            var pcSvcId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "URB-PC").Select(x => x.Id).FirstOrDefault();

            var entrants = new[]
            {
                new BOCourrierEntrant { NumeroOrdre=$"ENT-{y}-00001", NumeroExterne="W/ALG/2025/1547",   DateReception=DateTime.UtcNow.AddDays(-30), DateCourrier=DateTime.UtcNow.AddDays(-32), ObjetCourrier="Circulaire modernisation des services administratifs",            TypeDocument=TypeDocumentBO.Circulaire,   CategorieId=cat1.Id, DossierId=dos1.Id, ExpediteurContactId=ct1.Id, ModeReception=ModeReception.Recommande, ServiceDestinataireId=secId,    NombrePages=8,  Priorite=PrioriteCourrier.Normal,    Statut=StatutEntrant.Traite,      NecessiteReponse=true,  DelaiReponse=DateTime.UtcNow.AddDays(-10), EnregistreParId=boId },
                new BOCourrierEntrant { NumeroOrdre=$"ENT-{y}-00002", NumeroExterne="MI/2025/889",       DateReception=DateTime.UtcNow.AddDays(-20), DateCourrier=DateTime.UtcNow.AddDays(-22), ObjetCourrier="Instruction relative au recensement de la population",              TypeDocument=TypeDocumentBO.Instruction,  CategorieId=cat1.Id,              ExpediteurContactId=ct2.Id, ModeReception=ModeReception.Email,       ServiceDestinataireId=boSvcId,  NombrePages=4,  Priorite=PrioriteCourrier.Urgent,    Statut=StatutEntrant.EnCours,     NecessiteReponse=false, EnregistreParId=boId },
                new BOCourrierEntrant { NumeroOrdre=$"ENT-{y}-00003",                                    DateReception=DateTime.UtcNow.AddDays(-15), DateCourrier=DateTime.UtcNow.AddDays(-15), ObjetCourrier="Demande certificat de résidence — M. Amrani Hocine",              TypeDocument=TypeDocumentBO.Demande,      CategorieId=cat2.Id,              ExpediteurLibreNom="Amrani Hocine",         ModeReception=ModeReception.Guichet,     ServiceDestinataireId=secId,    NombrePages=2,  Priorite=PrioriteCourrier.Normal,    Statut=StatutEntrant.Traite,      NecessiteReponse=true,  EnregistreParId=boId },
                new BOCourrierEntrant { NumeroOrdre=$"ENT-{y}-00004", NumeroExterne="REF-LEGAL-2025-44", DateReception=DateTime.UtcNow.AddDays(-10), DateCourrier=DateTime.UtcNow.AddDays(-12), ObjetCourrier="Mise en demeure — occupation illicite du domaine public",          TypeDocument=TypeDocumentBO.Lettre,       CategorieId=cat3.Id,              ExpediteurLibreNom="Cabinet Benali",        ModeReception=ModeReception.Recommande,  ServiceDestinataireId=secId,    NombrePages=5,  Priorite=PrioriteCourrier.TresUrgent,Statut=StatutEntrant.EnCours,     NecessiteReponse=true,  DelaiReponse=DateTime.UtcNow.AddDays(5), EstConfidentiel=true, EnregistreParId=boId },
                new BOCourrierEntrant { NumeroOrdre=$"ENT-{y}-00005",                                    DateReception=DateTime.UtcNow.AddDays(-5),  DateCourrier=DateTime.UtcNow.AddDays(-5),  ObjetCourrier="Plainte nuisances sonores — Quartier El Amir",                    TypeDocument=TypeDocumentBO.Recours,      CategorieId=cat2.Id,              ExpediteurLibreNom="Association Résidents El Amir", ModeReception=ModeReception.Guichet, ServiceDestinataireId=boSvcId,  NombrePages=3,  Priorite=PrioriteCourrier.Normal,    Statut=StatutEntrant.Enregistre,  NecessiteReponse=true,  EnregistreParId=boId },
                new BOCourrierEntrant { NumeroOrdre=$"ENT-{y}-00006",                                    DateReception=DateTime.UtcNow.AddDays(-2),  DateCourrier=DateTime.UtcNow.AddDays(-3),  ObjetCourrier="Facture entretien espaces verts — Lot 12",                         TypeDocument=TypeDocumentBO.Facture,      CategorieId=cat2.Id,              ExpediteurLibreNom="SARL VERDURE",          ModeReception=ModeReception.Email,       ServiceDestinataireId=boSvcId,  NombrePages=2,  Priorite=PrioriteCourrier.Normal,    Statut=StatutEntrant.Enregistre,  NecessiteReponse=false, EnregistreParId=boId },
            };
            await context.BOCourriersEntrants.AddRangeAsync(entrants);
            await context.SaveChangesAsync();

            var e1 = context.BOCourriersEntrants.IgnoreQueryFilters().FirstOrDefault(c => c.NumeroOrdre == $"ENT-{y}-00001")!;
            var e2 = context.BOCourriersEntrants.IgnoreQueryFilters().FirstOrDefault(c => c.NumeroOrdre == $"ENT-{y}-00002")!;
            var e3 = context.BOCourriersEntrants.IgnoreQueryFilters().FirstOrDefault(c => c.NumeroOrdre == $"ENT-{y}-00003")!;

            // Courriers sortants
            await context.BOCourriersSortants.AddRangeAsync(
                new BOCourrierSortant { NumeroOrdre = $"SRT-{y}-00001", NumeroReference = $"COM/SEC/{y}/001", CourrierEntrantRefId = e1.Id, DateRedaction = DateTime.UtcNow.AddDays(-28), DateSignature = DateTime.UtcNow.AddDays(-27), DateEnvoi = DateTime.UtcNow.AddDays(-26), ObjetCourrier = "Accusé de réception — Circulaire modernisation", TypeDocument = TypeDocumentBO.Lettre, CategorieId = cat1.Id, DossierId = dos1.Id, ServiceEmetteurId = secId, RedacteurId = boId, SignataireId = adminId, FonctionSignataire = "Président d'APC", EstSigne = true, DestinataireContactId = ct1.Id, ModeEnvoi = ModeEnvoi.Recommande, Priorite = PrioriteCourrier.Normal, Statut = StatutSortant.Envoye, AccuseReceptionRecu = true, CreatedById = boId },
                new BOCourrierSortant { NumeroOrdre = $"SRT-{y}-00002", NumeroReference = $"COM/SEC/{y}/002", DateRedaction = DateTime.UtcNow.AddDays(-18), DateSignature = DateTime.UtcNow.AddDays(-17), DateEnvoi = DateTime.UtcNow.AddDays(-16), ObjetCourrier = "Rapport mensuel d'activités — Janvier 2025", TypeDocument = TypeDocumentBO.Rapport, CategorieId = cat1.Id, ServiceEmetteurId = secId, RedacteurId = boId, SignataireId = adminId, FonctionSignataire = "Secrétaire Général", EstSigne = true, DestinataireContactId = ct1.Id, ModeEnvoi = ModeEnvoi.Email, Priorite = PrioriteCourrier.Normal, Statut = StatutSortant.Envoye, CreatedById = boId },
                new BOCourrierSortant { NumeroOrdre = $"SRT-{y}-00003", CourrierEntrantRefId = e3.Id, DateRedaction = DateTime.UtcNow.AddDays(-14), DateSignature = DateTime.UtcNow.AddDays(-13), DateEnvoi = DateTime.UtcNow.AddDays(-12), ObjetCourrier = "Réponse demande certificat résidence — Amrani Hocine", TypeDocument = TypeDocumentBO.Lettre, CategorieId = cat2.Id, ServiceEmetteurId = secId, RedacteurId = boId, SignataireId = adminId, FonctionSignataire = "Secrétaire Général", EstSigne = true, DestinataireLibreNom = "Amrani Hocine", ModeEnvoi = ModeEnvoi.Guichet, Priorite = PrioriteCourrier.Normal, Statut = StatutSortant.Envoye, CreatedById = boId }
            );
            await context.SaveChangesAsync();

            // Circuit
            await context.BOCircuits.AddRangeAsync(
                new BOCircuitTraitement { CourrierEntrantId = e2.Id, NumeroEtape = 1, ServiceEmetteurId = boSvcId, AgentEmetteurId = boId, ServiceRecepteurId = secId, TypeAction = TypeActionCircuit.PourAction, InstructionTransmission = "Prendre connaissance et diffuser.", StatutEtape = StatutEtapeCircuit.Traite, DateTraitement = DateTime.UtcNow.AddDays(-18), CommentaireTraitement = "Transmis au Secrétariat.", CreatedById = boId },
                new BOCircuitTraitement { CourrierEntrantId = e2.Id, NumeroEtape = 2, ServiceEmetteurId = secId, AgentEmetteurId = adminId, ServiceRecepteurId = pcSvcId, TypeAction = TypeActionCircuit.PourInformation, InstructionTransmission = "Pour mise en application.", StatutEtape = StatutEtapeCircuit.EnAttente, CreatedById = adminId }
            );

            // Archives
            var s1 = context.BOCourriersSortants.IgnoreQueryFilters().FirstOrDefault(c => c.NumeroOrdre == $"SRT-{y}-00001")!;
            await context.BOArchives.AddRangeAsync(
                new BOArchive { CourrierEntrantId = e1.Id, NumeroArchive = $"ARC-{y}-00001", SalleArchive = "Salle A", Rayon = "R03", Boite = "B-2025-01", Classification = ClassificationArchive.Courant, DureeConservationAns = 10, DateDebutConservation = DateTime.UtcNow.AddDays(-25), DateFinConservation = DateTime.UtcNow.AddDays(-25).AddYears(10), ArchiveParId = boId, Observation = "Archivé après traitement complet." },
                new BOArchive { CourrierSortantId = s1.Id, NumeroArchive = $"ARC-{y}-00002", SalleArchive = "Salle A", Rayon = "R03", Boite = "B-2025-01", Classification = ClassificationArchive.Courant, DureeConservationAns = 10, DateDebutConservation = DateTime.UtcNow.AddDays(-24), DateFinConservation = DateTime.UtcNow.AddDays(-24).AddYears(10), ArchiveParId = boId }
            );
            await context.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════
        //  RÉCLAMATIONS
        // ════════════════════════════════════════════════════════════

        if (!context.TypesReclamation.IgnoreQueryFilters().Any())
        {
            var envId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ENV-PRO").Select(x => x.Id).FirstOrDefault();
            var pcId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "URB-PC").Select(x => x.Id).FirstOrDefault();
            var secId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ADM-SEC").Select(x => x.Id).FirstOrDefault();

            await context.TypesReclamation.AddRangeAsync(
                new TypeReclamation { Code = "VOIRIE", Libelle = "Voirie et Infrastructure", DelaiTraitementJours = 10, ServiceResponsableId = envId },
                new TypeReclamation { Code = "HYGIENE", Libelle = "Hygiène et Propreté", DelaiTraitementJours = 7, ServiceResponsableId = envId },
                new TypeReclamation { Code = "ECLAIRAGE", Libelle = "Éclairage Public", DelaiTraitementJours = 5, ServiceResponsableId = envId },
                new TypeReclamation { Code = "URBANISME", Libelle = "Urbanisme et Construction", DelaiTraitementJours = 20, ServiceResponsableId = pcId },
                new TypeReclamation { Code = "BRUIT", Libelle = "Nuisances Sonores", DelaiTraitementJours = 7, ServiceResponsableId = envId },
                new TypeReclamation { Code = "ADMIN", Libelle = "Service Administratif", DelaiTraitementJours = 15, ServiceResponsableId = secId }
            );
            await context.SaveChangesAsync();
        }

        if (!context.CategoriesReclamation.IgnoreQueryFilters().Any())
        {
            await context.CategoriesReclamation.AddRangeAsync(
                new CategorieReclamation { Code = "NID-POULE", Libelle = "Nids de Poule", CouleurHex = "#F59E0B", Niveau = 1 },
                new CategorieReclamation { Code = "TROTTOIR", Libelle = "Trottoir Dégradé", CouleurHex = "#F59E0B", Niveau = 1 },
                new CategorieReclamation { Code = "DECHETS", Libelle = "Dépôt Sauvage", CouleurHex = "#EF4444", Niveau = 1 },
                new CategorieReclamation { Code = "LAMPADAIRE", Libelle = "Lampadaire En Panne", CouleurHex = "#6B7280", Niveau = 1 },
                new CategorieReclamation { Code = "CONSTRUCTION-ILL", Libelle = "Construction Illicite", CouleurHex = "#8B5CF6", Niveau = 1 },
                new CategorieReclamation { Code = "BRUIT-NUIT", Libelle = "Bruit Nocturne", CouleurHex = "#3B82F6", Niveau = 1 },
                new CategorieReclamation { Code = "BRUIT-CHANT", Libelle = "Bruit de Chantier", CouleurHex = "#3B82F6", Niveau = 1 },
                new CategorieReclamation { Code = "DOC-LENT", Libelle = "Lenteur Administrative", CouleurHex = "#10B981", Niveau = 1 }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Citoyens.IgnoreQueryFilters().Any())
        {
            await context.Citoyens.AddRangeAsync(
                new Citoyen { CIN = "10283746", Nom = "Amrani", Prenom = "Hocine", DateNaissance = new DateTime(1978, 5, 12), Sexe = SexeCitoyen.Masculin, Adresse = "12 Rue Ben Badis", Ville = "Alger", CodePostal = "16000", Telephone = "0558123456", Email = "h.amrani@gmail.com", SituationFamiliale = SituationFamiliale.Marie },
                new Citoyen { CIN = "11394857", Nom = "Kaddour", Prenom = "Yasmine", DateNaissance = new DateTime(1985, 8, 22), Sexe = SexeCitoyen.Feminin, Adresse = "5 Cité des Fleurs", Ville = "Alger", CodePostal = "16001", Telephone = "0667234567", Email = "y.kaddour@gmail.com", SituationFamiliale = SituationFamiliale.Celibataire },
                new Citoyen { CIN = "12405968", Nom = "Berrahal", Prenom = "Mourad", DateNaissance = new DateTime(1972, 3, 5), Sexe = SexeCitoyen.Masculin, Adresse = "8 Bd Colonel Amirouche", Ville = "Alger", CodePostal = "16002", Telephone = "0778345678", Email = "m.berrahal@gmail.com", SituationFamiliale = SituationFamiliale.Marie },
                new Citoyen { CIN = "13516079", Nom = "Haddad", Prenom = "Meriem", DateNaissance = new DateTime(1990, 11, 18), Sexe = SexeCitoyen.Feminin, Adresse = "20 Rue Didouche Mourad", Ville = "Alger", CodePostal = "16003", Telephone = "0559456789", Email = "m.haddad@gmail.com", SituationFamiliale = SituationFamiliale.Celibataire },
                new Citoyen { CIN = "14627180", Nom = "Ziani", Prenom = "Abdelkader", DateNaissance = new DateTime(1965, 7, 30), Sexe = SexeCitoyen.Masculin, Adresse = "3 Cité ZHUN Ouest", Ville = "Alger", CodePostal = "16004", Telephone = "0660567890", Email = "a.ziani@gmail.com", SituationFamiliale = SituationFamiliale.Marie },
                new Citoyen { CIN = "15738291", Nom = "Ferhat", Prenom = "Nadia", DateNaissance = new DateTime(1982, 2, 14), Sexe = SexeCitoyen.Feminin, Adresse = "15 Rue Ali Boumendjel", Ville = "Alger", CodePostal = "16005", Telephone = "0771678901", Email = "n.ferhat@gmail.com", SituationFamiliale = SituationFamiliale.Divorce },
                new Citoyen { CIN = "16849302", Nom = "Laib", Prenom = "Rachid", DateNaissance = new DateTime(1958, 9, 9), Sexe = SexeCitoyen.Masculin, Adresse = "7 Avenue 1er Novembre", Ville = "Alger", CodePostal = "16006", Telephone = "0552789012", Email = "r.laib@gmail.com", SituationFamiliale = SituationFamiliale.Veuf }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Reclamations.IgnoreQueryFilters().Any())
        {
            var y = DateTime.UtcNow.Year;
            var envId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ENV-PRO").Select(x => x.Id).FirstOrDefault();
            var pcId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "URB-PC").Select(x => x.Id).FirstOrDefault();
            var secId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "ADM-SEC").Select(x => x.Id).FirstOrDefault();

            var tV = context.TypesReclamation.IgnoreQueryFilters().Where(t => t.Code == "VOIRIE").Select(x => x.Id).FirstOrDefault();
            var tH = context.TypesReclamation.IgnoreQueryFilters().Where(t => t.Code == "HYGIENE").Select(x => x.Id).FirstOrDefault();
            var tE = context.TypesReclamation.IgnoreQueryFilters().Where(t => t.Code == "ECLAIRAGE").Select(x => x.Id).FirstOrDefault();
            var tU = context.TypesReclamation.IgnoreQueryFilters().Where(t => t.Code == "URBANISME").Select(x => x.Id).FirstOrDefault();
            var tB = context.TypesReclamation.IgnoreQueryFilters().Where(t => t.Code == "BRUIT").Select(x => x.Id).FirstOrDefault();
            var tA = context.TypesReclamation.IgnoreQueryFilters().Where(t => t.Code == "ADMIN").Select(x => x.Id).FirstOrDefault();

            var cN = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "NID-POULE").Select(x => x.Id).FirstOrDefault();
            var cD = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "DECHETS").Select(x => x.Id).FirstOrDefault();
            var cL = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "LAMPADAIRE").Select(x => x.Id).FirstOrDefault();
            var cC = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "CONSTRUCTION-ILL").Select(x => x.Id).FirstOrDefault();
            var cB = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "BRUIT-NUIT").Select(x => x.Id).FirstOrDefault();
            var cDoc = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "DOC-LENT").Select(x => x.Id).FirstOrDefault();
            var cT = context.CategoriesReclamation.IgnoreQueryFilters().Where(c => c.Code == "TROTTOIR").Select(x => x.Id).FirstOrDefault();

            var cits = context.Citoyens.IgnoreQueryFilters().ToList();

            var recs = new[]
            {
                new Reclamation { NumeroReclamation=$"REC-{y}-00001", DateDepot=DateTime.UtcNow.AddDays(-45), DateIncident=DateTime.UtcNow.AddDays(-46), TypeReclamationId=tV, CategorieId=cN, Priorite=PrioriteReclamation.Haute,    Statut=StatutReclamation.Traitee, CitoyenId=cits[0].Id, Canal=CanalReclamation.Guichet,    Objet="Nid de poule — Rue Krim Belkacem",              Description="Grand nid de poule causant des accidents.", Localisation="Rue Krim Belkacem n°12-18",     ServiceConcerneId=envId, AffecteAId=recId, DateAffectation=DateTime.UtcNow.AddDays(-44), DateCloture=DateTime.UtcNow.AddDays(-30), SolutionApportee="Bouchage réalisé le 10/01/2025.", SatisfactionCitoyen=4, EnregistreParId=recId },
                new Reclamation { NumeroReclamation=$"REC-{y}-00002", DateDepot=DateTime.UtcNow.AddDays(-30),                                            TypeReclamationId=tH, CategorieId=cD, Priorite=PrioriteReclamation.Critique,  Statut=StatutReclamation.EnCours, CitoyenId=cits[1].Id, Canal=CanalReclamation.Mobile,     Objet="Dépôt sauvage — Impasse des Roses",             Description="Accumulation de déchets depuis 3 semaines.", Localisation="Impasse des Roses, Cité des Fleurs",  ServiceConcerneId=envId, AffecteAId=recId, DateAffectation=DateTime.UtcNow.AddDays(-29), EnregistreParId=recId },
                new Reclamation { NumeroReclamation=$"REC-{y}-00003", DateDepot=DateTime.UtcNow.AddDays(-20),                                            TypeReclamationId=tE, CategorieId=cL, Priorite=PrioriteReclamation.Moyenne,   Statut=StatutReclamation.EnCours, CitoyenId=cits[2].Id, Canal=CanalReclamation.Email,     Objet="4 lampadaires en panne — Ave Colonel Amirouche", Description="Zone obscure depuis 10 jours.", Localisation="Ave Colonel Amirouche n°40-60",           ServiceConcerneId=envId, EnregistreParId=recId },
                new Reclamation { NumeroReclamation=$"REC-{y}-00004", DateDepot=DateTime.UtcNow.AddDays(-15),                                            TypeReclamationId=tU, CategorieId=cC, Priorite=PrioriteReclamation.Haute,    Statut=StatutReclamation.EnCours, CitoyenId=cits[3].Id, Canal=CanalReclamation.Guichet,    Objet="Construction illicite — Parcelle 227B",         Description="Étage supplémentaire sans autorisation.", Localisation="Rue Didouche Mourad, Parcelle 227B",     ServiceConcerneId=pcId,  AffecteAId=urbId, DateAffectation=DateTime.UtcNow.AddDays(-14), EnregistreParId=recId },
                new Reclamation { NumeroReclamation=$"REC-{y}-00005", DateDepot=DateTime.UtcNow.AddDays(-10),                                            TypeReclamationId=tB, CategorieId=cB, Priorite=PrioriteReclamation.Moyenne,   Statut=StatutReclamation.Nouvelle, CitoyenId=cits[4].Id, Canal=CanalReclamation.Telephone, Objet="Musique forte après minuit — Café Le Soleil",   Description="Musique jusqu'à 3h chaque week-end.", Localisation="Cité ZHUN Ouest",                              ServiceConcerneId=envId, EnregistreParId=recId },
                new Reclamation { NumeroReclamation=$"REC-{y}-00006", DateDepot=DateTime.UtcNow.AddDays(-8),                                             TypeReclamationId=tA, CategorieId=cDoc,Priorite=PrioriteReclamation.Basse,    Statut=StatutReclamation.Nouvelle, CitoyenId=cits[5].Id, Canal=CanalReclamation.Web,       Objet="Délai excessif — Attestation de résidence",     Description="Demande sans réponse depuis 3 semaines.",                                                                  ServiceConcerneId=secId, EnregistreParId=recId },
                new Reclamation { NumeroReclamation=$"REC-{y}-00007", DateDepot=DateTime.UtcNow.AddDays(-60),                                            TypeReclamationId=tV, CategorieId=cT, Priorite=PrioriteReclamation.Moyenne,   Statut=StatutReclamation.Rejetee, CitoyenId=cits[6].Id, Canal=CanalReclamation.Email,     Objet="Trottoir dégradé — Rue Ali Boumendjel",         Description="Trottoir fissuré dangereux.", Localisation="Rue Ali Boumendjel n°42-50",              ServiceConcerneId=envId, DateCloture=DateTime.UtcNow.AddDays(-40), SolutionApportee="Hors périmètre communal — transmis wilaya.", EnregistreParId=recId },
            };
            await context.Reclamations.AddRangeAsync(recs);
            await context.SaveChangesAsync();

            var r1 = context.Reclamations.IgnoreQueryFilters().FirstOrDefault(r => r.NumeroReclamation == $"REC-{y}-00001")!;
            var r2 = context.Reclamations.IgnoreQueryFilters().FirstOrDefault(r => r.NumeroReclamation == $"REC-{y}-00002")!;
            var r4 = context.Reclamations.IgnoreQueryFilters().FirstOrDefault(r => r.NumeroReclamation == $"REC-{y}-00004")!;

            await context.SuivisReclamation.AddRangeAsync(
                new SuiviReclamation { ReclamationId = r1.Id, StatutPrecedent = StatutReclamation.Nouvelle, NouveauStatut = StatutReclamation.EnCours, DateChangement = DateTime.UtcNow.AddDays(-44), UtilisateurId = recId, UtilisateurNom = "Agent Réclamations", Commentaire = "Transmis au service Voirie.", VisibleCitoyen = true },
                new SuiviReclamation { ReclamationId = r1.Id, StatutPrecedent = StatutReclamation.EnCours, NouveauStatut = StatutReclamation.Traitee, DateChangement = DateTime.UtcNow.AddDays(-30), UtilisateurId = recId, UtilisateurNom = "Agent Réclamations", Commentaire = "Travaux réalisés. Clôturée.", VisibleCitoyen = true },
                new SuiviReclamation { ReclamationId = r2.Id, StatutPrecedent = StatutReclamation.Nouvelle, NouveauStatut = StatutReclamation.EnCours, DateChangement = DateTime.UtcNow.AddDays(-29), UtilisateurId = recId, UtilisateurNom = "Agent Réclamations", Commentaire = "Équipe nettoiement mobilisée.", VisibleCitoyen = true },
                new SuiviReclamation { ReclamationId = r4.Id, StatutPrecedent = StatutReclamation.Nouvelle, NouveauStatut = StatutReclamation.EnCours, DateChangement = DateTime.UtcNow.AddDays(-14), UtilisateurId = recId, UtilisateurNom = "Agent Réclamations", Commentaire = "Inspection sur site programmée.", VisibleCitoyen = true }
            );
            await context.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════
        //  PERMIS DE BÂTIR
        // ════════════════════════════════════════════════════════════

        if (!context.ZonagesUrbanisme.IgnoreQueryFilters().Any())
        {
            await context.ZonagesUrbanisme.AddRangeAsync(
                new ZonageUrbanisme { Code = "HA", Libelle = "Habitat Individuel", CoefficientOccupationSol = 0.4m, CoefficientUtilisationSol = 1.0m, HauteurMaximale = 9m },
                new ZonageUrbanisme { Code = "HC", Libelle = "Habitat Collectif", CoefficientOccupationSol = 0.6m, CoefficientUtilisationSol = 2.5m, HauteurMaximale = 28m },
                new ZonageUrbanisme { Code = "ACT", Libelle = "Activités Commerciales", CoefficientOccupationSol = 0.7m, CoefficientUtilisationSol = 2.0m, HauteurMaximale = 15m },
                new ZonageUrbanisme { Code = "IND", Libelle = "Zone Industrielle", CoefficientOccupationSol = 0.5m, CoefficientUtilisationSol = 1.5m, HauteurMaximale = 12m },
                new ZonageUrbanisme { Code = "ESP", Libelle = "Espaces Verts", CoefficientOccupationSol = 0.1m, CoefficientUtilisationSol = 0.1m, HauteurMaximale = 5m }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TypesDemandePermis.IgnoreQueryFilters().Any())
        {
            await context.TypesDemandePermis.AddRangeAsync(
                new TypeDemandePermis { Code = "PC", Libelle = "Permis de Construire", DelaiTraitementJours = 60, TarifBase = 15000m },
                new TypeDemandePermis { Code = "PA", Libelle = "Permis d'Aménager", DelaiTraitementJours = 45, TarifBase = 10000m },
                new TypeDemandePermis { Code = "PD", Libelle = "Permis de Démolir", DelaiTraitementJours = 30, TarifBase = 8000m },
                new TypeDemandePermis { Code = "CU", Libelle = "Certificat d'Urbanisme", DelaiTraitementJours = 15, TarifBase = 3000m },
                new TypeDemandePermis { Code = "AT", Libelle = "Autorisation de Travaux", DelaiTraitementJours = 21, TarifBase = 5000m }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TypesTaxe.IgnoreQueryFilters().Any())
        {
            await context.TypesTaxe.AddRangeAsync(
                new TypeTaxe { Code = "TAXE-BASE", Libelle = "Taxe de Base Permis", TauxCalcul = 2.5m, UniteCalcul = "m²" },
                new TypeTaxe { Code = "TAXE-INFRA", Libelle = "Participation Infrastructure", TauxCalcul = 1.5m, UniteCalcul = "m²" },
                new TypeTaxe { Code = "TAXE-TIMBRE", Libelle = "Droit de Timbre", TauxCalcul = 500m, UniteCalcul = "Forfait" },
                new TypeTaxe { Code = "TAXE-ENV", Libelle = "Taxe Environnementale", TauxCalcul = 0.5m, UniteCalcul = "m²" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Architectes.IgnoreQueryFilters().Any())
        {
            await context.Architectes.AddRangeAsync(
                new Architecte { NumeroOrdre = "ARCH-ALG-001", CIN = "74839201", Nom = "Bouzid", Prenom = "Rachid", Telephone = "0661122334", Email = "r.bouzid.arch@gmail.com" },
                new Architecte { NumeroOrdre = "ARCH-ALG-002", CIN = "83920341", Nom = "Talbi", Prenom = "Leila", Telephone = "0772233445", Email = "l.talbi.arch@gmail.com" },
                new Architecte { NumeroOrdre = "ARCH-ALG-003", CIN = "91031452", Nom = "Guerrouf", Prenom = "Hassen", Telephone = "0553344556", Email = "h.guerrouf.arch@gmail.com" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Demandeurs.IgnoreQueryFilters().Any())
        {
            await context.Demandeurs.AddRangeAsync(
                new Demandeur { Type = TypeDemandeur.Personne, CIN = "78192837", Nom = "Brahimi", Prenom = "Ali", Adresse = "22 Rue des Oliviers, Alger", Telephone = "0558001122", Email = "a.brahimi@gmail.com" },
                new Demandeur { Type = TypeDemandeur.Personne, CIN = "82304958", Nom = "Bouchenak", Prenom = "Houria", Adresse = "7 Cité Université, Alger", Telephone = "0669112233", Email = "h.bouchenak@gmail.com" },
                new Demandeur { Type = TypeDemandeur.SocieteConstruction, CIN = null, Nom = "SARL", RaisonSociale = "SARL BATI-PLUS", Adresse = "Zone Industrielle Réghaia", Telephone = "0233445566", Email = "contact@bati-plus.dz" },
                new Demandeur { Type = TypeDemandeur.Personne, CIN = "69415069", Nom = "Ferdjani", Prenom = "Mohamed", Adresse = "14 Avenue Pasteur, Alger", Telephone = "0770223344", Email = "m.ferdjani@gmail.com" },
                new Demandeur { Type = TypeDemandeur.SocieteConstruction, CIN = null, Nom = "SPA", RaisonSociale = "SPA IMMO-CONSTRUCT", Adresse = "Bd Zirout Youcef, Alger", Telephone = "0233556677", Email = "info@immo-construct.dz" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.CommissionsExamen.IgnoreQueryFilters().Any())
        {
            await context.CommissionsExamen.AddRangeAsync(
                new CommissionExamen { Libelle = "Commission Urbanisme Janvier 2025", DateReunion = new DateTime(2025, 1, 20), PresidentId = adminId, StatutReunion = StatutCommission.Tenue },
                new CommissionExamen { Libelle = "Commission Urbanisme Février 2025", DateReunion = new DateTime(2025, 2, 17), PresidentId = adminId, StatutReunion = StatutCommission.Tenue },
                new CommissionExamen { Libelle = "Commission Urbanisme Mars 2025", DateReunion = new DateTime(2025, 3, 17), PresidentId = adminId, StatutReunion = StatutCommission.Programmee }
            );
            await context.SaveChangesAsync();
        }

        if (!context.DemandesPermisBatir.IgnoreQueryFilters().Any())
        {
            var y = DateTime.UtcNow.Year;
            var pcSvcId = context.Services.IgnoreQueryFilters().Where(s => s.Code == "URB-PC").Select(x => x.Id).FirstOrDefault();

            var tPC = context.TypesDemandePermis.IgnoreQueryFilters().Where(t => t.Code == "PC").Select(x => x.Id).FirstOrDefault();
            var tAT = context.TypesDemandePermis.IgnoreQueryFilters().Where(t => t.Code == "AT").Select(x => x.Id).FirstOrDefault();

            var zHA = context.ZonagesUrbanisme.IgnoreQueryFilters().Where(z => z.Code == "HA").Select(x => x.Id).FirstOrDefault();
            var zHC = context.ZonagesUrbanisme.IgnoreQueryFilters().Where(z => z.Code == "HC").Select(x => x.Id).FirstOrDefault();
            var zACT = context.ZonagesUrbanisme.IgnoreQueryFilters().Where(z => z.Code == "ACT").Select(x => x.Id).FirstOrDefault();

            var a1 = context.Architectes.IgnoreQueryFilters().Where(a => a.NumeroOrdre == "ARCH-ALG-001").Select(x => x.Id).FirstOrDefault();
            var a2 = context.Architectes.IgnoreQueryFilters().Where(a => a.NumeroOrdre == "ARCH-ALG-002").Select(x => x.Id).FirstOrDefault();
            var a3 = context.Architectes.IgnoreQueryFilters().Where(a => a.NumeroOrdre == "ARCH-ALG-003").Select(x => x.Id).FirstOrDefault();

            var dm1 = context.Demandeurs.IgnoreQueryFilters().Where(d => d.Nom == "Brahimi").Select(x => x.Id).FirstOrDefault();
            var dm2 = context.Demandeurs.IgnoreQueryFilters().Where(d => d.Nom == "Bouchenak").Select(x => x.Id).FirstOrDefault();
            var dm3 = context.Demandeurs.IgnoreQueryFilters().Where(d => d.RaisonSociale == "SARL BATI-PLUS").Select(x => x.Id).FirstOrDefault();
            var dm4 = context.Demandeurs.IgnoreQueryFilters().Where(d => d.Nom == "Ferdjani").Select(x => x.Id).FirstOrDefault();
            var dm5 = context.Demandeurs.IgnoreQueryFilters().Where(d => d.RaisonSociale == "SPA IMMO-CONSTRUCT").Select(x => x.Id).FirstOrDefault();

            var cm1 = context.CommissionsExamen.IgnoreQueryFilters().FirstOrDefault(c => c.Libelle.Contains("Janvier")!).Id;
            var cm2 = context.CommissionsExamen.IgnoreQueryFilters().FirstOrDefault(c => c.Libelle.Contains("Février")!).Id;

            var txBase = context.TypesTaxe.IgnoreQueryFilters().Where(t => t.Code == "TAXE-BASE").Select(x => x.Id).FirstOrDefault();
            var txInfra = context.TypesTaxe.IgnoreQueryFilters().Where(t => t.Code == "TAXE-INFRA").Select(x => x.Id).FirstOrDefault();
            var txTimbre = context.TypesTaxe.IgnoreQueryFilters().Where(t => t.Code == "TAXE-TIMBRE").Select(x => x.Id).FirstOrDefault();

            // ── Demande 1 — Approuvée + Permis délivré ───────────────
            var d1 = new DemandePermisBatir
            {
                NumeroDemande = $"PB-{y}-00001",
                DateDepot = DateTime.UtcNow.AddDays(-90),
                TypeDemandeId = tPC,
                DemandeurId = dm1,
                ArchitecteId = a1,
                Statut = StatutDemande.Approuvee,
                AdresseProjet = "22 Rue des Oliviers, Alger",
                NumeroParcelle = "ALG-127-A",
                SuperficieTerrain = 250m,
                SuperficieAConstruire = 180m,
                NombreNiveaux = 2,
                TypeConstruction = TypeConstruction.Residential,
                CoutEstimatif = 12000000m,
                ZonageId = zHA,
                ServiceInstructeurId = pcSvcId,
                AgentInstructeurId = urbId,
                DateDebutInstruction = DateTime.UtcNow.AddDays(-85),
                CommissionExamenId = cm1,
                DateDecision = DateTime.UtcNow.AddDays(-60),
                ConditionsSpeciales = "Respect du gabarit R+2.",
                EnregistreParId = urbId,
                Suivis = new List<SuiviPermis>
                {
                    new() { NouveauStatut=StatutDemande.Deposee,   DateChangement=DateTime.UtcNow.AddDays(-90), UtilisateurId=urbId,   UtilisateurNom="Agent Urbanisme", Commentaire="Dossier déposé.",                 VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.Deposee,  NouveauStatut=StatutDemande.EnExamen,  DateChangement=DateTime.UtcNow.AddDays(-85), UtilisateurId=urbId,   UtilisateurNom="Agent Urbanisme", Commentaire="Instruction démarrée.",           VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.EnExamen, NouveauStatut=StatutDemande.Approuvee, DateChangement=DateTime.UtcNow.AddDays(-60), UtilisateurId=adminId, UtilisateurNom="Président APC",   Commentaire="Approuvé en commission Jan 2025.", VisibleCitoyen=true },
                },
                Taxes = new List<TaxePermis>
                {
                    new() { TypeTaxeId=txBase,   Montant=4500m, Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-75), NumeroRecu="REC-2025-0442" },
                    new() { TypeTaxeId=txInfra,  Montant=2700m, Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-75), NumeroRecu="REC-2025-0443" },
                    new() { TypeTaxeId=txTimbre, Montant=500m,  Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-88), NumeroRecu="REC-2025-0401" },
                },
                Inspections = new List<InspectionPermis>
                {
                    new() { Objet="Vérification fondations", DateInspection=DateTime.UtcNow.AddDays(-50), InspecteurId=urbId, NomInspecteur="Karim Meziane", Observations="Fondations conformes."},
                },
                PermisDelivre = new PermisDelivre
                {
                    NumeroPermis = $"PC-{y}-00001",
                    DateDelivrance = DateTime.UtcNow.AddDays(-55),
                    DateValidite = DateTime.UtcNow.AddDays(-55).AddYears(3),
                    Conditions = "Valable 3 ans. Début travaux dans 6 mois.",
                    DelivreParId = adminId
                }
            };

            // ── Demande 2 — En examen ─────────────────────────────────
            var d2 = new DemandePermisBatir
            {
                NumeroDemande = $"PB-{y}-00002",
                DateDepot = DateTime.UtcNow.AddDays(-45),
                TypeDemandeId = tPC,
                DemandeurId = dm3,
                ArchitecteId = a2,
                Statut = StatutDemande.EnExamen,
                AdresseProjet = "Zone Industrielle Réghaia, Lot 12",
                NumeroParcelle = "REG-045-C",
                SuperficieTerrain = 2000m,
                SuperficieAConstruire = 1200m,
                NombreNiveaux = 3,
                TypeConstruction = TypeConstruction.Commercial,
                CoutEstimatif = 85000000m,
                ZonageId = zACT,
                ServiceInstructeurId = pcSvcId,
                AgentInstructeurId = urbId,
                DateDebutInstruction = DateTime.UtcNow.AddDays(-40),
                CommissionExamenId = cm2,
                EnregistreParId = urbId,
                Suivis = new List<SuiviPermis>
                {
                    new() { NouveauStatut=StatutDemande.Deposee, DateChangement=DateTime.UtcNow.AddDays(-45), UtilisateurId=urbId, UtilisateurNom="Agent Urbanisme", Commentaire="Dossier reçu.", VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.Deposee, NouveauStatut=StatutDemande.EnExamen, DateChangement=DateTime.UtcNow.AddDays(-40), UtilisateurId=urbId, UtilisateurNom="Agent Urbanisme", Commentaire="Instruction en cours.", VisibleCitoyen=true },
                },
                Taxes = new List<TaxePermis>
                {
                    new() { TypeTaxeId=txBase,   Montant=30000m, Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-42), NumeroRecu="REC-2025-0511" },
                    new() { TypeTaxeId=txInfra,  Montant=18000m, Statut=StatutTaxe.EnAttente },
                    new() { TypeTaxeId=txTimbre, Montant=500m,   Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-44), NumeroRecu="REC-2025-0498" },
                },
            };

            // ── Demande 3 — Déposée ───────────────────────────────────
            var d3 = new DemandePermisBatir
            {
                NumeroDemande = $"PB-{y}-00003",
                DateDepot = DateTime.UtcNow.AddDays(-10),
                TypeDemandeId = tAT,
                DemandeurId = dm2,
                ArchitecteId = a3,
                Statut = StatutDemande.Deposee,
                AdresseProjet = "7 Cité Université, Alger",
                NumeroParcelle = "ALG-089-B",
                SuperficieTerrain = 120m,
                SuperficieAConstruire = 60m,
                NombreNiveaux = 1,
                TypeConstruction = TypeConstruction.Residential,
                CoutEstimatif = 2500000m,
                ZonageId = zHA,
                EnregistreParId = urbId,
                Suivis = new List<SuiviPermis>
                {
                    new() { NouveauStatut=StatutDemande.Deposee, DateChangement=DateTime.UtcNow.AddDays(-10), UtilisateurId=urbId, UtilisateurNom="Agent Urbanisme", Commentaire="Demande enregistrée.", VisibleCitoyen=true },
                },
                Taxes = new List<TaxePermis>
                {
                    new() { TypeTaxeId=txTimbre, Montant=500m, Statut=StatutTaxe.EnAttente },
                },
            };

            // ── Demande 4 — Rejetée ───────────────────────────────────
            var d4 = new DemandePermisBatir
            {
                NumeroDemande = $"PB-{y}-00004",
                DateDepot = DateTime.UtcNow.AddDays(-70),
                TypeDemandeId = tPC,
                DemandeurId = dm4,
                ArchitecteId = a1,
                Statut = StatutDemande.Rejetee,
                AdresseProjet = "14 Avenue Pasteur, Alger",
                NumeroParcelle = "ALG-312-D",
                SuperficieTerrain = 180m,
                SuperficieAConstruire = 160m,
                NombreNiveaux = 4,
                TypeConstruction = TypeConstruction.Residential,
                CoutEstimatif = 18000000m,
                ZonageId = zHA,
                ServiceInstructeurId = pcSvcId,
                AgentInstructeurId = urbId,
                DateDebutInstruction = DateTime.UtcNow.AddDays(-65),
                CommissionExamenId = cm1,
                DateDecision = DateTime.UtcNow.AddDays(-50),
                MotifRejet = "COS dépassé (0.89 > 0.4 autorisé). Gabarit non conforme PLU zone HA (max R+2).",
                EnregistreParId = urbId,
                Suivis = new List<SuiviPermis>
                {
                    new() { NouveauStatut=StatutDemande.Deposee,   DateChangement=DateTime.UtcNow.AddDays(-70), UtilisateurId=urbId,   UtilisateurNom="Agent Urbanisme", Commentaire="Dossier reçu.",          VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.Deposee,  NouveauStatut=StatutDemande.EnExamen,  DateChangement=DateTime.UtcNow.AddDays(-65), UtilisateurId=urbId,   UtilisateurNom="Agent Urbanisme", Commentaire="Instruction démarrée.",  VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.EnExamen, NouveauStatut=StatutDemande.Rejetee,   DateChangement=DateTime.UtcNow.AddDays(-50), UtilisateurId=adminId, UtilisateurNom="Président APC",   Commentaire="Rejeté — non-conformité PLU.", VisibleCitoyen=true },
                },
                Taxes = new List<TaxePermis>
                {
                    new() { TypeTaxeId=txTimbre, Montant=500m, Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-68), NumeroRecu="REC-2025-0389" },
                },
            };

            // ── Demande 5 — Collectif approuvé + Permis ──────────────
            var d5 = new DemandePermisBatir
            {
                NumeroDemande = $"PB-{y}-00005",
                DateDepot = DateTime.UtcNow.AddDays(-120),
                TypeDemandeId = tPC,
                DemandeurId = dm5,
                ArchitecteId = a2,
                Statut = StatutDemande.Approuvee,
                AdresseProjet = "Bd Zirout Youcef, Parcelle HC-12",
                NumeroParcelle = "HC-012-A",
                SuperficieTerrain = 3500m,
                SuperficieAConstruire = 2100m,
                NombreNiveaux = 7,
                TypeConstruction = TypeConstruction.Residential,
                CoutEstimatif = 250000000m,
                ZonageId = zHC,
                ServiceInstructeurId = pcSvcId,
                AgentInstructeurId = urbId,
                DateDebutInstruction = DateTime.UtcNow.AddDays(-115),
                CommissionExamenId = cm1,
                DateDecision = DateTime.UtcNow.AddDays(-90),
                ConditionsSpeciales = "R+7 autorisé. Parking souterrain 80 places min. Étude parasismique exigée.",
                EnregistreParId = urbId,
                Suivis = new List<SuiviPermis>
                {
                    new() { NouveauStatut=StatutDemande.Deposee,   DateChangement=DateTime.UtcNow.AddDays(-120), UtilisateurId=urbId,   UtilisateurNom="Agent Urbanisme", Commentaire="Dossier déposé.",            VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.Deposee,  NouveauStatut=StatutDemande.EnExamen,  DateChangement=DateTime.UtcNow.AddDays(-115), UtilisateurId=urbId,   UtilisateurNom="Agent Urbanisme", Commentaire="Instruction démarrée.",      VisibleCitoyen=true },
                    new() { StatutPrecedent=StatutDemande.EnExamen, NouveauStatut=StatutDemande.Approuvee, DateChangement=DateTime.UtcNow.AddDays(-90),  UtilisateurId=adminId, UtilisateurNom="Président APC",   Commentaire="Approuvé avec conditions.",  VisibleCitoyen=true },
                },
                Taxes = new List<TaxePermis>
                {
                    new() { TypeTaxeId=txBase,   Montant=52500m, Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-110), NumeroRecu="REC-2025-0312" },
                    new() { TypeTaxeId=txInfra,  Montant=31500m, Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-110), NumeroRecu="REC-2025-0313" },
                    new() { TypeTaxeId=txTimbre, Montant=500m,   Statut=StatutTaxe.Payee, DatePaiement=DateTime.UtcNow.AddDays(-118), NumeroRecu="REC-2025-0298" },
                },
                Inspections = new List<InspectionPermis>
                {
                    new() { Objet="Inspection fondations R-1", DateInspection=DateTime.UtcNow.AddDays(-80), InspecteurId=urbId, NomInspecteur="Karim Meziane", Observations="Fondations conformes."},
                    new() { Objet="Inspection gros œuvre R+2", DateInspection=DateTime.UtcNow.AddDays(-40), InspecteurId=urbId, NomInspecteur="Karim Meziane", Observations="Réserve: isolation thermique à vérifier."},
                },
                PermisDelivre = new PermisDelivre
                {
                    NumeroPermis = $"PC-{y}-00002",
                    DateDelivrance = DateTime.UtcNow.AddDays(-85),
                    DateValidite = DateTime.UtcNow.AddDays(-85).AddYears(4),
                    Conditions = "Valable 4 ans. Rapport parasismique avant coulage R+3.",
                    DelivreParId = adminId
                }
            };

            await context.DemandesPermisBatir.AddRangeAsync(d1, d2, d3, d4, d5);
            await context.SaveChangesAsync();
        }
    }
}
