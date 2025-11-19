using PopulationSimulator.Models;
using PopulationSimulator.Data;

namespace PopulationSimulator.Core;

public class Simulator
{
    private readonly Random _random;
    private readonly NameGenerator _nameGenerator;
    private readonly GeneticsEngine _geneticsEngine;
    private readonly GeniusSystem _geniusSystem;
    private readonly DataAccessLayer _dataAccess;
    
    // In-memory collections
    private readonly List<Person> _people = new();
    private readonly Dictionary<long, Person> _peopleById = new();
    private readonly Dictionary<long, Person> _deadPeopleById = new(); // Track dead people for name lookup
    private readonly List<Person> _livingPeopleCache = new(); // Cache of living people for performance
    private readonly List<City> _cities = new();
    private readonly Dictionary<long, City> _citiesById = new();
    private readonly List<Country> _countries = new();
    private readonly Dictionary<long, Country> _countriesById = new();
    private readonly List<Religion> _religions = new();
    private readonly Dictionary<long, Religion> _religionsById = new();
    private readonly List<Job> _jobs = new();
    private readonly Dictionary<long, Job> _jobsById = new();
    private readonly List<Invention> _inventions = new();
    private readonly Dictionary<long, Invention> _inventionsById = new();
    private readonly List<War> _wars = new();
    private readonly List<Dynasty> _dynasties = new();
    private readonly Dictionary<long, Dynasty> _dynastiesById = new();
    private readonly List<TradeRoute> _tradeRoutes = new();
    private readonly Dictionary<long, TradeRoute> _tradeRoutesById = new();
    private readonly List<Business> _businesses = new();
    private readonly Dictionary<long, Business> _businessesById = new();
    private readonly List<BusinessEmployee> _businessEmployees = new();
    private readonly List<NaturalDisaster> _disasters = new();
    private readonly List<Event> _recentEvents = new();
    private readonly List<School> _schools = new();
    private readonly Dictionary<long, School> _schoolsById = new();
    private readonly List<University> _universities = new();
    private readonly Dictionary<long, University> _universitiesById = new();
    
    private DateTime _currentDate;
    private long _nextTempId = -1;
    private int _generationNumber = 0;
    private int _syncCounter = 0;
    private const int SYNC_INTERVAL = 100; // Sync every 100 days
    private bool _livingCacheValid = false;

    private long? _adamId;
    private long? _eveId;
    
    public Simulator()
    {
        _random = new Random();
        _nameGenerator = new NameGenerator(_random);
        _geneticsEngine = new GeneticsEngine(_random);
        _geniusSystem = new GeniusSystem(_random);
        _dataAccess = new DataAccessLayer();
        _currentDate = new DateTime(1, 1, 1); // Start at year 1
    }
    
    public void Initialize()
    {
        _dataAccess.InitializeDatabase();
        _dataAccess.ClearDatabase(); // Clear any old data to avoid ID conflicts
        
        // Always seed jobs first (they use temp IDs and don't persist across restarts properly)
        SeedJobs();
        
        // Seed initial Adam and Eve
        SeedInitialData();
    }
    
    private void SeedInitialData()
    {
        // Set current date to year 20 so Adam and Eve start at age 20
        _currentDate = new DateTime(21, 1, 1);

        // Create Adam and Eve at age 20 with perfect traits (100 for all stats)
        var adam = CreatePerson("Adam", "", "Male", null, null);
        adam.BirthDate = new DateTime(1, 1, 1); // Born in year 1, currently age 20
        adam.Intelligence = 100;
        adam.Strength = 100;
        adam.Health = 100;
        adam.Fertility = 100;
        adam.Charisma = 100;
        adam.Creativity = 100;
        adam.Leadership = 100;
        adam.Aggression = 100;
        adam.Wisdom = 100;
        adam.Beauty = 100;
        adam.Height = 180;
        adam.Weight = 80;
        adam.BuildType = "Muscular";
        adam.SkinTone = "Fair";
        adam.DNASequence = _geneticsEngine.GeneratePrimordialDNA();
        adam.BloodType = "O+";
        adam.GeneticMarkers = "HLA-A,HLA-B,APOE";
        adam.HereditaryConditions = "None";
        adam.DiseaseResistance = 100;
        adam.Longevity = 100;
        adam.GenerationNumber = 0;
        adam.IsNotable = true;
        adam.NotableFor = "First Human - Father of Humanity";

        var eve = CreatePerson("Eve", "", "Female", null, null);
        eve.BirthDate = new DateTime(1, 1, 1); // Born in year 1, currently age 20
        eve.Intelligence = 100;
        eve.Strength = 100;
        eve.Health = 100;
        eve.Fertility = 100;
        eve.Charisma = 100;
        eve.Creativity = 100;
        eve.Leadership = 100;
        eve.Aggression = 100;
        eve.Wisdom = 100;
        eve.Beauty = 100;
        eve.Height = 168;
        eve.Weight = 65;
        eve.BuildType = "Average";
        eve.SkinTone = "Fair";
        eve.DNASequence = _geneticsEngine.GeneratePrimordialDNA();
        eve.BloodType = "O+";
        eve.GeneticMarkers = "HLA-C,BRCA1,CCR5";
        eve.HereditaryConditions = "None";
        eve.DiseaseResistance = 100;
        eve.Longevity = 100;
        eve.GenerationNumber = 0;
        eve.IsNotable = true;
        eve.NotableFor = "First Human - Mother of Humanity";
        
        AddPerson(adam);
        AddPerson(eve);
        _adamId = adam.Id;
        _eveId = eve.Id;
        
        // Marry Adam and Eve
        MarryCouple(adam, eve);
        
        LogEvent("Birth", $"{adam.FirstName} was created", adam.Id);
        LogEvent("Birth", $"{eve.FirstName} was created", eve.Id);
        LogEvent("Marriage", $"{adam.FirstName} married {eve.FirstName}", adam.Id);
    }
    
    private void SeedJobs()
    {
        // ENHANCED: Use ExpandedContent for 75+ jobs with gender/age restrictions
        var jobsData = ExpandedContent.GetAllJobs();

        foreach (var (name, intel, str, minAge, maxAge, gender, salary, status, risk, requiredInvention) in jobsData)
        {
            var job = new Job
            {
                Id = _nextTempId--,
                Name = name,
                MinIntelligence = intel,
                MinStrength = str,
                MinAge = minAge,
                MaxAge = maxAge,
                GenderRestriction = gender,
                BaseSalary = salary,
                SocialStatusBonus = status,
                DeathRiskModifier = risk,
                RequiresInvention = requiredInvention != null,
                RequiredInventionId = null // Will be set dynamically when inventions are discovered
            };
            _jobs.Add(job);
            _jobsById[job.Id] = job;
        }

        // Save jobs to database (this updates their IDs to database auto-increment values)
        _dataAccess.SaveJobs(_jobs);

        // Rebuild the _jobsById dictionary with the new database IDs
        _jobsById.Clear();
        foreach (var job in _jobs)
        {
            _jobsById[job.Id] = job;
        }
    }
    
    private Person CreatePerson(string firstName, string lastName, string gender, long? fatherId, long? motherId)
    {
        // Try to find father and mother in both living and dead people
        Person? father = null;
        Person? mother = null;

        if (fatherId.HasValue)
        {
            father = _peopleById.ContainsKey(fatherId.Value) ? _peopleById[fatherId.Value] :
                     _deadPeopleById.ContainsKey(fatherId.Value) ? _deadPeopleById[fatherId.Value] : null;
        }

        if (motherId.HasValue)
        {
            mother = _peopleById.ContainsKey(motherId.Value) ? _peopleById[motherId.Value] :
                     _deadPeopleById.ContainsKey(motherId.Value) ? _deadPeopleById[motherId.Value] : null;
        }

        var person = new Person
        {
            Id = _nextTempId--,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            BirthDate = _currentDate,
            FatherId = fatherId,
            MotherId = motherId,
            IsAlive = true,
            EyeColor = _nameGenerator.GenerateEyeColor(),
            HairColor = _nameGenerator.GenerateHairColor(),
            SkinTone = "Fair" // Will be inherited if parents exist
        };

        // Inherit and mutate traits using advanced genetics
        if (father != null && mother != null)
        {
            // Use genetics engine for trait inheritance
            person.Intelligence = _geneticsEngine.InheritTrait(father.Intelligence, mother.Intelligence);
            person.Strength = _geneticsEngine.InheritTrait(father.Strength, mother.Strength);
            person.Health = _geneticsEngine.InheritTrait(father.Health, mother.Health);
            person.Fertility = _geneticsEngine.InheritTrait(father.Fertility, mother.Fertility);
            person.Charisma = _geneticsEngine.InheritTrait(father.Charisma, mother.Charisma);
            person.Creativity = _geneticsEngine.InheritTrait(father.Creativity, mother.Creativity);
            person.Leadership = _geneticsEngine.InheritTrait(father.Leadership, mother.Leadership);
            person.Aggression = _geneticsEngine.InheritTrait(father.Aggression, mother.Aggression);
            person.Wisdom = _geneticsEngine.InheritTrait(father.Wisdom, mother.Wisdom);
            person.Beauty = _geneticsEngine.InheritTrait(father.Beauty, mother.Beauty);
            person.Height = _geneticsEngine.InheritTrait(father.Height, mother.Height);
            person.Weight = _geneticsEngine.InheritTrait(father.Weight, mother.Weight);

            // Advanced genetics
            person.DNASequence = _geneticsEngine.InheritDNA(father.DNASequence, mother.DNASequence);
            person.BloodType = _geneticsEngine.InheritBloodType(father.BloodType, mother.BloodType);
            person.GeneticMarkers = _geneticsEngine.GenerateGeneticMarkers(father.GeneticMarkers, mother.GeneticMarkers);

            var (conditions, hasDisease) = _geneticsEngine.InheritConditions(father, mother);
            person.HereditaryConditions = conditions;
            person.HasHereditaryDisease = hasDisease;

            person.DiseaseResistance = _geneticsEngine.CalculateDiseaseResistance(father, mother, person.DNASequence);
            person.Longevity = _geneticsEngine.CalculateLongevity(father, mother, person.DNASequence);

            // Determine build type based on strength and weight
            if (person.Strength > 70 && person.Weight > 75)
                person.BuildType = "Muscular";
            else if (person.Weight < 60)
                person.BuildType = "Slim";
            else if (person.Weight > 85)
                person.BuildType = "Heavy";
            else
                person.BuildType = "Average";

            // Generation tracking
            person.GenerationNumber = Math.Max(father.GenerationNumber, mother.GenerationNumber) + 1;

            // Inherit location and religion
            person.CityId = father.CityId ?? mother.CityId;
            person.CountryId = father.CountryId ?? mother.CountryId;
            person.ReligionId = father.ReligionId ?? mother.ReligionId;

            // Update parent's children count
            if (father.IsAlive && _peopleById.ContainsKey(father.Id))
            {
                father.ChildrenBorn++;
                father.TotalChildren++;
            }
            if (mother.IsAlive && _peopleById.ContainsKey(mother.Id))
            {
                mother.ChildrenBorn++;
                mother.TotalChildren++;
            }
        }
        else
        {
            // Default traits for first generation (should not happen with Adam/Eve)
            person.Intelligence = _random.Next(40, 90);
            person.Strength = _random.Next(40, 90);
            person.Health = _random.Next(80, 100);
            person.Fertility = _random.Next(70, 100);
            person.Charisma = _random.Next(40, 90);
            person.Creativity = _random.Next(40, 90);
            person.Leadership = _random.Next(40, 90);
            person.Aggression = _random.Next(20, 70);
            person.Wisdom = _random.Next(40, 90);
            person.Beauty = _random.Next(40, 90);
            person.Height = gender == "Male" ? _random.Next(165, 190) : _random.Next(155, 180);
            person.Weight = gender == "Male" ? _random.Next(60, 90) : _random.Next(50, 75);
            person.BuildType = "Average";
            person.DNASequence = _geneticsEngine.GeneratePrimordialDNA();
            person.BloodType = "O+";
            person.DiseaseResistance = 70;
            person.Longevity = 70;
            person.GenerationNumber = 1;
        }

        // ENHANCED: Evaluate for genius status based on genetics
        // Note: Genius evaluation happens when traits are mature (age 18+), but we set potential here
        if (father != null && mother != null && (father.Intelligence >= 80 || mother.Intelligence >= 80))
        {
            // High-intelligence parents increase genius potential
            var (isGenius, geniusType, description) = _geniusSystem.EvaluateGenius(person);
            if (isGenius && geniusType != null && description != null)
            {
                person.IsNotable = true;
                person.NotableFor = _geniusSystem.GetNotableDescription(geniusType, person);
                LogEvent("Genius", $"{person.FirstName} {person.LastName} born with exceptional potential: {geniusType}", person.Id);
            }
        }

        return person;
    }
    
