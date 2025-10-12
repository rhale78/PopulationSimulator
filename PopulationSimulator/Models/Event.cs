namespace PopulationSimulator.Models;

public class Event
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long? PersonId { get; set; }
    public long? CityId { get; set; }
    public long? CountryId { get; set; }
    public long? ReligionId { get; set; }
    public long? WarId { get; set; }
    public long? InventionId { get; set; }
}
