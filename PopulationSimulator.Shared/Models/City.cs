namespace PopulationSimulator.Models;

public class City
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? CountryId { get; set; }
    public int FoundedDay { get; set; } // Days since simulation start
    public int Population { get; set; }
    public long? FounderId { get; set; }
    public decimal Wealth { get; set; }
}
