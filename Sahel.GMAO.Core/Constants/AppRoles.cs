namespace Sahel.GMAO.Core.Constants;

public static class AppRoles
{
    public const string DSI = "DSI";
    public const string Demandeur = "Demandeur"; // Production
    public const string BM = "BM"; // Bureau de Méthode
    public const string Executant = "Executant";
    public const string Magazin = "Magazin";
    
    public static readonly string[] TousRoles = { DSI, Demandeur, BM, Executant, Magazin };
}
