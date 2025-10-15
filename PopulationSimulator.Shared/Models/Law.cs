namespace PopulationSimulator.Models;

public class Law
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long? CountryId { get; set; }
    public long? ReligionId { get; set; }
    public int EnactedDay { get; set; } // Days since simulation start
    public string Category { get; set; } = string.Empty;
}
