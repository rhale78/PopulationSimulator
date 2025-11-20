using PopulationSimulator.Models;

namespace PopulationSimulator.Core;

/// <summary>
/// Identifies and tracks geniuses based on exceptional genetic traits
/// Geniuses can accelerate invention discovery, improve war outcomes, found religions, etc.
/// </summary>
public class GeniusSystem
{
    private readonly Random _random;

    public GeniusSystem(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Determine if a person is a genius in any field based on their genetics
    /// </summary>
    public (bool isGenius, string? geniusType, string? description) EvaluateGenius(Person person)
    {
        // Genius threshold: 90+ in primary trait, 70+ in secondary
        // Plus good genetics (Longevity 60+, Intelligence 70+)

        // Scientific Genius - accelerates inventions (Intelligence + Creativity + Wisdom)
        if (person.Intelligence >= 90 && person.Creativity >= 70 && person.Wisdom >= 70)
        {
            return (true, "Scientific Genius", $"Exceptional mind capable of revolutionary discoveries (Int:{person.Intelligence}, Cre:{person.Creativity}, Wis:{person.Wisdom})");
        }

        // Military Genius - improves war outcomes (Leadership + Aggression + Intelligence)
        if (person.Leadership >= 90 && person.Aggression >= 60 && person.Intelligence >= 70)
        {
            return (true, "Military Genius", $"Brilliant strategist and commander (Lead:{person.Leadership}, Agg:{person.Aggression}, Int:{person.Intelligence})");
        }

        // Diplomatic Genius - improves relations, prevents wars (Charisma + Wisdom + Intelligence)
        if (person.Charisma >= 90 && person.Wisdom >= 70 && person.Intelligence >= 70)
        {
            return (true, "Diplomatic Genius", $"Master negotiator and peacemaker (Char:{person.Charisma}, Wis:{person.Wisdom}, Int:{person.Intelligence})");
        }

        // Religious Leader - founds influential religions (Charisma + Wisdom + Creativity)
        if (person.Charisma >= 85 && person.Wisdom >= 85 && person.Creativity >= 70)
        {
            return (true, "Religious Visionary", $"Spiritual leader with profound influence (Char:{person.Charisma}, Wis:{person.Wisdom}, Cre:{person.Creativity})");
        }

        // Artistic Genius - creates cultural movements (Creativity + Beauty + Charisma)
        if (person.Creativity >= 90 && person.Beauty >= 70 && person.Charisma >= 70)
        {
            return (true, "Artistic Genius", $"Visionary artist shaping culture (Cre:{person.Creativity}, Beauty:{person.Beauty}, Char:{person.Charisma})");
        }

        // Economic Genius - builds wealth, trade (Intelligence + Charisma + Wisdom)
        if (person.Intelligence >= 85 && person.Charisma >= 80 && person.Wisdom >= 75)
        {
            return (true, "Economic Genius", $"Master of commerce and prosperity (Int:{person.Intelligence}, Char:{person.Charisma}, Wis:{person.Wisdom})");
        }

        // Builder/Engineer Genius - constructs wonders (Intelligence + Creativity + Strength)
        if (person.Intelligence >= 85 && person.Creativity >= 80 && person.Strength >= 70)
        {
            return (true, "Engineering Genius", $"Master builder and architect (Int:{person.Intelligence}, Cre:{person.Creativity}, Str:{person.Strength})");
        }

        // Medical Genius - advances health (Intelligence + Wisdom + Fertility as healing touch)
        if (person.Intelligence >= 85 && person.Wisdom >= 80 && person.Health >= 85)
        {
            return (true, "Medical Genius", $"Revolutionary healer and physician (Int:{person.Intelligence}, Wis:{person.Wisdom}, Health:{person.Health})");
        }

        return (false, null, null);
    }

    /// <summary>
    /// Calculate invention discovery bonus for scientific geniuses
    /// </summary>
    public double GetInventionBonus(Person person, string geniusType)
    {
        if (geniusType == "Scientific Genius")
        {
            // 2x to 4x faster invention discovery based on intelligence
            return 1.0 + (person.Intelligence / 50.0); // 1.8x to 2.0x typical
        }
        if (geniusType == "Engineering Genius")
        {
            // 50% bonus for construction/engineering inventions
            return 1.5;
        }
        if (geniusType == "Medical Genius")
        {
            // 2x bonus for medical inventions
            return 2.0;
        }
        return 1.0;
    }

    /// <summary>
    /// Calculate military strength bonus for military geniuses
    /// </summary>
    public double GetMilitaryBonus(Person person, string geniusType)
    {
        if (geniusType == "Military Genius")
        {
            // 20% to 50% bonus based on leadership
            return 1.0 + (person.Leadership / 200.0);
        }
        return 1.0;
    }

    /// <summary>
    /// Calculate trade/wealth bonus for economic geniuses
    /// </summary>
    public double GetEconomicBonus(Person person, string geniusType)
    {
        if (geniusType == "Economic Genius")
        {
            // 30% to 70% bonus to wealth generation
            return 1.0 + (person.Intelligence / 150.0);
        }
        return 1.0;
    }

    /// <summary>
    /// Calculate religion founding bonus for religious visionaries
    /// </summary>
    public double GetReligiousBonus(Person person, string geniusType)
    {
        if (geniusType == "Religious Visionary")
        {
            // Much higher follower attraction
            return 2.0 + (person.Charisma / 100.0);
        }
        return 1.0;
    }

    /// <summary>
    /// Determine if a genius should be marked as Notable
    /// </summary>
    public bool ShouldMarkAsNotable(string geniusType)
    {
        // All geniuses are notable by default
        return true;
    }

    /// <summary>
    /// Get a notable description for a genius
    /// </summary>
    public string GetNotableDescription(string geniusType, Person person)
    {
        return geniusType switch
        {
            "Scientific Genius" => $"Revolutionary scientist who advanced human knowledge",
            "Military Genius" => $"Legendary military commander and strategist",
            "Diplomatic Genius" => $"Master diplomat who shaped international relations",
            "Religious Visionary" => $"Spiritual leader who founded influential beliefs",
            "Artistic Genius" => $"Visionary artist who transformed culture",
            "Economic Genius" => $"Master merchant who built vast wealth",
            "Engineering Genius" => $"Master builder who created architectural wonders",
            "Medical Genius" => $"Revolutionary physician who saved countless lives",
            _ => "Notable individual"
        };
    }
}
