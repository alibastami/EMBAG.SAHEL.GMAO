using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class FabricationService : IFabricationService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public FabricationService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<DemandeFabrication>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.DemandesFabrication
            .Include(f => f.Equipement)
            .OrderByDescending(f => f.DateEmission)
            .ToListAsync();
    }

    public async Task<DemandeFabrication?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.DemandesFabrication
            .Include(f => f.Equipement)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<DemandeFabrication> CreateAsync(DemandeFabrication df)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        // Generate number: FAB-YYYY-XXX
        var year = DateTime.Now.Year;
        var count = await context.DemandesFabrication.CountAsync(f => f.DateEmission.Year == year) + 1;
        df.NumeroFabrication = $"FAB-{year}-{count:D3}";
        
        context.DemandesFabrication.Add(df);
        await context.SaveChangesAsync();
        return df;
    }

    public async Task UpdateStatusAsync(int id, StatutFabrication nouveauStatut)
    {
        using var context = await _factory.CreateDbContextAsync();
        var df = await context.DemandesFabrication.FindAsync(id);
        if (df != null)
        {
            df.Statut = nouveauStatut;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var df = await context.DemandesFabrication.FindAsync(id);
        if (df != null)
        {
            context.DemandesFabrication.Remove(df);
            await context.SaveChangesAsync();
        }
    }
}
