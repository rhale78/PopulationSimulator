namespace PopulationSimulator.Models;

public class Job
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinIntelligence { get; set; }
    public int MinStrength { get; set; }
    public int MinAge { get; set; }
    public decimal BaseSalary { get; set; }
    public int SocialStatusBonus { get; set; }
    public double DeathRiskModifier { get; set; } = 1.0; // 1.0 = normal, >1.0 = more dangerous
    public bool RequiresInvention { get; set; }
    public long? RequiredInventionId { get; set; }
}
