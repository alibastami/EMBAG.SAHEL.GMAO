using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Enums;

namespace Sahel.GMAO.Core.Interfaces;

public interface IDtService
{
    Task<List<DemandeTravail>> GetAllAsync();
    Task<List<DemandeTravail>> GetForUserAsync(string username, string role);
    Task<List<DemandeTravail>> GetByStatusAsync(StatutDT statut);
    Task<DemandeTravail?> GetByIdAsync(int id);
    Task<DemandeTravail> CreateAsync(DemandeTravail dt);
    Task UpdateStatusAsync(int dtId, StatutDT nouveauStatut);
    Task UpdateExecutionAsync(DemandeTravail dt);
    Task UpdateAsync(DemandeTravail dt);
    Task AddConsommableAsync(int dtId, int articleId, double quantite);
    Task AddIntervenantAsync(int dtId, int userId, double heures, string qualification);
}
