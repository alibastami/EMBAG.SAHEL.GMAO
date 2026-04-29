using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Constants;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Enums;
using System.Text;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(GmaoDbContext context)
    {
        // 0. Seed Working Profiles
        if (!await context.WorkingProfiles.AnyAsync())
        {
            context.WorkingProfiles.AddRange(
                new WorkingProfile { Name = "Mécanicien Senior", HourlyRate = 1200, Specialite = Specialite.Mecanique },
                new WorkingProfile { Name = "Électricien Senior", HourlyRate = 1100, Specialite = Specialite.Electrique },
                new WorkingProfile { Name = "Technicien Junior", HourlyRate = 800, Specialite = Specialite.Autre }
            );
            await context.SaveChangesAsync();
        }

        var mecProfile = await context.WorkingProfiles.FirstOrDefaultAsync(p => p.Name == "Mécanicien Senior");
        var elecProfile = await context.WorkingProfiles.FirstOrDefaultAsync(p => p.Name == "Électricien Senior");

        // 1. Seed Users
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (admin == null)
        {
            admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@2026"),
                FullName = "Administrateur Système",
                Role = AppRoles.DSI,
                Position = "Directeur DSI",
                Specialite = Specialite.Automatisme,
                CanManageUsers = true,
                CanViewAudit = true,
                CanEditInventory = true
            };
            context.Users.Add(admin);
        }
        else
        {
            admin.Specialite = Specialite.Automatisme; // Ensure specialty is set
        }

        var tech1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "tech1");
        if (tech1 == null)
        {
            tech1 = new User
            {
                Username = "tech1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech@2026"),
                FullName = "Ahmed Mécanicien",
                Role = AppRoles.Executant,
                Position = "Technicien Maintenance",
                Specialite = Specialite.Mecanique,
                WorkingProfileId = mecProfile?.Id,
                CanManageUsers = false,
                CanViewAudit = false,
                CanEditInventory = false
            };
            context.Users.Add(tech1);
        }
        else
        {
            tech1.Specialite = Specialite.Mecanique; // FORCE UPDATE
            tech1.Role = AppRoles.Executant;
            if (tech1.WorkingProfileId == null) tech1.WorkingProfileId = mecProfile?.Id;
        }

        var tech2 = await context.Users.FirstOrDefaultAsync(u => u.Username == "tech2");
        if (tech2 == null)
        {
            tech2 = new User
            {
                Username = "tech2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech@2026"),
                FullName = "Said Électricien",
                Role = AppRoles.Executant,
                Position = "Technicien Électricité",
                Specialite = Specialite.Electrique,
                WorkingProfileId = elecProfile?.Id,
                CanManageUsers = false,
                CanViewAudit = false,
                CanEditInventory = false
            };
            context.Users.Add(tech2);
        }
        else
        {
            tech2.Specialite = Specialite.Electrique; // FORCE UPDATE
            tech2.Role = AppRoles.Executant;
            if (tech2.WorkingProfileId == null) tech2.WorkingProfileId = elecProfile?.Id;
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

        // 1.7 Seed Settings
        if (!await context.AppSettings.AnyAsync(s => s.Key == "EquipmentFilePath"))
        {
            context.AppSettings.Add(new AppSetting 
            { 
                Key = "EquipmentFilePath", 
                Value = @"D:\Embag-Sahel-Workspace\Sahel.GMAO\listmachine.txt" 
            });
            await context.SaveChangesAsync();
        }

        // 1.8 Cleanup dummies if requested (one-time logic)
        await CleanupDummyEquipmentsAsync(context);

        // 2. Seed equipments from listmachine.txt
        if (!await context.Equipements.AnyAsync() || (await context.Equipements.CountAsync() <= 3 && await context.Equipements.AnyAsync(e => e.Code == "OFF-001")))
        {
            await SeedEquipmentsFromFileAsync(context);
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

    private static async Task CleanupDummyEquipmentsAsync(GmaoDbContext context)
    {
        var dummyCodes = new[] { "OFF-001", "FAC-001", "CTP-001" };
        var dummies = await context.Equipements
            .Where(e => dummyCodes.Contains(e.Code))
            .ToListAsync();

        if (dummies.Any())
        {
            Console.WriteLine(">>> GMAO: Cleaning up dummy machines and their history...");
            var dummyIds = dummies.Select(d => d.Id).ToList();

            // Delete associated records manually to avoid FK conflicts
            // 1. Fabrication
            var fabrications = await context.DemandesFabrication.Where(f => dummyIds.Contains(f.EquipementId)).ToListAsync();
            context.DemandesFabrication.RemoveRange(fabrications);

            // 2. DTs
            var dts = await context.DemandesTravail.Where(d => dummyIds.Contains(d.EquipementId)).ToListAsync();
            context.DemandesTravail.RemoveRange(dts);

            // 3. Preventives
            var preventives = await context.MaintenancePreventives.Where(m => dummyIds.Contains(m.EquipementId)).ToListAsync();
            context.MaintenancePreventives.RemoveRange(preventives);

            // 4. Incidents
            var incidents = await context.RapportsIncidents.Where(r => dummyIds.Contains(r.EquipementId)).ToListAsync();
            context.RapportsIncidents.RemoveRange(incidents);

            // 5. Consignations
            var consignations = await context.BonsDeConsignation.Where(b => dummyIds.Contains(b.EquipementId)).ToListAsync();
            context.BonsDeConsignation.RemoveRange(consignations);

            // 6. Entretien
            var entretiens = await context.FichesEntretienPreventif.Where(f => dummyIds.Contains(f.EquipementId)).ToListAsync();
            context.FichesEntretienPreventif.RemoveRange(entretiens);

            await context.SaveChangesAsync();

            // Finally delete the machines
            context.Equipements.RemoveRange(dummies);
            await context.SaveChangesAsync();
            Console.WriteLine(">>> GMAO: Dummy cleanup complete.");
        }
    }

    private static async Task SeedEquipmentsFromFileAsync(GmaoDbContext context)
    {
        try
        {
            var setting = await context.AppSettings.FirstOrDefaultAsync(s => s.Key == "EquipmentFilePath");
            string? configuredPath = setting?.Value;

            // Try to find listmachine.txt in several locations if configured path fails
            string[] possiblePaths = { 
                configuredPath ?? "",
                "listmachine.txt", 
                "../listmachine.txt", 
                "../../listmachine.txt",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "listmachine.txt"),
                Path.Combine(Directory.GetCurrentDirectory(), "listmachine.txt")
            };
 
            string? filePath = possiblePaths.Where(p => !string.IsNullOrEmpty(p)).FirstOrDefault(File.Exists);

            if (filePath == null)
            {
                if (await context.Equipements.AnyAsync()) return; // Don't add fallbacks if we already have some

                // Fallback to manual seeding if file not found
                context.Equipements.AddRange(
                    new Equipement { Code = "OFF-001", Designation = "Presse OFFSET HEIDELBERG", Section = "OFFSET", Marque = "Heidelberg", IsActive = true },
                    new Equipement { Code = "FAC-001", Designation = "Plieuse automatique", Section = "FACONNAGE", Marque = "Stahl", IsActive = true },
                    new Equipement { Code = "CTP-001", Designation = "Machine CTP", Section = "PRE-PRESSE", Marque = "Agfa", IsActive = true }
                );
                await context.SaveChangesAsync();
                return;
            }

            Console.WriteLine($">>> GMAO: Loading machines from {filePath}...");
            
            // Handle UTF-16LE specifically
            var lines = await File.ReadAllLinesAsync(filePath, System.Text.Encoding.Unicode);
            if (lines.Length <= 1) 
            {
                // Try UTF-8 if Unicode yielded nothing
                lines = await File.ReadAllLinesAsync(filePath, System.Text.Encoding.UTF8);
            }

            string currentSection = "GÉNÉRAL";
            var equipments = new List<Equipement>();
            var existingCodes = await context.Equipements.Select(e => e.Code).ToListAsync();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                // Detect Section (Keywords)
                if (trimmedLine.StartsWith("ATELIER", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("DEPARTEMENT", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("SERVICE", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("MAGASIN", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("PARK", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("BLOC", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("STATION", StringComparison.OrdinalIgnoreCase) || 
                    trimmedLine.StartsWith("MICRO", StringComparison.OrdinalIgnoreCase) ||
                    trimmedLine.StartsWith("FLEXOGRAPHIE", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = trimmedLine;
                    continue;
                }

                // Detect Machine
                string code = "";
                string designation = "";

                var parts = trimmedLine.Split(' ', 2);
                if (parts.Length > 1 && int.TryParse(parts[0], out _))
                {
                    code = parts[0];
                    designation = parts[1];
                }
                else if (int.TryParse(trimmedLine, out _))
                {
                    code = trimmedLine;
                    designation = "Machine " + code;
                }
                else
                {
                    designation = trimmedLine;
                    code = designation.Length > 5 ? designation.Substring(0, 5).ToUpper() : designation.ToUpper();
                    code = code.Replace(" ", "-");
                }

                // Avoid duplicates
                if (!existingCodes.Contains(code) && !equipments.Any(e => e.Code == code))
                {
                    equipments.Add(new Equipement
                    {
                        Code = code,
                        Designation = designation,
                        Section = currentSection,
                        IsActive = true
                    });
                }
            }

            if (equipments.Any())
            {
                context.Equipements.AddRange(equipments);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> GMAO SEED ERROR: {ex.Message}");
        }
    }
}
