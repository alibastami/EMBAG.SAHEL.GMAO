using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class ConsignationService : IConsignationService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public ConsignationService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<BonDeConsignation>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.BonsDeConsignation
            .Include(b => b.Equipement)
            .Include(b => b.DemandeTravail)
            .Include(b => b.AgentConsignation)
            .Include(b => b.AgentDeconsignation)
            .OrderByDescending(b => b.DateHeureConsignation)
            .ToListAsync();
    }

    public async Task<List<BonDeConsignation>> GetByDemandeIdAsync(int dtId)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.BonsDeConsignation
            .Include(b => b.Equipement)
            .Include(b => b.AgentConsignation)
            .Where(b => b.DemandeTravailId == dtId)
            .ToListAsync();
    }

    public async Task<BonDeConsignation?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.BonsDeConsignation
            .Include(b => b.Equipement)
            .Include(b => b.DemandeTravail)
            .Include(b => b.AgentConsignation)
            .Include(b => b.AgentDeconsignation)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<BonDeConsignation> CreateAsync(BonDeConsignation bon)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.BonsDeConsignation.Add(bon);
        await context.SaveChangesAsync();
        return bon;
    }

    public async Task UpdateAsync(BonDeConsignation bon)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.BonsDeConsignation.Update(bon);
        await context.SaveChangesAsync();
    }

    public async Task DeconsignerAsync(int bonId, int userDeconsigId)
    {
        using var context = await _factory.CreateDbContextAsync();
        var bon = await context.BonsDeConsignation.FindAsync(bonId);
        if (bon != null)
        {
            bon.DateHeureDeconsignation = DateTime.Now;
            bon.AgentDeconsignationId = userDeconsigId;
            bon.VisaDeconsignation = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var bon = await context.BonsDeConsignation.FindAsync(id);
        if (bon != null)
        {
            context.BonsDeConsignation.Remove(bon);
            await context.SaveChangesAsync();
        }
    }
}
