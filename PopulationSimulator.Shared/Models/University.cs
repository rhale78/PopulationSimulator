namespace PopulationSimulator.Models;

public class University
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long CityId { get; set; }
    public DateTime FoundedDate { get; set; }
    public long? FounderId { get; set; }
    public int Capacity { get; set; }
    public int CurrentStudents { get; set; }
    public int PrestigeRating { get; set; } // 0-100, affects research and education quality
    public decimal Funding { get; set; }
    public int ResearchOutput { get; set; } // Contributes to invention discovery rate

    // Faculty tracking
    public long? ChancellorId { get; set; }
    public int ProfessorCount { get; set; }

    // Specializations
    public string PrimaryField { get; set; } = "General"; // General, Science, Arts, Medicine, Engineering, Philosophy
}