    private int InheritTrait(int trait1, int trait2)
    {
        // Average of parents with random variation and mutation
        int average = (trait1 + trait2) / 2;
        int variation = _random.Next(-10, 11);
        int mutation = _random.Next(100) < 5 ? _random.Next(-20, 21) : 0; // 5% chance of mutation
        int result = average + variation + mutation;
        return Math.Clamp(result, 0, 100);
    }
    
    private void AddPerson(Person person)
    {
        _people.Add(person);
        _peopleById[person.Id] = person;
        _livingCacheValid = false; // Invalidate cache when adding person
    }
    
    private List<Person> GetLivingPeople()
    {
        if (!_livingCacheValid)
        {
            _livingPeopleCache.Clear();
            foreach (var person in _people)
            {
                if (person.IsAlive)
                    _livingPeopleCache.Add(person);
            }
            _livingCacheValid = true;
        }
        return _livingPeopleCache;
    }
    
    private void MarryCouple(Person person1, Person person2)
    {
        person1.SpouseId = person2.Id;
        person2.SpouseId = person1.Id;
        person1.MarriageDate = _currentDate;
        person2.MarriageDate = _currentDate;
    }
    
    private void LogEvent(string eventType, string description, long? personId = null, long? cityId = null, long? countryId = null)
    {
        var evt = new Event
        {
            Id = _nextTempId--,
            Date = _currentDate,
            EventType = eventType,
            Description = description,
            PersonId = personId,
            CityId = cityId,
            CountryId = countryId
        };
        _recentEvents.Add(evt);
        
        // Keep only last 1000 events in memory
        if (_recentEvents.Count > 1000)
        {
            _recentEvents.RemoveAt(0);
        }
    }
    
    public void SimulateDay()
    {
        _syncCounter++;

        ProcessDeaths();
        ProcessEducation(); // Education before jobs
        AssignJobs();
        ProcessMarriages();
        ProcessPregnancies();
        ProcessBirths();
        
        // Yearly events
        if (_currentDate.DayOfYear == 1)
        {
            ProcessYearlyEvents();
        }
        
        // Periodic database sync
        if (_syncCounter >= SYNC_INTERVAL)
        {
            SyncToDatabase();
            _syncCounter = 0;
        }
        
        _currentDate = _currentDate.AddDays(1);
    }
    
    private void ProcessDeaths()
    {
        // PERFORMANCE: Direct array iteration - no LINQ, no ToList allocation
        // Process in reverse to avoid issues if we need to remove from list later
        for (int i = _people.Count - 1; i >= 0; i--)
        {
            var person = _people[i];
            if (!person.IsAlive) continue; // Skip already dead

            // PERFORMANCE: Cache age calculation (expensive DateTime math)
            int age = person.GetAge(_currentDate);
            double deathChance = CalculateDeathChance(person, age);

            if (_random.NextDouble() < deathChance)
            {
                person.IsAlive = false;
                person.DeathDate = _currentDate;
                _livingCacheValid = false; // Invalidate cache when someone dies

                // Determine cause of death
                person.CauseOfDeath = DetermineCauseOfDeath(person, age);

                // Add to dead people dictionary for later lookup
                _deadPeopleById[person.Id] = person; // Direct assignment, no ContainsKey check needed

                // PERFORMANCE: Only log 10% of deaths to reduce string allocations
                if (_random.Next(100) < 10)
                    LogEvent("Death", $"{person.FirstName} {person.LastName} died at age {age} ({person.CauseOfDeath})", person.Id);

                // Handle succession if ruler
                if (person.IsRuler && person.CountryId.HasValue)
                {
                    HandleSuccession(person);
                }
            }
        }
    }

    private string DetermineCauseOfDeath(Person person, int age)
    {
        // Determine most likely cause based on age, health, job, etc.
        if (age < 1)
            return "Infant mortality";
        if (age < 5)
            return "Childhood illness";

        // Job-related deaths
        if (person.JobId.HasValue && _jobsById.ContainsKey(person.JobId.Value))
        {
            var job = _jobsById[person.JobId.Value];
            if (job.DeathRiskModifier > 2.0 && _random.Next(100) < 40)
            {
                if (job.Name.Contains("Warrior") || job.Name.Contains("Guard"))
                    return "Combat";
                if (job.Name.Contains("Miner"))
                    return "Mining accident";
                if (job.Name.Contains("Sailor"))
                    return "Shipwreck";
                return "Occupational hazard";
            }
        }

        // Health-related deaths
        if (person.Health < 30)
            return "Chronic illness";
        if (person.HasHereditaryDisease && _random.Next(100) < 30)
            return $"Hereditary condition ({person.HereditaryConditions})";

        // Age-related deaths
        if (age > 70)
            return "Old age";
        if (age > 50)
            return "Age-related illness";

        // Random causes
        var randomCauses = new[] { "Disease", "Accident", "Natural causes", "Fever", "Infection" };
        return randomCauses[_random.Next(randomCauses.Length)];
    }
    
    private double CalculateDeathChance(Person person, int age)
    {
        // Adam/Eve are immortal until 100
        if ((_adamId.HasValue && person.Id == _adamId.Value) || (_eveId.HasValue && person.Id == _eveId.Value))
        {
            if (age < 100)
                return 0.0;
        }

        // ENHANCED: Use genetic longevity to extend lifespan
        // High longevity genes can add up to 20 years of effective lifespan
        int geneticLifespanBonus = (person.Longevity - 50) / 5; // -10 to +10 years

        // Calculate total lifespan bonus from inventions
        int lifespanBonus = _inventions.Sum(i => i.LifespanBonus) + geneticLifespanBonus;
        int effectiveAge = Math.Max(0, age - lifespanBonus);

        // Base death chance per day - significantly reduced for young people
        double baseChance = effectiveAge switch
        {
            < 1 => 0.00005,  // ~1.8% yearly infant mortality
            < 5 => 0.00002,  // ~0.7% yearly child mortality
            < 16 => 0.00001, // ~0.36% yearly teen mortality
            < 50 => 0.00003, // ~1.1% yearly adult mortality
            < 70 => 0.0002 + (effectiveAge - 50) * 0.00005, // Increasing middle age mortality
            _ => 0.001 + (effectiveAge - 70) * 0.0001   // Increasing old age mortality
        };

        // ENHANCED: Use genetics for health modifiers
        double healthMod = 1.0 - (person.Health / 200.0);

        // ENHANCED: Disease resistance reduces death chance
        double diseaseResistanceMod = 1.0 - (person.DiseaseResistance / 200.0); // 0.5 to 1.0

        // ENHANCED: Hereditary diseases increase death risk
        double diseasePenalty = person.HasHereditaryDisease ? 1.3 : 1.0;

        double jobRisk = 1.0;
        if (person.JobId.HasValue && _jobsById.ContainsKey(person.JobId.Value))
            jobRisk = _jobsById[person.JobId.Value].DeathRiskModifier;

        return baseChance * (1.0 + healthMod) * diseaseResistanceMod * diseasePenalty * jobRisk;
    }
    
    private void HandleSuccession(Person deadRuler)
    {
        if (!deadRuler.CountryId.HasValue || !_countriesById.ContainsKey(deadRuler.CountryId.Value))
            return;
        
        var country = _countriesById[deadRuler.CountryId.Value];
        
        // Find oldest living child
        var children = _people.Where(p => 
            p.IsAlive && 
            (p.FatherId == deadRuler.Id || p.MotherId == deadRuler.Id) &&
            p.GetAge(_currentDate) >= 18)
            .OrderByDescending(p => p.GetAge(_currentDate))
            .ToList();
        
        if (children.Any())
        {
            var heir = children.First();
            heir.IsRuler = true;
            country.RulerId = heir.Id;
            
            LogEvent("Succession", $"{heir.FirstName} {heir.LastName} became ruler of {country.Name}", heir.Id, countryId: country.Id);
        }
    }
    
    private void AssignJobs()
    {
        // Get list of people who need jobs (alive, old enough, no current job)
        var peopleNeedingJobs = _people
            .Where(p => p.IsAlive && p.GetAge(_currentDate) >= 12 && !p.JobId.HasValue)
            .ToList();
        
        if (peopleNeedingJobs.Count == 0) return;
        
        // Build set of discovered inventions for quick lookup
        var discoveredInventions = _inventions.Select(i => i.Name).ToHashSet();
        bool hasActiveWars = _wars.Count > 0;
        
        // Map job names to required inventions
        var jobInventionRequirements = new Dictionary<string, string>
        {
            { "Potter", "Pottery" },
            { "Weaver", "Weaving" },
            { "Tanner", "Tanning" },
            { "Glassmaker", "Glassmaking" },
            { "Copper Smith", "Copper Working" },
            { "Bronze Smith", "Bronze" },
            { "Iron Smith", "Iron Working" },
            { "Blacksmith", "Steel" },
            { "Goldsmith", "Gold Working" },
            { "Scribe", "Writing" },
            { "Healer", "Herbal Medicine" },
            { "Physician", "Surgery" },
            { "Scholar", "Writing" },
            { "Teacher", "Writing" },
            { "Architect", "Architecture" },
            { "Engineer", "Mathematics" },
            { "Mason", "Brick Making" },
            { "Artist", "Painting" },
            { "Sculptor", "Sculpture" },
            { "Musician", "Music" },
            { "Poet", "Poetry" },
            { "Baker", "Bread Baking" },
            { "Brewer", "Beer Brewing" },
            { "Carter", "Cart" },
            { "Sailor", "Ship" },
            { "Charioteer", "Chariot" },
            { "Archer", "Bow and Arrow" }
        };
        
        // Assign jobs to each eligible person
        foreach (var person in peopleNeedingJobs)
        {
            Job? assignedJob = null;
            int bestFitScore = int.MinValue;
            
            // Find the best job match for this person
            foreach (var job in _jobs)
            {
                // Check age requirements (min and max)
                int personAge = person.GetAge(_currentDate);
                if (personAge < job.MinAge) continue;
                if (job.MaxAge.HasValue && personAge > job.MaxAge.Value) continue;

                // Check gender restriction
                if (job.GenderRestriction != "Any")
                {
                    if (job.GenderRestriction != person.Gender) continue;
                }

                // Check intelligence requirement
                if (person.Intelligence < job.MinIntelligence) continue;

                // Check strength requirement
                if (person.Strength < job.MinStrength) continue;
                
                // Check if job requires an invention that hasn't been discovered
                if (job.RequiresInvention)
                {
                    if (jobInventionRequirements.TryGetValue(job.Name, out var requiredInvention))
                    {
                        if (!discoveredInventions.Contains(requiredInvention))
                            continue;
                    }
                }
                
                // Military jobs require active wars
                if ((job.Name == "Warrior" || job.Name == "Guard" || job.Name == "Archer") && !hasActiveWars)
                    continue;
                
                // Calculate fit score (how well person exceeds requirements)
                int fitScore = (person.Intelligence - job.MinIntelligence) + 
                              (person.Strength - job.MinStrength);
                
                // This person qualifies and has a better fit than previous options
                if (fitScore > bestFitScore)
                {
                    bestFitScore = fitScore;
                    assignedJob = job;
                }
            }
            
            // Assign the best job found
            if (assignedJob != null)
            {
                person.JobId = assignedJob.Id;
                person.JobStartDate = _currentDate;
                person.SocialStatus += assignedJob.SocialStatusBonus;
                
                // Log some job assignments (10% to avoid spam)
                if (_random.Next(100) < 10)
                {
                    LogEvent("Job", $"{person.FirstName} {person.LastName} became a {assignedJob.Name}", person.Id);
                }
            }
        }
    }
    
