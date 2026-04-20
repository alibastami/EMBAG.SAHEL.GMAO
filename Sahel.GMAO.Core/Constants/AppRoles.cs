namespace Sahel.GMAO.Core.Constants;

public static class AppRoles
{
    public const string Ordonnanceur = "Ordonnanceur";
    public const string Mecanicien = "Mécanicien";
    public const string Electricien = "Électricien";
    
    public static readonly string[] Techniciens = { Mecanicien, Electricien };
}
