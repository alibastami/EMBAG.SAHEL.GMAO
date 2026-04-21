using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Constants;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(GmaoDbContext context)
    {
        // 1. Seed Users
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@2026"),
                FullName = "Administrateur Système",
                Role = AppRoles.DSI,
                Position = "Directeur DSI",
                CanManageUsers = true,
                CanViewAudit = true,
                CanEditInventory = true
            };

            context.Users.Add(admin);
        }

        if (!await context.Users.AnyAsync(u => u.Username == "tech1"))
        {
            var tech = new User
            {
                Username = "tech1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech@2026"),
                FullName = "Ahmed Mécanicien",
                Role = AppRoles.Executant,
                Position = "Technicien Maintenance",
                CanManageUsers = false,
                CanViewAudit = false,
                CanEditInventory = false
            };
            context.Users.Add(tech);
        }

        if (!await context.Users.AnyAsync(u => u.Username == "zone1"))
        {
            var demandeur = new User
            {
                Username = "zone1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Zone@2026"),
                FullName = "Atelier OFFSET",
                Role = AppRoles.Demandeur,
                Position = "Demandeur",
                CanManageUsers = false,
                CanViewAudit = false,
                CanEditInventory = false
            };
            context.Users.Add(demandeur);
        }

        // 1.5 Migrate existing users with legacy roles
        var allUsers = await context.Users.ToListAsync();
        foreach (var user in allUsers)
        {
            var oldRole = user.Role;
            var newRole = oldRole switch
            {
                "Admin" or "Directeur" or "Ordonnanceur" => AppRoles.DSI,
                "Mecanicien" or "Mécanicien" or "Electricien" or "Électricien" or "Technicien" => AppRoles.Executant,
                "Zone" or "Client" => AppRoles.Demandeur,
                _ => oldRole
            };

            if (newRole != oldRole)
            {
                user.Role = newRole;
            }
        }
        await context.SaveChangesAsync();

        // 2. Seed equipments
        if (!await context.Equipements.AnyAsync())
        {
            context.Equipements.AddRange(
                new Equipement { Code = "OFF-001", Designation = "Presse OFFSET HEIDELBERG", Section = "OFFSET", Marque = "Heidelberg" },
                new Equipement { Code = "FAC-001", Designation = "Plieuse automatique", Section = "FACONNAGE", Marque = "Stahl" },
                new Equipement { Code = "CTP-001", Designation = "Machine CTP", Section = "PRE-PRESSE", Marque = "Agfa" }
            );
        }

        // 3. Seed PDR Articles
        if (!await context.ArticlesPdr.AnyAsync())
        {
            context.ArticlesPdr.AddRange(
                new ArticlePdr { CodeArticle = "RUL-001", Designation = "Roulement à billes 6204", Unite = "U", PrixUnitaire = 1200, QuantiteEnStock = 50, SeuilAlerte = 10 },
                new ArticlePdr { CodeArticle = "HUI-001", Designation = "Huile hydraulique ISO 46", Unite = "L", PrixUnitaire = 850, QuantiteEnStock = 100, SeuilAlerte = 20 },
                new ArticlePdr { CodeArticle = "COU-001", Designation = "Courroie trapézoïdale A-42", Unite = "U", PrixUnitaire = 3500, QuantiteEnStock = 5, SeuilAlerte = 5 }
            );
        }

        await context.SaveChangesAsync();
    }
}
