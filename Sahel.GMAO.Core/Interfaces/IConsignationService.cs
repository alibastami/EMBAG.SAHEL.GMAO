using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IConsignationService
{
    Task<List<BonDeConsignation>> GetAllAsync();
    Task<List<BonDeConsignation>> GetByDemandeIdAsync(int dtId);
    Task<BonDeConsignation?> GetByIdAsync(int id);
    Task<BonDeConsignation> CreateAsync(BonDeConsignation bon);
    Task UpdateAsync(BonDeConsignation bon);
    Task DeconsignerAsync(int bonId, int userDeconsigId);
    Task DeleteAsync(int id);
}
