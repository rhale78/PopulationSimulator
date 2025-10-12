using PopulationSimulator.Models;
using PopulationSimulator.Data;

namespace PopulationSimulator.Core;

public class Simulator
{
    private readonly Random _random;
    private readonly NameGenerator _nameGenerator;
    private readonly DataAccessLayer _dataAccess;
    
    // In-memory collections
    private readonly List<Person> _people = new();
    private readonly Dictionary<long, Person> _peopleById = new();
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
    private readonly List<Event> _recentEvents = new();
    
    private DateTime _currentDate;
    private long _nextTempId = -1;
    private int _generationNumber = 0;
    private int _syncCounter = 0;
    private const int SYNC_INTERVAL = 100; // Sync every 100 days

    private long? _adamId;
    private long? _eveId;
    
    public Simulator()
    {
        _random = new Random();
        _nameGenerator = new NameGenerator(_random);
        _dataAccess = new DataAccessLayer();
        _currentDate = new DateTime(20, 1, 2); // Year 100 to avoid underflow
    }
    
    public void Initialize()
    {
        _dataAccess.InitializeDatabase();
        SeedInitialData();
    }
    
    private void SeedInitialData()
    {
        // Seed jobs
        SeedJobs();
        
        // Create Adam and Eve at age 20 with perfect traits (100 for all stats)
        var adam = CreatePerson("Adam", "", "Male", null, null);
        adam.BirthDate = _currentDate.AddYears(-20); // Set Adam to be 20 years old
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
        
        var eve = CreatePerson("Eve", "", "Female", null, null);
        eve.BirthDate = _currentDate.AddYears(-20); // Set Eve to be 20 years old
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
        var jobsData = new[]
        {
            // Basic jobs (no requirements)
            ("Farmer", 20, 40, 12, 10m, 1, 1.0, false, null),
            ("Hunter", 30, 60, 14, 15m, 2, 1.5, false, null),
            ("Gatherer", 20, 30, 12, 8m, 1, 0.8, false, null),
            ("Fisherman", 25, 50, 14, 12m, 2, 1.2, false, null),
            ("Shepherd", 20, 40, 12, 10m, 1, 0.9, false, null),
            ("Builder", 30, 70, 16, 20m, 3, 1.3, false, null),
            ("Servant", 15, 25, 12, 5m, 0, 0.9, false, null),
            
            // Crafts requiring inventions
            ("Potter", 40, 30, 16, 18m, 3, 0.9, true, "Pottery"),
            ("Weaver", 35, 30, 16, 16m, 3, 0.8, true, "Weaving"),
            ("Tanner", 30, 40, 16, 15m, 2, 1.0, true, "Tanning"),
            ("Glassmaker", 60, 40, 18, 35m, 5, 1.2, true, "Glassmaking"),
            
            // Metallurgy jobs
            ("Copper Smith", 50, 70, 18, 28m, 4, 1.4, true, "Copper Working"),
            ("Bronze Smith", 55, 75, 18, 32m, 5, 1.5, true, "Bronze"),
            ("Iron Smith", 60, 80, 18, 38m, 6, 1.6, true, "Iron Working"),
            ("Blacksmith", 65, 80, 18, 42m, 6, 1.6, true, "Steel"),
            ("Goldsmith", 70, 50, 20, 50m, 7, 1.0, true, "Gold Working"),
            
            // Professional jobs
            ("Merchant", 60, 20, 18, 35m, 5, 0.9, false, null),
            ("Scribe", 80, 10, 20, 30m, 6, 0.6, true, "Writing"),
            ("Priest", 70, 10, 20, 30m, 7, 0.7, false, null),
            ("Healer", 75, 20, 20, 40m, 6, 0.8, true, "Herbal Medicine"),
            ("Physician", 85, 20, 22, 55m, 8, 0.7, true, "Surgery"),
            ("Scholar", 85, 10, 22, 35m, 7, 0.6, true, "Writing"),
            ("Teacher", 75, 10, 20, 28m, 6, 0.6, true, "Writing"),
            
            // Engineering and Architecture
            ("Architect", 80, 40, 22, 60m, 8, 0.8, true, "Architecture"),
            ("Engineer", 85, 50, 22, 65m, 8, 1.0, true, "Mathematics"),
            ("Mason", 40, 75, 16, 22m, 3, 1.4, true, "Brick Making"),
            
            // Arts
            ("Artist", 60, 20, 18, 25m, 4, 0.7, true, "Painting"),
            ("Sculptor", 65, 40, 18, 28m, 5, 0.8, true, "Sculpture"),
            ("Musician", 50, 20, 16, 20m, 4, 0.7, true, "Music"),
            ("Poet", 70, 10, 18, 22m, 5, 0.6, true, "Poetry"),
            
            // Labor
            ("Miner", 25, 85, 16, 25m, 3, 2.5, false, null),
            ("Quarryman", 25, 80, 16, 22m, 2, 2.3, false, null),
            ("Laborer", 15, 60, 14, 8m, 1, 1.5, false, null),
            
            // Food production
            ("Baker", 30, 30, 14, 12m, 2, 0.8, true, "Bread Baking"),
            ("Brewer", 35, 30, 16, 15m, 2, 0.8, true, "Beer Brewing"),
            ("Cook", 30, 30, 14, 12m, 2, 0.8, false, null),
            ("Butcher", 25, 50, 14, 14m, 2, 1.0, false, null),
            
            // Transportation
            ("Carter", 25, 50, 16, 16m, 2, 1.1, true, "Cart"),
            ("Sailor", 35, 60, 16, 20m, 3, 1.8, true, "Ship"),
            ("Charioteer", 40, 65, 18, 25m, 4, 1.5, true, "Chariot"),
            
            // Military (only available after wars start - handled separately)
            ("Warrior", 40, 80, 16, 20m, 5, 3.0, false, null), // Will be restricted by logic
            ("Guard", 35, 70, 18, 18m, 4, 1.8, false, null),
            ("Archer", 45, 60, 16, 22m, 5, 2.0, true, "Bow and Arrow"),
            
            // Leadership
            ("Leader", 75, 50, 25, 100m, 10, 1.0, false, null)
        };
        
        foreach (var (name, intel, str, age, salary, status, risk, requiresInv, invName) in jobsData)
        {
            var job = new Job
            {
                Id = _nextTempId--,
                Name = name,
                MinIntelligence = intel,
                MinStrength = str,
                MinAge = age,
                BaseSalary = salary,
                SocialStatusBonus = status,
                DeathRiskModifier = risk,
                RequiresInvention = requiresInv,
                RequiredInventionId = null // Will be set dynamically when inventions are discovered
            };
            _jobs.Add(job);
            _jobsById[job.Id] = job;
        }
        
        _dataAccess.SaveJobs(_jobs);
    }
    
