namespace Sahel.GMAO.Core.Models;

public class UserSession
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    
    public bool CanManageUsers { get; set; }
    public bool CanViewAudit { get; set; }
    public bool CanEditInventory { get; set; }
    
    public DateTime Expiry { get; set; }
}
