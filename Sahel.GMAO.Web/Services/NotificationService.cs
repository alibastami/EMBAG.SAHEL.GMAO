using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Infrastructure.Data;
using Sahel.GMAO.Web.Hubs;

namespace Sahel.GMAO.Web.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<GmaoHub> _hubContext;
    private readonly IDbContextFactory<GmaoDbContext> _factory;

    public NotificationService(IHubContext<GmaoHub> hubContext, IDbContextFactory<GmaoDbContext> factory)
    {
        _hubContext = hubContext;
        _factory = factory;
    }

    public async Task NotifyNewDTAsync(int userId, string numeroDT)
    {
        var message = $"Une nouvelle DT ({numeroDT}) vous a été assignée.";
        await SaveNotificationAsync(userId, message);
        await _hubContext.Clients.User(userId.ToString()).SendAsync("NewDTAssigned", numeroDT);
    }

    public async Task NotifyStatutChangedAsync(int dtId, string numeroDT, string newStatut)
    {
        var message = $"La DT {numeroDT} est passée au statut : {newStatut}.";
        await SaveNotificationAsync(null, message, $"/demandes/{dtId}");
        await _hubContext.Clients.All.SendAsync("StatutChanged", dtId, numeroDT, newStatut);
    }

    public async Task NotifyLowStockAsync(string articleCode, double currentStock)
    {
        var message = $"Alerte Stock Bas : L'article {articleCode} est à {currentStock}.";
        await SaveNotificationAsync(null, message, "/stock");
        await _hubContext.Clients.All.SendAsync("LowStockAlert", articleCode, currentStock);
    }

    public async Task<List<AppNotification>> GetUserNotificationsAsync(int? userId)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.Notifications
            .Where(n => n.UserId == null || n.UserId == userId)
            .OrderByDescending(n => n.DateCreated)
            .Take(50)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        using var context = await _factory.CreateDbContextAsync();
        var notif = await context.Notifications.FindAsync(notificationId);
        if (notif != null)
        {
            notif.IsRead = true;
            await context.SaveChangesAsync();
        }
    }

    private async Task SaveNotificationAsync(int? userId, string message, string? link = null)
    {
        using var context = await _factory.CreateDbContextAsync();
        context.Notifications.Add(new AppNotification
        {
            UserId = userId,
            Message = message,
            Link = link,
            DateCreated = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}
