namespace PopulationSimulator.Models;

public class War
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartDay { get; set; } // Days since simulation start
    public int? EndDay { get; set; } // Days since simulation start
    public long AttackerCountryId { get; set; }
    public long DefenderCountryId { get; set; }
    public long? WinnerCountryId { get; set; }
    public int Casualties { get; set; }
    public bool IsActive { get; set; } = true;
}
