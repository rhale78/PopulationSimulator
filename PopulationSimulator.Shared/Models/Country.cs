namespace PopulationSimulator.Models;

public class Country
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FoundedDay { get; set; } // Days since simulation start
    public long? RulerId { get; set; }
    public long? CapitalCityId { get; set; }
    public int Population { get; set; }
    public decimal Wealth { get; set; }
    public int MilitaryStrength { get; set; }
    public string GovernmentType { get; set; } = "Monarchy";
}