    private void ProcessMarriages()
    {
        // Cache eligible people lists to avoid multiple enumerations
        var eligibleMales = new List<Person>();
        var eligibleFemales = new List<Person>();
        
        // Single pass through people list
        foreach (var person in _people)
        {
            if (!person.IsEligibleForMarriage(_currentDate)) continue;
            
            if (person.Gender == "Male")
                eligibleMales.Add(person);
            else
                eligibleFemales.Add(person);
        }
        
        if (eligibleMales.Count == 0 || eligibleFemales.Count == 0)
            return;
        
        // Early population relaxed rules
        int livingCount = _people.Count(p => p.IsAlive);
        bool earlyPopulation = livingCount < 100;
        
        foreach (var male in eligibleMales)
        {
            if (!male.IsEligibleForMarriage(_currentDate)) continue;
            
            // Random chance to marry - only process 10% to improve performance
            if (_random.NextDouble() >= 0.1) continue;
            
            // Find potential spouses
            Person? bestSpouse = null;
            foreach (var female in eligibleFemales)
            {
                if (!female.IsEligibleForMarriage(_currentDate))
                    continue;
                
                // Avoid parent-child marriages
                if (female.Id == male.FatherId || female.Id == male.MotherId ||
                    male.Id == female.FatherId || male.Id == female.MotherId)
                    continue;
                
                // Same country and religion if available (unless early population)
                if (!earlyPopulation)
                {
                    if (male.CountryId.HasValue && female.CountryId.HasValue && 
                        male.CountryId != female.CountryId)
                        continue;
                    
                    if (male.ReligionId.HasValue && female.ReligionId.HasValue && 
                        male.ReligionId != female.ReligionId)
                        continue;
                }
                
                // Avoid sibling marriages after early population
                if (!earlyPopulation && male.FatherId.HasValue && 
                    male.FatherId == female.FatherId)
                    continue;
                
                bestSpouse = female;
                break; // Take first compatible match for performance
            }
            
            if (bestSpouse != null)
            {
                MarryCouple(male, bestSpouse);
                LogEvent("Marriage", $"{male.FirstName} {male.LastName} married {bestSpouse.FirstName} {bestSpouse.LastName}", male.Id);
            }
        }
    }
    
    private bool IsAdamOrEve(Person p)
    {
        return (_adamId.HasValue && p.Id == _adamId.Value) || (_eveId.HasValue && p.Id == _eveId.Value);
    }
    
    private bool CanHaveChildren(Person p)
    {
        if (p.Gender != "Female" || !p.IsAlive || !p.IsMarried) return false;
        int age = p.GetAge(_currentDate);
        if (IsAdamOrEve(p))
            return age >= 14 && age <= 100 && !p.IsPregnant;
        return age >= 14 && age <= 50 && !p.IsPregnant;
    }
    
    private void ProcessPregnancies()
    {
        // Get living people once
        var livingPeople = GetLivingPeople();

        // Cache living count for pregnancy chance calculation
        int totalPeople = livingPeople.Count;

        // IMPROVED: Higher base rates to ensure sustainable growth, especially early on
        double basePregnancyChance = totalPeople < 20 ? 0.30 :   // Very high for Adam/Eve's children
                                     totalPeople < 50 ? 0.25 :   // High for early generations
                                     totalPeople < 150 ? 0.18 :  // Moderate as population grows
                                     totalPeople < 300 ? 0.12 :  // Still good growth rate
                                     totalPeople < 500 ? 0.08 :  // Slowing down
                                     totalPeople < 1000 ? 0.04 : // Controlled growth
                                     0.02;                       // Steady state

        // Single pass through people to find eligible females
        foreach (var female in livingPeople)
        {
            if (!CanHaveChildren(female)) continue;

            // Calculate pregnancy chance with modifiers
            double pregnancyChance = basePregnancyChance;
            pregnancyChance *= (female.Fertility / 100.0);
            pregnancyChance *= (female.Health / 100.0);

            // IMPROVED: Bonus for high longevity genetics (better reproductive health)
            if (female.Longevity > 70)
                pregnancyChance *= 1.1;

            // IMPROVED: Early generation bonus (Adam/Eve and first few generations)
            if (female.GenerationNumber <= 2)
                pregnancyChance *= 1.2;

            // IMPROVED: Reduce disease resistance penalty
            if (female.DiseaseResistance < 30)
                pregnancyChance *= 0.8;

            if (_random.NextDouble() < pregnancyChance)
            {
                female.IsPregnant = true;
                female.PregnancyDueDate = _currentDate.AddDays(270); // 9 months
                female.PregnancyFatherId = female.SpouseId;

                // IMPROVED: Higher chance of twins/triplets for high fertility
                double multipleChance = _random.NextDouble();
                double twinBonus = female.Fertility > 80 ? 1.5 : 1.0;

                if (multipleChance < (0.01 * twinBonus))
                    female.PregnancyMultiplier = 3; // Triplets
                else if (multipleChance < (0.06 * twinBonus))
                    female.PregnancyMultiplier = 2; // Twins (increased from 5% to 6%)
                else
                    female.PregnancyMultiplier = 1;

                string multiplier = female.PregnancyMultiplier > 1 ? $" (expecting {female.PregnancyMultiplier})" : "";
                LogEvent("Pregnancy", $"{female.FirstName} {female.LastName} is pregnant{multiplier}", female.Id);
            }
        }
    }
    
    private void ProcessBirths()
    {
        // Cache pregnancies due today (avoid creating list if no pregnancies)
        var duePregnancies = new List<Person>();
        foreach (var person in _people)
        {
            if (person.IsPregnant && person.PregnancyDueDate.HasValue && 
                person.PregnancyDueDate.Value <= _currentDate)
            {
                // Only process if mother is still alive
                if (person.IsAlive)
                {
                    duePregnancies.Add(person);
                }
                else
                {
                    // Mother died while pregnant - unborn child dies too
                    person.IsPregnant = false;
                    person.PregnancyDueDate = null;
                    person.PregnancyFatherId = null;
                    person.PregnancyMultiplier = 1;
                }
            }
        }
        
        if (duePregnancies.Count == 0) return;
        
        foreach (var mother in duePregnancies)
        {
            for (int i = 0; i < mother.PregnancyMultiplier; i++)
            {
                string gender = _random.Next(2) == 0 ? "Male" : "Female";
                string firstName = gender == "Male" ? 
                    _nameGenerator.GenerateMaleFirstName() : 
                    _nameGenerator.GenerateFemaleFirstName();
                
                // Get father - check living first, then dead people dictionary
                Person? father = null;
                if (mother.PregnancyFatherId.HasValue)
                {
                    // Try living people first
                    if (_peopleById.ContainsKey(mother.PregnancyFatherId.Value))
                    {
                        father = _peopleById[mother.PregnancyFatherId.Value];
                    }
                    // If not found in living, check dead people
                    else if (_deadPeopleById.ContainsKey(mother.PregnancyFatherId.Value))
                    {
                        father = _deadPeopleById[mother.PregnancyFatherId.Value];
                    }
                }
                
                // If no father found, use spouse as fallback (check both living and dead)
                if (father == null && mother.SpouseId.HasValue)
                {
                    if (_peopleById.ContainsKey(mother.SpouseId.Value))
                    {
                        father = _peopleById[mother.SpouseId.Value];
                    }
                    else if (_deadPeopleById.ContainsKey(mother.SpouseId.Value))
                    {
                        father = _deadPeopleById[mother.SpouseId.Value];
                    }
                }
                
                // Calculate current generation for this child
                int childGeneration = 1;
                if (mother.MotherId.HasValue || mother.FatherId.HasValue)
                {
                    int motherGen = CountGenerations(mother.Id);
                    childGeneration = motherGen + 1;
                }
                else
                {
                    // Mother is first generation (Adam/Eve), so child is generation 2
                    childGeneration = 2;
                }
                
                // Update global generation counter
                _generationNumber = Math.Max(_generationNumber, childGeneration);

                // Gather parent information for surname generation
                string fatherName = father?.FirstName ?? string.Empty;
                string motherName = mother.FirstName;
                string? fatherLastName = father?.LastName;
                string? motherLastName = mother.LastName;
                long? fatherId = father?.Id;
                string cityName = mother.CityId.HasValue && _citiesById.ContainsKey(mother.CityId.Value)
                    ? _citiesById[mother.CityId.Value].Name
                    : string.Empty;
                string jobName = father?.JobId.HasValue == true && _jobsById.ContainsKey(father.JobId.Value)
                    ? _jobsById[father.JobId.Value].Name
                    : string.Empty;

                // Use enhanced surname generation with proper evolution
                string lastName = _nameGenerator.GenerateLastName(
                    fatherName, motherName, fatherLastName, motherLastName,
                    cityName, jobName, childGeneration, gender);

                // If lastName is "Unknown", use mother's last name instead
                if (lastName == "Unknown" && !string.IsNullOrEmpty(mother.LastName))
                {
                    lastName = mother.LastName;
                }
                
                var child = CreatePerson(firstName, lastName, gender, fatherId, mother.Id);
                AddPerson(child);
                
                // Log birth with stats
                string stats = $"Hair: {child.HairColor}, Eyes: {child.EyeColor}";
                LogEvent("Birth", $"{child.FirstName} {child.LastName} was born to {mother.FirstName} {mother.LastName} ({stats})", child.Id);
            }
            
            mother.IsPregnant = false;
            mother.PregnancyDueDate = null;
            mother.PregnancyFatherId = null;
            mother.PregnancyMultiplier = 1;
        }
    }
    
    private int CountGenerations(long personId)
    {
        // Check both living and dead people
        Person? person = null;
        if (_peopleById.ContainsKey(personId))
        {
            person = _peopleById[personId];
        }
        else if (_deadPeopleById.ContainsKey(personId))
        {
            person = _deadPeopleById[personId];
        }
        
        if (person == null)
            return 0;
        
        if (!person.MotherId.HasValue && !person.FatherId.HasValue)
            return 1;
        
        int motherGen = person.MotherId.HasValue ? CountGenerations(person.MotherId.Value) : 0;
        int fatherGen = person.FatherId.HasValue ? CountGenerations(person.FatherId.Value) : 0;
        
        return Math.Max(motherGen, fatherGen) + 1;
    }
    
    private void ProcessYearlyEvents()
    {
        int population = _people.Count(p => p.IsAlive);
        
        // Found cities
        if (population > 100 && _cities.Count < population / 100 && _random.NextDouble() < 0.3)
        {
            FoundCity();
        }
        
        // Found countries
        if (_cities.Count > 3 && _countries.Count < _cities.Count / 3 && _random.NextDouble() < 0.2)
        {
            FoundCountry();
        }
        
        // Found religions
        if (population > 200 && _religions.Count < 5 && _random.NextDouble() < 0.1)
        {
            FoundReligion();
        }
        
        // Discover inventions
        if (population > 50 && _random.NextDouble() < 0.15)
        {
            DiscoverInvention();
        }

        // Create new businesses
        if (population > 30 && _businesses.Count < population / 20 && _random.NextDouble() < 0.25)
        {
            CreateBusiness();
        }

        // Process existing businesses (rise/fall, innovation)
        ProcessBusinesses();

        // Found schools
        if (population > 100 && _cities.Count > 0 && _schools.Count < _cities.Count * 3 && _random.NextDouble() < 0.20)
        {
            FoundSchool();
        }

        // Found universities
        if (population > 1000 && _cities.Count > 2 && _schools.Count > 5 && _universities.Count < _cities.Count && _random.NextDouble() < 0.10)
        {
            FoundUniversity();
        }

        // Natural disasters (now geography-aware with individual probabilities)
        // Increased frequency since each disaster has its own probability check
        if (population > 100 && _cities.Count > 0 && _random.NextDouble() < 0.40)
        {
            TriggerNaturalDisaster(); // Now checks geography-specific probabilities internally
        }

        // Wars
        if (_countries.Count > 1 && _random.NextDouble() < 0.05)
        {
            StartWar();
        }
    }
    
    private void FoundCity()
    {
        var founder = _people.Where(p => p.IsAlive && p.GetAge(_currentDate) >= 25)
            .OrderByDescending(p => p.Leadership)
            .FirstOrDefault();
        
        if (founder == null) return;
        
        // Assign random geography and climate
        string[] geographies = { "Coastal", "Mountain", "Plains", "Desert", "Forest", "RiverValley", "Island" };
        string[] climates = { "Tropical", "Temperate", "Arid", "Arctic", "Mediterranean" };

        var city = new City
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateCityName(),
            FoundedDate = _currentDate,
            Population = 0,
            FounderId = founder.Id,
            Wealth = 0,
            Geography = geographies[_random.Next(geographies.Length)],
            Climate = climates[_random.Next(climates.Length)]
        };
        
        _cities.Add(city);
        _citiesById[city.Id] = city;
        
        // Assign some people to the city
        var nearbyPeople = _people.Where(p => p.IsAlive && !p.CityId.HasValue).Take(20).ToList();
        foreach (var person in nearbyPeople)
        {
            person.CityId = city.Id;
            city.Population++;
        }
        
