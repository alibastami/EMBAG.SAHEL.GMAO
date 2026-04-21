using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class NatureTravailService : INatureTravailService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public NatureTravailService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<NatureTravail>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.NaturesTravail.ToListAsync();
    }

    public async Task<NatureTravail?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.NaturesTravail.FindAsync(id);
    }

    public async Task CreateAsync(NatureTravail nature)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.NaturesTravail.Add(nature);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NatureTravail nature)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.NaturesTravail.Update(nature);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var nature = await context.NaturesTravail.FindAsync(id);
        if (nature != null)
        {
            context.NaturesTravail.Remove(nature);
            await context.SaveChangesAsync();
        }
    }
}
