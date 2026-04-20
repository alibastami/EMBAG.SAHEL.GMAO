using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface IMaintenanceService
{
    Task<List<MaintenancePreventive>> GetAllPreventiveAsync();
    Task<MaintenancePreventive?> GetPreventiveByIdAsync(int id);
    Task RegisterRealizationAsync(int id, DateTime realizationDate);
    Task UpdatePreventiveAsync(MaintenancePreventive mp);
    Task CreatePreventiveAsync(MaintenancePreventive mp);
    Task DeletePreventiveAsync(int id);
}
