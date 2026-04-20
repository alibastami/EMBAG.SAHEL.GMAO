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
        var equipement = await context.Equipements.FindAsync(id);
        if (equipement != null)
        {
            context.Equipements.Remove(equipement);
            await context.SaveChangesAsync();
        }
    }
}
