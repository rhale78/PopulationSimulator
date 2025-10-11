namespace PopulationSimulator.Models;

public class Invention
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DiscoveredDate { get; set; }
    public long? InventorId { get; set; }
    public int RequiredIntelligence { get; set; }
    public string Category { get; set; } = string.Empty;
}
