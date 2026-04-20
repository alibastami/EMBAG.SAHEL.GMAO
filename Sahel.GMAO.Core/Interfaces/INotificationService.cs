using System.Threading.Tasks;

namespace Sahel.GMAO.Core.Interfaces;

public interface INotificationService
{
    Task NotifyNewDTAsync(int userId, string numeroDT);
    Task NotifyStatutChangedAsync(int dtId, string numeroDT, string newStatut);
    Task NotifyLowStockAsync(string articleCode, double currentStock);
}