    private Person CreatePerson(string firstName, string lastName, string gender, long? fatherId, long? motherId)
    {
        Person? father = fatherId.HasValue && _peopleById.ContainsKey(fatherId.Value) ? _peopleById[fatherId.Value] : null;
        Person? mother = motherId.HasValue && _peopleById.ContainsKey(motherId.Value) ? _peopleById[motherId.Value] : null;
        
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
            HairColor = _nameGenerator.GenerateHairColor()
        };
        
        // Inherit and mutate traits
        if (father != null && mother != null)
        {
            person.Intelligence = InheritTrait(father.Intelligence, mother.Intelligence);
            person.Strength = InheritTrait(father.Strength, mother.Strength);
            person.Health = InheritTrait(father.Health, mother.Health);
            person.Fertility = InheritTrait(father.Fertility, mother.Fertility);
            person.Charisma = InheritTrait(father.Charisma, mother.Charisma);
            person.Creativity = InheritTrait(father.Creativity, mother.Creativity);
            person.Leadership = InheritTrait(father.Leadership, mother.Leadership);
            person.Aggression = InheritTrait(father.Aggression, mother.Aggression);
            person.Wisdom = InheritTrait(father.Wisdom, mother.Wisdom);
            person.Beauty = InheritTrait(father.Beauty, mother.Beauty);
            person.Height = InheritTrait(father.Height, mother.Height);
            
            // Inherit location and religion
            person.CityId = father.CityId ?? mother.CityId;
            person.CountryId = father.CountryId ?? mother.CountryId;
            person.ReligionId = father.ReligionId ?? mother.ReligionId;
        }
        else
        {
            // Default traits for first generation
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
        var livingPeople = _people.Where(p => p.IsAlive).ToList();
        
        foreach (var person in livingPeople)
        {
            int age = person.GetAge(_currentDate);
            double deathChance = CalculateDeathChance(person, age);
            
            if (_random.NextDouble() < deathChance)
            {
                person.IsAlive = false;
                person.DeathDate = _currentDate;
                
                LogEvent("Death", $"{person.FirstName} {person.LastName} died at age {age}", person.Id);
                
                // Handle succession if ruler
                if (person.IsRuler && person.CountryId.HasValue)
                {
                    HandleSuccession(person);
                }
            }
        }
    }
    
