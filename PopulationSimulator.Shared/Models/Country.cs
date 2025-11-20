namespace PopulationSimulator.Models;

public class Country
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime FoundedDate { get; set; }
    public long? RulerId { get; set; }
    public long? CapitalCityId { get; set; }
    public int Population { get; set; }
    public decimal Wealth { get; set; }
    public int MilitaryStrength { get; set; }
    public string GovernmentType { get; set; } = "Monarchy";

    // Geography for realistic disasters
    public string DominantGeography { get; set; } = "Plains"; // Coastal, Mountain, Plains, Desert, Forest, RiverValley, Island, Mixed
    public string DominantClimate { get; set; } = "Temperate"; // Tropical, Temperate, Arid, Arctic, Mediterranean
}
