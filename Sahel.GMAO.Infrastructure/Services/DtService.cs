using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Enums;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;

namespace Sahel.GMAO.Infrastructure.Services;

public class DtService : IDtService
{
    private readonly IDbContextFactory<GmaoDbContext> _factory;
    private readonly INotificationService _notificationService;

    public DtService(IDbContextFactory<GmaoDbContext> factory, INotificationService notificationService)
    {
        _factory = factory;
        _notificationService = notificationService;
    }

    public async Task<List<DemandeTravail>> GetAllAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.DemandesTravail
            .Include(d => d.Equipement)
            .Include(d => d.Demandeur)
            .OrderByDescending(d => d.DateEmission)
            .ToListAsync();
    }

    public async Task<List<DemandeTravail>> GetForUserAsync(string username, string role, bool mineOnly = false)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        var query = context.DemandesTravail
            .Include(d => d.Equipement)
            .Include(d => d.Demandeur)
            .AsQueryable();

        if (role == Sahel.GMAO.Core.Constants.AppRoles.Demandeur)
        {
            query = query.Where(d => d.Demandeur != null && d.Demandeur.Username == username);
        }
        else if (role == Sahel.GMAO.Core.Constants.AppRoles.Executant)
        {
            if (mineOnly)
            {
                // Only those assigned to this user
                query = query.Where(d => d.Intervenants.Any(i => i.Intervenant != null && i.Intervenant.Username == username));
            }
            // If not mineOnly, they see all (as requested: "YES all")
        }
        // If DSI, returns all

        return await query.OrderByDescending(d => d.DateEmission).ToListAsync();
    }

    public async Task<List<DemandeTravail>> GetByStatusAsync(StatutDT statut)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.DemandesTravail
            .Include(d => d.Equipement)
            .Where(d => d.Statut == statut)
            .ToListAsync();
    }

    public async Task<DemandeTravail?> GetByIdAsync(int id)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.DemandesTravail
            .Include(d => d.Equipement)
            .Include(d => d.Demandeur)
            .Include(d => d.Consommables).ThenInclude(c => c.ArticlePdr)
            .Include(d => d.Intervenants).ThenInclude(i => i.Intervenant)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<DemandeTravail> CreateAsync(DemandeTravail dt)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        // Generate unique number: DT-YYYY-XXXX
        var year = DateTime.Now.Year;
        var count = await context.DemandesTravail.CountAsync(d => d.DateEmission.Year == year) + 1;
        dt.NumeroDT = $"DT-{year}-{count:D3}";
        
        context.DemandesTravail.Add(dt);
        await context.SaveChangesAsync();
        
        // Notify DSI (Broadcast for new DT)
        await _notificationService.NotifyStatutChangedAsync(dt.Id, dt.NumeroDT, dt.Statut.ToString());
        
        return dt;
    }

    public async Task UpdateStatusAsync(int dtId, StatutDT nouveauStatut)
    {
        using var context = await _factory.CreateDbContextAsync();
        var dt = await context.DemandesTravail.FindAsync(dtId);
        if (dt != null)
        {
            dt.Statut = nouveauStatut;
            await context.SaveChangesAsync();
            
            await _notificationService.NotifyStatutChangedAsync(dt.Id, dt.NumeroDT, dt.Statut.ToString());
        }
    }

    public async Task UpdateExecutionAsync(DemandeTravail dt)
    {
        using var context = await _factory.CreateDbContextAsync();
        var existing = await context.DemandesTravail.FindAsync(dt.Id);
        if (existing != null)
        {
            existing.TravailExecute = dt.TravailExecute;
            existing.DateExecutionDebut = dt.DateExecutionDebut;
            existing.DateExecutionFin = dt.DateExecutionFin;
            existing.DureeArretProductionHeures = dt.DureeArretProductionHeures;
            existing.Statut = dt.Statut;

            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(DemandeTravail dt)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        // Recalculate main d'oeuvre totals
        dt.TotalCoutMainOeuvre = 0;
        foreach (var intervenant in dt.Intervenants)
        {
            if (intervenant.Pointages != null)
            {
                intervenant.HeuresTravaillees = intervenant.Pointages.Sum(p => p.HeuresTravaillees);
            }
            dt.TotalCoutMainOeuvre += (decimal)intervenant.HeuresTravaillees * intervenant.TauxHoraire;
        }
        dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

        context.DemandesTravail.Update(dt);
        await context.SaveChangesAsync();
    }

    public async Task AddConsommableAsync(int dtId, int articleId, double quantite)
    {
        using var context = await _factory.CreateDbContextAsync();
        var dt = await context.DemandesTravail.Include(d => d.Consommables).FirstOrDefaultAsync(d => d.Id == dtId);
        var article = await context.ArticlesPdr.FindAsync(articleId);

        if (dt != null && article != null)
        {
            var usage = new ConsommableUsage
            {
                DemandeTravailId = dtId,
                ArticlePdrId = articleId,
                Quantite = quantite,
                PrixUnitaireApplique = article.PrixUnitaire
            };

            context.ConsommableUsages.Add(usage);
            
            // Reduce stock
            article.QuantiteEnStock -= quantite;

            // Update DT Totals
            dt.TotalCoutPieces += (decimal)quantite * article.PrixUnitaire;
            dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

            await context.SaveChangesAsync();
        }
    }

    public async Task AddIntervenantAsync(int dtId, int userId, double heures, string qualification)
    {
        using var context = await _factory.CreateDbContextAsync();
        var dt = await context.DemandesTravail.Include(d => d.Intervenants).FirstOrDefaultAsync(d => d.Id == dtId);
        
        if (dt != null)
        {
            var role = new InterventionRole
            {
                DemandeTravailId = dtId,
                IntervenantId = userId,
                HeuresTravaillees = heures,
                Qualification = qualification,
                TauxHoraire = 500 // Optional: fetch from user profile if exists
            };

            context.InterventionRoles.Add(role);

            // Update DT Totals
            dt.TotalCoutMainOeuvre += (decimal)heures * role.TauxHoraire;
            dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

            await context.SaveChangesAsync();

            if (heures == 0) // It's an assignment
            {
                await _notificationService.NotifyNewDTAsync(userId, dt.NumeroDT);
            }
        }
    }
}
