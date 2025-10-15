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
    private readonly List<Event> _recentEvents = new();
    
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
            // Basic jobs (no requirements) - LOWERED REQUIREMENTS for realism
            ("Farmer", 10, 20, 12, 10m, 1, 1.0, false, null),
            ("Hunter", 15, 30, 14, 15m, 2, 1.5, false, null),
            ("Gatherer", 10, 15, 12, 8m, 1, 0.8, false, null),
            ("Fisherman", 12, 25, 14, 12m, 2, 1.2, false, null),
            ("Shepherd", 10, 20, 12, 10m, 1, 0.9, false, null),
            ("Builder", 15, 35, 16, 20m, 3, 1.3, false, null),
            ("Servant", 8, 12, 12, 5m, 0, 0.9, false, null),
            
            // Crafts requiring inventions
            ("Potter", 20, 15, 16, 18m, 3, 0.9, true, "Pottery"),
            ("Weaver", 18, 15, 16, 16m, 3, 0.8, true, "Weaving"),
            ("Tanner", 15, 20, 16, 15m, 2, 1.0, true, "Tanning"),
            ("Glassmaker", 30, 20, 18, 35m, 5, 1.2, true, "Glassmaking"),
            
            // Metallurgy jobs
            ("Copper Smith", 25, 35, 18, 28m, 4, 1.4, true, "Copper Working"),
            ("Bronze Smith", 28, 38, 18, 32m, 5, 1.5, true, "Bronze"),
            ("Iron Smith", 30, 40, 18, 38m, 6, 1.6, true, "Iron Working"),
            ("Blacksmith", 32, 40, 18, 42m, 6, 1.6, true, "Steel"),
            ("Goldsmith", 35, 25, 20, 50m, 7, 1.0, true, "Gold Working"),
            
            // Professional jobs
            ("Merchant", 30, 10, 18, 35m, 5, 0.9, false, null),
            ("Scribe", 40, 8, 20, 30m, 6, 0.6, true, "Writing"),
            ("Priest", 35, 8, 20, 30m, 7, 0.7, false, null),
            ("Healer", 38, 10, 20, 40m, 6, 0.8, true, "Herbal Medicine"),
            ("Physician", 42, 10, 22, 55m, 8, 0.7, true, "Surgery"),
            ("Scholar", 42, 8, 22, 35m, 7, 0.6, true, "Writing"),
            ("Teacher", 38, 8, 20, 28m, 6, 0.6, true, "Writing"),
            
            // Engineering and Architecture
            ("Architect", 40, 20, 22, 60m, 8, 0.8, true, "Architecture"),
            ("Engineer", 42, 25, 22, 65m, 8, 1.0, true, "Mathematics"),
            ("Mason", 20, 38, 16, 22m, 3, 1.4, true, "Brick Making"),
            
            // Arts
            ("Artist", 30, 10, 18, 25m, 4, 0.7, true, "Painting"),
            ("Sculptor", 32, 20, 18, 28m, 5, 0.8, true, "Sculpture"),
            ("Musician", 25, 10, 16, 20m, 4, 0.7, true, "Music"),
            ("Poet", 35, 8, 18, 22m, 5, 0.6, true, "Poetry"),
            
            // Labor
            ("Miner", 12, 42, 16, 25m, 3, 2.5, false, null),
            ("Quarryman", 12, 40, 16, 22m, 2, 2.3, false, null),
            ("Laborer", 8, 30, 14, 8m, 1, 1.5, false, null),
            
            // Food production
            ("Baker", 15, 15, 14, 12m, 2, 0.8, true, "Bread Baking"),
            ("Brewer", 18, 15, 16, 15m, 2, 0.8, true, "Beer Brewing"),
            ("Cook", 15, 15, 14, 12m, 2, 0.8, false, null),
            ("Butcher", 12, 25, 14, 14m, 2, 1.0, false, null),
            
            // Transportation
            ("Carter", 12, 25, 16, 16m, 2, 1.1, true, "Cart"),
            ("Sailor", 18, 30, 16, 20m, 3, 1.8, true, "Ship"),
            ("Charioteer", 20, 32, 18, 25m, 4, 1.5, true, "Chariot"),
            
            // Military (only available after wars start - handled separately)
            ("Warrior", 20, 40, 16, 20m, 5, 3.0, false, null), // Will be restricted by logic
            ("Guard", 18, 35, 18, 18m, 4, 1.8, false, null),
            ("Archer", 22, 30, 16, 22m, 5, 2.0, true, "Bow and Arrow"),
            
            // Leadership
            ("Leader", 38, 25, 25, 100m, 10, 1.0, false, null)
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
        // Cache living people to avoid multiple enumerations
        var livingPeople = GetLivingPeople().ToList(); // ToList to avoid collection modification
        
        foreach (var person in livingPeople)
        {
            int age = person.GetAge(_currentDate);
            double deathChance = CalculateDeathChance(person, age);
            
            if (_random.NextDouble() < deathChance)
            {
                person.IsAlive = false;
                person.DeathDate = _currentDate;
                _livingCacheValid = false; // Invalidate cache when someone dies
                
                // Add to dead people dictionary for later lookup
                if (!_deadPeopleById.ContainsKey(person.Id))
                {
                    _deadPeopleById[person.Id] = person;
                }
                
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
                // Check age requirement
                if (person.GetAge(_currentDate) < job.MinAge) continue;
                
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
        
        // Determine pregnancy chance based on population size
        double basePregnancyChance = totalPeople < 50 ? 0.20 :
                                totalPeople < 150 ? 0.15 :
                                totalPeople < 300 ? 0.10 :
                                totalPeople < 500 ? 0.05 :
                                0.02;
        
        // Single pass through people to find eligible females
        foreach (var female in livingPeople)
        {
            if (!CanHaveChildren(female)) continue;
            
            // Calculate pregnancy chance with modifiers
            double pregnancyChance = basePregnancyChance;
            pregnancyChance *= (female.Fertility / 100.0);
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
                
                // Use father's name for patronymic, or mother's last name as fallback
                string fatherName = father?.FirstName ?? string.Empty;
                long? fatherId = father?.Id;
                string cityName = mother.CityId.HasValue && _citiesById.ContainsKey(mother.CityId.Value) 
                    ? _citiesById[mother.CityId.Value].Name 
                    : string.Empty;
                string jobName = father?.JobId.HasValue == true && _jobsById.ContainsKey(father.JobId.Value) 
                    ? _jobsById[father.JobId.Value].Name 
                    : string.Empty;
                
                // Use global generation number for naming convention, not child's individual generation
                string lastName = _nameGenerator.GenerateLastName(fatherName, cityName, jobName, _generationNumber);
                
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
            FamilyTrees = GetActiveFamilyTrees()
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
