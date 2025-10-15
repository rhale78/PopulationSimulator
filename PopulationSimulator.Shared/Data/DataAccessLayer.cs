using Microsoft.Data.Sqlite;
using PopulationSimulator.Models;

namespace PopulationSimulator.Data;

public class DataAccessLayer
{
    private readonly string _connectionString;
    
    public DataAccessLayer(string databasePath = "population.db")
    {
        _connectionString = $"Data Source={databasePath}";
    }
    
    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS People (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                Gender TEXT NOT NULL,
                BirthDay INTEGER NOT NULL,
                DeathDay INTEGER,
                IsAlive INTEGER NOT NULL,
                FatherId INTEGER,
                MotherId INTEGER,
                SpouseId INTEGER,
                SecondarySpouseId INTEGER,
                MarriageDay INTEGER,
                CityId INTEGER,
                CountryId INTEGER,
                ReligionId INTEGER,
                JobId INTEGER,
                JobStartDay INTEGER,
                Intelligence INTEGER NOT NULL,
                Strength INTEGER NOT NULL,
                Health INTEGER NOT NULL,
                Fertility INTEGER NOT NULL,
                Charisma INTEGER NOT NULL,
                Creativity INTEGER NOT NULL,
                Leadership INTEGER NOT NULL,
                Aggression INTEGER NOT NULL,
                Wisdom INTEGER NOT NULL,
                Beauty INTEGER NOT NULL,
                EyeColor TEXT NOT NULL,
                HairColor TEXT NOT NULL,
                Height INTEGER NOT NULL,
                IsPregnant INTEGER NOT NULL,
                PregnancyDueDay INTEGER,
                PregnancyFatherId INTEGER,
                PregnancyMultiplier INTEGER NOT NULL,
                DynastyId INTEGER,
                IsRuler INTEGER NOT NULL,
                Wealth REAL NOT NULL,
                SocialStatus INTEGER NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Cities (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                CountryId INTEGER,
                FoundedDay INTEGER NOT NULL,
                Population INTEGER NOT NULL,
                FounderId INTEGER,
                Wealth REAL NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Countries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                FoundedDay INTEGER NOT NULL,
                RulerId INTEGER,
                CapitalCityId INTEGER,
                Population INTEGER NOT NULL,
                Wealth REAL NOT NULL,
                MilitaryStrength INTEGER NOT NULL,
                GovernmentType TEXT NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Religions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                FoundedDay INTEGER NOT NULL,
                FounderId INTEGER,
                Followers INTEGER NOT NULL,
                Beliefs TEXT NOT NULL,
                AllowsPolygamy INTEGER NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Jobs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                MinIntelligence INTEGER NOT NULL,
                MinStrength INTEGER NOT NULL,
                MinAge INTEGER NOT NULL,
                BaseSalary REAL NOT NULL,
                SocialStatusBonus INTEGER NOT NULL,
                DeathRiskModifier REAL NOT NULL,
                RequiresInvention INTEGER NOT NULL,
                RequiredInventionId INTEGER
            );
            
            CREATE TABLE IF NOT EXISTS Inventions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                DiscoveredDay INTEGER NOT NULL,
                InventorId INTEGER,
                RequiredIntelligence INTEGER NOT NULL,
                Category TEXT NOT NULL,
                HealthBonus INTEGER NOT NULL DEFAULT 0,
                LifespanBonus INTEGER NOT NULL DEFAULT 0
            );
            
