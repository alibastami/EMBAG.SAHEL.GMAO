namespace Sahel.GMAO.Core.Models;

public class DashboardStats
{
    public int TotalDtThisMonth { get; set; }
    public int PendingDt { get; set; }
    public int StockAlerts { get; set; }
    public double RealizationRate { get; set; }
    public List<MonthlyActivity> MonthlyActivities { get; set; } = new();
}

public class MonthlyActivity
{
    public string MonthName { get; set; } = string.Empty;
    public int Count { get; set; }
}
