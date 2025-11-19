namespace PopulationSimulator.Models;

public class Person
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public bool IsAlive { get; set; } = true;
    public string? CauseOfDeath { get; set; }

    // Parents
    public long? FatherId { get; set; }
    public long? MotherId { get; set; }

    // Spouse
    public long? SpouseId { get; set; }
    public long? SecondarySpouseId { get; set; }
    public DateTime? MarriageDate { get; set; }

    // Location and Religion
    public long? CityId { get; set; }
    public long? CountryId { get; set; }
    public long? ReligionId { get; set; }

    // Occupation
    public long? JobId { get; set; }
    public DateTime? JobStartDate { get; set; }

    // Genetics and Traits (0-100 scale)
    public int Intelligence { get; set; }
    public int Strength { get; set; }
    public int Health { get; set; }
    public int Fertility { get; set; }
    public int Charisma { get; set; }
    public int Creativity { get; set; }
    public int Leadership { get; set; }
    public int Aggression { get; set; }
    public int Wisdom { get; set; }
    public int Beauty { get; set; }

    // Advanced Genetics
    public string DNASequence { get; set; } = string.Empty; // Simplified 32-character sequence
    public string BloodType { get; set; } = string.Empty; // A+, A-, B+, B-, AB+, AB-, O+, O-
    public string GeneticMarkers { get; set; } = string.Empty; // Comma-separated markers
    public string HereditaryConditions { get; set; } = string.Empty; // Comma-separated conditions
    public bool HasHereditaryDisease { get; set; }
    public int DiseaseResistance { get; set; } // 0-100 scale
    public int Longevity { get; set; } // 0-100 scale (genetic predisposition to long life)

    // Physical traits
    public string EyeColor { get; set; } = string.Empty;
    public string HairColor { get; set; } = string.Empty;
    public string SkinTone { get; set; } = string.Empty;
    public int Height { get; set; } // in cm
    public int Weight { get; set; } // in kg
    public string BuildType { get; set; } = string.Empty; // Slim, Average, Muscular, Heavy

    // Pregnancy tracking
    public bool IsPregnant { get; set; }
    public DateTime? PregnancyDueDate { get; set; }
    public long? PregnancyFatherId { get; set; }
    public int PregnancyMultiplier { get; set; } = 1; // 1, 2, or 3 for twins/triplets
    public int TotalChildren { get; set; } // Total number of children born

    // Dynasty
    public long? DynastyId { get; set; }
    public bool IsRuler { get; set; }
    public int GenerationNumber { get; set; } // Generation from Adam/Eve

    // Wealth and Status
    public decimal Wealth { get; set; }
    public int SocialStatus { get; set; }

    // Achievements and Notable Status
    public bool IsNotable { get; set; }
    public string NotableFor { get; set; } = string.Empty; // Reason for being notable
    public int ChildrenBorn { get; set; } // Counter for children produced
    public int DescendantCount { get; set; } // Total living descendants
    
    // Helper methods
    public int GetAge(DateTime currentDate)
    {
        if (!IsAlive && DeathDate.HasValue)
            return (int)((DeathDate.Value - BirthDate).TotalDays / 365.25);
        return (int)((currentDate - BirthDate).TotalDays / 365.25);
    }
    
    public bool IsMarried => SpouseId.HasValue;
    
    public bool CanHaveChildren(DateTime currentDate)
    {
        if (Gender != "Female" || !IsAlive || !IsMarried) return false;
        int age = GetAge(currentDate);
        return age >= 14 && age <= 50 && !IsPregnant;
    }
    
    public bool IsEligibleForMarriage(DateTime currentDate)
    {
        if (!IsAlive || IsMarried) return false;
        int age = GetAge(currentDate);
        return age >= 14;
    }
    
    public bool IsEligibleForJob(DateTime currentDate)
    {
        if (!IsAlive || JobId.HasValue) return false;
        int age = GetAge(currentDate);
        return age >= 12;
    }
}
