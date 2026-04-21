using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public MaintenanceService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<MaintenancePreventive>> GetAllPreventiveAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.MaintenancePreventives
            .Include(m => m.Equipement)
            .OrderBy(m => m.DateProchaineEcheance)
            .ToListAsync();
    }

    public async Task<MaintenancePreventive?> GetPreventiveByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.MaintenancePreventives
            .Include(m => m.Equipement)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task RegisterRealizationAsync(int id, DateTime realizationDate)
    {
        using var context = await _factory.CreateDbContextAsync();
        var mp = await context.MaintenancePreventives.FindAsync(id);
        if (mp != null)
        {
            mp.DateDerniereRealisation = realizationDate;
            mp.DateProchaineEcheance = realizationDate.AddDays(mp.FrequenceJours);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdatePreventiveAsync(MaintenancePreventive mp)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.MaintenancePreventives.Update(mp);
        await context.SaveChangesAsync();
    }

    public async Task CreatePreventiveAsync(MaintenancePreventive mp)
    {
        using var context = await _factory.CreateDbContextAsync();
        // Initialize next date if not set
        if (mp.DateProchaineEcheance == default)
        {
            mp.DateProchaineEcheance = DateTime.Now.AddDays(mp.FrequenceJours);
        }
        context.MaintenancePreventives.Add(mp);
        await context.SaveChangesAsync();
    }

    public async Task DeletePreventiveAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var mp = await context.MaintenancePreventives.FindAsync(id);
        if (mp != null)
        {
            context.MaintenancePreventives.Remove(mp);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<FicheEntretienPreventif>> GetAllFichesAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.FichesEntretienPreventif
            .Include(f => f.Equipement)
            .Include(f => f.Intervenant)
            .OrderByDescending(f => f.DateFaitLe)
            .ToListAsync();
    }

    public async Task<FicheEntretienPreventif?> GetFicheByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.FichesEntretienPreventif
            .Include(f => f.Equipement)
            .Include(f => f.Intervenant)
            .Include(f => f.Taches)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<FicheEntretienPreventif> GenerateFicheFromScheduleAsync(int maintenanceId)
    {
        using var context = await _factory.CreateDbContextAsync();
        var mp = await context.MaintenancePreventives
            .Include(m => m.Equipement)
            .FirstOrDefaultAsync(m => m.Id == maintenanceId);

        if (mp == null) throw new Exception("Maintenance schedule not found");

        var fiche = new FicheEntretienPreventif
        {
            EquipementId = mp.EquipementId,
            MaintenancePreventiveId = mp.Id,
            Section = mp.Equipement.Section,
            Periodicite = $"{mp.FrequenceJours} Jours",
            Partie = "MEC/ELEC", // Default
            NumeroOT = $"OT-{DateTime.Now:yyyyMMdd}-{mp.Id}",
            Taches = new List<TacheEntretien>
            {
                new TacheEntretien
                {
                    Position = 1,
                    Organes = "Général",
                    OperationAEffectuer = mp.Operation,
                    TempsPrevuHeures = 1.0 // Default
                }
            }
        };

        context.FichesEntretienPreventif.Add(fiche);
        await context.SaveChangesAsync();
        return fiche;
    }

    public async Task UpdateFicheExecutionAsync(FicheEntretienPreventif fiche)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        // Update the tasks
        foreach (var tache in fiche.Taches)
        {
            context.TachesEntretien.Update(tache);
        }

        context.FichesEntretienPreventif.Update(fiche);
        await context.SaveChangesAsync();
    }

    public async Task DeleteFicheAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var fiche = await context.FichesEntretienPreventif.FindAsync(id);
        if (fiche != null)
        {
            context.FichesEntretienPreventif.Remove(fiche);
            await context.SaveChangesAsync();
        }
    }
}
