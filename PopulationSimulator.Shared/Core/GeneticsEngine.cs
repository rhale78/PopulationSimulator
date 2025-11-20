using PopulationSimulator.Models;

namespace PopulationSimulator.Core;

public class GeneticsEngine
{
    private readonly Random _random;
    private static readonly string[] BloodTypes = { "O+", "O-", "A+", "A-", "B+", "B-", "AB+", "AB-" };
    private static readonly string[] HereditaryConditions =
    {
        "None", "Hemophilia", "Color Blindness", "Sickle Cell Trait", "Thalassemia",
        "Lactose Intolerance", "G6PD Deficiency", "Cystic Fibrosis Carrier"
    };
    private static readonly string[] GeneticMarkers =
    {
        "HLA-A", "HLA-B", "HLA-C", "BRCA1", "BRCA2", "APOE", "CCR5", "MC1R"
    };
    private static readonly char[] DNABases = { 'A', 'T', 'G', 'C' };

    public GeneticsEngine(Random random)
    {
        _random = random;
    }

    /// <summary>
    /// Generate initial DNA sequence for Adam or Eve
    /// </summary>
    public string GeneratePrimordialDNA()
    {
        var dna = new char[32];
        for (int i = 0; i < 32; i++)
        {
            dna[i] = DNABases[_random.Next(4)];
        }
        return new string(dna);
    }

    /// <summary>
    /// Inherit DNA from both parents with recombination and rare mutations
    /// </summary>
    public string InheritDNA(string fatherDNA, string motherDNA)
    {
        if (string.IsNullOrEmpty(fatherDNA) || string.IsNullOrEmpty(motherDNA))
            return GeneratePrimordialDNA();

        var childDNA = new char[32];
        for (int i = 0; i < 32; i++)
        {
            // Each gene has 50% chance from each parent
            char gene = _random.Next(2) == 0 ? fatherDNA[i] : motherDNA[i];

            // 2% chance of mutation per gene
            if (_random.NextDouble() < 0.02)
            {
                gene = DNABases[_random.Next(4)];
            }

            childDNA[i] = gene;
        }
        return new string(childDNA);
    }

    /// <summary>
    /// Determine blood type from parents using Mendelian genetics
    /// </summary>
    public string InheritBloodType(string fatherBlood, string motherBlood)
    {
        if (string.IsNullOrEmpty(fatherBlood) || string.IsNullOrEmpty(motherBlood))
            return BloodTypes[_random.Next(BloodTypes.Length)];

        // Simplified Mendelian inheritance
        // A and B are codominant, O is recessive
        // Rh+ is dominant over Rh-

        string fatherType = fatherBlood.TrimEnd('+', '-');
        string motherType = motherBlood.TrimEnd('+', '-');
        bool fatherRhPos = fatherBlood.EndsWith('+');
        bool motherRhPos = motherBlood.EndsWith('+');

        // Determine ABO type
        string childType = DetermineABOType(fatherType, motherType);

        // Determine Rh factor (simplified - if either parent is Rh+, 75% chance of Rh+)
        bool childRhPos = (fatherRhPos || motherRhPos) ? _random.Next(100) < 75 : false;

        return childType + (childRhPos ? "+" : "-");
    }

    private string DetermineABOType(string father, string mother)
    {
        // Simplified genetics - treating each type as if homozygous for simplicity
        var possibleTypes = new List<string>();

        if (father == "O" && mother == "O") return "O";
        if (father == "AB" || mother == "AB")
        {
            if (father == "O" || mother == "O")
                possibleTypes.AddRange(new[] { "A", "B" });
            else
                possibleTypes.AddRange(new[] { "A", "B", "AB" });
        }
        else if (father == "A" && mother == "A")
            possibleTypes.AddRange(new[] { "A", "O" });
        else if (father == "B" && mother == "B")
            possibleTypes.AddRange(new[] { "B", "O" });
        else if ((father == "A" && mother == "O") || (father == "O" && mother == "A"))
            possibleTypes.AddRange(new[] { "A", "O" });
        else if ((father == "B" && mother == "O") || (father == "O" && mother == "B"))
            possibleTypes.AddRange(new[] { "B", "O" });
        else if ((father == "A" && mother == "B") || (father == "B" && mother == "A"))
            possibleTypes.AddRange(new[] { "A", "B", "AB", "O" });

        return possibleTypes.Any() ? possibleTypes[_random.Next(possibleTypes.Count)] : "O";
    }

