using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Enums;
using Sahel.GMAO.Core.Models;

namespace Sahel.GMAO.Core.Interfaces;

public interface IDtService
{
    Task<List<DemandeTravail>> GetAllAsync();
    Task<List<DemandeTravail>> GetForUserAsync(string username, string role, bool mineOnly = false);
    Task<List<DemandeTravail>> GetByStatusAsync(StatutDT statut);
    Task<DemandeTravail?> GetByIdAsync(int id);
    Task<DemandeTravail> CreateAsync(DemandeTravail dt);
    Task UpdateStatusAsync(int dtId, StatutDT nouveauStatut);
    Task UpdateExecutionAsync(DemandeTravail dt);
    Task UpdateAsync(DemandeTravail dt);
    Task AddConsommableAsync(int dtId, int articleId, double quantite, decimal? prixUnitaire = null, string? nBsm = null, string? observation = null);
    Task AddIntervenantAsync(int dtId, int userId, double heures, string qualification, decimal? tauxHoraire = null, LaborUnit? unit = null);
    Task AddInterventionLogAsync(InterventionLog log);
    Task<DashboardStats> GetDashboardStatsAsync();
}
