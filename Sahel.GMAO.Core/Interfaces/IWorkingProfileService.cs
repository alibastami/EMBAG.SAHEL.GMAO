using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IWorkingProfileService
{
    Task<List<WorkingProfile>> GetAllAsync();
    Task<WorkingProfile?> GetByIdAsync(int id);
    Task CreateAsync(WorkingProfile profile);
    Task UpdateAsync(WorkingProfile profile);
    Task DeleteAsync(int id);
}