    /// <summary>
    /// Generate genetic markers (simplified)
    /// </summary>
    public string GenerateGeneticMarkers(string? fatherMarkers, string? motherMarkers)
    {
        var markers = new List<string>();
        int numMarkers = _random.Next(2, 5); // 2-4 markers

        for (int i = 0; i < numMarkers; i++)
        {
            markers.Add(GeneticMarkers[_random.Next(GeneticMarkers.Length)]);
        }

        return string.Join(",", markers.Distinct());
    }

    /// <summary>
    /// Determine hereditary conditions from parents
    /// </summary>
    public (string conditions, bool hasDisease) InheritConditions(Person father, Person mother)
    {
        var conditions = new List<string>();
        bool hasDisease = false;

        // 10% chance to inherit condition from father
        if (!string.IsNullOrEmpty(father.HereditaryConditions) && _random.Next(100) < 10)
        {
            var fatherConditions = father.HereditaryConditions.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (fatherConditions.Any() && fatherConditions[0] != "None")
            {
                conditions.AddRange(fatherConditions);
            }
        }

        // 10% chance to inherit condition from mother
        if (!string.IsNullOrEmpty(mother.HereditaryConditions) && _random.Next(100) < 10)
        {
            var motherConditions = mother.HereditaryConditions.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (motherConditions.Any() && motherConditions[0] != "None")
            {
                conditions.AddRange(motherConditions);
            }
        }

        // 3% chance of new genetic condition
        if (_random.Next(100) < 3)
        {
            conditions.Add(HereditaryConditions[_random.Next(1, HereditaryConditions.Length)]);
        }

        var distinctConditions = conditions.Distinct().ToList();

        // Some conditions are diseases, others are just traits
        hasDisease = distinctConditions.Any(c =>
            c.Contains("Hemophilia") ||
            c.Contains("Sickle Cell") ||
            c.Contains("Cystic Fibrosis") ||
            c.Contains("Thalassemia"));

        return (distinctConditions.Any() ? string.Join(",", distinctConditions) : "None", hasDisease);
    }

    /// <summary>
    /// Calculate disease resistance based on genetics
    /// </summary>
    public int CalculateDiseaseResistance(Person father, Person mother, string dnaSequence)
    {
        // Base resistance is average of parents
        int baseResistance = (father.DiseaseResistance + mother.DiseaseResistance) / 2;

        // Count number of 'G' and 'C' bases (stronger bonds) for a bonus
        int strongBases = dnaSequence.Count(c => c == 'G' || c == 'C');
        int bonus = (strongBases - 16) * 2; // +/- bonus based on deviation from average

        // Random variation
        int variation = _random.Next(-10, 11);

        return Math.Clamp(baseResistance + bonus + variation, 0, 100);
    }

    /// <summary>
    /// Calculate genetic longevity predisposition
    /// </summary>
    public int CalculateLongevity(Person father, Person mother, string dnaSequence)
    {
        // Base longevity is average of parents with slight increase (evolution)
        int baseLongevity = (father.Longevity + mother.Longevity) / 2 + 1;

        // DNA quality affects longevity
        int dnaQuality = CalculateDNAQuality(dnaSequence);

        // Random variation
        int variation = _random.Next(-8, 9);

        return Math.Clamp(baseLongevity + dnaQuality + variation, 0, 100);
    }

    private int CalculateDNAQuality(string dna)
    {
        // Check for repeating patterns (lower quality)
        int repeats = 0;
        for (int i = 0; i < dna.Length - 1; i++)
        {
            if (dna[i] == dna[i + 1])
                repeats++;
        }

        // Fewer repeats = higher quality
        return (16 - repeats) / 2; // Range approximately -8 to +8
    }

    /// <summary>
    /// Inherit trait with genetic influence
    /// </summary>
    public int InheritTrait(int fatherTrait, int motherTrait, int longevityBonus = 0)
    {
        // Average of parents
        int baseTrait = (fatherTrait + motherTrait) / 2;

        // Random variation (-15 to +15)
        int variation = _random.Next(-15, 16);

        // 5% chance of significant mutation
        if (_random.Next(100) < 5)
        {
            variation += _random.Next(-20, 21);
        }

        return Math.Clamp(baseTrait + variation + longevityBonus, 0, 100);
    }
}
