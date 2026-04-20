using Microsoft.AspNetCore.SignalR;

namespace Sahel.GMAO.Web.Hubs;

public class GmaoHub : Hub
{
    public async Task SendNewDTAssigned(string userId, string numeroDT)
    {
        await Clients.User(userId).SendAsync("NewDTAssigned", numeroDT);
    }

    public async Task SendStatutChanged(int dtId, string numeroDT, string newStatut)
    {
        await Clients.All.SendAsync("StatutChanged", dtId, numeroDT, newStatut);
    }
}
