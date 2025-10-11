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
        _currentDate = new DateTime(100, 1, 1); // Year 100 to avoid underflow
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
        
        // Create Adam and Eve with idealized traits
        var adam = CreatePerson("Adam", "", "Male", null, null);
        adam.Intelligence = 90;
        adam.Strength = 90;
        adam.Health = 100;
        adam.Fertility = 95;
        adam.Charisma = 85;
        adam.Creativity = 80;
        adam.Leadership = 90;
        adam.Aggression = 50;
        adam.Wisdom = 85;
        adam.Beauty = 80;
        adam.Height = 180;
        
        var eve = CreatePerson("Eve", "", "Female", null, null);
        eve.Intelligence = 90;
        eve.Strength = 70;
        eve.Health = 100;
        eve.Fertility = 100;
        eve.Charisma = 90;
        eve.Creativity = 85;
        eve.Leadership = 80;
        eve.Aggression = 40;
        eve.Wisdom = 90;
        eve.Beauty = 95;
        eve.Height = 168;
        
        AddPerson(adam);
        AddPerson(eve);
        
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
            ("Farmer", 20, 40, 12, 10m, 1, 1.0),
            ("Hunter", 30, 60, 14, 15m, 2, 1.5),
            ("Gatherer", 20, 30, 12, 8m, 1, 0.8),
            ("Builder", 30, 70, 16, 20m, 3, 1.3),
            ("Craftsman", 50, 40, 16, 25m, 4, 1.0),
            ("Merchant", 60, 20, 18, 35m, 5, 0.9),
            ("Priest", 70, 10, 20, 30m, 7, 0.7),
            ("Warrior", 40, 80, 16, 20m, 5, 3.0),
            ("Healer", 75, 20, 20, 40m, 6, 0.8),
            ("Scholar", 85, 10, 22, 35m, 7, 0.6),
            ("Artist", 60, 20, 18, 25m, 4, 0.7),
            ("Miner", 25, 85, 16, 25m, 3, 2.5),
            ("Fisherman", 25, 50, 14, 12m, 2, 1.2),
            ("Shepherd", 20, 40, 12, 10m, 1, 0.9),
            ("Smith", 50, 70, 18, 30m, 5, 1.5),
            ("Guard", 35, 70, 18, 18m, 4, 1.8),
            ("Scribe", 80, 10, 20, 30m, 6, 0.6),
            ("Cook", 30, 30, 14, 12m, 2, 0.8),
            ("Servant", 15, 25, 12, 5m, 0, 0.9),
            ("Leader", 75, 50, 25, 100m, 10, 1.0)
        };
        
        foreach (var (name, intel, str, age, salary, status, risk) in jobsData)
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
                RequiresInvention = false
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
        // Early population protection
        int totalPeople = _people.Count(p => p.IsAlive);
        if (totalPeople < 100 && age < 50)
            return 0.0001; // Very low death rate for early population
        
        // Base death chance by age
        double baseChance = age < 1 ? 0.05 : // Infant mortality
                           age < 5 ? 0.01 : // Childhood
                           age < 50 ? 0.001 : // Prime years
                           age < 70 ? 0.01 + (age - 50) * 0.002 : // Middle age
                           0.05 + (age - 70) * 0.01; // Old age
        
        // Health modifier
        double healthMod = 1.0 - (person.Health / 200.0);
        
        // Job risk modifier
        double jobRisk = 1.0;
        if (person.JobId.HasValue && _jobsById.ContainsKey(person.JobId.Value))
        {
            jobRisk = _jobsById[person.JobId.Value].DeathRiskModifier;
        }
        
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
            // Significantly increased pregnancy chances to boost population growth
            int totalPeople = _people.Count(p => p.IsAlive);
            double pregnancyChance = totalPeople < 50 ? 0.20 : // 20% for very early - much higher
                                    totalPeople < 150 ? 0.15 : // 15% for early - higher to get past 100
                                    totalPeople < 300 ? 0.10 : // 10% for mid growth
                                    totalPeople < 500 ? 0.05 : // 5% for growing
                                    0.02; // 2% for established
            
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
                    female.PregnancyMultiplier = 3; // Triplets
                else if (multipleChance < 0.05)
                    female.PregnancyMultiplier = 2; // Twins
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
                if (mother.PregnancyFatherId.HasValue)
                {
                    father = _peopleById.ContainsKey(mother.PregnancyFatherId.Value) 
                        ? _peopleById[mother.PregnancyFatherId.Value] 
                        : null;
                }
                
                // If no father found, use spouse as fallback
                if (father == null && mother.SpouseId.HasValue && _peopleById.ContainsKey(mother.SpouseId.Value))
                {
                    father = _peopleById[mother.SpouseId.Value];
                }
                
                string fatherName = father?.FirstName ?? "Adam"; // Fallback to Adam for very early generations
                long? fatherId = father?.Id;
                string cityName = mother.CityId.HasValue && _citiesById.ContainsKey(mother.CityId.Value) 
                    ? _citiesById[mother.CityId.Value].Name 
                    : string.Empty;
                string jobName = father?.JobId.HasValue == true && _jobsById.ContainsKey(father.JobId.Value) 
                    ? _jobsById[father.JobId.Value].Name 
                    : string.Empty;
                
                _generationNumber = Math.Max(_generationNumber, CountGenerations(mother.Id) + 1);
                string lastName = _nameGenerator.GenerateLastName(fatherName, cityName, jobName, _generationNumber);
                
                var child = CreatePerson(firstName, lastName, gender, fatherId, mother.Id);
                AddPerson(child);
                
                LogEvent("Birth", $"{child.FirstName} {child.LastName} was born to {mother.FirstName} {mother.LastName}", child.Id);
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
        
        string[] inventionNames = { "Wheel", "Writing", "Agriculture", "Metallurgy", "Architecture", 
            "Mathematics", "Medicine", "Navigation", "Astronomy", "Engineering" };
        string[] categories = { "Technology", "Science", "Art", "Agriculture", "Medicine" };
        
        var invention = new Invention
        {
            Id = _nextTempId--,
            Name = inventionNames[_random.Next(inventionNames.Length)],
            Description = "A groundbreaking discovery",
            DiscoveredDate = _currentDate,
            InventorId = inventor.Id,
            RequiredIntelligence = 70,
            Category = categories[_random.Next(categories.Length)]
        };
        
        _inventions.Add(invention);
        _inventionsById[invention.Id] = invention;
        
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
