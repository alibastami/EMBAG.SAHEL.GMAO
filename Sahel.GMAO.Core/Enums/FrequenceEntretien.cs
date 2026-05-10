namespace Sahel.GMAO.Core.Enums;

/// <summary>
/// Represents the running-hour threshold for preventive maintenance tasks.
/// Used in DOC 07 (Planning Entretien Préventif) where the grid columns are
/// 200H, 600H, 1200H, and 2400H operating hours.
/// </summary>
public enum FrequenceEntretien
{
    H200 = 1,
    H600 = 2,
    H1200 = 3,
    H2400 = 4
}
