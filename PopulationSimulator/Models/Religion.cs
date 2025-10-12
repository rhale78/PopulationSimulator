namespace PopulationSimulator.Models;

public class Religion
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime FoundedDate { get; set; }
    public long? FounderId { get; set; }
    public int Followers { get; set; }
    public string Beliefs { get; set; } = string.Empty;
    public bool AllowsPolygamy { get; set; }
}
