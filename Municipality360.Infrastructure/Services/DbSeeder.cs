using Microsoft.AspNetCore.Identity;
using Municipality360.Domain.Entities;
using Municipality360.Infrastructure.Data;
using Municipality360.Infrastructure.Identity;

namespace Municipality360.Infrastructure.Services;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.EnsureCreatedAsync();

        // Seed Roles
        string[] roles = { "SuperAdmin", "Admin", "Manager", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed Admin User
        if (await userManager.FindByEmailAsync("admin@municipality360.dz") == null)
        {
            var admin = new ApplicationUser
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@municipality360.dz",
                UserName = "admin@municipality360.dz",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
        }

        // Seed Departements
        if (!context.Departements.Any())
        {
            var departements = new[]
            {
                new Departement { Nom = "Administration Generale", Code = "ADM", Description = "Direction et administration generale" },
                new Departement { Nom = "Finances et Budget", Code = "FIN", Description = "Gestion financiere et budgetaire" },
                new Departement { Nom = "Urbanisme et Construction", Code = "URB", Description = "Permis de construire et amenagement" },
                new Departement { Nom = "Etat Civil", Code = "EC", Description = "Registre civil et documents administratifs" },
                new Departement { Nom = "Environnement", Code = "ENV", Description = "Proprete et espace vert" }
            };
            await context.Departements.AddRangeAsync(departements);
            await context.SaveChangesAsync();

            // Seed Services
            var admDept = context.Departements.First(d => d.Code == "ADM");
            var finDept = context.Departements.First(d => d.Code == "FIN");

            var services = new[]
            {
                new Service { Nom = "Secretariat", Code = "ADM-SEC", DepartementId = admDept.Id },
                new Service { Nom = "Ressources Humaines", Code = "ADM-RH", DepartementId = admDept.Id },
                new Service { Nom = "Comptabilite", Code = "FIN-CPT", DepartementId = finDept.Id },
                new Service { Nom = "Budget", Code = "FIN-BDG", DepartementId = finDept.Id }
            };
            await context.Services.AddRangeAsync(services);

            // Seed Postes
            var postes = new[]
            {
                new Poste { Titre = "Directeur", Code = "DIR", SalaireMin = 80000, SalaireMax = 120000 },
                new Poste { Titre = "Chef de Service", Code = "CS", SalaireMin = 60000, SalaireMax = 90000 },
                new Poste { Titre = "Cadre Administratif", Code = "CA", SalaireMin = 45000, SalaireMax = 70000 },
                new Poste { Titre = "Agent Administratif", Code = "AA", SalaireMin = 30000, SalaireMax = 50000 }
            };
            await context.Postes.AddRangeAsync(postes);
            await context.SaveChangesAsync();
        }
    }
}
