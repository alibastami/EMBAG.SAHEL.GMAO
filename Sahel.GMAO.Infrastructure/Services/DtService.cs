using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Enums;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Core.Models;
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
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    Console.WriteLine($"[DT Service] Filtering for Executant: {username}, Specialty: {user.Specialite}");
                    
                    if (user.Specialite.HasValue)
                    {
                        var userSpec = user.Specialite.Value;
                        query = query.Where(d => 
                            d.Intervenants.Any(i => i.Intervenant != null && i.Intervenant.Username == username) ||
                            d.SpecialiteRequise == userSpec);
                    }
                    else
                    {
                        query = query.Where(d => d.Intervenants.Any(i => i.Intervenant != null && i.Intervenant.Username == username));
                    }
                }
                else
                {
                    Console.WriteLine($"[DT Service] Executant user not found in DB: {username}");
                    query = query.Where(d => d.Intervenants.Any(i => i.Intervenant != null && i.Intervenant.Username == username));
                }
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
            .Include(d => d.Intervenants).ThenInclude(i => i.Pointages)
            .Include(d => d.JournalInterventions).ThenInclude(l => l.Intervenant)
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
        
        // Auto-tag / assign technicians based on specialty
        if (dt.SpecialiteRequise.HasValue)
        {
            var matchingTechs = await context.Users
                .Where(u => u.Role == Sahel.GMAO.Core.Constants.AppRoles.Executant && u.Specialite == dt.SpecialiteRequise.Value)
                .ToListAsync();

            foreach (var tech in matchingTechs)
            {
                var role = new InterventionRole
                {
                    DemandeTravailId = dt.Id,
                    IntervenantId = tech.Id,
                    HeuresTravaillees = 0,
                    Qualification = tech.Position,
                    TauxHoraire = 500
                };
                context.InterventionRoles.Add(role);
                
                // Notify them
                await _notificationService.NotifyNewDTAsync(tech.Id, dt.NumeroDT);
            }
            await context.SaveChangesAsync();
        }
        
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
            var rate = intervenant.Unit == LaborUnit.Jour ? intervenant.TauxHoraire / 8 : intervenant.TauxHoraire;
            dt.TotalCoutMainOeuvre += (decimal)intervenant.HeuresTravaillees * rate;
        }
        dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

        context.DemandesTravail.Update(dt);
        await context.SaveChangesAsync();
    }

    public async Task AddConsommableAsync(int dtId, int articleId, double quantite, decimal? prixUnitaire = null, string? nBsm = null, string? observation = null)
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
                PrixUnitaireApplique = prixUnitaire ?? article.PrixUnitaire,
                N_BSM = nBsm,
                Observation = observation
            };

            context.ConsommableUsages.Add(usage);
            
            // Reduce stock
            article.QuantiteEnStock -= quantite;

            // Update DT Totals
            dt.TotalCoutPieces += (decimal)quantite * usage.PrixUnitaireApplique;
            dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

            await context.SaveChangesAsync();
        }
    }

    public async Task AddIntervenantAsync(int dtId, int userId, double heures, string qualification, decimal? tauxHoraire = null, LaborUnit? unit = null)
    {
        using var context = await _factory.CreateDbContextAsync();
        var dt = await context.DemandesTravail.Include(d => d.Intervenants).FirstOrDefaultAsync(d => d.Id == dtId);
        
        if (dt != null)
        {
            if (tauxHoraire == null)
            {
                // Try to get from user profile
                var user = await context.Users.Include(u => u.WorkingProfile).FirstOrDefaultAsync(u => u.Id == userId);
                tauxHoraire = user?.WorkingProfile?.HourlyRate ?? 500;
                unit ??= user?.WorkingProfile?.Unit ?? LaborUnit.Heure;
            }

            unit ??= LaborUnit.Heure;

            var role = new InterventionRole
            {
                DemandeTravailId = dtId,
                IntervenantId = userId,
                HeuresTravaillees = heures,
                Qualification = qualification,
                TauxHoraire = tauxHoraire.Value,
                Unit = unit.Value
            };

            context.InterventionRoles.Add(role);

            // Update DT Totals
            var rate = role.Unit == LaborUnit.Jour ? role.TauxHoraire / 8 : role.TauxHoraire;
            dt.TotalCoutMainOeuvre += (decimal)heures * rate;
            dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

            await context.SaveChangesAsync();

            if (heures == 0) // It's an assignment
            {
                await _notificationService.NotifyNewDTAsync(userId, dt.NumeroDT);
            }
        }
    }

    public async Task AddInterventionLogAsync(InterventionLog log)
    {
        using var context = await _factory.CreateDbContextAsync();
        
        var dt = await context.DemandesTravail
            .Include(d => d.Intervenants).ThenInclude(i => i.Pointages)
            .FirstOrDefaultAsync(d => d.Id == log.DemandeTravailId);
            
        if (dt != null)
        {
            context.InterventionLogs.Add(log);
            
            // 1. Update the main DT fields with the latest log info
            dt.TravailExecute = log.TravailExecute;
            dt.DateExecutionFin = log.DateIntervention;
            if (dt.DateExecutionDebut == null) dt.DateExecutionDebut = log.DateIntervention;
            dt.DureeArretProductionHeures += log.ArretProductionHeures;

            // 2. Update the technician's role hours and costs
            var role = dt.Intervenants.FirstOrDefault(r => r.IntervenantId == log.IntervenantId);
            if (role == null)
            {
                var user = await context.Users.Include(u => u.WorkingProfile).FirstOrDefaultAsync(u => u.Id == log.IntervenantId);
                var rate = user?.WorkingProfile?.HourlyRate ?? 500;
                var unit = user?.WorkingProfile?.Unit ?? LaborUnit.Heure;

                role = new InterventionRole
                {
                    DemandeTravailId = dt.Id,
                    IntervenantId = log.IntervenantId,
                    Qualification = user?.Position ?? "Intervenant",
                    TauxHoraire = rate,
                    Unit = unit
                };
                context.InterventionRoles.Add(role);
                dt.Intervenants.Add(role);
            }

            role.HeuresTravaillees += log.DureeHeures;
            
            // 3. Update Pointage (for PDF Page 1)
            var jourDuMois = log.DateIntervention.Day;
            var pointage = role.Pointages.FirstOrDefault(p => p.DateIntervention.Date == log.DateIntervention.Date);
            if (pointage == null)
            {
                pointage = new PointageIntervention
                {
                    InterventionRoleId = role.Id,
                    DateIntervention = log.DateIntervention.Date,
                    JourDuMois = jourDuMois,
                    HeuresTravaillees = log.DureeHeures
                };
                context.PointageInterventions.Add(pointage);
            }
            else
            {
                pointage.HeuresTravaillees += log.DureeHeures;
            }

            // 4. Update Global Totals
            var hourlyRate = role.Unit == LaborUnit.Jour ? role.TauxHoraire / 8 : role.TauxHoraire;
            dt.TotalCoutMainOeuvre += (decimal)log.DureeHeures * hourlyRate;
            dt.TotalCoutOperation = dt.TotalCoutPieces + dt.TotalCoutMainOeuvre;

            await context.SaveChangesAsync();
        }
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var totalDtThisMonth = await context.DemandesTravail
            .CountAsync(d => d.DateEmission >= startOfMonth);

        var closedDtThisMonth = await context.DemandesTravail
            .CountAsync(d => d.DateEmission >= startOfMonth && d.Statut == StatutDT.Cloturee);

        var pendingDt = await context.DemandesTravail
            .CountAsync(d => d.Statut == StatutDT.EnCours || d.Statut == StatutDT.EnAttente);

        var stockAlerts = await context.ArticlesPdr
            .CountAsync(a => a.QuantiteEnStock <= a.SeuilAlerte);

        var stats = new DashboardStats
        {
            TotalDtThisMonth = totalDtThisMonth,
            PendingDt = pendingDt,
            StockAlerts = stockAlerts,
            RealizationRate = totalDtThisMonth > 0 ? (double)closedDtThisMonth / totalDtThisMonth * 100 : 0,
            MonthlyActivities = new List<MonthlyActivity>()
        };

        // Last 6 months for chart
        for (int i = 5; i >= 0; i--)
        {
            var date = now.AddMonths(-i);
            var monthStart = new DateTime(date.Year, date.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            
            var count = await context.DemandesTravail
                .CountAsync(d => d.DateEmission >= monthStart && d.DateEmission < monthEnd);

            stats.MonthlyActivities.Add(new MonthlyActivity
            {
                MonthName = date.ToString("MMM"),
                Count = count
            });
        }

        return stats;
    }
}
