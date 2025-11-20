namespace PopulationSimulator.Models;

public class School
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long CityId { get; set; }
    public DateTime FoundedDate { get; set; }
    public long? FounderId { get; set; }
    public int Capacity { get; set; } // Maximum number of students
    public int CurrentStudents { get; set; }
    public int QualityRating { get; set; } // 0-100, affects education quality
    public string Type { get; set; } = "Primary"; // Primary, Secondary
    public decimal Funding { get; set; }

    // Teacher tracking
    public long? HeadTeacherId { get; set; }
    public int TeacherCount { get; set; }
}
