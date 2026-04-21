using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface INatureTravailService
{
    Task<List<NatureTravail>> GetAllAsync();
    Task<NatureTravail?> GetByIdAsync(int id);
    Task CreateAsync(NatureTravail nature);
    Task UpdateAsync(NatureTravail nature);
    Task DeleteAsync(int id);
}
