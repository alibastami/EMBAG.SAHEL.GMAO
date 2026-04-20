using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Models;

namespace Sahel.GMAO.Web.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private const string UserSessionKey = "GmaoUserSession";
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_currentUser.Identity?.IsAuthenticated == true)
            {
                return new AuthenticationState(_currentUser);
            }

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var result = await _localStorage.GetAsync<UserSession>(UserSessionKey);
                    
                    if (result.Success && result.Value != null)
                    {
                        var session = result.Value;
                        if (session.Expiry > DateTime.UtcNow)
                        {
                            _currentUser = CreatePrincipalFromSession(session);
                            return new AuthenticationState(_currentUser);
                        }
                    }
                    break;
                }
                catch
                {
                    await Task.Delay(150);
                }
            }
        }
        catch
        {
            // Fail silently
        }
        finally
        {
            _semaphore.Release();
        }

        return new AuthenticationState(_currentUser);
    }

    public async Task MarkUserAsAuthenticated(User user)
    {
        var session = new UserSession
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            Position = user.Position,
            CanManageUsers = user.CanManageUsers,
            CanViewAudit = user.CanViewAudit,
            CanEditInventory = user.CanEditInventory,
            Expiry = DateTime.UtcNow.AddHours(8)
        };

        await _localStorage.SetAsync(UserSessionKey, session);
        _currentUser = CreatePrincipalFromSession(session);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.DeleteAsync(UserSessionKey);
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    private ClaimsPrincipal CreatePrincipalFromSession(UserSession session)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, session.Id.ToString()),
            new Claim(ClaimTypes.Name, session.Username),
            new Claim(ClaimTypes.Role, session.Role),
            new Claim("FullName", session.FullName),
            new Claim("Position", session.Position),
            new Claim("CanManageUsers", session.CanManageUsers.ToString().ToLower()),
            new Claim("CanViewAudit", session.CanViewAudit.ToString().ToLower()),
            new Claim("CanEditInventory", session.CanEditInventory.ToString().ToLower())
        }, "Authentication");

        return new ClaimsPrincipal(identity);
    }
}
