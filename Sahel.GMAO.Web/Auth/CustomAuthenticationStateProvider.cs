using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Sahel.GMAO.Core.Entities;
using Sahel.GMAO.Core.Models;
using Serilog;

namespace Sahel.GMAO.Web.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private const string UserSessionKey = "UserSession";
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage, IHttpContextAccessor httpContextAccessor)
    {
        _localStorage = localStorage;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            // 1. If already authenticated in memory (current circuit), return it
            if (_currentUser.Identity?.IsAuthenticated == true)
            {
                return new AuthenticationState(_currentUser);
            }

            // 2. Try to recover from HttpContext (Cookie)
            // This is populated during the initial request that started the circuit
            var httpUser = _httpContextAccessor.HttpContext?.User;
            if (httpUser?.Identity?.IsAuthenticated == true)
            {
                _currentUser = httpUser;
                Serilog.Log.Information("[Auth] Session recovered from HttpContext (Cookie) for: {Username}", _currentUser.Identity.Name);
                return new AuthenticationState(_currentUser);
            }

            // 3. Fallback: Try to recover from local storage (handles scenarios where HttpContext is lost)
            Serilog.Log.Information("[Auth] HttpContext anonymous. Attempting session recovery from local storage...");

            for (int i = 0; i < 5; i++) // Reduced retries since HttpContext handles most cases
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
                            Serilog.Log.Information("[Auth] Session recovered from LocalStorage for: {Username}", _currentUser.Identity?.Name);
                            _ = UpdateSessionExpiry(session);
                            return new AuthenticationState(_currentUser);
                        }
                    }
                }
                catch 
                {
                    await Task.Delay(200);
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "[Auth] Critical error during GetAuthenticationStateAsync");
        }
        finally
        {
            _semaphore.Release();
        }

        return new AuthenticationState(_currentUser);
    }

    private async Task UpdateSessionExpiry(UserSession session)
    {
        try 
        {
            session.Expiry = DateTime.UtcNow.AddHours(8);
            await _localStorage.SetAsync(UserSessionKey, session);
        } catch { /* Ignore background update errors */ }
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
