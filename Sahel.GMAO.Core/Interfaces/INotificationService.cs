using System.Collections.Generic;
using System.Threading.Tasks;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Core.Interfaces;

public interface INotificationService
{
    Task NotifyNewDTAsync(int userId, string numeroDT);
    Task NotifyStatutChangedAsync(int dtId, string numeroDT, string newStatut);
    Task NotifyLowStockAsync(string articleCode, double currentStock);
    Task<List<AppNotification>> GetUserNotificationsAsync(int? userId);
    Task MarkAsReadAsync(int notificationId);
}
