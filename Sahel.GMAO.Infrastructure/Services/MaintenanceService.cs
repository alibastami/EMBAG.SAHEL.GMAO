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
}
