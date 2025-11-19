namespace PopulationSimulator.Models;

public class TradeRoute
{
    public long Id { get; set; }
    public long City1Id { get; set; }
    public long City2Id { get; set; }
    public DateTime EstablishedDate { get; set; }
    public decimal TradeVolume { get; set; }
    public string GoodsTraded { get; set; } = string.Empty; // Comma-separated
    public bool IsActive { get; set; } = true;
    public decimal TotalWealth Generated { get; set; }
}
