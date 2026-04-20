using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IEquipementService
{
    Task<List<Equipement>> GetAllAsync();
    Task<Equipement?> GetByIdAsync(int id);
    Task<Equipement?> GetByCodeAsync(string code);
    Task CreateAsync(Equipement equipement);
    Task UpdateAsync(Equipement equipement);
    Task DeleteAsync(int id);
}
