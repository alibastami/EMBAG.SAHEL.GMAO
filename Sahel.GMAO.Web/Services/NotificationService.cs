using Microsoft.AspNetCore.SignalR;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Web.Hubs;

namespace Sahel.GMAO.Web.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<GmaoHub> _hubContext;

    public NotificationService(IHubContext<GmaoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewDTAsync(int userId, string numeroDT)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("NewDTAssigned", numeroDT);
    }

    public async Task NotifyStatutChangedAsync(int dtId, string numeroDT, string newStatut)
    {
        await _hubContext.Clients.All.SendAsync("StatutChanged", dtId, numeroDT, newStatut);
    }

    public async Task NotifyLowStockAsync(string articleCode, double currentStock)
    {
        await _hubContext.Clients.All.SendAsync("LowStockAlert", articleCode, currentStock);
    }
}