            CREATE TABLE IF NOT EXISTS Wars (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                StartDay INTEGER NOT NULL,
                EndDay INTEGER,
                AttackerCountryId INTEGER NOT NULL,
                DefenderCountryId INTEGER NOT NULL,
                WinnerCountryId INTEGER,
                Casualties INTEGER NOT NULL,
                IsActive INTEGER NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Events (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Day INTEGER NOT NULL,
                EventType TEXT NOT NULL,
                Description TEXT NOT NULL,
                PersonId INTEGER,
                CityId INTEGER,
                CountryId INTEGER,
                ReligionId INTEGER,
                WarId INTEGER,
                InventionId INTEGER
            );
            
            CREATE TABLE IF NOT EXISTS Dynasties (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                FounderId INTEGER NOT NULL,
                FoundedDay INTEGER NOT NULL,
                CurrentRulerId INTEGER,
                MemberCount INTEGER NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Laws (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                CountryId INTEGER,
                ReligionId INTEGER,
                EnactedDay INTEGER NOT NULL,
                Category TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
        
        // Migrate existing Inventions table to add HealthBonus and LifespanBonus if they don't exist
        try
        {
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = "PRAGMA table_info(Inventions)";
            var reader = checkCommand.ExecuteReader();
            bool hasHealthBonus = false;
            bool hasLifespanBonus = false;
            
            while (reader.Read())
            {
                string columnName = reader.GetString(1);
                if (columnName == "HealthBonus") hasHealthBonus = true;
                if (columnName == "LifespanBonus") hasLifespanBonus = true;
            }
            reader.Close();
            
            // Add missing columns if needed
            if (!hasHealthBonus)
            {
                var alterCommand = connection.CreateCommand();
                alterCommand.CommandText = "ALTER TABLE Inventions ADD COLUMN HealthBonus INTEGER NOT NULL DEFAULT 0";
                alterCommand.ExecuteNonQuery();
            }
            
            if (!hasLifespanBonus)
            {
                var alterCommand = connection.CreateCommand();
                alterCommand.CommandText = "ALTER TABLE Inventions ADD COLUMN LifespanBonus INTEGER NOT NULL DEFAULT 0";
                alterCommand.ExecuteNonQuery();
            }
        }
        catch
        {
            // Ignore errors - table might not exist yet or columns might already be there
        }
    }
    
    public void SavePeople(List<Person> people)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        
        foreach (var person in people)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            
            if (person.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO People (FirstName, LastName, Gender, BirthDay, DeathDay, IsAlive,
                        FatherId, MotherId, SpouseId, SecondarySpouseId, MarriageDay,
                        CityId, CountryId, ReligionId, JobId, JobStartDay,
                        Intelligence, Strength, Health, Fertility, Charisma, Creativity,
                        Leadership, Aggression, Wisdom, Beauty, EyeColor, HairColor, Height,
                        IsPregnant, PregnancyDueDay, PregnancyFatherId, PregnancyMultiplier,
                        DynastyId, IsRuler, Wealth, SocialStatus)
                    VALUES (@FirstName, @LastName, @Gender, @BirthDay, @DeathDay, @IsAlive,
                        @FatherId, @MotherId, @SpouseId, @SecondarySpouseId, @MarriageDay,
                        @CityId, @CountryId, @ReligionId, @JobId, @JobStartDay,
                        @Intelligence, @Strength, @Health, @Fertility, @Charisma, @Creativity,
                        @Leadership, @Aggression, @Wisdom, @Beauty, @EyeColor, @HairColor, @Height,
                        @IsPregnant, @PregnancyDueDay, @PregnancyFatherId, @PregnancyMultiplier,
                        @DynastyId, @IsRuler, @Wealth, @SocialStatus)
                ";
            }
            else
            {
                command.CommandText = @"
                    UPDATE People SET FirstName=@FirstName, LastName=@LastName, Gender=@Gender,
                        BirthDay=@BirthDay, DeathDay=@DeathDay, IsAlive=@IsAlive,
                        FatherId=@FatherId, MotherId=@MotherId, SpouseId=@SpouseId,
                        SecondarySpouseId=@SecondarySpouseId, MarriageDay=@MarriageDay,
                        CityId=@CityId, CountryId=@CountryId, ReligionId=@ReligionId,
                        JobId=@JobId, JobStartDay=@JobStartDay,
                        Intelligence=@Intelligence, Strength=@Strength, Health=@Health,
                        Fertility=@Fertility, Charisma=@Charisma, Creativity=@Creativity,
                        Leadership=@Leadership, Aggression=@Aggression, Wisdom=@Wisdom,
                        Beauty=@Beauty, EyeColor=@EyeColor, HairColor=@HairColor, Height=@Height,
                        IsPregnant=@IsPregnant, PregnancyDueDay=@PregnancyDueDay,
                        PregnancyFatherId=@PregnancyFatherId, PregnancyMultiplier=@PregnancyMultiplier,
                        DynastyId=@DynastyId, IsRuler=@IsRuler, Wealth=@Wealth, SocialStatus=@SocialStatus
                    WHERE Id=@Id
                ";
                command.Parameters.AddWithValue("@Id", person.Id);
            }
            
            AddPersonParameters(command, person);
            command.ExecuteNonQuery();
            
            if (person.Id <= 0)
            {
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                person.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
        
        transaction.Commit();
    }
    
    private void AddPersonParameters(SqliteCommand command, Person person)
    {
        command.Parameters.AddWithValue("@FirstName", person.FirstName);
        command.Parameters.AddWithValue("@LastName", person.LastName);
        command.Parameters.AddWithValue("@Gender", person.Gender);
        command.Parameters.AddWithValue("@BirthDay", person.BirthDay);
        command.Parameters.AddWithValue("@DeathDay", person.DeathDay.HasValue ? (object)person.DeathDay.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@IsAlive", person.IsAlive ? 1 : 0);
        command.Parameters.AddWithValue("@FatherId", person.FatherId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@MotherId", person.MotherId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@SpouseId", person.SpouseId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@SecondarySpouseId", person.SecondarySpouseId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@MarriageDay", person.MarriageDay.HasValue ? (object)person.MarriageDay.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@CityId", person.CityId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CountryId", person.CountryId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@ReligionId", person.ReligionId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@JobId", person.JobId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@JobStartDay", person.JobStartDay.HasValue ? (object)person.JobStartDay.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@Intelligence", person.Intelligence);
        command.Parameters.AddWithValue("@Strength", person.Strength);
        command.Parameters.AddWithValue("@Health", person.Health);
        command.Parameters.AddWithValue("@Fertility", person.Fertility);
        command.Parameters.AddWithValue("@Charisma", person.Charisma);
        command.Parameters.AddWithValue("@Creativity", person.Creativity);
        command.Parameters.AddWithValue("@Leadership", person.Leadership);
        command.Parameters.AddWithValue("@Aggression", person.Aggression);
        command.Parameters.AddWithValue("@Wisdom", person.Wisdom);
        command.Parameters.AddWithValue("@Beauty", person.Beauty);
        command.Parameters.AddWithValue("@EyeColor", person.EyeColor);
        command.Parameters.AddWithValue("@HairColor", person.HairColor);
        command.Parameters.AddWithValue("@Height", person.Height);
        command.Parameters.AddWithValue("@IsPregnant", person.IsPregnant ? 1 : 0);
        command.Parameters.AddWithValue("@PregnancyDueDay", person.PregnancyDueDay.HasValue ? (object)person.PregnancyDueDay.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@PregnancyFatherId", person.PregnancyFatherId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@PregnancyMultiplier", person.PregnancyMultiplier);
        command.Parameters.AddWithValue("@DynastyId", person.DynastyId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@IsRuler", person.IsRuler ? 1 : 0);
        command.Parameters.AddWithValue("@Wealth", person.Wealth);
        command.Parameters.AddWithValue("@SocialStatus", person.SocialStatus);
    }
    
    public void SaveJobs(List<Job> jobs)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var job in jobs)
        {
            var command = connection.CreateCommand();
            if (job.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO Jobs (Name, MinIntelligence, MinStrength, MinAge, BaseSalary,
                        SocialStatusBonus, DeathRiskModifier, RequiresInvention, RequiredInventionId)
                    VALUES (@Name, @MinIntelligence, @MinStrength, @MinAge, @BaseSalary,
                        @SocialStatusBonus, @DeathRiskModifier, @RequiresInvention, @RequiredInventionId)
                ";
                command.Parameters.AddWithValue("@Name", job.Name);
                command.Parameters.AddWithValue("@MinIntelligence", job.MinIntelligence);
                command.Parameters.AddWithValue("@MinStrength", job.MinStrength);
                command.Parameters.AddWithValue("@MinAge", job.MinAge);
                command.Parameters.AddWithValue("@BaseSalary", job.BaseSalary);
                command.Parameters.AddWithValue("@SocialStatusBonus", job.SocialStatusBonus);
                command.Parameters.AddWithValue("@DeathRiskModifier", job.DeathRiskModifier);
                command.Parameters.AddWithValue("@RequiresInvention", job.RequiresInvention ? 1 : 0);
                command.Parameters.AddWithValue("@RequiredInventionId", job.RequiredInventionId ?? (object)DBNull.Value);
                command.ExecuteNonQuery();
                
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                job.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveCities(List<City> cities)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var city in cities)
        {
            var command = connection.CreateCommand();
            if (city.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO Cities (Name, CountryId, FoundedDay, Population, FounderId, Wealth)
                    VALUES (@Name, @CountryId, @FoundedDay, @Population, @FounderId, @Wealth)
                ";
            }
            else
            {
                command.CommandText = @"
                    UPDATE Cities SET Name=@Name, CountryId=@CountryId, FoundedDay=@FoundedDay,
                        Population=@Population, FounderId=@FounderId, Wealth=@Wealth
                    WHERE Id=@Id
                ";
                command.Parameters.AddWithValue("@Id", city.Id);
            }
            
            command.Parameters.AddWithValue("@Name", city.Name);
            command.Parameters.AddWithValue("@CountryId", city.CountryId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FoundedDay", city.FoundedDay);
            command.Parameters.AddWithValue("@Population", city.Population);
            command.Parameters.AddWithValue("@FounderId", city.FounderId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Wealth", city.Wealth);
            command.ExecuteNonQuery();
            
            if (city.Id <= 0)
            {
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                city.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveCountries(List<Country> countries)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var country in countries)
        {
            var command = connection.CreateCommand();
            if (country.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO Countries (Name, FoundedDay, RulerId, CapitalCityId, Population, Wealth, MilitaryStrength, GovernmentType)
                    VALUES (@Name, @FoundedDay, @RulerId, @CapitalCityId, @Population, @Wealth, @MilitaryStrength, @GovernmentType)
                ";
            }
            else
            {
                command.CommandText = @"
                    UPDATE Countries SET Name=@Name, FoundedDay=@FoundedDay, RulerId=@RulerId,
                        CapitalCityId=@CapitalCityId, Population=@Population, Wealth=@Wealth,
                        MilitaryStrength=@MilitaryStrength, GovernmentType=@GovernmentType
                    WHERE Id=@Id
                ";
                command.Parameters.AddWithValue("@Id", country.Id);
            }
            
            command.Parameters.AddWithValue("@Name", country.Name);
            command.Parameters.AddWithValue("@FoundedDay", country.FoundedDay);
            command.Parameters.AddWithValue("@RulerId", country.RulerId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CapitalCityId", country.CapitalCityId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Population", country.Population);
            command.Parameters.AddWithValue("@Wealth", country.Wealth);
            command.Parameters.AddWithValue("@MilitaryStrength", country.MilitaryStrength);
            command.Parameters.AddWithValue("@GovernmentType", country.GovernmentType);
            command.ExecuteNonQuery();
            
            if (country.Id <= 0)
            {
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                country.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveReligions(List<Religion> religions)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var religion in religions)
        {
            var command = connection.CreateCommand();
            if (religion.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO Religions (Name, FoundedDay, FounderId, Followers, Beliefs, AllowsPolygamy)
                    VALUES (@Name, @FoundedDay, @FounderId, @Followers, @Beliefs, @AllowsPolygamy)
                ";
            }
            else
            {
                command.CommandText = @"
                    UPDATE Religions SET Name=@Name, FoundedDay=@FoundedDay, FounderId=@FounderId,
                        Followers=@Followers, Beliefs=@Beliefs, AllowsPolygamy=@AllowsPolygamy
                    WHERE Id=@Id
                ";
                command.Parameters.AddWithValue("@Id", religion.Id);
            }
            
            command.Parameters.AddWithValue("@Name", religion.Name);
            command.Parameters.AddWithValue("@FoundedDay", religion.FoundedDay);
            command.Parameters.AddWithValue("@FounderId", religion.FounderId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Followers", religion.Followers);
            command.Parameters.AddWithValue("@Beliefs", religion.Beliefs);
            command.Parameters.AddWithValue("@AllowsPolygamy", religion.AllowsPolygamy ? 1 : 0);
            command.ExecuteNonQuery();
            
            if (religion.Id <= 0)
            {
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                religion.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveEvents(List<Event> events)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var evt in events)
        {
            if (evt.Id <= 0)
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Events (Day, EventType, Description, PersonId, CityId, CountryId, ReligionId, WarId, InventionId)
                    VALUES (@Day, @EventType, @Description, @PersonId, @CityId, @CountryId, @ReligionId, @WarId, @InventionId)
                ";
                command.Parameters.AddWithValue("@Day", evt.Day);
                command.Parameters.AddWithValue("@EventType", evt.EventType);
                command.Parameters.AddWithValue("@Description", evt.Description);
                command.Parameters.AddWithValue("@PersonId", evt.PersonId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CityId", evt.CityId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CountryId", evt.CountryId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ReligionId", evt.ReligionId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@WarId", evt.WarId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@InventionId", evt.InventionId ?? (object)DBNull.Value);
                command.ExecuteNonQuery();
                
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                evt.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveInventions(List<Invention> inventions)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var invention in inventions)
        {
            if (invention.Id <= 0)
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Inventions (Name, Description, DiscoveredDay, InventorId, RequiredIntelligence, Category, HealthBonus, LifespanBonus)
                    VALUES (@Name, @Description, @DiscoveredDay, @InventorId, @RequiredIntelligence, @Category, @HealthBonus, @LifespanBonus)
                ";
                command.Parameters.AddWithValue("@Name", invention.Name);
                command.Parameters.AddWithValue("@Description", invention.Description);
                command.Parameters.AddWithValue("@DiscoveredDay", invention.DiscoveredDay);
                command.Parameters.AddWithValue("@InventorId", invention.InventorId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RequiredIntelligence", invention.RequiredIntelligence);
                command.Parameters.AddWithValue("@Category", invention.Category);
                command.Parameters.AddWithValue("@HealthBonus", invention.HealthBonus);
                command.Parameters.AddWithValue("@LifespanBonus", invention.LifespanBonus);
                command.ExecuteNonQuery();
                
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                invention.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveWars(List<War> wars)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var war in wars)
        {
            var command = connection.CreateCommand();
            if (war.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO Wars (Name, StartDay, EndDay, AttackerCountryId, DefenderCountryId, WinnerCountryId, Casualties, IsActive)
                    VALUES (@Name, @StartDay, @EndDay, @AttackerCountryId, @DefenderCountryId, @WinnerCountryId, @Casualties, @IsActive)
                ";
            }
            else
            {
                command.CommandText = @"
                    UPDATE Wars SET Name=@Name, StartDay=@StartDay, EndDay=@EndDay,
                        AttackerCountryId=@AttackerCountryId, DefenderCountryId=@DefenderCountryId,
                        WinnerCountryId=@WinnerCountryId, Casualties=@Casualties, IsActive=@IsActive
                    WHERE Id=@Id
                ";
                command.Parameters.AddWithValue("@Id", war.Id);
            }
            
            command.Parameters.AddWithValue("@Name", war.Name);
            command.Parameters.AddWithValue("@StartDay", war.StartDay);
            command.Parameters.AddWithValue("@EndDay", war.EndDay.HasValue ? (object)war.EndDay.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@AttackerCountryId", war.AttackerCountryId);
            command.Parameters.AddWithValue("@DefenderCountryId", war.DefenderCountryId);
            command.Parameters.AddWithValue("@WinnerCountryId", war.WinnerCountryId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Casualties", war.Casualties);
            command.Parameters.AddWithValue("@IsActive", war.IsActive ? 1 : 0);
            command.ExecuteNonQuery();
            
            if (war.Id <= 0)
            {
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                war.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void SaveDynasties(List<Dynasty> dynasties)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        foreach (var dynasty in dynasties)
        {
            var command = connection.CreateCommand();
            if (dynasty.Id <= 0)
            {
                command.CommandText = @"
                    INSERT INTO Dynasties (Name, FounderId, FoundedDay, CurrentRulerId, MemberCount)
                    VALUES (@Name, @FounderId, @FoundedDay, @CurrentRulerId, @MemberCount)
                ";
            }
            else
            {
                command.CommandText = @"
                    UPDATE Dynasties SET Name=@Name, FounderId=@FounderId, FoundedDay=@FoundedDay,
                        CurrentRulerId=@CurrentRulerId, MemberCount=@MemberCount
                    WHERE Id=@Id
                ";
                command.Parameters.AddWithValue("@Id", dynasty.Id);
            }
            
            command.Parameters.AddWithValue("@Name", dynasty.Name);
            command.Parameters.AddWithValue("@FounderId", dynasty.FounderId);
            command.Parameters.AddWithValue("@FoundedDay", dynasty.FoundedDay);
            command.Parameters.AddWithValue("@CurrentRulerId", dynasty.CurrentRulerId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MemberCount", dynasty.MemberCount);
            command.ExecuteNonQuery();
            
            if (dynasty.Id <= 0)
            {
                var idCommand = connection.CreateCommand();
                idCommand.CommandText = "SELECT last_insert_rowid()";
                dynasty.Id = (long)idCommand.ExecuteScalar()!;
            }
        }
    }
    
    public void ClearDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM People;
            DELETE FROM Cities;
            DELETE FROM Countries;
            DELETE FROM Religions;
            DELETE FROM Jobs;
            DELETE FROM Inventions;
            DELETE FROM Wars;
            DELETE FROM Events;
            DELETE FROM Dynasties;
            DELETE FROM Laws;
        ";
        command.ExecuteNonQuery();
    }
}
