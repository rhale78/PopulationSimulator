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
    
    public Simulator()
    {
        _random = new Random();
        _nameGenerator = new NameGenerator(_random);
        _dataAccess = new DataAccessLayer();
        _currentDate = new DateTime(1, 1, 1); // Year 1
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
        
        // Create Adam and Eve with perfect traits, starting at age 20
        var adam = CreatePerson("Adam", "", "Male", null, null);
        adam.BirthDate = _currentDate.AddYears(-20); // Age 20
        adam.Intelligence = 100;
        adam.Strength = 100;
        adam.Health = 100;
        adam.Fertility = 100;
        adam.Charisma = 100;
        adam.Creativity = 100;
        adam.Leadership = 100;
        adam.Aggression = 50;
        adam.Wisdom = 100;
        adam.Beauty = 100;
        adam.Height = 180;
        
        var eve = CreatePerson("Eve", "", "Female", null, null);
        eve.BirthDate = _currentDate.AddYears(-20); // Age 20
        eve.Intelligence = 100;
        eve.Strength = 100;
        eve.Health = 100;
        eve.Fertility = 100;
        eve.Charisma = 100;
        eve.Creativity = 100;
        eve.Leadership = 100;
        eve.Aggression = 50;
        eve.Wisdom = 100;
        eve.Beauty = 100;
        eve.Height = 168;
        
        AddPerson(adam);
        AddPerson(eve);
        
        // Marry Adam and Eve
        MarryCouple(adam, eve);
        
        LogEvent("Birth", $"{adam.FirstName} was created (Age 20, Perfect traits)", adam.Id);
        LogEvent("Birth", $"{eve.FirstName} was created (Age 20, Perfect traits)", eve.Id);
        LogEvent("Marriage", $"{adam.FirstName} married {eve.FirstName}", adam.Id);
    }
    
    private void SeedJobs()
    {
        // Basic jobs available from the start
        var basicJobs = new (string, int, int, int, decimal, int, double, bool, string?)[]
        {
            ("Farmer", 20, 40, 12, 10m, 1, 1.0, false, null),
            ("Hunter", 30, 60, 14, 15m, 2, 1.5, false, null),
            ("Gatherer", 20, 30, 12, 8m, 1, 0.8, false, null),
            ("Builder", 30, 70, 16, 20m, 3, 1.3, false, null),
            ("Shepherd", 20, 40, 12, 10m, 1, 0.9, false, null),
            ("Fisherman", 25, 50, 14, 12m, 2, 1.2, false, null),
            ("Cook", 30, 30, 14, 12m, 2, 0.8, false, null),
            ("Servant", 15, 25, 12, 5m, 0, 0.9, false, null),
            ("Healer", 75, 20, 20, 40m, 6, 0.8, false, null),
            ("Leader", 75, 50, 25, 100m, 10, 1.0, false, null)
        };
        
        foreach (var (name, intel, str, age, salary, status, risk, reqInv, invName) in basicJobs)
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
                RequiresInvention = reqInv
            };
            _jobs.Add(job);
            _jobsById[job.Id] = job;
        }
        
        _dataAccess.SaveJobs(_jobs);
    }
    
    // Add jobs that require inventions or wars
    private void AddAdvancedJobs()
    {
        // Check which advanced jobs can now be added
        var advancedJobTemplates = new[]
        {
            ("Potter", 40, 40, 16, 20m, 3, 1.0, "Pottery"),
            ("Smith", 50, 70, 18, 30m, 5, 1.5, "Metallurgy"),
            ("Bronze Smith", 55, 75, 20, 35m, 5, 1.5, "Bronze Working"),
            ("Iron Smith", 60, 80, 20, 45m, 6, 1.5, "Iron Working"),
            ("Scribe", 80, 10, 20, 30m, 6, 0.6, "Writing"),
            ("Architect", 70, 50, 22, 50m, 7, 1.2, "Architecture"),
            ("Engineer", 80, 50, 24, 60m, 8, 1.3, "Engineering"),
            ("Mathematician", 90, 10, 22, 55m, 8, 0.6, "Mathematics"),
            ("Physician", 85, 20, 24, 70m, 8, 0.7, "Medicine"),
            ("Navigator", 75, 40, 22, 45m, 6, 1.3, "Navigation"),
            ("Astronomer", 85, 10, 24, 50m, 7, 0.6, "Astronomy"),
            ("Merchant", 60, 20, 18, 35m, 5, 0.9, "Currency"),
            ("Banker", 70, 10, 22, 60m, 7, 0.8, "Banking"),
            ("Cartographer", 75, 20, 20, 40m, 6, 0.7, "Cartography"),
            ("Glassmaker", 60, 60, 18, 35m, 5, 1.2, "Glassmaking"),
            ("Papermaker", 55, 50, 18, 30m, 4, 1.0, "Papermaking"),
            ("Sailor", 40, 60, 18, 25m, 4, 1.8, "Sailing"),
            ("Printer", 70, 40, 20, 45m, 6, 1.0, "Printing"),
            ("Artist", 65, 20, 18, 30m, 5, 0.7, "Painting"),
            ("Sculptor", 70, 50, 20, 35m, 5, 0.9, "Sculpture"),
            ("Musician", 60, 20, 16, 25m, 4, 0.7, "Music")
        };
        
        foreach (var (name, intel, str, age, salary, status, risk, requiredInvention) in advancedJobTemplates)
        {
            // Check if job already exists
            if (_jobs.Any(j => j.Name == name))
                continue;
            
            // Check if required invention exists
            var invention = _inventions.FirstOrDefault(i => i.Name == requiredInvention);
            if (invention == null)
                continue;
            
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
                RequiresInvention = true,
                RequiredInventionId = invention.Id
            };
            _jobs.Add(job);
            _jobsById[job.Id] = job;
        }
        
        // Add war-dependent jobs only when wars exist
        if (_wars.Any())
        {
            var warJobs = new[]
            {
                ("Warrior", 40, 80, 16, 20m, 5, 3.0),
                ("Guard", 35, 70, 18, 18m, 4, 1.8),
                ("General", 70, 70, 30, 80m, 9, 2.5),
                ("Cavalry", 45, 85, 20, 30m, 6, 3.5),
                ("Archer", 50, 70, 18, 25m, 5, 2.8),
                ("Siege Engineer", 75, 60, 25, 55m, 7, 2.0)
            };
            
            foreach (var (name, intel, str, age, salary, status, risk) in warJobs)
            {
                if (_jobs.Any(j => j.Name == name))
                    continue;
                
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
                    RequiresInvention = false
                };
                _jobs.Add(job);
                _jobsById[job.Id] = job;
            }
        }
        
        // Save new jobs to database
        if (_jobs.Any(j => j.Id < 0))
        {
            _dataAccess.SaveJobs(_jobs.Where(j => j.Id < 0).ToList());
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
        // Strong early population protection - protect until age 100 when population < 100
        int totalPeople = _people.Count(p => p.IsAlive);
        if (totalPeople < 100 && age < 100)
            return 0.00001; // Extremely low death rate for early population protection
        
        // Base death chance by age
        double baseChance = age < 1 ? 0.01 : // Reduced infant mortality
                           age < 5 ? 0.005 : // Reduced childhood mortality
                           age < 50 ? 0.0005 : // Very low prime years mortality
                           age < 70 ? 0.005 + (age - 50) * 0.001 : // Middle age
                           age < 100 ? 0.01 + (age - 70) * 0.005 : // Elderly
                           0.05 + (age - 100) * 0.02; // Very old age
        
        // Health modifier
        double healthMod = 1.0 - (person.Health / 200.0);
        
        // Job risk modifier
        double jobRisk = 1.0;
        if (person.JobId.HasValue && _jobsById.ContainsKey(person.JobId.Value))
        {
            jobRisk = _jobsById[person.JobId.Value].DeathRiskModifier;
        }
        
        // Invention effects (e.g., medicine extends life)
        double inventionMod = 1.0;
        if (_inventions.Any(i => i.Name == "Medicine" || i.Name == "Penicillin"))
        {
            inventionMod = 0.7; // 30% reduction in death rate
        }
        
        return baseChance * (1.0 + healthMod) * jobRisk * inventionMod;
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
        
        foreach (var person in eligiblePeople)
        {
            // Find suitable jobs
            var suitableJobs = _jobs.Where(j => 
                person.Intelligence >= j.MinIntelligence &&
                person.Strength >= j.MinStrength &&
                person.GetAge(_currentDate) >= j.MinAge &&
                (!j.RequiresInvention || (j.RequiredInventionId.HasValue && _inventionsById.ContainsKey(j.RequiredInventionId.Value)))
            ).ToList();
            
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
    
    private void ProcessPregnancies()
    {
        var marriedFemales = _people.Where(p => 
            p.CanHaveChildren(_currentDate)
        ).ToList();
        
        foreach (var female in marriedFemales)
        {
            // Early population boost
            int totalPeople = _people.Count(p => p.IsAlive);
            double pregnancyChance = totalPeople < 50 ? 0.1 : // 10% for very early
                                    totalPeople < 100 ? 0.05 : // 5% for early
                                    totalPeople < 500 ? 0.02 : // 2% for growing
                                    0.01; // 1% for established
            
            // Fertility modifier
            pregnancyChance *= (female.Fertility / 100.0);
            
            // Health modifier
            pregnancyChance *= (female.Health / 100.0);
            
            if (_random.NextDouble() < pregnancyChance)
            {
                female.IsPregnant = true;
                female.PregnancyDueDate = _currentDate.AddDays(270); // 9 months
                female.PregnancyFatherId = female.SpouseId;
                
                // Twins/triplets chance
                double multipleChance = _random.NextDouble();
                if (multipleChance < 0.01)
                    female.PregnancyMultiplier = 3; // Triplets 1%
                else if (multipleChance < 0.05)  // 4% for twins (5% - 1%)
                    female.PregnancyMultiplier = 2; // Twins 4%
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
                
                Person? father = mother.PregnancyFatherId.HasValue && _peopleById.ContainsKey(mother.PregnancyFatherId.Value) 
                    ? _peopleById[mother.PregnancyFatherId.Value] 
                    : null;
                
                string fatherName = father?.FirstName ?? "Unknown";
                string cityName = mother.CityId.HasValue && _citiesById.ContainsKey(mother.CityId.Value) 
                    ? _citiesById[mother.CityId.Value].Name 
                    : string.Empty;
                string jobName = father?.JobId.HasValue == true && _jobsById.ContainsKey(father.JobId.Value) 
                    ? _jobsById[father.JobId.Value].Name 
                    : string.Empty;
                
                // Calculate generation number properly
                int childGeneration = Math.Max(
                    father != null ? CountGenerations(father.Id) : 1,
                    CountGenerations(mother.Id)
                ) + 1;
                _generationNumber = Math.Max(_generationNumber, childGeneration);
                
                string lastName = _nameGenerator.GenerateLastName(fatherName, cityName, jobName, childGeneration);
                
                var child = CreatePerson(firstName, lastName, gender, mother.PregnancyFatherId, mother.Id);
                AddPerson(child);
                
                // Enhanced birth event with details
                string details = $"{child.FirstName} {child.LastName} was born to {mother.FirstName} {mother.LastName} " +
                                $"(Eyes: {child.EyeColor}, Hair: {child.HairColor}, Height: {child.Height}cm)";
                LogEvent("Birth", details, child.Id);
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
        
        // Add advanced jobs based on inventions and wars
        AddAdvancedJobs();
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
        
        // Comprehensive invention list with categories and requirements
        var possibleInventions = new[]
        {
            ("Fire", "Technology", 60, "Enables cooking and warmth"),
            ("Wheel", "Technology", 65, "Revolutionary transportation"),
            ("Writing", "Science", 70, "Record keeping and communication"),
            ("Agriculture", "Agriculture", 65, "Systematic food production"),
            ("Pottery", "Craft", 60, "Storage and cooking vessels"),
            ("Metallurgy", "Technology", 75, "Working with metals"),
            ("Bronze Working", "Technology", 78, "Advanced metalworking"),
            ("Iron Working", "Technology", 82, "Superior tools and weapons"),
            ("Architecture", "Technology", 75, "Advanced building techniques"),
            ("Mathematics", "Science", 80, "Numerical systems and calculation"),
            ("Medicine", "Medicine", 80, "Healing and treatment"),
            ("Penicillin", "Medicine", 90, "Antibiotic treatment - extends life"),
            ("Navigation", "Science", 75, "Sea and land travel"),
            ("Astronomy", "Science", 85, "Study of celestial bodies"),
            ("Engineering", "Science", 82, "Advanced construction"),
            ("Aqueducts", "Technology", 80, "Water transport systems"),
            ("Philosophy", "Science", 85, "Study of wisdom and ethics"),
            ("Literature", "Art", 70, "Written artistic works"),
            ("Music", "Art", 65, "Organized sound and rhythm"),
            ("Painting", "Art", 68, "Visual artistic expression"),
            ("Sculpture", "Art", 70, "Three-dimensional art"),
            ("Law Code", "Governance", 78, "Systematic legal framework"),
            ("Democracy", "Governance", 85, "Participatory government"),
            ("Currency", "Economy", 75, "Standardized exchange medium"),
            ("Banking", "Economy", 80, "Financial management"),
            ("Calendar", "Science", 72, "Time measurement system"),
            ("Printing", "Technology", 85, "Mass text reproduction"),
            ("Compass", "Technology", 76, "Directional navigation"),
            ("Gunpowder", "Technology", 88, "Explosive powder"),
            ("Telescope", "Science", 87, "Distant viewing"),
            ("Microscope", "Science", 87, "Minute viewing"),
            ("Steam Engine", "Technology", 90, "Mechanical power"),
            ("Glassmaking", "Craft", 72, "Transparent materials"),
            ("Papermaking", "Technology", 70, "Writing material"),
            ("Sailing", "Technology", 68, "Wind-powered transport"),
            ("Cartography", "Science", 75, "Map making")
        };
        
        // Filter to inventions not yet discovered
        var undiscovered = possibleInventions
            .Where(i => !_inventions.Any(inv => inv.Name == i.Item1) && inventor.Intelligence >= i.Item3)
            .ToList();
        
        if (!undiscovered.Any()) return;
        
        var selected = undiscovered[_random.Next(undiscovered.Count)];
        
        var invention = new Invention
        {
            Id = _nextTempId--,
            Name = selected.Item1,
            Description = selected.Item4,
            DiscoveredDate = _currentDate,
            InventorId = inventor.Id,
            RequiredIntelligence = selected.Item3,
            Category = selected.Item2
        };
        
        _inventions.Add(invention);
        _inventionsById[invention.Id] = invention;
        
        // Apply invention effects
        ApplyInventionEffects(invention);
        
        LogEvent("Invention", $"{inventor.FirstName} {inventor.LastName} discovered {invention.Name} - {invention.Description}", inventor.Id);
    }
    
    private void ApplyInventionEffects(Invention invention)
    {
        // Some inventions have global effects on the population
        switch (invention.Name)
        {
            case "Medicine":
            case "Penicillin":
                // Health boost for all living people
                foreach (var person in _people.Where(p => p.IsAlive))
                {
                    person.Health = Math.Min(100, person.Health + 5);
                }
                break;
            case "Agriculture":
                // Fertility boost from better nutrition
                foreach (var person in _people.Where(p => p.IsAlive))
                {
                    person.Fertility = Math.Min(100, person.Fertility + 3);
                }
                break;
            case "Philosophy":
                // Wisdom boost
                foreach (var person in _people.Where(p => p.IsAlive && p.Intelligence > 60))
                {
                    person.Wisdom = Math.Min(100, person.Wisdom + 2);
                }
                break;
        }
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
        // Calculate job statistics
        var jobStats = _people
            .Where(p => p.IsAlive && p.JobId.HasValue)
            .GroupBy(p => p.JobId!.Value)
            .Select(g => new JobStat
            {
                JobName = _jobsById.ContainsKey(g.Key) ? _jobsById[g.Key].Name : "Unknown",
                Count = g.Count()
            })
            .OrderByDescending(j => j.Count)
            .Take(10)
            .ToList();
        
        // Build family trees for most active families
        var familyTrees = BuildFamilyTrees();
        
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
            RecentEvents = _recentEvents.TakeLast(10).ToList(),
            TopJobs = jobStats,
            FamilyTrees = familyTrees
        };
    }
    
    private List<FamilyTree> BuildFamilyTrees()
    {
        var trees = new List<FamilyTree>();
        
        // Find people with most living descendants
        var livingPeople = _people.Where(p => p.IsAlive).ToList();
        var peopleWithDescendants = new Dictionary<long, int>();
        
        foreach (var person in livingPeople)
        {
            int descendants = CountLivingDescendants(person.Id);
            if (descendants > 0)
            {
                // Find root ancestor
                var root = FindRootAncestor(person.Id);
                if (!peopleWithDescendants.ContainsKey(root.Id))
                {
                    peopleWithDescendants[root.Id] = CountLivingDescendants(root.Id);
                }
            }
        }
        
        // Get top 3 families
        var topFamilies = peopleWithDescendants
            .OrderByDescending(kvp => kvp.Value)
            .Take(3)
            .Select(kvp => kvp.Key)
            .ToList();
        
        foreach (var rootId in topFamilies)
        {
            if (_peopleById.ContainsKey(rootId))
            {
                var root = _peopleById[rootId];
                trees.Add(new FamilyTree
                {
                    RootName = $"{root.FirstName} {root.LastName}",
                    RootAge = root.GetAge(_currentDate),
                    IsRootAlive = root.IsAlive,
                    LivingDescendants = CountLivingDescendants(rootId),
                    Members = BuildTreeMembers(rootId, 0)
                });
            }
        }
        
        return trees;
    }
    
    private Person FindRootAncestor(long personId)
    {
        if (!_peopleById.ContainsKey(personId))
            return _peopleById[personId];
        
        var person = _peopleById[personId];
        
        // If has father, go up
        if (person.FatherId.HasValue && _peopleById.ContainsKey(person.FatherId.Value))
        {
            return FindRootAncestor(person.FatherId.Value);
        }
        
        // If has mother, go up
        if (person.MotherId.HasValue && _peopleById.ContainsKey(person.MotherId.Value))
        {
            return FindRootAncestor(person.MotherId.Value);
        }
        
        return person;
    }
    
    private int CountLivingDescendants(long personId)
    {
        var descendants = _people.Where(p => p.IsAlive && 
            (p.FatherId == personId || p.MotherId == personId)).ToList();
        
        int count = descendants.Count;
        foreach (var descendant in descendants)
        {
            count += CountLivingDescendants(descendant.Id);
        }
        
        return count;
    }
    
    private List<TreeMember> BuildTreeMembers(long personId, int level)
    {
        var members = new List<TreeMember>();
        
        if (!_peopleById.ContainsKey(personId) || level > 6) // Limit depth
            return members;
        
        var person = _peopleById[personId];
        
        // Add this person
        members.Add(new TreeMember
        {
            Name = $"{person.FirstName} {person.LastName}",
            Age = person.GetAge(_currentDate),
            IsAlive = person.IsAlive,
            Level = level
        });
        
        // Add children (only if person is alive to keep tree focused)
        if (person.IsAlive)
        {
            var children = _people.Where(p => p.FatherId == personId || p.MotherId == personId).ToList();
            foreach (var child in children)
            {
                members.AddRange(BuildTreeMembers(child.Id, level + 1));
            }
        }
        
        return members;
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
    public List<JobStat> TopJobs { get; set; } = new();
    public List<FamilyTree> FamilyTrees { get; set; } = new();
}

public class JobStat
{
    public string JobName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class FamilyTree
{
    public string RootName { get; set; } = string.Empty;
    public int RootAge { get; set; }
    public bool IsRootAlive { get; set; }
    public int LivingDescendants { get; set; }
    public List<TreeMember> Members { get; set; } = new();
}

public class TreeMember
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsAlive { get; set; }
    public int Level { get; set; }
}
