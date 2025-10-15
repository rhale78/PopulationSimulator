namespace PopulationSimulator.Models;

public class Dynasty
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long FounderId { get; set; }
    public int FoundedDay { get; set; } // Days since simulation start
    public long? CurrentRulerId { get; set; }
    public int MemberCount { get; set; }
}
