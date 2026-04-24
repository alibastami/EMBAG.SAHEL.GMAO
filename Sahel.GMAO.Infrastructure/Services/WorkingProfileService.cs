using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class WorkingProfileService : IWorkingProfileService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public WorkingProfileService(IDbContextFactory<GmaoDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<WorkingProfile>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.WorkingProfiles.ToListAsync();
    }

    public async Task<WorkingProfile?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.WorkingProfiles.FindAsync(id);
    }

    public async Task CreateAsync(WorkingProfile profile)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.WorkingProfiles.Add(profile);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WorkingProfile profile)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.WorkingProfiles.Update(profile);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        var profile = await context.WorkingProfiles.FindAsync(id);
        if (profile != null)
        {
            context.WorkingProfiles.Remove(profile);
            await context.SaveChangesAsync();
        }
    }
}