        LogEvent("City", $"The city of {city.Name} was founded by {founder.FirstName} {founder.LastName}", founder.Id, city.Id);
    }
    
    private void FoundCountry()
    {
        var city = _cities.Where(c => !c.CountryId.HasValue).OrderByDescending(c => c.Population).FirstOrDefault();
        if (city == null) return;
        
        var ruler = _people.Where(p => p.IsAlive && p.CityId == city.Id && p.GetAge(_currentDate) >= 25)
            .OrderByDescending(p => p.Leadership + p.Charisma)
            .FirstOrDefault();
        
        if (ruler == null) return;
        
        var country = new Country
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateCountryName(),
            FoundedDate = _currentDate,
            RulerId = ruler.Id,
            CapitalCityId = city.Id,
            Population = city.Population,
            Wealth = 0,
            MilitaryStrength = city.Population / 10,
            DominantGeography = city.Geography, // Inherit from capital
            DominantClimate = city.Climate
        };
        
        _countries.Add(country);
        _countriesById[country.Id] = country;
        
        ruler.IsRuler = true;
        ruler.CountryId = country.Id;
        city.CountryId = country.Id;
        
        // Create dynasty
        var dynasty = new Dynasty
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateDynastyName(ruler.FirstName),
            FounderId = ruler.Id,
            FoundedDate = _currentDate,
            CurrentRulerId = ruler.Id,
            MemberCount = 1
        };
        
        _dynasties.Add(dynasty);
        _dynastiesById[dynasty.Id] = dynasty;
        ruler.DynastyId = dynasty.Id;
        
        LogEvent("Country", $"The country of {country.Name} was founded by {ruler.FirstName} {ruler.LastName}", ruler.Id, countryId: country.Id);
    }
    
    private void FoundReligion()
    {
        var founder = _people.Where(p => p.IsAlive && p.Intelligence > 70 && p.Charisma > 60)
            .OrderByDescending(p => p.Intelligence + p.Charisma + p.Wisdom)
            .FirstOrDefault();
        
        if (founder == null) return;
        
        var religion = new Religion
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateReligionName(),
            FoundedDate = _currentDate,
            FounderId = founder.Id,
            Followers = 1,
            Beliefs = "Ancient teachings and traditions",
            AllowsPolygamy = _random.NextDouble() < 0.3
        };
        
        _religions.Add(religion);
        _religionsById[religion.Id] = religion;
        
        founder.ReligionId = religion.Id;
        
        LogEvent("Religion", $"The religion {religion.Name} was founded by {founder.FirstName} {founder.LastName}", founder.Id);
    }
    
    private void DiscoverInvention()
    {
        var inventor = _people.Where(p => p.IsAlive && p.Intelligence > 75)
            .OrderByDescending(p => p.Intelligence + p.Creativity)
            .FirstOrDefault();
        
        if (inventor == null) return;
        
        // Get inventions that haven't been discovered yet
        var discoveredNames = _inventions.Select(i => i.Name).ToHashSet();
        var availableInventions = NameGenerator.InventionData
            .Where(inv => !discoveredNames.Contains(inv.Name) && inventor.Intelligence >= inv.RequiredIntel)
            .ToList();
        
        if (!availableInventions.Any()) return;
        
        var inventionData = availableInventions[_random.Next(availableInventions.Count)];
        
        var invention = new Invention
        {
            Id = _nextTempId--,
            Name = inventionData.Name,
            Description = inventionData.Description,
            DiscoveredDate = _currentDate,
            InventorId = inventor.Id,
            RequiredIntelligence = inventionData.RequiredIntel,
            Category = inventionData.Category,
            HealthBonus = inventionData.HealthBonus,
            LifespanBonus = inventionData.LifespanBonus
        };
        
        _inventions.Add(invention);
        _inventionsById[invention.Id] = invention;
        
        // Apply invention effects to all living people
        if (invention.HealthBonus > 0 || invention.LifespanBonus > 0)
        {
            foreach (var person in _people.Where(p => p.IsAlive))
            {
                person.Health = Math.Min(100, person.Health + invention.HealthBonus);
            }
        }
        
        LogEvent("Invention", $"{inventor.FirstName} {inventor.LastName} discovered {invention.Name}", inventor.Id);
    }
    
    private void StartWar()
    {
        if (_countries.Count < 2) return;
        
        var attacker = _countries[_random.Next(_countries.Count)];
        var defender = _countries.Where(c => c.Id != attacker.Id).OrderBy(_ => _random.Next()).FirstOrDefault();
        
        if (defender == null) return;
        
        var war = new War
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateWarName(attacker.Name, defender.Name),
            StartDate = _currentDate,
            AttackerCountryId = attacker.Id,
            DefenderCountryId = defender.Id,
            Casualties = 0,
            IsActive = true
        };
        
        _wars.Add(war);
        
        // Simulate casualties
        int attackerLosses = _random.Next(attacker.MilitaryStrength / 10);
        int defenderLosses = _random.Next(defender.MilitaryStrength / 10);
        war.Casualties = attackerLosses + defenderLosses;
        
        // Determine winner
        if (attacker.MilitaryStrength > defender.MilitaryStrength * 1.5)
        {
            war.WinnerCountryId = attacker.Id;
            war.EndDate = _currentDate.AddDays(_random.Next(30, 365));
            war.IsActive = false;
        }
        else if (defender.MilitaryStrength > attacker.MilitaryStrength * 1.5)
        {
            war.WinnerCountryId = defender.Id;
            war.EndDate = _currentDate.AddDays(_random.Next(30, 365));
            war.IsActive = false;
        }
        
        LogEvent("War", $"War broke out: {war.Name}", countryId: attacker.Id);
    }

    private void CreateBusiness()
    {
        // Find a suitable business owner (high intelligence/charisma, has a job or wealth)
        var owner = _people.Where(p => p.IsAlive && p.GetAge(_currentDate) >= 20 &&
                                       p.Intelligence >= 30 && p.Charisma >= 25)
            .OrderByDescending(p => p.Intelligence + p.Charisma + p.Wisdom)
            .FirstOrDefault();

        if (owner == null) return;

        // Determine business type based on available inventions and city
        string[] businessTypes = { "Farm", "Workshop", "Merchant House", "Smithy", "Tavern", "Inn", "Market Stall" };
        string businessType = businessTypes[_random.Next(businessTypes.Length)];

        var business = new Business
        {
            Id = _nextTempId--,
            Name = $"{owner.FirstName}'s {businessType}",
            Type = businessType,
            OwnerId = owner.Id,
            CityId = owner.CityId,
            FoundedDate = _currentDate,
            IsActive = true,
            Wealth = _random.Next(100, 1000),
            MaxEmployees = _random.Next(3, 15),
            EmployeeCount = 0,
            Reputation = _random.Next(40, 70),
            Status = "Growing",
            CanInnovate = owner.Intelligence >= 50 && owner.Creativity >= 40,
            InnovationPoints = 0,
            YearsInBusiness = 0
        };

        _businesses.Add(business);
        _businessesById[business.Id] = business;

        LogEvent("Business", $"{business.Name} was founded by {owner.FirstName} {owner.LastName}", owner.Id);
    }

    private void ProcessBusinesses()
    {
        foreach (var business in _businesses.Where(b => b.IsActive).ToList())
        {
            business.YearsInBusiness++;

            // Calculate revenue and costs
            business.AnnualRevenue = business.Reputation * business.EmployeeCount * _random.Next(50, 200);
            business.AnnualCosts = business.EmployeeCount * _random.Next(100, 300);
            decimal profit = business.AnnualRevenue - business.AnnualCosts;

            business.Wealth += profit;
            if (business.Wealth > business.PeakWealth)
                business.PeakWealth = business.Wealth;

            // Update status based on performance
            if (profit > business.AnnualRevenue * 0.3m)
            {
                business.Status = "Growing";
                business.Reputation = Math.Min(100, business.Reputation + _random.Next(1, 5));
            }
            else if (profit > 0)
            {
                business.Status = "Stable";
            }
            else if (profit > -business.AnnualRevenue * 0.2m)
            {
                business.Status = "Declining";
                business.Reputation = Math.Max(0, business.Reputation - _random.Next(1, 3));
            }
            else
            {
                business.Status = "Failing";
                business.Reputation = Math.Max(0, business.Reputation - _random.Next(3, 10));
            }

            // Business innovation (can discover inventions)
            if (business.CanInnovate && business.Status == "Growing")
            {
                business.InnovationPoints += _random.Next(1, 5);
                if (business.InnovationPoints >= 50 && _random.NextDouble() < 0.15)
                {
                    // Business discovers an invention
                    var owner = _peopleById.ContainsKey(business.OwnerId) ? _peopleById[business.OwnerId] : null;
                    if (owner != null)
                    {
                        DiscoverInventionBy(owner, $"{business.Name}");
                        business.InnovationPoints = 0;
                        business.Reputation += 10;
                    }
                }
            }

            // Business may close if failing for too long
            if (business.Status == "Failing" && business.Wealth < -1000)
            {
                business.IsActive = false;
                business.ClosedDate = _currentDate;
                LogEvent("Business", $"{business.Name} has closed due to financial troubles", business.OwnerId);
            }
        }
    }

    private void DiscoverInventionBy(Person inventor, string context)
    {
        // Similar to DiscoverInvention() but attributes it to a specific person/business
        var allInventions = ExpandedContent.GetAllInventions();
        var undiscovered = allInventions.Where(inv => !_inventions.Any(i => i.Name == inv.name)).ToList();

        if (!undiscovered.Any()) return;

        var possibleInventions = undiscovered.Where(inv => inventor.Intelligence >= inv.reqIntel).ToList();
        if (!possibleInventions.Any())
            possibleInventions = undiscovered;

        var inventionData = possibleInventions[_random.Next(possibleInventions.Count)];

        var invention = new Invention
        {
            Id = _nextTempId--,
            Name = inventionData.name,
            DiscoveryDate = _currentDate,
            InventorId = inventor.Id,
            Category = inventionData.category,
            HealthBonus = inventionData.healthBonus,
            LifespanBonus = inventionData.lifespanBonus
        };

        _inventions.Add(invention);
        _inventionsById[invention.Id] = invention;

        LogEvent("Invention", $"{inventor.FirstName} {inventor.LastName} ({context}) discovered {invention.Name}", inventor.Id);
    }

    // ============================================================================
    // EDUCATION SYSTEM
    // ============================================================================

    private void ProcessEducation()
    {
        // Process schools
        foreach (var school in _schools.ToList())
        {
            school.CurrentStudents = 0; // Reset daily count
        }

        // Process universities
        foreach (var university in _universities.ToList())
        {
            university.CurrentStudents = 0; // Reset daily count
        }

        // Assign people to schools and universities
        foreach (var person in _people.Where(p => p.IsAlive).ToList())
        {
            int age = person.GetAge(_currentDate);

            // Children attend school (ages 6-18)
            if (person.IsEligibleForSchool(_currentDate) && _schools.Any())
            {
                AssignToSchool(person);
            }
            // Graduated from school, update education level
            else if (person.SchoolId.HasValue && age > 18)
            {
                GraduateFromSchool(person);
            }

            // Young adults attend university (ages 18-30, requires secondary education)
            if (person.IsEligibleForUniversity(_currentDate) && _universities.Any())
            {
                AssignToUniversity(person);
            }
            // Graduated from university, update education level
            else if (person.UniversityId.HasValue && age > 25)
            {
                GraduateFromUniversity(person);
            }

            // Knowledge transfer from parents to children (education boost)
            if (age < 18 && (person.FatherId.HasValue || person.MotherId.HasValue))
            {
                TransferKnowledgeFromParents(person);
            }
        }
    }

    private void AssignToSchool(Person person)
    {
        // Find a school in the person's city or nearby
        var eligibleSchools = _schools.Where(s =>
            s.CityId == person.CityId &&
            s.CurrentStudents < s.Capacity
        ).ToList();

        if (!eligibleSchools.Any()) return;

        var school = eligibleSchools[_random.Next(eligibleSchools.Count)];
        person.SchoolId = school.Id;
        school.CurrentStudents++;

        // Improve literacy and intelligence based on school quality
        if (!person.IsLiterate && _random.Next(100) < school.QualityRating)
        {
            person.IsLiterate = true;
            person.Intelligence = Math.Min(100, person.Intelligence + _random.Next(5, 15));
        }

        // Progress education level
        int age = person.GetAge(_currentDate);
        if (age >= 6 && age < 12 && person.EducationLevel == "None")
        {
            person.EducationLevel = "Primary";
            person.EducationQuality = school.QualityRating;
        }
        else if (age >= 12 && age <= 18 && person.EducationLevel == "Primary")
        {
            person.EducationLevel = "Secondary";
            person.EducationQuality = (person.EducationQuality + school.QualityRating) / 2; // Average quality
        }
    }

    private void GraduateFromSchool(Person person)
    {
        if (!person.SchoolId.HasValue) return;

        var school = _schoolsById.ContainsKey(person.SchoolId.Value) ? _schoolsById[person.SchoolId.Value] : null;

        person.GraduationDate = _currentDate;
        person.SchoolId = null;

        // Log notable graduations (10% chance)
        if (_random.Next(100) < 10 && school != null)
        {
            LogEvent("Education", $"{person.FirstName} {person.LastName} graduated from {school.Name}", person.Id);
        }
    }

    private void AssignToUniversity(Person person)
    {
        // Find a university in the person's city or nearby
        var eligibleUniversities = _universities.Where(u =>
            u.CityId == person.CityId &&
            u.CurrentStudents < u.Capacity
        ).ToList();

        if (!eligibleUniversities.Any()) return;

        var university = eligibleUniversities[_random.Next(eligibleUniversities.Count)];
        person.UniversityId = university.Id;
        university.CurrentStudents++;

        // Major intelligence boost from university
        person.Intelligence = Math.Min(100, person.Intelligence + _random.Next(10, 25));
        person.Creativity = Math.Min(100, person.Creativity + _random.Next(5, 15));
        person.Wisdom = Math.Min(100, person.Wisdom + _random.Next(5, 15));

        person.EducationLevel = "University";
        person.EducationQuality = university.PrestigeRating;

        // University research output contributes to inventions
        university.ResearchOutput += _random.Next(1, 5);
        if (university.ResearchOutput >= 100 && _random.NextDouble() < 0.20)
        {
            DiscoverInventionBy(person, $"{university.Name}");
            university.ResearchOutput = 0;
            university.PrestigeRating = Math.Min(100, university.PrestigeRating + 5);
        }
    }

    private void GraduateFromUniversity(Person person)
    {
        if (!person.UniversityId.HasValue) return;

        var university = _universitiesById.ContainsKey(person.UniversityId.Value) ? _universitiesById[person.UniversityId.Value] : null;

        person.GraduationDate = _currentDate;
        person.UniversityId = null;

        // Make person notable if they graduated from a prestigious university
        if (university != null && university.PrestigeRating >= 80 && !person.IsNotable)
        {
            person.IsNotable = true;
            person.NotableFor = $"Graduate of {university.Name}";
            LogEvent("Education", $"{person.FirstName} {person.LastName} graduated from the prestigious {university.Name}", person.Id);
        }
    }

    private void TransferKnowledgeFromParents(Person child)
    {
        // Educated parents boost children's intelligence
        var father = child.FatherId.HasValue && _peopleById.ContainsKey(child.FatherId.Value) ? _peopleById[child.FatherId.Value] : null;
        var mother = child.MotherId.HasValue && _peopleById.ContainsKey(child.MotherId.Value) ? _peopleById[child.MotherId.Value] : null;

        int intelligenceBoost = 0;

        if (father != null && father.IsLiterate)
        {
            intelligenceBoost += _random.Next(0, 2);
            if (father.EducationLevel == "University")
                intelligenceBoost += _random.Next(1, 3);
        }

        if (mother != null && mother.IsLiterate)
        {
            intelligenceBoost += _random.Next(0, 2);
            if (mother.EducationLevel == "University")
                intelligenceBoost += _random.Next(1, 3);
        }

        if (intelligenceBoost > 0)
        {
            // Apply boost once per year on child's birthday
            if (child.BirthDate.Month == _currentDate.Month && child.BirthDate.Day == _currentDate.Day)
            {
                child.Intelligence = Math.Min(100, child.Intelligence + intelligenceBoost);
            }
        }
    }

    private void FoundSchool()
    {
        if (_cities.Count == 0) return;

        // Select a city for the school (prefer larger cities)
        var city = _cities.OrderByDescending(c => c.Population).Take(3).ToList()[_random.Next(Math.Min(3, _cities.Count))];

        // Find a suitable founder (educated, intelligent person)
        var potentialFounders = _people.Where(p =>
            p.IsAlive &&
            p.CityId == city.Id &&
            p.IsLiterate &&
            p.Intelligence >= 70
        ).ToList();

        long? founderId = null;
        if (potentialFounders.Any())
        {
            var founder = potentialFounders[_random.Next(potentialFounders.Count)];
            founderId = founder.Id;
        }

        var schoolTypes = new[] { "Primary", "Secondary" };
        var schoolType = schoolTypes[_random.Next(schoolTypes.Length)];

        var school = new School
        {
            Id = _nextTempId--,
            Name = GenerateSchoolName(city),
            CityId = city.Id,
            FoundedDate = _currentDate,
            FounderId = founderId,
            Capacity = _random.Next(50, 200),
            QualityRating = _random.Next(40, 90),
            Type = schoolType,
            Funding = _random.Next(1000, 5000),
            TeacherCount = _random.Next(3, 15)
        };

        _schools.Add(school);
        _schoolsById[school.Id] = school;

        LogEvent("Education", $"{school.Name} founded in {city.Name}", founderId);
    }

    private void FoundUniversity()
    {
        if (_cities.Count == 0) return;

        // Universities only in large, prosperous cities
        var eligibleCities = _cities.Where(c => c.Population > 500 && c.Wealth > 5000).ToList();
        if (!eligibleCities.Any())
            eligibleCities = _cities.OrderByDescending(c => c.Population).Take(3).ToList();

        var city = eligibleCities[_random.Next(eligibleCities.Count)];

        // Find a suitable founder (highly educated, very intelligent person)
        var potentialFounders = _people.Where(p =>
            p.IsAlive &&
            p.CityId == city.Id &&
            p.EducationLevel == "Secondary" &&
            p.Intelligence >= 85
        ).ToList();

        long? founderId = null;
        if (potentialFounders.Any())
        {
            var founder = potentialFounders[_random.Next(potentialFounders.Count)];
            founderId = founder.Id;
        }

        var fields = new[] { "General", "Science", "Arts", "Medicine", "Engineering", "Philosophy" };
        var field = fields[_random.Next(fields.Length)];

        var university = new University
        {
            Id = _nextTempId--,
            Name = GenerateUniversityName(city),
            CityId = city.Id,
            FoundedDate = _currentDate,
            FounderId = founderId,
            Capacity = _random.Next(100, 500),
            PrestigeRating = _random.Next(50, 85),
            Funding = _random.Next(10000, 50000),
            ProfessorCount = _random.Next(10, 50),
            PrimaryField = field,
            ResearchOutput = 0
        };

        _universities.Add(university);
        _universitiesById[university.Id] = university;

        LogEvent("Education", $"{university.Name} founded in {city.Name} specializing in {field}", founderId);
    }

    private string GenerateSchoolName(City city)
    {
        var prefixes = new[] { "St.", "Royal", "Central", "New", "Old", "Grand" };
        var suffixes = new[] { "Academy", "School", "Institute", "College", "Hall" };

        if (_random.Next(100) < 50)
            return $"{city.Name} {suffixes[_random.Next(suffixes.Length)]}";
        else
            return $"{prefixes[_random.Next(prefixes.Length)]} {city.Name} {suffixes[_random.Next(suffixes.Length)]}";
    }

    private string GenerateUniversityName(City city)
    {
        var prefixes = new[] { "Royal", "Imperial", "Grand", "Ancient" };

        if (_random.Next(100) < 60)
            return $"University of {city.Name}";
        else
            return $"{prefixes[_random.Next(prefixes.Length)]} {city.Name} University";
    }

    private void TriggerNaturalDisaster()
    {
        if (_cities.Count == 0) return;

        // PERFORMANCE: Pre-build invention lookup HashSet (O(1) lookups)
        var discoveredInventions = new HashSet<string>(_inventions.Select(i => i.Name));

        // Select eligible cities based on geography-aware disaster probabilities
        var eligibleCities = new List<(City city, string disasterType, double probability)>();

        // OPTIMIZED: Single pass through cities to build eligible list
        for (int i = 0; i < _cities.Count; i++)
        {
            var city = _cities[i];
            string geo = city.Geography;
            string climate = city.Climate;

            // Geography-specific disasters with realistic probabilities
            if (geo == "Coastal" || geo == "Island")
            {
                if (climate == "Tropical") eligibleCities.Add((city, "Hurricane", 0.12)); // 12% for tropical coasts
                eligibleCities.Add((city, "Tsunami", 0.03)); // 3% for coasts
                eligibleCities.Add((city, "Flood", 0.08)); // 8% for coasts
            }

            if (geo == "Mountain")
            {
                eligibleCities.Add((city, "Volcano", 0.04)); // 4% for mountains
                eligibleCities.Add((city, "Earthquake", 0.10)); // 10% for mountains
                eligibleCities.Add((city, "Landslide", 0.06)); // 6% for mountains
                if (climate == "Arctic") eligibleCities.Add((city, "Blizzard", 0.15)); // 15% arctic mountains
            }

            if (geo == "Plains")
            {
                eligibleCities.Add((city, "Tornado", 0.05)); // 5% for plains
                if (climate == "Temperate") eligibleCities.Add((city, "Flood", 0.06)); // 6% temperate plains
            }

            if (geo == "Desert" || climate == "Arid")
            {
                eligibleCities.Add((city, "Drought", 0.15)); // 15% for deserts
            }

            if (geo == "Forest")
            {
                eligibleCities.Add((city, "Wildfire", 0.08)); // 8% for forests
                if (climate == "Arid") eligibleCities.Add((city, "Wildfire", 0.12)); // 12% dry forests
            }

            if (geo == "RiverValley")
            {
                eligibleCities.Add((city, "Flood", 0.12)); // 12% for river valleys
            }

            if (climate == "Arctic")
            {
                eligibleCities.Add((city, "Blizzard", 0.10)); // 10% arctic regions
            }

            // Universal disasters (can happen anywhere but less frequent)
            eligibleCities.Add((city, "Earthquake", 0.02)); // 2% anywhere

            // Plague and Famine - affected by inventions (mitigated by technology)
            double plagueProbability = 0.08; // Base 8%
            double famineProbability = 0.10; // Base 10%

            // MITIGATION: Medicine reduces plague
            if (discoveredInventions.Contains("Vaccination")) plagueProbability *= 0.1; // 90% reduction
            else if (discoveredInventions.Contains("Antiseptics")) plagueProbability *= 0.3; // 70% reduction
            else if (discoveredInventions.Contains("Herbal Medicine")) plagueProbability *= 0.6; // 40% reduction

            // MITIGATION: Agriculture/Food reduces famine
            if (discoveredInventions.Contains("Crop Rotation") && discoveredInventions.Contains("Irrigation"))
                famineProbability *= 0.2; // 80% reduction
            else if (discoveredInventions.Contains("Agriculture") && discoveredInventions.Contains("Granary"))
                famineProbability *= 0.4; // 60% reduction
            else if (discoveredInventions.Contains("Agriculture"))
                famineProbability *= 0.7; // 30% reduction

            if (plagueProbability > 0.001) eligibleCities.Add((city, "Plague", plagueProbability));
            if (famineProbability > 0.001) eligibleCities.Add((city, "Famine", famineProbability));
        }

        if (eligibleCities.Count == 0) return;

        // Select disaster based on probability weights
        var selected = eligibleCities[_random.Next(eligibleCities.Count)];
        if (_random.NextDouble() > selected.probability) return; // Check if disaster actually occurs

        var affectedCity = selected.city;
        var disasterType = selected.disasterType;
        var affectedCountry = affectedCity.CountryId.HasValue && _countriesById.ContainsKey(affectedCity.CountryId.Value)
            ? _countriesById[affectedCity.CountryId.Value]
            : null;

        int severity = _random.Next(1, 11); // 1-10 severity scale

        // PERFORMANCE: Direct array access instead of LINQ for affected people
        int deaths = 0;
        int peopleDisplaced = 0;
        int maxAffected = severity * 5;
        int affected = 0;

        // OPTIMIZED: Use for loop instead of foreach for better performance
        for (int i = 0; i < _people.Count && affected < maxAffected; i++)
        {
            var person = _people[i];
            if (!person.IsAlive || person.CityId != affectedCity.Id) continue;

            affected++;

            // Calculate survival chance based on disaster type and mitigations
            double survivalChance = 1.0 - (severity / 20.0);

            if (disasterType == "Plague")
            {
                survivalChance = person.DiseaseResistance / 100.0;
                // Medicine inventions help survival
                if (discoveredInventions.Contains("Vaccination")) survivalChance *= 1.5;
                else if (discoveredInventions.Contains("Surgery")) survivalChance *= 1.3;
            }
            else if (disasterType == "Famine")
            {
                survivalChance = person.Health / 150.0; // Health affects famine survival
                // Food storage helps
                if (discoveredInventions.Contains("Granary")) survivalChance *= 1.4;
                if (discoveredInventions.Contains("Salt Preservation")) survivalChance *= 1.2;
            }

            if (_random.NextDouble() > Math.Min(survivalChance, 0.95))
            {
                person.IsAlive = false;
                person.DeathDate = _currentDate;
                person.CauseOfDeath = disasterType;
                deaths++;
                MoveToDead(person);
            }
            else if (_random.NextDouble() < 0.3)
            {
                peopleDisplaced++;
                person.CityId = null; // Displaced from city
            }
        }

        int buildingsDestroyed = severity * _random.Next(5, 20);
        decimal economicDamage = severity * _random.Next(1000, 10000);

        // MITIGATION: Construction technologies reduce building damage
        if (discoveredInventions.Contains("Concrete") || discoveredInventions.Contains("Architecture"))
        {
            buildingsDestroyed = (int)(buildingsDestroyed * 0.6); // 40% reduction
            economicDamage *= 0.7m; // 30% reduction
        }

        // Reduce city/country wealth
        if (affectedCity != null)
            affectedCity.Wealth = Math.Max(0, affectedCity.Wealth - economicDamage);
        if (affectedCountry != null)
            affectedCountry.Wealth = Math.Max(0, affectedCountry.Wealth - economicDamage);

        var disaster = new NaturalDisaster
        {
            Id = _nextTempId--,
            Type = disasterType,
            OccurredDate = _currentDate,
            CityId = affectedCity?.Id,
            CountryId = affectedCountry?.Id,
            Severity = severity,
            Deaths = deaths,
            EconomicDamage = economicDamage,
            BuildingsDestroyed = buildingsDestroyed,
            PeopleDisplaced = peopleDisplaced,
            Description = $"{disasterType} struck {affectedCity?.Name ?? "unknown location"} ({affectedCity.Geography}/{affectedCity.Climate}) with severity {severity}/10"
        };

        _disasters.Add(disaster);

        LogEvent("Disaster", $"{disasterType} struck {affectedCity.Name} - {deaths} dead, {peopleDisplaced} displaced", cityId: affectedCity?.Id, countryId: affectedCountry?.Id);
    }

    private void SyncToDatabase()
    {
        _dataAccess.SavePeople(_people);
        _dataAccess.SaveCities(_cities);
        _dataAccess.SaveCountries(_countries);
        _dataAccess.SaveReligions(_religions);
        _dataAccess.SaveInventions(_inventions);
        _dataAccess.SaveWars(_wars);
        _dataAccess.SaveDynasties(_dynasties);
        _dataAccess.SaveEvents(_recentEvents);
    }
    
    public SimulationStats GetStats()
    {
        var livingPeople = GetLivingPeople();
        
        // Check if simulation has ended
        bool simulationEnded = false;
        string? endReason = null;
        
        if (livingPeople.Count == 0)
        {
            simulationEnded = true;
            endReason = "All people have died. The civilization has ended.";
        }
        else
        {
            // Check for breeding-age females
            var breedingFemales = livingPeople.Count(p => 
                p.Gender == "Female" && p.GetAge(_currentDate) >= 16 && p.GetAge(_currentDate) <= 45);
            
            // Check for young females who might reach breeding age
            var youngFemales = livingPeople.Count(p => 
                p.Gender == "Female" && p.GetAge(_currentDate) < 16);
            
            if (breedingFemales == 0 && youngFemales == 0)
            {
                simulationEnded = true;
                endReason = "No females of breeding age remain and no young females to continue the population. The civilization has ended.";
            }
            else if (breedingFemales == 0 && youngFemales > 0)
            {
                // Check if there are any males who could breed with future females
                var breedingMales = livingPeople.Count(p => 
                    p.Gender == "Male" && p.GetAge(_currentDate) >= 16 && p.GetAge(_currentDate) <= 60);
                var youngMales = livingPeople.Count(p => 
                    p.Gender == "Male" && p.GetAge(_currentDate) < 16);
                
                if (breedingMales == 0 && youngMales == 0)
                {
                    simulationEnded = true;
                    endReason = "No males of breeding age remain and no young males to continue the population. The civilization will end.";
                }
            }
        }
        
        // Calculate gender distribution
        var maleCount = livingPeople.Count(p => p.Gender == "Male");
        var femaleCount = livingPeople.Count(p => p.Gender == "Female");
        
        // Calculate education statistics
        var literateCount = livingPeople.Count(p => p.IsLiterate);
        var literacyRate = livingPeople.Count > 0 ? (double)literateCount / livingPeople.Count * 100 : 0;
        var totalStudents = _schools.Sum(s => s.CurrentStudents);
        var totalUniversityStudents = _universities.Sum(u => u.CurrentStudents);

        return new SimulationStats
        {
            CurrentDate = _currentDate,
            TotalPopulation = _people.Count,
            LivingPopulation = livingPeople.Count,
            TotalBirths = _people.Count,
            TotalDeaths = _people.Count - livingPeople.Count,
            TotalMarriages = livingPeople.Count(p => p.IsMarried) / 2,
            TotalCities = _cities.Count,
            TotalCountries = _countries.Count,
            TotalReligions = _religions.Count,
            TotalInventions = _inventions.Count,
            TotalWars = _wars.Count,
            GenerationNumber = _generationNumber,
            SimulationEnded = simulationEnded,
            EndReason = endReason,
            MaleCount = maleCount,
            FemaleCount = femaleCount,
            RecentEvents = _recentEvents.TakeLast(10).ToList(),
            TopJobs = GetTopJobs(),
            FamilyTrees = GetActiveFamilyTrees(),

            // Education statistics
            TotalSchools = _schools.Count,
            TotalUniversities = _universities.Count,
            TotalStudents = totalStudents,
            TotalUniversityStudents = totalUniversityStudents,
            LiteratePopulation = literateCount,
            LiteracyRate = literacyRate,
            Schools = GetSchoolSummaries(),
            Universities = GetUniversitySummaries(),

            // Businesses and disasters
            TotalBusinesses = _businesses.Count,
            TotalDisasters = _disasters.Count
        };
    }
    
    private List<JobStatistic> GetTopJobs()
    {
        var livingPeople = GetLivingPeople();
        
        // Group people by their job
        var jobGroups = new Dictionary<string, int>();
        
        foreach (var person in livingPeople)
        {
            string jobName = !person.JobId.HasValue ? "Unemployed" 
                : (_jobsById.ContainsKey(person.JobId.Value) ? _jobsById[person.JobId.Value].Name : "Unknown");
            
            if (!jobGroups.ContainsKey(jobName))
                jobGroups[jobName] = 0;
            jobGroups[jobName]++;
        }
        
        // Convert to list and return top 10
        return jobGroups
            .Select(kvp => new JobStatistic { JobName = kvp.Key, Count = kvp.Value })
            .OrderByDescending(j => j.Count)
            .Take(10)
            .ToList();
    }

    private List<SchoolSummary> GetSchoolSummaries()
    {
        return _schools.Select(s =>
        {
            var city = _citiesById.ContainsKey(s.CityId) ? _citiesById[s.CityId].Name : "Unknown";
            var founder = s.FounderId.HasValue && _peopleById.ContainsKey(s.FounderId.Value)
                ? $"{_peopleById[s.FounderId.Value].FirstName} {_peopleById[s.FounderId.Value].LastName}"
                : (s.FounderId.HasValue && _deadPeopleById.ContainsKey(s.FounderId.Value)
                    ? $"{_deadPeopleById[s.FounderId.Value].FirstName} {_deadPeopleById[s.FounderId.Value].LastName}"
                    : "Unknown");

            return new SchoolSummary
            {
                Name = s.Name,
                Type = s.Type,
                City = city,
                Founder = founder,
                CurrentStudents = s.CurrentStudents,
                Capacity = s.Capacity,
                QualityRating = s.QualityRating,
                YearsOpen = (_currentDate - s.FoundedDate).Days / 365
            };
        }).ToList();
    }

    private List<UniversitySummary> GetUniversitySummaries()
    {
        return _universities.Select(u =>
        {
            var city = _citiesById.ContainsKey(u.CityId) ? _citiesById[u.CityId].Name : "Unknown";
            var founder = u.FounderId.HasValue && _peopleById.ContainsKey(u.FounderId.Value)
                ? $"{_peopleById[u.FounderId.Value].FirstName} {_peopleById[u.FounderId.Value].LastName}"
                : (u.FounderId.HasValue && _deadPeopleById.ContainsKey(u.FounderId.Value)
                    ? $"{_deadPeopleById[u.FounderId.Value].FirstName} {_deadPeopleById[u.FounderId.Value].LastName}"
                    : "Unknown");

            return new UniversitySummary
            {
                Name = u.Name,
                City = city,
                PrimaryField = u.PrimaryField,
                Founder = founder,
                CurrentStudents = u.CurrentStudents,
                Capacity = u.Capacity,
                PrestigeRating = u.PrestigeRating,
                YearsOpen = (_currentDate - u.FoundedDate).Days / 365
            };
        }).ToList();
    }

    private List<FamilyTreeNode> GetActiveFamilyTrees()
    {
        var trees = new List<FamilyTreeNode>();
        
        // Always try to show Adam's tree first (if we have Adam ID saved)
        if (_adamId.HasValue)
        {
            Person? adam = null;
            if (_peopleById.ContainsKey(_adamId.Value))
                adam = _peopleById[_adamId.Value];
            else if (_deadPeopleById.ContainsKey(_adamId.Value))
                adam = _deadPeopleById[_adamId.Value];
            
            if (adam != null)
            {
                trees.Add(BuildFamilyTreeNode(adam));
                return trees;
            }
        }
        
        // Fallback: Find male roots - people with no parents
        var roots = _people.Where(p => 
            !p.FatherId.HasValue && 
            !p.MotherId.HasValue &&
            p.Gender == "Male"
        ).ToList();
        
        // If no roots, find male children with most descendants
        if (roots.Count == 0)
        {
            var livingPeople = GetLivingPeople();
            var potentialRoots = livingPeople
                .Where(p => p.Gender == "Male" && HasLivingDescendants(p.Id))
                .OrderByDescending(p => CountLivingDescendants(p.Id))
                .Take(10) // Top 10 most active male lines
                .ToList();
            
            roots = potentialRoots;
        }
        
        // Build tree for each root (limit to top 1 tree to prevent overflow)
        foreach (var root in roots.Take(1))
        {
            trees.Add(BuildFamilyTreeNode(root));
        }
        
        return trees;
    }
    
    private bool HasLivingDescendants(long personId)
    {
        // Use cached living people for better performance
        var livingPeople = GetLivingPeople();
        
        // Check if any living person has this person as parent
        if (livingPeople.Any(p => p.FatherId == personId || p.MotherId == personId))
            return true;
        
        // Check descendants recursively
        var children = _people.Where(p => p.FatherId == personId || p.MotherId == personId);
        foreach (var child in children)
        {
            if (HasLivingDescendants(child.Id))
                return true;
        }
        
        return false;
    }
    
    private int CountLivingDescendants(long personId)
    {
        var livingPeople = GetLivingPeople();
        
        // Get all children
        var children = _people.Where(p => p.FatherId == personId || p.MotherId == personId).ToList();
        
        // Count living children
        int count = children.Count(c => c.IsAlive);
        
        // Count descendants of all children recursively
        foreach (var child in children)
        {
            count += CountLivingDescendants(child.Id);
        }
        
        return count;
    }
    
    private FamilyTreeNode BuildFamilyTreeNode(Person person)
    {
        var node = new FamilyTreeNode
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Gender = person.Gender,
            Age = person.GetAge(_currentDate),
            IsAlive = person.IsAlive,
            SpouseName = GetSpouseName(person),
            Children = new List<FamilyTreeNode>()
        };
        
        // Only add living children or children with living descendants
        var children = _people
            .Where(p => p.FatherId == person.Id || p.MotherId == person.Id)
            .Where(c => c.IsAlive || HasLivingDescendants(c.Id))
            .OrderByDescending(c => c.IsAlive)
            .ThenBy(c => c.BirthDate)
            .Take(10) // Limit children shown to prevent huge trees
            .ToList();
        
        foreach (var child in children)
        {
            node.Children.Add(BuildFamilyTreeNode(child));
        }
        
        return node;
    }
    
    private string GetSpouseName(Person person)
    {
        if (!person.SpouseId.HasValue || !_peopleById.ContainsKey(person.SpouseId.Value))
            return string.Empty;

        var spouse = _peopleById[person.SpouseId.Value];
        return $"{spouse.FirstName} {spouse.LastName}";
    }

    // ============================================================================
    // EXPORT FUNCTIONALITY
    // ============================================================================

    public string ExportPopulationAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,FirstName,LastName,Gender,BirthDate,DeathDate,IsAlive,Age,Intelligence,Strength,Health,Fertility,Charisma,Creativity,Leadership,Aggression,Wisdom,Beauty,BloodType,Job,City,Country,Married,SpouseName,MotherName,FatherName");

        foreach (var person in _people)
        {
            var age = person.GetAge(_currentDate);
            var job = person.JobId.HasValue && _jobsById.ContainsKey(person.JobId.Value) ? _jobsById[person.JobId.Value].Name : "Unemployed";
            var city = person.CityId.HasValue && _citiesById.ContainsKey(person.CityId.Value) ? _citiesById[person.CityId.Value].Name : "Nomad";
            var country = person.CountryId.HasValue && _countriesById.ContainsKey(person.CountryId.Value) ? _countriesById[person.CountryId.Value].Name : "None";
            var spouse = person.SpouseId.HasValue && _peopleById.ContainsKey(person.SpouseId.Value) ? $"{_peopleById[person.SpouseId.Value].FirstName} {_peopleById[person.SpouseId.Value].LastName}" : "";
            var mother = person.MotherId.HasValue && (_peopleById.ContainsKey(person.MotherId.Value) || _deadPeopleById.ContainsKey(person.MotherId.Value))
                ? (_peopleById.ContainsKey(person.MotherId.Value) ? $"{_peopleById[person.MotherId.Value].FirstName} {_peopleById[person.MotherId.Value].LastName}"
                : $"{_deadPeopleById[person.MotherId.Value].FirstName} {_deadPeopleById[person.MotherId.Value].LastName}") : "";
            var father = person.FatherId.HasValue && (_peopleById.ContainsKey(person.FatherId.Value) || _deadPeopleById.ContainsKey(person.FatherId.Value))
                ? (_peopleById.ContainsKey(person.FatherId.Value) ? $"{_peopleById[person.FatherId.Value].FirstName} {_peopleById[person.FatherId.Value].LastName}"
                : $"{_deadPeopleById[person.FatherId.Value].FirstName} {_deadPeopleById[person.FatherId.Value].LastName}") : "";

            sb.AppendLine($"{person.Id},\"{person.FirstName}\",\"{person.LastName}\",{person.Gender},{person.BirthDate:yyyy-MM-dd},{person.DeathDate:yyyy-MM-dd},{person.IsAlive},{age},{person.Intelligence},{person.Strength},{person.Health},{person.Fertility},{person.Charisma},{person.Creativity},{person.Leadership},{person.Aggression},{person.Wisdom},{person.Beauty},\"{person.BloodType}\",\"{job}\",\"{city}\",\"{country}\",{person.IsMarried},\"{spouse}\",\"{mother}\",\"{father}\"");
        }

        return sb.ToString();
    }

    public string ExportCitiesAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Name,Country,Population,Wealth,Geography,Climate,FoundedDate,YearsOld,Founder");

        foreach (var city in _cities)
        {
            var country = city.CountryId.HasValue && _countriesById.ContainsKey(city.CountryId.Value) ? _countriesById[city.CountryId.Value].Name : "Independent";
            var yearsOld = (_currentDate - city.FoundedDate).Days / 365;
            var founder = city.FounderId.HasValue && (_peopleById.ContainsKey(city.FounderId.Value) || _deadPeopleById.ContainsKey(city.FounderId.Value))
                ? (_peopleById.ContainsKey(city.FounderId.Value) ? $"{_peopleById[city.FounderId.Value].FirstName} {_peopleById[city.FounderId.Value].LastName}"
                : $"{_deadPeopleById[city.FounderId.Value].FirstName} {_deadPeopleById[city.FounderId.Value].LastName}") : "Unknown";

            sb.AppendLine($"{city.Id},\"{city.Name}\",\"{country}\",{city.Population},{city.Wealth},\"{city.Geography}\",\"{city.Climate}\",{city.FoundedDate:yyyy-MM-dd},{yearsOld},\"{founder}\"");
        }

        return sb.ToString();
    }

    public string ExportCountriesAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Name,Population,Wealth,MilitaryStrength,GovernmentType,Geography,Climate,FoundedDate,YearsOld,Ruler,Capital");

        foreach (var country in _countries)
        {
            var yearsOld = (_currentDate - country.FoundedDate).Days / 365;
            var ruler = country.RulerId.HasValue && _peopleById.ContainsKey(country.RulerId.Value) ? $"{_peopleById[country.RulerId.Value].FirstName} {_peopleById[country.RulerId.Value].LastName}" : "None";
            var capital = country.CapitalCityId.HasValue && _citiesById.ContainsKey(country.CapitalCityId.Value) ? _citiesById[country.CapitalCityId.Value].Name : "None";

            sb.AppendLine($"{country.Id},\"{country.Name}\",{country.Population},{country.Wealth},{country.MilitaryStrength},\"{country.GovernmentType}\",\"{country.DominantGeography}\",\"{country.DominantClimate}\",{country.FoundedDate:yyyy-MM-dd},{yearsOld},\"{ruler}\",\"{capital}\"");
        }

        return sb.ToString();
    }

    public string ExportInventionsAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Name,Category,YearDiscovered,Inventor,HealthBonus,LifespanBonus,Description");

        foreach (var invention in _inventions)
        {
            var yearDiscovered = invention.DiscoveredDate.Year;
            var inventor = invention.InventorId.HasValue && (_peopleById.ContainsKey(invention.InventorId.Value) || _deadPeopleById.ContainsKey(invention.InventorId.Value))
                ? (_peopleById.ContainsKey(invention.InventorId.Value) ? $"{_peopleById[invention.InventorId.Value].FirstName} {_peopleById[invention.InventorId.Value].LastName}"
                : $"{_deadPeopleById[invention.InventorId.Value].FirstName} {_deadPeopleById[invention.InventorId.Value].LastName}") : "Unknown";

            sb.AppendLine($"\"{invention.Name}\",\"{invention.Category}\",{yearDiscovered},\"{inventor}\",{invention.HealthBonus},{invention.LifespanBonus},\"{invention.Description}\"");
        }

        return sb.ToString();
    }

    public string ExportDisastersAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Type,Location,Severity,Deaths,EconomicDamage,Date,Description");

        foreach (var disaster in _disasters)
        {
            var location = disaster.CityId.HasValue && _citiesById.ContainsKey(disaster.CityId.Value) ? _citiesById[disaster.CityId.Value].Name : "Unknown";

            sb.AppendLine($"\"{disaster.Type}\",\"{location}\",{disaster.Severity},{disaster.Deaths},{disaster.EconomicDamage},{disaster.Date:yyyy-MM-dd},\"{disaster.Description}\"");
        }

        return sb.ToString();
    }

    public string ExportBusinessesAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Name,Type,Owner,City,Wealth,Reputation,IsActive,FoundedDate,YearsInBusiness,EmployeeCount");

        foreach (var business in _businesses)
        {
            var owner = business.OwnerId.HasValue && _peopleById.ContainsKey(business.OwnerId.Value) ? $"{_peopleById[business.OwnerId.Value].FirstName} {_peopleById[business.OwnerId.Value].LastName}" : "Unknown";
            var city = business.CityId.HasValue && _citiesById.ContainsKey(business.CityId.Value) ? _citiesById[business.CityId.Value].Name : "Unknown";
            var yearsInBusiness = (_currentDate - business.FoundedDate).Days / 365;
            var employeeCount = _businessEmployees.Count(e => e.BusinessId == business.Id);

            sb.AppendLine($"\"{business.Name}\",\"{business.Type}\",\"{owner}\",\"{city}\",{business.Wealth},{business.Reputation},{business.IsActive},{business.FoundedDate:yyyy-MM-dd},{yearsInBusiness},{employeeCount}");
        }

        return sb.ToString();
    }

    public string ExportEventsAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Type,Date,Description,PersonId");

        foreach (var evt in _recentEvents)
        {
            sb.AppendLine($"\"{evt.EventType}\",{evt.Date:yyyy-MM-dd},\"{evt.Description}\",{evt.PersonId}");
        }

        return sb.ToString();
    }

    public string ExportSchoolsAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Name,Type,City,Founder,FoundedDate,CurrentStudents,Capacity,QualityRating,TeacherCount,Funding");

        foreach (var school in _schools)
        {
            var city = _citiesById.ContainsKey(school.CityId) ? _citiesById[school.CityId].Name : "Unknown";
            var founder = school.FounderId.HasValue && _peopleById.ContainsKey(school.FounderId.Value)
                ? $"{_peopleById[school.FounderId.Value].FirstName} {_peopleById[school.FounderId.Value].LastName}"
                : (school.FounderId.HasValue && _deadPeopleById.ContainsKey(school.FounderId.Value)
                    ? $"{_deadPeopleById[school.FounderId.Value].FirstName} {_deadPeopleById[school.FounderId.Value].LastName}"
                    : "Unknown");

            sb.AppendLine($"\"{school.Name}\",\"{school.Type}\",\"{city}\",\"{founder}\",{school.FoundedDate:yyyy-MM-dd},{school.CurrentStudents},{school.Capacity},{school.QualityRating},{school.TeacherCount},{school.Funding}");
        }

        return sb.ToString();
    }

    public string ExportUniversitiesAsCSV()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Name,PrimaryField,City,Founder,FoundedDate,CurrentStudents,Capacity,PrestigeRating,ProfessorCount,Funding,ResearchOutput");

        foreach (var university in _universities)
        {
            var city = _citiesById.ContainsKey(university.CityId) ? _citiesById[university.CityId].Name : "Unknown";
            var founder = university.FounderId.HasValue && _peopleById.ContainsKey(university.FounderId.Value)
                ? $"{_peopleById[university.FounderId.Value].FirstName} {_peopleById[university.FounderId.Value].LastName}"
                : (university.FounderId.HasValue && _deadPeopleById.ContainsKey(university.FounderId.Value)
                    ? $"{_deadPeopleById[university.FounderId.Value].FirstName} {_deadPeopleById[university.FounderId.Value].LastName}"
                    : "Unknown");

            sb.AppendLine($"\"{university.Name}\",\"{university.PrimaryField}\",\"{city}\",\"{founder}\",{university.FoundedDate:yyyy-MM-dd},{university.CurrentStudents},{university.Capacity},{university.PrestigeRating},{university.ProfessorCount},{university.Funding},{university.ResearchOutput}");
        }

        return sb.ToString();
    }

    public string ExportAllAsJSON()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("  \"CurrentDate\": \"" + _currentDate.ToString("yyyy-MM-dd") + "\",");
        sb.AppendLine("  \"GenerationNumber\": " + _generationNumber + ",");
        sb.AppendLine("  \"Statistics\": {");

        var stats = GetStats();
        sb.AppendLine("    \"TotalPopulation\": " + stats.TotalPopulation + ",");
        sb.AppendLine("    \"LivingPopulation\": " + stats.LivingPopulation + ",");
        sb.AppendLine("    \"TotalBirths\": " + stats.TotalBirths + ",");
        sb.AppendLine("    \"TotalDeaths\": " + stats.TotalDeaths + ",");
        sb.AppendLine("    \"TotalCities\": " + stats.TotalCities + ",");
        sb.AppendLine("    \"TotalCountries\": " + stats.TotalCountries + ",");
        sb.AppendLine("    \"TotalInventions\": " + stats.TotalInventions + ",");
        sb.AppendLine("    \"TotalWars\": " + stats.TotalWars + ",");
        sb.AppendLine("    \"TotalDisasters\": " + _disasters.Count);
        sb.AppendLine("  },");

        sb.AppendLine("  \"People\": [");
        for (int i = 0; i < _people.Count; i++)
        {
            var p = _people[i];
            sb.Append($"    {{\"Id\": {p.Id}, \"Name\": \"{p.FirstName} {p.LastName}\", \"Gender\": \"{p.Gender}\", \"Age\": {p.GetAge(_currentDate)}, \"IsAlive\": {p.IsAlive.ToString().ToLower()}}}");
            if (i < _people.Count - 1) sb.AppendLine(",");
        }
        sb.AppendLine();
        sb.AppendLine("  ],");

        sb.AppendLine("  \"Cities\": [");
        for (int i = 0; i < _cities.Count; i++)
        {
            var c = _cities[i];
            sb.Append($"    {{\"Id\": {c.Id}, \"Name\": \"{c.Name}\", \"Population\": {c.Population}, \"Geography\": \"{c.Geography}\", \"Climate\": \"{c.Climate}\"}}");
            if (i < _cities.Count - 1) sb.AppendLine(",");
        }
        sb.AppendLine();
        sb.AppendLine("  ],");

        sb.AppendLine("  \"Countries\": [");
        for (int i = 0; i < _countries.Count; i++)
        {
            var c = _countries[i];
            sb.Append($"    {{\"Id\": {c.Id}, \"Name\": \"{c.Name}\", \"Population\": {c.Population}, \"GovernmentType\": \"{c.GovernmentType}\"}}");
            if (i < _countries.Count - 1) sb.AppendLine(",");
        }
        sb.AppendLine();
        sb.AppendLine("  ]");

        sb.AppendLine("}");
        return sb.ToString();
    }

    // ============================================================================
    // SEARCH & FILTER ACCESS
    // ============================================================================

    public List<Person> GetAllPeople() => _people.ToList();
    public List<City> GetAllCities() => _cities.ToList();
    public List<Country> GetAllCountries() => _countries.ToList();
    public List<Invention> GetAllInventions() => _inventions.ToList();
    public List<School> GetAllSchools() => _schools.ToList();
    public List<University> GetAllUniversities() => _universities.ToList();
    public Dictionary<long, Person> GetPeopleById() => new(_peopleById);
    public Dictionary<long, Person> GetDeadPeopleById() => new(_deadPeopleById);
    public Dictionary<long, City> GetCitiesById() => new(_citiesById);
    public Dictionary<long, Country> GetCountriesById() => new(_countriesById);
    public Dictionary<long, School> GetSchoolsById() => new(_schoolsById);
    public Dictionary<long, University> GetUniversitiesById() => new(_universitiesById);
    public DateTime GetCurrentDate() => _currentDate;

    // ============================================================================
    // SAVE/LOAD SYSTEM
    // ============================================================================

    public string SaveSimulation()
    {
        var saveState = new SimulationSaveState
        {
            CurrentDate = _currentDate,
            NextTempId = _nextTempId,
            GenerationNumber = _generationNumber,
            AdamId = _adamId,
            EveId = _eveId,
            People = _people.ToList(),
            Cities = _cities.ToList(),
            Countries = _countries.ToList(),
            Religions = _religions.ToList(),
            Jobs = _jobs.ToList(),
            Inventions = _inventions.ToList(),
            Wars = _wars.ToList(),
            Dynasties = _dynasties.ToList(),
            TradeRoutes = _tradeRoutes.ToList(),
            Businesses = _businesses.ToList(),
            BusinessEmployees = _businessEmployees.ToList(),
            Disasters = _disasters.ToList(),
            RecentEvents = _recentEvents.ToList(),
            Schools = _schools.ToList(),
            Universities = _universities.ToList()
        };

        return System.Text.Json.JsonSerializer.Serialize(saveState, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        });
    }

    public bool LoadSimulation(string jsonData)
    {
        try
        {
            var saveState = System.Text.Json.JsonSerializer.Deserialize<SimulationSaveState>(jsonData);
            if (saveState == null) return false;

            // Clear existing data
            _people.Clear();
            _peopleById.Clear();
            _deadPeopleById.Clear();
            _livingPeopleCache.Clear();
            _cities.Clear();
            _citiesById.Clear();
            _countries.Clear();
            _countriesById.Clear();
            _religions.Clear();
            _religionsById.Clear();
            _jobs.Clear();
            _jobsById.Clear();
            _inventions.Clear();
            _inventionsById.Clear();
            _wars.Clear();
            _dynasties.Clear();
            _dynastiesById.Clear();
            _tradeRoutes.Clear();
            _tradeRoutesById.Clear();
            _businesses.Clear();
            _businessesById.Clear();
            _businessEmployees.Clear();
            _disasters.Clear();
            _recentEvents.Clear();
            _schools.Clear();
            _schoolsById.Clear();
            _universities.Clear();
            _universitiesById.Clear();

            // Restore state
            _currentDate = saveState.CurrentDate;
            _nextTempId = saveState.NextTempId;
            _generationNumber = saveState.GenerationNumber;
            _adamId = saveState.AdamId;
            _eveId = saveState.EveId;

            // Restore collections
            _people.AddRange(saveState.People);
            _cities.AddRange(saveState.Cities);
            _countries.AddRange(saveState.Countries);
            _religions.AddRange(saveState.Religions);
            _jobs.AddRange(saveState.Jobs);
            _inventions.AddRange(saveState.Inventions);
            _wars.AddRange(saveState.Wars);
            _dynasties.AddRange(saveState.Dynasties);
            _tradeRoutes.AddRange(saveState.TradeRoutes);
            _businesses.AddRange(saveState.Businesses);
            _businessEmployees.AddRange(saveState.BusinessEmployees);
            _disasters.AddRange(saveState.Disasters);
            _recentEvents.AddRange(saveState.RecentEvents);
            _schools.AddRange(saveState.Schools);
            _universities.AddRange(saveState.Universities);

            // Rebuild dictionaries
            foreach (var person in _people)
            {
                _peopleById[person.Id] = person;
                if (!person.IsAlive)
                    _deadPeopleById[person.Id] = person;
            }

            foreach (var city in _cities)
                _citiesById[city.Id] = city;

            foreach (var country in _countries)
                _countriesById[country.Id] = country;

            foreach (var religion in _religions)
                _religionsById[religion.Id] = religion;

            foreach (var job in _jobs)
                _jobsById[job.Id] = job;

            foreach (var invention in _inventions)
                _inventionsById[invention.Id] = invention;

            foreach (var dynasty in _dynasties)
                _dynastiesById[dynasty.Id] = dynasty;

            foreach (var tradeRoute in _tradeRoutes)
                _tradeRoutesById[tradeRoute.Id] = tradeRoute;

            foreach (var business in _businesses)
                _businessesById[business.Id] = business;

            foreach (var school in _schools)
                _schoolsById[school.Id] = school;

            foreach (var university in _universities)
                _universitiesById[university.Id] = university;

            _livingCacheValid = false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class SimulationSaveState
{
    public DateTime CurrentDate { get; set; }
    public long NextTempId { get; set; }
    public int GenerationNumber { get; set; }
    public long? AdamId { get; set; }
    public long? EveId { get; set; }
    public List<Person> People { get; set; } = new();
    public List<City> Cities { get; set; } = new();
    public List<Country> Countries { get; set; } = new();
    public List<Religion> Religions { get; set; } = new();
    public List<Job> Jobs { get; set; } = new();
    public List<Invention> Inventions { get; set; } = new();
    public List<War> Wars { get; set; } = new();
    public List<Dynasty> Dynasties { get; set; } = new();
    public List<TradeRoute> TradeRoutes { get; set; } = new();
    public List<Business> Businesses { get; set; } = new();
    public List<BusinessEmployee> BusinessEmployees { get; set; } = new();
    public List<NaturalDisaster> Disasters { get; set; } = new();
    public List<Event> RecentEvents { get; set; } = new();
    public List<School> Schools { get; set; } = new();
    public List<University> Universities { get; set; } = new();
}

public class SimulationStats
{
    public DateTime CurrentDate { get; set; }
    public int TotalPopulation { get; set; }
    public int LivingPopulation { get; set; }
    public int TotalBirths { get; set; }
    public int TotalDeaths { get; set; }
    public int TotalMarriages { get; set; }
    public int TotalCities { get; set; }
    public int TotalCountries { get; set; }
    public int TotalReligions { get; set; }
    public int TotalInventions { get; set; }
    public int TotalWars { get; set; }
    public int GenerationNumber { get; set; }
    public bool SimulationEnded { get; set; }
    public string? EndReason { get; set; }
    public int MaleCount { get; set; }
    public int FemaleCount { get; set; }
    public List<Event> RecentEvents { get; set; } = new();
    public List<JobStatistic> TopJobs { get; set; } = new();
    public List<FamilyTreeNode> FamilyTrees { get; set; } = new();

    // ENHANCED: New detailed statistics
    public List<PersonSummary> AlivePeople { get; set; } = new();
    public List<PersonSummary> RecentDeaths { get; set; } = new();
    public List<PersonSummary> RecentBirths { get; set; } = new();
    public List<PersonSummary> NotablePeople { get; set; } = new();
    public Dictionary<string, int> BloodTypeDistribution { get; set; } = new();
    public int TotalWithHereditaryDiseases { get; set; }
    public double AverageLongevity { get; set; }
    public double AverageDiseaseResistance { get; set; }

    // WEB VIEWER: Comprehensive data for tabbed UI
    public List<CitySummary> Cities { get; set; } = new();
    public List<CountrySummary> Countries { get; set; } = new();
    public List<InventionSummary> Inventions { get; set; } = new();
    public List<DisasterSummary> RecentDisasters { get; set; } = new();
    public List<BusinessSummary> Businesses { get; set; } = new();
    public int TotalBusinesses { get; set; }
    public int TotalDisasters { get; set; }

    // EDUCATION: Education statistics
    public int TotalSchools { get; set; }
    public int TotalUniversities { get; set; }
    public int TotalStudents { get; set; }
    public int TotalUniversityStudents { get; set; }
    public int LiteratePopulation { get; set; }
    public double LiteracyRate { get; set; }
    public List<SchoolSummary> Schools { get; set; } = new();
    public List<UniversitySummary> Universities { get; set; } = new();

    // Population history for charts (last 100 data points)
    public List<PopulationDataPoint> PopulationHistory { get; set; } = new();
}

public class JobStatistic
{
    public string JobName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class FamilyTreeNode
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsAlive { get; set; }
    public string SpouseName { get; set; } = string.Empty;
    public List<FamilyTreeNode> Children { get; set; } = new();
}

public class PersonSummary
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsAlive { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CauseOfDeath { get; set; } = string.Empty;
    public int ChildrenCount { get; set; }
    public int GenerationNumber { get; set; }
    public bool IsNotable { get; set; }
    public string NotableFor { get; set; } = string.Empty;
    public bool HasHereditaryDisease { get; set; }
    public string HereditaryConditions { get; set; } = string.Empty;
}

public class CitySummary
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
    public decimal Wealth { get; set; }
    public string Geography { get; set; } = string.Empty;
    public string Climate { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int YearsOld { get; set; }
}

public class CountrySummary
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
    public decimal Wealth { get; set; }
    public string DominantGeography { get; set; } = string.Empty;
    public string DominantClimate { get; set; } = string.Empty;
    public int MilitaryStrength { get; set; }
    public string GovernmentType { get; set; } = string.Empty;
    public string Ruler { get; set; } = string.Empty;
    public string Capital { get; set; } = string.Empty;
    public int YearsOld { get; set; }
}

public class InventionSummary
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Inventor { get; set; } = string.Empty;
    public int YearDiscovered { get; set; }
    public int HealthBonus { get; set; }
    public int LifespanBonus { get; set; }
}

public class DisasterSummary
{
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Severity { get; set; }
    public int Deaths { get; set; }
    public decimal EconomicDamage { get; set; }
    public int YearOccurred { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class BusinessSummary
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Wealth { get; set; }
    public int EmployeeCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Reputation { get; set; }
    public int YearsInBusiness { get; set; }
    public bool IsActive { get; set; }
}

public class SchoolSummary
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Founder { get; set; } = string.Empty;
    public int CurrentStudents { get; set; }
    public int Capacity { get; set; }
    public int QualityRating { get; set; }
    public int YearsOpen { get; set; }
}

public class UniversitySummary
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PrimaryField { get; set; } = string.Empty;
    public string Founder { get; set; } = string.Empty;
    public int CurrentStudents { get; set; }
    public int Capacity { get; set; }
    public int PrestigeRating { get; set; }
    public int YearsOpen { get; set; }
}

public class PopulationDataPoint
{
    public int Year { get; set; }
    public int Living { get; set; }
    public int Deaths { get; set; }
    public int Births { get; set; }
}
