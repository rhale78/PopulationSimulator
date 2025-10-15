namespace PopulationSimulator.Models;

public class Person
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int BirthDay { get; set; } // Days since simulation start
    public int? DeathDay { get; set; } // Days since simulation start
    public bool IsAlive { get; set; } = true;
    
    // Parents
    public long? FatherId { get; set; }
    public long? MotherId { get; set; }
    
    // Spouse
    public long? SpouseId { get; set; }
    public long? SecondarySpouseId { get; set; }
    public int? MarriageDay { get; set; } // Days since simulation start
    
    // Location and Religion
    public long? CityId { get; set; }
    public long? CountryId { get; set; }
    public long? ReligionId { get; set; }
    
    // Occupation
    public long? JobId { get; set; }
    public int? JobStartDay { get; set; } // Days since simulation start
    
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
    
    // Physical traits
    public string EyeColor { get; set; } = string.Empty;
    public string HairColor { get; set; } = string.Empty;
    public int Height { get; set; } // in cm
    
    // Pregnancy tracking
    public bool IsPregnant { get; set; }
    public int? PregnancyDueDay { get; set; } // Days since simulation start
    public long? PregnancyFatherId { get; set; }
    public int PregnancyMultiplier { get; set; } = 1; // 1, 2, or 3 for twins/triplets
    
    // Dynasty
    public long? DynastyId { get; set; }
    public bool IsRuler { get; set; }
    
    // Wealth and Status
    public decimal Wealth { get; set; }
    public int SocialStatus { get; set; }
    
    // Helper methods
    public int GetAge(int currentDay)
    {
        if (!IsAlive && DeathDay.HasValue)
            return (DeathDay.Value - BirthDay) / 365;
        return (currentDay - BirthDay) / 365;
    }
    
    public bool IsMarried => SpouseId.HasValue;
    
    public bool CanHaveChildren(int currentDay)
    {
        if (Gender != "Female" || !IsAlive || !IsMarried) return false;
        int age = GetAge(currentDay);
        return age >= 14 && age <= 50 && !IsPregnant;
    }
    
    public bool IsEligibleForMarriage(int currentDay)
    {
        if (!IsAlive || IsMarried) return false;
        int age = GetAge(currentDay);
        return age >= 14;
    }
    
    public bool IsEligibleForJob(int currentDay)
    {
        if (!IsAlive || JobId.HasValue) return false;
        int age = GetAge(currentDay);
        return age >= 12;
    }
}