    private double CalculateDeathChance(Person person, int age)
    {
        // Adam/Eve are immortal until 100
        if ((_adamId.HasValue && person.Id == _adamId.Value) || (_eveId.HasValue && person.Id == _eveId.Value))
        {
            if (age < 100)
                return 0.0;
        }

        // Calculate total lifespan bonus from all inventions
        int lifespanBonus = _inventions.Sum(i => i.LifespanBonus);
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
        
        double healthMod = 1.0 - (person.Health / 200.0);
        double jobRisk = 1.0;
        if (person.JobId.HasValue && _jobsById.ContainsKey(person.JobId.Value))
            jobRisk = _jobsById[person.JobId.Value].DeathRiskModifier;
        return baseChance * (1.0 + healthMod) * jobRisk;
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
        var eligiblePeople = _people.Where(p => p.IsEligibleForJob(_currentDate)).ToList();
        
        // Track which inventions have been discovered
        var discoveredInventionNames = _inventions.Select(i => i.Name).ToHashSet();
        
        foreach (var person in eligiblePeople)
        {
            // Find suitable jobs
            var suitableJobs = _jobs.Where(j => 
            {
                // Check basic requirements
                if (person.Intelligence < j.MinIntelligence) return false;
                if (person.Strength < j.MinStrength) return false;
                if (person.GetAge(_currentDate) < j.MinAge) return false;
                
                // Check if job requires an invention
                if (j.RequiresInvention)
                {
                    // Find the required invention by checking job-invention mapping
                    bool hasRequiredInvention = false;
                    
                    // Map job names to required inventions
                    var jobInventionMap = new Dictionary<string, string>
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
                    
                    if (jobInventionMap.ContainsKey(j.Name))
                    {
                        hasRequiredInvention = discoveredInventionNames.Contains(jobInventionMap[j.Name]);
                    }
                    
                    if (!hasRequiredInvention) return false;
                }
                
                // Restrict military jobs until wars exist
                if ((j.Name == "Warrior" || j.Name == "Archer") && _wars.Count == 0)
                    return false;
                
                return true;
            }).ToList();
            
            if (suitableJobs.Any())
            {
                // Prefer jobs that match traits better
                var bestJob = suitableJobs.OrderByDescending(j => 
                    (person.Intelligence >= j.MinIntelligence ? person.Intelligence - j.MinIntelligence : 0) +
                    (person.Strength >= j.MinStrength ? person.Strength - j.MinStrength : 0)
                ).First();
                
                person.JobId = bestJob.Id;
                person.JobStartDate = _currentDate;
                person.SocialStatus += bestJob.SocialStatusBonus;
                
                if (_random.Next(10) == 0) // Log 10% of job assignments
                    LogEvent("Job", $"{person.FirstName} {person.LastName} became a {bestJob.Name}", person.Id);
            }
        }
    }
    
