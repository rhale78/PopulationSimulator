namespace PopulationSimulator.Models;

public class Dynasty
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long FounderId { get; set; }
    public DateTime FoundedDate { get; set; }
    public long? CurrentRulerId { get; set; }
    public int MemberCount { get; set; }
}
