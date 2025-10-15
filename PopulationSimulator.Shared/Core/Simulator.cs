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
    private readonly Dictionary<long, List<Person>> _childrenByParentId = new(); // O(1) lookup for children by parent ID
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
    
    private int _currentDay; // Days since simulation start (year 1, day 1 = 0)
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
        _currentDay = 0; // Start at year 1, day 1
    }
    
    // Helper methods for date conversion (365 days per year, for simplicity)
    private int DayToYear(int day) => (day / 365) + 1;
    private int DayOfYear(int day) => (day % 365) + 1;
    
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
        // Set current day to year 20 so Adam and Eve start at age 20 (20 years * 365 days = 7300 days)
        _currentDay = 20 * 365;
        
        // Create Adam and Eve at age 20 with perfect traits (100 for all stats)
        var adam = CreatePerson("Adam", "", "Male", null, null);
        adam.BirthDay = 0; // Born on day 0 (year 1, day 1), currently age 20
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
        eve.BirthDay = 0; // Born on day 0 (year 1, day 1), currently age 20
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
            BirthDay = _currentDay,
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
        
        // Maintain children lookup for O(1) access
        if (person.FatherId.HasValue)
        {
            if (!_childrenByParentId.ContainsKey(person.FatherId.Value))
                _childrenByParentId[person.FatherId.Value] = new List<Person>();
            _childrenByParentId[person.FatherId.Value].Add(person);
        }
        
        if (person.MotherId.HasValue)
        {
            if (!_childrenByParentId.ContainsKey(person.MotherId.Value))
                _childrenByParentId[person.MotherId.Value] = new List<Person>();
            _childrenByParentId[person.MotherId.Value].Add(person);
        }
    }
    
    private List<Person> GetChildrenOfPerson(long personId)
    {
        // O(1) lookup instead of LINQ query
        if (_childrenByParentId.ContainsKey(personId))
            return _childrenByParentId[personId];
        return new List<Person>();
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
        person1.MarriageDay = _currentDay;
        person2.MarriageDay = _currentDay;
    }
    
    private void LogEvent(string eventType, string description, long? personId = null, long? cityId = null, long? countryId = null)
    {
        var evt = new Event
        {
            Id = _nextTempId--,
            Day = _currentDay,
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
        
        // Yearly events (every 365 days)
        if (_currentDay % 365 == 0)
        {
            ProcessYearlyEvents();
        }
        
        // Periodic database sync
        if (_syncCounter >= SYNC_INTERVAL)
        {
            SyncToDatabase();
            _syncCounter = 0;
        }
        
        _currentDay++;
    }
    
    private void ProcessDeaths()
    {
        // Cache living people to avoid multiple enumerations
        var livingPeople = GetLivingPeople().ToList(); // ToList to avoid collection modification
        
        foreach (var person in livingPeople)
        {
            if (person is null) continue;
            int age = person.GetAge(_currentDay);
            double deathChance = CalculateDeathChance(person, age);
            
            if (_random.NextDouble() < deathChance)
            {
                person.IsAlive = false;
                person.DeathDay = _currentDay;
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
        //int lifespanBonus = _inventions.Sum(i => i.LifespanBonus);
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
        
        // Find oldest living child - using O(1) lookup instead of LINQ
        var children = GetChildrenOfPerson(deadRuler.Id);
        Person? heir = null;
        int oldestAge = 0;
        
        foreach (var child in children)
        {
            if (!child.IsAlive)
                continue;
                
            int age = child.GetAge(_currentDay);
            if (age >= 18 && age > oldestAge)
            {
                oldestAge = age;
                heir = child;
            }
        }
        
        if (heir != null)
        {
            heir.IsRuler = true;
            country.RulerId = heir.Id;
            
            LogEvent("Succession", $"{heir.FirstName} {heir.LastName} became ruler of {country.Name}", heir.Id, countryId: country.Id);
        }
    }
    
    private void AssignJobs()
    {
        // Get list of people who need jobs (alive, old enough, no current job)
        var peopleNeedingJobs = _people
            .Where(p => p.IsAlive && p.GetAge(_currentDay) >= 12 && !p.JobId.HasValue)
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
                if (person.GetAge(_currentDay) < job.MinAge) continue;
                
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
                person.JobStartDay = _currentDay;
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
            if (!person.IsEligibleForMarriage(_currentDay)) continue;
            
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
            if (!male.IsEligibleForMarriage(_currentDay)) continue;
            
            // Random chance to marry - only process 10% to improve performance
            if (_random.NextDouble() >= 0.1) continue;
            
            // Find potential spouses
            Person? bestSpouse = null;
            foreach (var female in eligibleFemales)
            {
                if (!female.IsEligibleForMarriage(_currentDay))
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
        if (p is null) return false;
        if (p.Gender != "Female" || !p.IsAlive || !p.IsMarried) return false;
        int age = p.GetAge(_currentDay);
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
        
        // Iterate over a snapshot to avoid collection modification issues
        // (in case something invalidates the cache during iteration)
        var peopleSnapshot = new List<Person>(livingPeople);
        
        foreach (var female in peopleSnapshot)
        {
            if (!CanHaveChildren(female)) continue;
            
            // Calculate pregnancy chance with modifiers
            double pregnancyChance = basePregnancyChance;
            pregnancyChance *= (female.Fertility / 100.0);
            pregnancyChance *= (female.Health / 100.0);
            
            if (_random.NextDouble() < pregnancyChance)
            {
                female.IsPregnant = true;
                female.PregnancyDueDay = _currentDay + 270; // 9 months (approximately 270 days)
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
            if (person.IsPregnant && person.PregnancyDueDay.HasValue && 
                person.PregnancyDueDay.Value <= _currentDay)
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
                    person.PregnancyDueDay = null;
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
            mother.PregnancyDueDay = null;
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
        // Find best founder without LINQ - iterate through living people cache
        var livingPeople = GetLivingPeople();
        Person? founder = null;
        int bestLeadership = 0;
        
        foreach (var person in livingPeople)
        {
            if (person.GetAge(_currentDay) >= 25 && person.Leadership > bestLeadership)
            {
                bestLeadership = person.Leadership;
                founder = person;
            }
        }
        
        if (founder == null) return;
        
        var city = new City
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateCityName(),
            FoundedDay = _currentDay,
            Population = 0,
            FounderId = founder.Id,
            Wealth = 0
        };
        
        _cities.Add(city);
        _citiesById[city.Id] = city;
        
        // Assign some people to the city - take first 20 without city
        int assignedCount = 0;
        foreach (var person in livingPeople)
        {
            if (!person.CityId.HasValue)
            {
                person.CityId = city.Id;
                city.Population++;
                assignedCount++;
                if (assignedCount >= 20) break;
            }
        }
        
        LogEvent("City", $"The city of {city.Name} was founded by {founder.FirstName} {founder.LastName}", founder.Id, city.Id);
    }
    
    private void FoundCountry()
    {
        // Find best city without LINQ
        City? bestCity = null;
        int bestPopulation = 0;
        
        foreach (var city in _cities)
        {
            if (!city.CountryId.HasValue && city.Population > bestPopulation)
            {
                bestPopulation = city.Population;
                bestCity = city;
            }
        }
        
        if (bestCity == null) return;
        
        // Find best ruler in that city without LINQ
        var livingPeople = GetLivingPeople();
        Person? ruler = null;
        int bestScore = 0;
        
        foreach (var person in livingPeople)
        {
            if (person.CityId == bestCity.Id && person.GetAge(_currentDay) >= 25)
            {
                int score = person.Leadership + person.Charisma;
                if (score > bestScore)
                {
                    bestScore = score;
                    ruler = person;
                }
            }
        }
        
        if (ruler == null) return;
        
        var country = new Country
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateCountryName(),
            FoundedDay = _currentDay,
            RulerId = ruler.Id,
            CapitalCityId = bestCity.Id,
            Population = bestCity.Population,
            Wealth = 0,
            MilitaryStrength = bestCity.Population / 10
        };
        
        _countries.Add(country);
        _countriesById[country.Id] = country;
        
        ruler.IsRuler = true;
        ruler.CountryId = country.Id;
        bestCity.CountryId = country.Id;
        
        // Create dynasty
        var dynasty = new Dynasty
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateDynastyName(ruler.FirstName),
            FounderId = ruler.Id,
            FoundedDay = _currentDay,
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
        // Find best founder without LINQ
        var livingPeople = GetLivingPeople();
        Person? founder = null;
        int bestScore = 0;
        
        foreach (var person in livingPeople)
        {
            if (person.Intelligence > 70 && person.Charisma > 60)
            {
                int score = person.Intelligence + person.Charisma + person.Wisdom;
                if (score > bestScore)
                {
                    bestScore = score;
                    founder = person;
                }
            }
        }
        
        if (founder == null) return;
        
        var religion = new Religion
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateReligionName(),
            FoundedDay = _currentDay,
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
        // Find best inventor without LINQ
        var livingPeople = GetLivingPeople();
        Person? inventor = null;
        int bestScore = 0;
        
        foreach (var person in livingPeople)
        {
            if (person.Intelligence > 75)
            {
                int score = person.Intelligence + person.Creativity;
                if (score > bestScore)
                {
                    bestScore = score;
                    inventor = person;
                }
            }
        }
        
        if (inventor == null) return;
        
        // Get inventions that haven't been discovered yet - avoid LINQ on large array
        var discoveredNames = new HashSet<string>();
        foreach (var inv in _inventions)
        {
            discoveredNames.Add(inv.Name);
        }
        
        var availableInventions = new List<(string Name, string Category, int RequiredIntel, int HealthBonus, int LifespanBonus, string Description)>();
        foreach (var inv in NameGenerator.InventionData)
        {
            if (!discoveredNames.Contains(inv.Name) && inventor.Intelligence >= inv.RequiredIntel)
            {
                availableInventions.Add(inv);
            }
        }
        
        if (availableInventions.Count == 0) return;
        
        var inventionData = availableInventions[_random.Next(availableInventions.Count)];
        
        var invention = new Invention
        {
            Id = _nextTempId--,
            Name = inventionData.Name,
            Description = inventionData.Description,
            DiscoveredDay = _currentDay,
            InventorId = inventor.Id,
            RequiredIntelligence = inventionData.RequiredIntel,
            Category = inventionData.Category,
            HealthBonus = inventionData.HealthBonus,
            LifespanBonus = inventionData.LifespanBonus
        };
        
        _inventions.Add(invention);
        _inventionsById[invention.Id] = invention;
        
        // Apply invention effects to all living people - avoid LINQ (reuse livingPeople from earlier)
        if (invention.HealthBonus > 0 || invention.LifespanBonus > 0)
        {
            foreach (var person in livingPeople)
            {
                person.Health = Math.Min(100, person.Health + invention.HealthBonus);
            }
        }
        
        lifespanBonus = _inventions.Sum(i => i.LifespanBonus);

        LogEvent("Invention", $"{inventor.FirstName} {inventor.LastName} discovered {invention.Name}", inventor.Id);
    }
    private int lifespanBonus;// = _inventions.Sum(i => i.LifespanBonus);
    private void StartWar()
    {
        if (_countries.Count < 2) return;
        
        var attacker = _countries[_random.Next(_countries.Count)];
        
        // Pick random defender without LINQ
        Country? defender = null;
        var potentialDefenders = new List<Country>();
        foreach (var country in _countries)
        {
            if (country.Id != attacker.Id)
                potentialDefenders.Add(country);
        }
        
        if (potentialDefenders.Count > 0)
            defender = potentialDefenders[_random.Next(potentialDefenders.Count)];
        
        if (defender == null) return;
        
        var war = new War
        {
            Id = _nextTempId--,
            Name = _nameGenerator.GenerateWarName(attacker.Name, defender.Name),
            StartDay = _currentDay,
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
            war.EndDay = _currentDay + _random.Next(30, 365);
            war.IsActive = false;
        }
        else if (defender.MilitaryStrength > attacker.MilitaryStrength * 1.5)
        {
            war.WinnerCountryId = defender.Id;
            war.EndDay = _currentDay + _random.Next(30, 365);
            war.IsActive = false;
        }
        
        LogEvent("War", $"War broke out: {war.Name}", countryId: attacker.Id);
    }
    
    private void SyncToDatabase()
    {
        try
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
        catch (Exception ex)
        {
            // Log error but don't stop simulation
            Console.WriteLine($"Database sync error: {ex.Message}");
        }
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
            // Calculate demographics in single pass - avoid multiple Count() calls
            int breedingFemales = 0;
            int youngFemales = 0;
            int breedingMales = 0;
            int youngMales = 0;
            int maleCount = 0;
            int femaleCount = 0;
            int marriedCount = 0;
            
            foreach (var person in livingPeople)
            {
                if (person is null) continue;
                int age = person.GetAge(_currentDay);
                
                if (person.Gender == "Male")
                {
                    maleCount++;
                    if (age >= 16 && age <= 60)
                        breedingMales++;
                    else if (age < 16)
                        youngMales++;
                }
                else // Female
                {
                    femaleCount++;
                    if (age >= 16 && age <= 45)
                        breedingFemales++;
                    else if (age < 16)
                        youngFemales++;
                }
                
                if (person.IsMarried)
                    marriedCount++;
            }
            
            if (breedingFemales == 0 && youngFemales == 0)
            {
                simulationEnded = true;
                endReason = "No females of breeding age remain and no young females to continue the population. The civilization has ended.";
            }
            else if (breedingFemales == 0 && youngFemales > 0)
            {
                if (breedingMales == 0 && youngMales == 0)
                {
                    simulationEnded = true;
                    endReason = "No males of breeding age remain and no young males to continue the population. The civilization will end.";
                }
            }
            
            return new SimulationStats
            {
                CurrentDay = _currentDay,
                CurrentYear = DayToYear(_currentDay),
                CurrentDayOfYear = DayOfYear(_currentDay),
                TotalPopulation = _people.Count,
                LivingPopulation = livingPeople.Count,
                TotalBirths = _people.Count,
                TotalDeaths = _people.Count - livingPeople.Count,
                TotalMarriages = marriedCount / 2,
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
                TopJobs = GetTopJobs(livingPeople),
                FamilyTrees = GetActiveFamilyTrees(livingPeople),
                Cities = _cities.OrderByDescending(c => c.Population).Take(10).Select(c => new CityInfo 
                { 
                    Name = c.Name, 
                    Population = c.Population, 
                    Year = DayToYear(c.FoundedDay) 
                }).ToList(),
                Countries = _countries.OrderByDescending(c => c.Population).Take(10).Select(c => new CountryInfo 
                { 
                    Name = c.Name, 
                    Population = c.Population, 
                    Year = DayToYear(c.FoundedDay) 
                }).ToList(),
                Religions = _religions.OrderByDescending(r => r.Followers).Take(10).Select(r => new ReligionInfo 
                { 
                    Name = r.Name, 
                    Followers = r.Followers, 
                    Year = DayToYear(r.FoundedDay) 
                }).ToList(),
                Inventions = _inventions.OrderBy(i => i.DiscoveredDay).Take(15).Select(i => new InventionInfo 
                { 
                    Name = i.Name, 
                    Category = i.Category, 
                    Year = DayToYear(i.DiscoveredDay) 
                }).ToList()
            };
        }
        
        // If we get here, all people died
        return new SimulationStats
        {
            CurrentDay = _currentDay,
            CurrentYear = DayToYear(_currentDay),
            CurrentDayOfYear = DayOfYear(_currentDay),
            TotalPopulation = _people.Count,
            LivingPopulation = 0,
            TotalBirths = _people.Count,
            TotalDeaths = _people.Count,
            TotalMarriages = 0,
            TotalCities = _cities.Count,
            TotalCountries = _countries.Count,
            TotalReligions = _religions.Count,
            TotalInventions = _inventions.Count,
            TotalWars = _wars.Count,
            GenerationNumber = _generationNumber,
            SimulationEnded = simulationEnded,
            EndReason = endReason,
            MaleCount = 0,
            FemaleCount = 0,
            RecentEvents = _recentEvents.TakeLast(10).ToList(),
            TopJobs = new List<JobStatistic>(),
            FamilyTrees = new List<FamilyTreeNode>(),
            Cities = new List<CityInfo>(),
            Countries = new List<CountryInfo>(),
            Religions = new List<ReligionInfo>(),
            Inventions = new List<InventionInfo>()
        };
    }
    
    private List<JobStatistic> GetTopJobs(List<Person> livingPeople)
    {
        // Group people by their job
        var jobGroups = new Dictionary<string, int>();
        
        foreach (var person in livingPeople)
        {
            if (person is null) continue;
            string jobName = !person.JobId.HasValue ? "Unemployed" 
                : (_jobsById.ContainsKey(person.JobId.Value) ? _jobsById[person.JobId.Value].Name : "Unknown");
            
            if (!jobGroups.ContainsKey(jobName))
                jobGroups[jobName] = 0;
            jobGroups[jobName]++;
        }
        
        // Convert to list and sort without LINQ
        var jobList = new List<JobStatistic>();
        foreach (var kvp in jobGroups)
        {
            jobList.Add(new JobStatistic { JobName = kvp.Key, Count = kvp.Value });
        }
        
        // Sort descending by count
        jobList.Sort((a, b) => b.Count.CompareTo(a.Count));
        
        // Take top 10
        var result = new List<JobStatistic>();
        int count = Math.Min(10, jobList.Count);
        for (int i = 0; i < count; i++)
        {
            result.Add(jobList[i]);
        }
        
        return result;
    }
    
    private List<FamilyTreeNode> GetActiveFamilyTrees(List<Person> livingPeople)
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
                trees.Add(BuildFamilyTreeNode(adam, livingPeople));
                return trees;
            }
        }
        
        // Fallback: Find male roots - people with no parents - avoid LINQ
        var roots = new List<Person>();
        foreach (var person in _people)
        {
            if (!person.FatherId.HasValue && !person.MotherId.HasValue && person.Gender == "Male")
                roots.Add(person);
        }
        
        // If no roots, find male children with most descendants - avoid LINQ
        if (roots.Count == 0)
        {
            var potentialRoots = new List<(Person person, int descendants)>();
            
            foreach (var person in livingPeople)
            {
                if (person.Gender == "Male" && HasLivingDescendants(person.Id, livingPeople))
                {
                    int descendants = CountLivingDescendants(person.Id);
                    potentialRoots.Add((person, descendants));
                }
            }
            
            // Sort by descendant count descending
            potentialRoots.Sort((a, b) => b.descendants.CompareTo(a.descendants));
            
            // Take top 10
            int count = Math.Min(10, potentialRoots.Count);
            for (int i = 0; i < count; i++)
            {
                roots.Add(potentialRoots[i].person);
            }
        }
        
        // Build tree for first root only (limit to 1 tree to prevent overflow)
        if (roots.Count > 0)
        {
            trees.Add(BuildFamilyTreeNode(roots[0], livingPeople));
        }
        
        return trees;
    }
    
    private bool HasLivingDescendants(long personId, List<Person> livingPeople)
    {
        // Check if any living person has this person as parent - using O(1) lookup
        var children = GetChildrenOfPerson(personId);
        
        foreach (var child in children)
        {
            if (child.IsAlive)
                return true;
            
            // Check descendants recursively
            if (HasLivingDescendants(child.Id, livingPeople))
                return true;
        }
        
        return false;
    }
    
    private int CountLivingDescendants(long personId)
    {
        // Get all children using O(1) lookup instead of LINQ
        var children = GetChildrenOfPerson(personId);
        
        int count = 0;
        
        // Count living children and descendants
        foreach (var child in children)
        {
            if (child.IsAlive)
                count++;
            
            // Count descendants recursively
            count += CountLivingDescendants(child.Id);
        }
        
        return count;
    }
    
    private FamilyTreeNode BuildFamilyTreeNode(Person person, List<Person> livingPeople)
    {
        var node = new FamilyTreeNode
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Gender = person.Gender,
            Age = person.GetAge(_currentDay),
            IsAlive = person.IsAlive,
            SpouseName = GetSpouseName(person),
            Children = new List<FamilyTreeNode>()
        };
        
        // Only add living children or children with living descendants - using O(1) lookup
        var allChildren = GetChildrenOfPerson(person.Id);
        var eligibleChildren = new List<Person>();
        
        // Filter children without LINQ
        foreach (var child in allChildren)
        {
            if (child.IsAlive || HasLivingDescendants(child.Id, livingPeople))
                eligibleChildren.Add(child);
        }
        
        // Sort: alive children first, then by birth date
        eligibleChildren.Sort((a, b) => 
        {
            if (a.IsAlive != b.IsAlive)
                return b.IsAlive.CompareTo(a.IsAlive); // alive first
            return a.BirthDay.CompareTo(b.BirthDay);
        });
        
        // Take first 10 children
        int childCount = Math.Min(10, eligibleChildren.Count);
        for (int i = 0; i < childCount; i++)
        {
            node.Children.Add(BuildFamilyTreeNode(eligibleChildren[i], livingPeople));
        }
        
        return node;
    }
    
    private string GetSpouseName(Person person)
    {
        if (!person.SpouseId.HasValue)
            return string.Empty;
        
        // Check living people first
        if (_peopleById.ContainsKey(person.SpouseId.Value))
        {
            var spouse = _peopleById[person.SpouseId.Value];
            return $"{spouse.FirstName} {spouse.LastName}";
        }
        
        // Check dead people
        if (_deadPeopleById.ContainsKey(person.SpouseId.Value))
        {
            var spouse = _deadPeopleById[person.SpouseId.Value];
            return $"{spouse.FirstName} {spouse.LastName} †";
        }
        
        return string.Empty;
    }
}

public class SimulationStats
{
    public int CurrentDay { get; set; } // Days since simulation start
    public int CurrentYear { get; set; } // Calculated year (day / 365 + 1)
    public int CurrentDayOfYear { get; set; } // Day within the year (1-365)
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
    public List<CityInfo> Cities { get; set; } = new();
    public List<CountryInfo> Countries { get; set; } = new();
    public List<ReligionInfo> Religions { get; set; } = new();
    public List<InventionInfo> Inventions { get; set; } = new();
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

public class CityInfo
{
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
    public int Year { get; set; }
}

public class CountryInfo
{
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
    public int Year { get; set; }
}

public class ReligionInfo
{
    public string Name { get; set; } = string.Empty;
    public int Followers { get; set; }
    public int Year { get; set; }
}

public class InventionInfo
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Year { get; set; }
}
