namespace PopulationSimulator.Models;

public class NaturalDisaster
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty; // Earthquake, Flood, Plague, Famine, Drought, Hurricane, Volcano, Wildfire
    public DateTime OccurredDate { get; set; }
    public long? CityId { get; set; } // Affected city (null = regional/global)
    public long? CountryId { get; set; } // Affected country
    public int Severity { get; set; } // 1-10
    public int Deaths { get; set; }
    public decimal EconomicDamage { get; set; }
    public int BuildingsDestroyed { get; set; }
    public int PeopleDisplaced { get; set; }
    public string Description { get; set; } = string.Empty;
}

public enum DisasterType
{
    Earthquake,      // Destroys buildings, kills people
    Flood,           // Damages crops, destroys property
    Plague,          // Disease outbreak, kills based on disease resistance
    Famine,          // Starvation, kills based on wealth/food stores
    Drought,         // Crop failure, economic damage
    Hurricane,       // Coastal cities, property damage
    Volcano,         // Rare but devastating
    Wildfire,        // Destroys crops and buildings
    Tsunami,         // Coastal devastation
    Blizzard,        // Cold-related deaths
    Tornado,         // Localized destruction
    Landslide        // Mountain regions
}
