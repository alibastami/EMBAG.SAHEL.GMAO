using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IFabricationService
{
    Task<List<DemandeFabrication>> GetAllAsync();
    Task<DemandeFabrication?> GetByIdAsync(int id);
    Task<DemandeFabrication> CreateAsync(DemandeFabrication df);
    Task UpdateStatusAsync(int id, StatutFabrication nouveauStatut);
    Task DeleteAsync(int id);
}
