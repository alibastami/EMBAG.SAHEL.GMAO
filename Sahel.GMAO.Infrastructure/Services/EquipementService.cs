using System.Text;
using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class EquipementService : IEquipementService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public EquipementService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<Equipement>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Equipements
            .Include(e => e.DemandesTravail)
            .ToListAsync();
    }

    public async Task<Equipement?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Equipements
            .Include(e => e.DemandesTravail)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Equipement?> GetByCodeAsync(string code)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Equipements.FirstOrDefaultAsync(e => e.Code == code);
    }

    public async Task CreateAsync(Equipement equipement)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.Equipements.Add(equipement);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Equipement equipement)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.Entry(equipement).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        // Check dependencies
        var hasDts = await context.DemandesTravail.AnyAsync(d => d.EquipementId == id);
        if (hasDts) throw new InvalidOperationException("Impossible de supprimer cet équipement car il possède des historiques de maintenance (DT).");

        var hasPreventives = await context.MaintenancePreventives.AnyAsync(m => m.EquipementId == id);
        if (hasPreventives) throw new InvalidOperationException("Impossible de supprimer cet équipement car il est associé à des plans de maintenance préventive.");

        var equipement = await context.Equipements.FindAsync(id);
        if (equipement != null)
        {
            context.Equipements.Remove(equipement);
            await context.SaveChangesAsync();
        }
    }

    public async Task ImportFromFileAsync(string? path = null)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        if (string.IsNullOrEmpty(path))
        {
            var setting = await context.AppSettings.FirstOrDefaultAsync(s => s.Key == "EquipmentFilePath");
            path = setting?.Value ?? "listmachine.txt";
        }

        if (!File.Exists(path))
        {
            // Try relative paths if absolute fails
            if (!Path.IsPathRooted(path))
            {
                string[] possiblePaths = { 
                    path,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path),
                    Path.Combine(Directory.GetCurrentDirectory(), path),
                    "../" + path,
                    "../../" + path
                };
                path = possiblePaths.FirstOrDefault(File.Exists) ?? path;
            }
        }

        if (!File.Exists(path)) throw new FileNotFoundException($"Le fichier d'équipements est introuvable : {path}");

        // Use Encoding.GetEncoding(1200) for UTF-16LE or let it detect with BOM
        // But the user's file seems to be UTF-16LE without BOM or just standard.
        // File.ReadAllLinesAsync usually handles BOM.
        
        var lines = await File.ReadAllLinesAsync(path, Encoding.Unicode);
        if (lines.Length == 0 || (lines.Length == 1 && string.IsNullOrWhiteSpace(lines[0])))
        {
            // Try again with default encoding if Unicode returned nothing (maybe it's UTF-8)
            lines = await File.ReadAllLinesAsync(path, Encoding.UTF8);
        }

        string currentSection = "GÉNÉRAL";
        var existingCodes = await context.Equipements.Select(e => e.Code).ToListAsync();
        var newEquipments = new List<Equipement>();

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

            // Remove quotes if present
            var cleanLine = trimmedLine.Replace("\"", "").Trim();
            
            var parts = cleanLine.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
            
            // If it starts with a digit, assume the first part is the code
            if (parts.Length > 0 && char.IsDigit(parts[0][0]))
            {
                code = parts[0];
                designation = parts.Length > 1 ? parts[1] : ("Machine " + code);
            }
            else
            {
                designation = cleanLine;
                code = designation.Length > 5 ? designation.Substring(0, 5).ToUpper() : designation.ToUpper();
                code = code.Replace(" ", "-");
            }

            if (!existingCodes.Contains(code) && !newEquipments.Any(e => e.Code == code))
            {
                newEquipments.Add(new Equipement
                {
                    Code = code,
                    Designation = designation,
                    Section = currentSection,
                    IsActive = true
                });
            }
        }

        if (newEquipments.Any())
        {
            context.Equipements.AddRange(newEquipments);
            await context.SaveChangesAsync();
        }
    }
}