    private void ProcessMarriages()
    {
        var eligibleMales = _people.Where(p => 
            p.IsEligibleForMarriage(_currentDate) && p.Gender == "Male"
        ).ToList();
        
        var eligibleFemales = _people.Where(p => 
            p.IsEligibleForMarriage(_currentDate) && p.Gender == "Female"
        ).ToList();
        
        // Early population relaxed rules
        bool earlyPopulation = _people.Count(p => p.IsAlive) < 100;
        
        foreach (var male in eligibleMales)
        {
            if (!male.IsEligibleForMarriage(_currentDate)) continue;
            
            var potentialSpouses = eligibleFemales.Where(f => 
                f.IsEligibleForMarriage(_currentDate) &&
                // Avoid parent-child marriages
                f.Id != male.FatherId && f.Id != male.MotherId &&
                male.Id != f.FatherId && male.Id != f.MotherId &&
                // Same country and religion if available
                (earlyPopulation || f.CountryId == male.CountryId || f.CountryId == null || male.CountryId == null) &&
                (earlyPopulation || f.ReligionId == male.ReligionId || f.ReligionId == null || male.ReligionId == null) &&
                // Avoid sibling marriages after early population
                (earlyPopulation || f.FatherId != male.FatherId || f.FatherId == null)
            ).ToList();
            
            if (potentialSpouses.Any())
            {
                // Random chance to marry
                if (_random.NextDouble() < 0.1) // 10% chance per day
                {
                    var spouse = potentialSpouses[_random.Next(potentialSpouses.Count)];
                    MarryCouple(male, spouse);
                    
                    LogEvent("Marriage", $"{male.FirstName} {male.LastName} married {spouse.FirstName} {spouse.LastName}", male.Id);
                }
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
        var marriedFemales = _people.Where(CanHaveChildren).ToList();
        
        foreach (var female in marriedFemales)
        {
            // Significantly increased pregnancy chances to boost population growth
            int totalPeople = _people.Count(p => p.IsAlive);
            double pregnancyChance = totalPeople < 50 ? 0.20 :
                                    totalPeople < 150 ? 0.15 :
                                    totalPeople < 300 ? 0.10 :
                                    totalPeople < 500 ? 0.05 :
                                    0.02;
            
            // Fertility modifier
            pregnancyChance *= (female.Fertility / 100.0);
            
            // Health modifier
            pregnancyChance *= (female.Health / 100.0);
            
            if (_random.NextDouble() < pregnancyChance)
            {
                female.IsPregnant = true;
                female.PregnancyDueDate = _currentDate.AddDays(270); // 9 months
                female.PregnancyFatherId = female.SpouseId;
                
                // Twins/triplets chance - twins 4%, triplets 1%
                double multipleChance = _random.NextDouble();
                if (multipleChance < 0.01)
                    female.PregnancyMultiplier = 3; // 1% triplets
                else if (multipleChance < 0.05)
                    female.PregnancyMultiplier = 2; // 4% twins (1% + 4% = 5% total)
                else
                    female.PregnancyMultiplier = 1;
                
                string multiplier = female.PregnancyMultiplier > 1 ? $" (expecting {female.PregnancyMultiplier})" : "";
                LogEvent("Pregnancy", $"{female.FirstName} {female.LastName} is pregnant{multiplier}", female.Id);
            }
        }
    }
    
    private void ProcessBirths()
    {
        var duePregnancies = _people.Where(p => 
            p.IsPregnant && 
            p.PregnancyDueDate.HasValue && 
            p.PregnancyDueDate.Value <= _currentDate
        ).ToList();
        
        foreach (var mother in duePregnancies)
        {
            for (int i = 0; i < mother.PregnancyMultiplier; i++)
            {
                string gender = _random.Next(2) == 0 ? "Male" : "Female";
                string firstName = gender == "Male" ? 
                    _nameGenerator.GenerateMaleFirstName() : 
                    _nameGenerator.GenerateFemaleFirstName();
                
                // Get father - should always exist for children born during simulation
                Person? father = null;
                if (mother.PregnancyFatherId.HasValue && _peopleById.ContainsKey(mother.PregnancyFatherId.Value))
                {
                    father = _peopleById[mother.PregnancyFatherId.Value];
                }
                
                // If no father found, use spouse as fallback
                if (father == null && mother.SpouseId.HasValue && _peopleById.ContainsKey(mother.SpouseId.Value))
                {
                    father = _peopleById[mother.SpouseId.Value];
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
                
                string fatherName = father?.FirstName ?? "Unknown";
                long? fatherId = father?.Id;
                string cityName = mother.CityId.HasValue && _citiesById.ContainsKey(mother.CityId.Value) 
                    ? _citiesById[mother.CityId.Value].Name 
                    : string.Empty;
                string jobName = father?.JobId.HasValue == true && _jobsById.ContainsKey(father.JobId.Value) 
                    ? _jobsById[father.JobId.Value].Name 
                    : string.Empty;
                
                string lastName = _nameGenerator.GenerateLastName(fatherName, cityName, jobName, childGeneration);
                
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
        if (!_peopleById.ContainsKey(personId))
            return 0;
        
        var person = _peopleById[personId];
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
        
        var city = new City
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateCityName(),
            FoundedDate = _currentDate,
            Population = 0,
            FounderId = founder.Id,
            Wealth = 0
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
            MilitaryStrength = city.Population / 10
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
        return new SimulationStats
        {
            CurrentDate = _currentDate,
            TotalPopulation = _people.Count,
            LivingPopulation = _people.Count(p => p.IsAlive),
            TotalBirths = _people.Count,
            TotalDeaths = _people.Count(p => !p.IsAlive),
            TotalMarriages = _people.Count(p => p.IsMarried) / 2,
            TotalCities = _cities.Count,
            TotalCountries = _countries.Count,
            TotalReligions = _religions.Count,
            TotalInventions = _inventions.Count,
            TotalWars = _wars.Count,
            GenerationNumber = _generationNumber,
            RecentEvents = _recentEvents.TakeLast(10).ToList()
        };
    }
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
    public List<Event> RecentEvents { get; set; } = new();
}
