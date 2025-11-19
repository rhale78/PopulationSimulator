using Microsoft.Data.SqlClient;
using PopulationSimulator.Models;
using System.Data;

namespace PopulationSimulator.Data;

public class DataAccessLayer
{
    private readonly string _connectionString;

    public DataAccessLayer(string? connectionString = null)
    {
        // Default to LocalDB if no connection string provided
        _connectionString = connectionString ??
            @"Server=(localdb)\MSSQLLocalDB;Database=PopulationSimulator;Integrated Security=true;TrustServerCertificate=true;";
    }

    public void InitializeDatabase()
    {
        // First, ensure the database exists
        EnsureDatabaseExists();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='People' AND xtype='U')
            CREATE TABLE People (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                FirstName NVARCHAR(100) NOT NULL,
                LastName NVARCHAR(100) NOT NULL,
                Gender NVARCHAR(10) NOT NULL,
                BirthDate DATETIME2 NOT NULL,
                DeathDate DATETIME2 NULL,
                IsAlive BIT NOT NULL,
                CauseOfDeath NVARCHAR(200) NULL,
                FatherId BIGINT NULL,
                MotherId BIGINT NULL,
                SpouseId BIGINT NULL,
                SecondarySpouseId BIGINT NULL,
                MarriageDate DATETIME2 NULL,
                CityId BIGINT NULL,
                CountryId BIGINT NULL,
                ReligionId BIGINT NULL,
                JobId BIGINT NULL,
                JobStartDate DATETIME2 NULL,
                Intelligence INT NOT NULL,
                Strength INT NOT NULL,
                Health INT NOT NULL,
                Fertility INT NOT NULL,
                Charisma INT NOT NULL,
                Creativity INT NOT NULL,
                Leadership INT NOT NULL,
                Aggression INT NOT NULL,
                Wisdom INT NOT NULL,
                Beauty INT NOT NULL,
                DNASequence NVARCHAR(50) NOT NULL DEFAULT '',
                BloodType NVARCHAR(10) NOT NULL DEFAULT '',
                GeneticMarkers NVARCHAR(200) NOT NULL DEFAULT '',
                HereditaryConditions NVARCHAR(500) NOT NULL DEFAULT 'None',
                HasHereditaryDisease BIT NOT NULL DEFAULT 0,
                DiseaseResistance INT NOT NULL DEFAULT 50,
                Longevity INT NOT NULL DEFAULT 50,
                EyeColor NVARCHAR(50) NOT NULL,
                HairColor NVARCHAR(50) NOT NULL,
                SkinTone NVARCHAR(50) NOT NULL DEFAULT '',
                Height INT NOT NULL,
                Weight INT NOT NULL DEFAULT 70,
                BuildType NVARCHAR(50) NOT NULL DEFAULT 'Average',
                IsPregnant BIT NOT NULL,
                PregnancyDueDate DATETIME2 NULL,
                PregnancyFatherId BIGINT NULL,
                PregnancyMultiplier INT NOT NULL,
                TotalChildren INT NOT NULL DEFAULT 0,
                DynastyId BIGINT NULL,
                IsRuler BIT NOT NULL,
                GenerationNumber INT NOT NULL DEFAULT 0,
                Wealth DECIMAL(18,2) NOT NULL,
                SocialStatus INT NOT NULL,
                IsNotable BIT NOT NULL DEFAULT 0,
                NotableFor NVARCHAR(500) NOT NULL DEFAULT '',
                ChildrenBorn INT NOT NULL DEFAULT 0,
                DescendantCount INT NOT NULL DEFAULT 0
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cities' AND xtype='U')
            CREATE TABLE Cities (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                CountryId BIGINT NULL,
                FoundedDate DATETIME2 NOT NULL,
                Population INT NOT NULL,
                FounderId BIGINT NULL,
                Wealth DECIMAL(18,2) NOT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Countries' AND xtype='U')
            CREATE TABLE Countries (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                FoundedDate DATETIME2 NOT NULL,
                RulerId BIGINT NULL,
                CapitalCityId BIGINT NULL,
                Population INT NOT NULL,
                Wealth DECIMAL(18,2) NOT NULL,
                MilitaryStrength INT NOT NULL,
                GovernmentType NVARCHAR(50) NOT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Religions' AND xtype='U')
            CREATE TABLE Religions (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                FoundedDate DATETIME2 NOT NULL,
                FounderId BIGINT NULL,
                Followers INT NOT NULL,
                Beliefs NVARCHAR(500) NOT NULL,
                AllowsPolygamy BIT NOT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Jobs' AND xtype='U')
            CREATE TABLE Jobs (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                MinIntelligence INT NOT NULL,
                MinStrength INT NOT NULL,
                MinAge INT NOT NULL,
                BaseSalary DECIMAL(18,2) NOT NULL,
                SocialStatusBonus INT NOT NULL,
                DeathRiskModifier DECIMAL(10,2) NOT NULL,
                RequiresInvention BIT NOT NULL,
                RequiredInventionId BIGINT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Inventions' AND xtype='U')
            CREATE TABLE Inventions (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                Description NVARCHAR(500) NOT NULL,
                DiscoveredDate DATETIME2 NOT NULL,
                InventorId BIGINT NULL,
                RequiredIntelligence INT NOT NULL,
                Category NVARCHAR(50) NOT NULL,
                HealthBonus INT NOT NULL DEFAULT 0,
                LifespanBonus INT NOT NULL DEFAULT 0
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Wars' AND xtype='U')
            CREATE TABLE Wars (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(200) NOT NULL,
                StartDate DATETIME2 NOT NULL,
                EndDate DATETIME2 NULL,
                AttackerCountryId BIGINT NOT NULL,
                DefenderCountryId BIGINT NOT NULL,
                WinnerCountryId BIGINT NULL,
                Casualties INT NOT NULL,
                IsActive BIT NOT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Events' AND xtype='U')
            CREATE TABLE Events (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Date DATETIME2 NOT NULL,
                EventType NVARCHAR(50) NOT NULL,
                Description NVARCHAR(1000) NOT NULL,
                PersonId BIGINT NULL,
                CityId BIGINT NULL,
                CountryId BIGINT NULL,
                ReligionId BIGINT NULL,
                WarId BIGINT NULL,
                InventionId BIGINT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Dynasties' AND xtype='U')
            CREATE TABLE Dynasties (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                FounderId BIGINT NOT NULL,
                FoundedDate DATETIME2 NOT NULL,
                CurrentRulerId BIGINT NULL,
                MemberCount INT NOT NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Laws' AND xtype='U')
            CREATE TABLE Laws (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                Description NVARCHAR(500) NOT NULL,
                CountryId BIGINT NULL,
                ReligionId BIGINT NULL,
                EnactedDate DATETIME2 NOT NULL,
                Category NVARCHAR(50) NOT NULL
            );
        ";
        command.ExecuteNonQuery();
    }

    private void EnsureDatabaseExists()
    {
        // Parse database name from connection string
        var builder = new SqlConnectionStringBuilder(_connectionString);
        string dbName = builder.InitialCatalog;

        // Connect to master to check/create database
        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $@"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}')
            BEGIN
                CREATE DATABASE [{dbName}]
            END";
        command.ExecuteNonQuery();
    }

    public void ClearDatabase()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM Events;
            DELETE FROM Wars;
            DELETE FROM Inventions;
            DELETE FROM Laws;
            DELETE FROM Dynasties;
            DELETE FROM People;
            DELETE FROM Cities;
            DELETE FROM Countries;
            DELETE FROM Religions;
            DELETE FROM Jobs;

            DBCC CHECKIDENT ('Events', RESEED, 0);
            DBCC CHECKIDENT ('Wars', RESEED, 0);
            DBCC CHECKIDENT ('Inventions', RESEED, 0);
            DBCC CHECKIDENT ('Laws', RESEED, 0);
            DBCC CHECKIDENT ('Dynasties', RESEED, 0);
            DBCC CHECKIDENT ('People', RESEED, 0);
            DBCC CHECKIDENT ('Cities', RESEED, 0);
            DBCC CHECKIDENT ('Countries', RESEED, 0);
            DBCC CHECKIDENT ('Religions', RESEED, 0);
            DBCC CHECKIDENT ('Jobs', RESEED, 0);
        ";
        command.ExecuteNonQuery();
    }

    public void SavePeople(List<Person> people)
    {
        if (!people.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var person in people)
        {
            if (person.Id < 0) // New person with temp ID
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO People (FirstName, LastName, Gender, BirthDate, DeathDate, IsAlive, CauseOfDeath,
                        FatherId, MotherId, SpouseId, SecondarySpouseId, MarriageDate,
                        CityId, CountryId, ReligionId, JobId, JobStartDate,
                        Intelligence, Strength, Health, Fertility, Charisma, Creativity, Leadership, Aggression, Wisdom, Beauty,
                        DNASequence, BloodType, GeneticMarkers, HereditaryConditions, HasHereditaryDisease, DiseaseResistance, Longevity,
                        EyeColor, HairColor, SkinTone, Height, Weight, BuildType,
                        IsPregnant, PregnancyDueDate, PregnancyFatherId, PregnancyMultiplier, TotalChildren,
                        DynastyId, IsRuler, GenerationNumber, Wealth, SocialStatus,
                        IsNotable, NotableFor, ChildrenBorn, DescendantCount)
                    VALUES (@FirstName, @LastName, @Gender, @BirthDate, @DeathDate, @IsAlive, @CauseOfDeath,
                        @FatherId, @MotherId, @SpouseId, @SecondarySpouseId, @MarriageDate,
                        @CityId, @CountryId, @ReligionId, @JobId, @JobStartDate,
                        @Intelligence, @Strength, @Health, @Fertility, @Charisma, @Creativity, @Leadership, @Aggression, @Wisdom, @Beauty,
                        @DNASequence, @BloodType, @GeneticMarkers, @HereditaryConditions, @HasHereditaryDisease, @DiseaseResistance, @Longevity,
                        @EyeColor, @HairColor, @SkinTone, @Height, @Weight, @BuildType,
                        @IsPregnant, @PregnancyDueDate, @PregnancyFatherId, @PregnancyMultiplier, @TotalChildren,
                        @DynastyId, @IsRuler, @GenerationNumber, @Wealth, @SocialStatus,
                        @IsNotable, @NotableFor, @ChildrenBorn, @DescendantCount);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                AddPersonParameters(cmd, person);
                var newId = (long)(decimal)cmd.ExecuteScalar();
                person.Id = newId;
            }
            else // Update existing person
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE People SET
                        FirstName = @FirstName, LastName = @LastName, Gender = @Gender,
                        BirthDate = @BirthDate, DeathDate = @DeathDate, IsAlive = @IsAlive, CauseOfDeath = @CauseOfDeath,
                        FatherId = @FatherId, MotherId = @MotherId, SpouseId = @SpouseId, SecondarySpouseId = @SecondarySpouseId, MarriageDate = @MarriageDate,
                        CityId = @CityId, CountryId = @CountryId, ReligionId = @ReligionId, JobId = @JobId, JobStartDate = @JobStartDate,
                        Intelligence = @Intelligence, Strength = @Strength, Health = @Health, Fertility = @Fertility,
                        Charisma = @Charisma, Creativity = @Creativity, Leadership = @Leadership, Aggression = @Aggression, Wisdom = @Wisdom, Beauty = @Beauty,
                        DNASequence = @DNASequence, BloodType = @BloodType, GeneticMarkers = @GeneticMarkers,
                        HereditaryConditions = @HereditaryConditions, HasHereditaryDisease = @HasHereditaryDisease,
                        DiseaseResistance = @DiseaseResistance, Longevity = @Longevity,
                        EyeColor = @EyeColor, HairColor = @HairColor, SkinTone = @SkinTone, Height = @Height, Weight = @Weight, BuildType = @BuildType,
                        IsPregnant = @IsPregnant, PregnancyDueDate = @PregnancyDueDate, PregnancyFatherId = @PregnancyFatherId, PregnancyMultiplier = @PregnancyMultiplier,
                        TotalChildren = @TotalChildren,
                        DynastyId = @DynastyId, IsRuler = @IsRuler, GenerationNumber = @GenerationNumber, Wealth = @Wealth, SocialStatus = @SocialStatus,
                        IsNotable = @IsNotable, NotableFor = @NotableFor, ChildrenBorn = @ChildrenBorn, DescendantCount = @DescendantCount
                    WHERE Id = @Id";

                AddPersonParameters(cmd, person);
                cmd.Parameters.AddWithValue("@Id", person.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    private void AddPersonParameters(SqlCommand cmd, Person person)
    {
        cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
        cmd.Parameters.AddWithValue("@LastName", person.LastName);
        cmd.Parameters.AddWithValue("@Gender", person.Gender);
        cmd.Parameters.AddWithValue("@BirthDate", person.BirthDate);
        cmd.Parameters.AddWithValue("@DeathDate", (object?)person.DeathDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IsAlive", person.IsAlive);
        cmd.Parameters.AddWithValue("@CauseOfDeath", (object?)person.CauseOfDeath ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FatherId", (object?)person.FatherId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MotherId", (object?)person.MotherId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SpouseId", (object?)person.SpouseId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SecondarySpouseId", (object?)person.SecondarySpouseId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MarriageDate", (object?)person.MarriageDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CityId", (object?)person.CityId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CountryId", (object?)person.CountryId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReligionId", (object?)person.ReligionId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@JobId", (object?)person.JobId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@JobStartDate", (object?)person.JobStartDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Intelligence", person.Intelligence);
        cmd.Parameters.AddWithValue("@Strength", person.Strength);
        cmd.Parameters.AddWithValue("@Health", person.Health);
        cmd.Parameters.AddWithValue("@Fertility", person.Fertility);
        cmd.Parameters.AddWithValue("@Charisma", person.Charisma);
        cmd.Parameters.AddWithValue("@Creativity", person.Creativity);
        cmd.Parameters.AddWithValue("@Leadership", person.Leadership);
        cmd.Parameters.AddWithValue("@Aggression", person.Aggression);
        cmd.Parameters.AddWithValue("@Wisdom", person.Wisdom);
        cmd.Parameters.AddWithValue("@Beauty", person.Beauty);
        cmd.Parameters.AddWithValue("@DNASequence", person.DNASequence ?? "");
        cmd.Parameters.AddWithValue("@BloodType", person.BloodType ?? "");
        cmd.Parameters.AddWithValue("@GeneticMarkers", person.GeneticMarkers ?? "");
        cmd.Parameters.AddWithValue("@HereditaryConditions", person.HereditaryConditions ?? "None");
        cmd.Parameters.AddWithValue("@HasHereditaryDisease", person.HasHereditaryDisease);
        cmd.Parameters.AddWithValue("@DiseaseResistance", person.DiseaseResistance);
        cmd.Parameters.AddWithValue("@Longevity", person.Longevity);
        cmd.Parameters.AddWithValue("@EyeColor", person.EyeColor ?? "");
        cmd.Parameters.AddWithValue("@HairColor", person.HairColor ?? "");
        cmd.Parameters.AddWithValue("@SkinTone", person.SkinTone ?? "");
        cmd.Parameters.AddWithValue("@Height", person.Height);
        cmd.Parameters.AddWithValue("@Weight", person.Weight);
        cmd.Parameters.AddWithValue("@BuildType", person.BuildType ?? "Average");
        cmd.Parameters.AddWithValue("@IsPregnant", person.IsPregnant);
        cmd.Parameters.AddWithValue("@PregnancyDueDate", (object?)person.PregnancyDueDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PregnancyFatherId", (object?)person.PregnancyFatherId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PregnancyMultiplier", person.PregnancyMultiplier);
        cmd.Parameters.AddWithValue("@TotalChildren", person.TotalChildren);
        cmd.Parameters.AddWithValue("@DynastyId", (object?)person.DynastyId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IsRuler", person.IsRuler);
        cmd.Parameters.AddWithValue("@GenerationNumber", person.GenerationNumber);
        cmd.Parameters.AddWithValue("@Wealth", person.Wealth);
        cmd.Parameters.AddWithValue("@SocialStatus", person.SocialStatus);
        cmd.Parameters.AddWithValue("@IsNotable", person.IsNotable);
        cmd.Parameters.AddWithValue("@NotableFor", person.NotableFor ?? "");
        cmd.Parameters.AddWithValue("@ChildrenBorn", person.ChildrenBorn);
        cmd.Parameters.AddWithValue("@DescendantCount", person.DescendantCount);
    }

    public void SaveCities(List<City> cities)
    {
        if (!cities.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var city in cities)
        {
            if (city.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Cities (Name, CountryId, FoundedDate, Population, FounderId, Wealth)
                    VALUES (@Name, @CountryId, @FoundedDate, @Population, @FounderId, @Wealth);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", city.Name);
                cmd.Parameters.AddWithValue("@CountryId", (object?)city.CountryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FoundedDate", city.FoundedDate);
                cmd.Parameters.AddWithValue("@Population", city.Population);
                cmd.Parameters.AddWithValue("@FounderId", (object?)city.FounderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Wealth", city.Wealth);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                city.Id = newId;
            }
            else
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Cities SET Name = @Name, CountryId = @CountryId, FoundedDate = @FoundedDate,
                        Population = @Population, FounderId = @FounderId, Wealth = @Wealth
                    WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", city.Id);
                cmd.Parameters.AddWithValue("@Name", city.Name);
                cmd.Parameters.AddWithValue("@CountryId", (object?)city.CountryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FoundedDate", city.FoundedDate);
                cmd.Parameters.AddWithValue("@Population", city.Population);
                cmd.Parameters.AddWithValue("@FounderId", (object?)city.FounderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Wealth", city.Wealth);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveCountries(List<Country> countries)
    {
        if (!countries.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var country in countries)
        {
            if (country.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Countries (Name, FoundedDate, RulerId, CapitalCityId, Population, Wealth, MilitaryStrength, GovernmentType)
                    VALUES (@Name, @FoundedDate, @RulerId, @CapitalCityId, @Population, @Wealth, @MilitaryStrength, @GovernmentType);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", country.Name);
                cmd.Parameters.AddWithValue("@FoundedDate", country.FoundedDate);
                cmd.Parameters.AddWithValue("@RulerId", (object?)country.RulerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CapitalCityId", (object?)country.CapitalCityId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Population", country.Population);
                cmd.Parameters.AddWithValue("@Wealth", country.Wealth);
                cmd.Parameters.AddWithValue("@MilitaryStrength", country.MilitaryStrength);
                cmd.Parameters.AddWithValue("@GovernmentType", country.GovernmentType);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                country.Id = newId;
            }
            else
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Countries SET Name = @Name, FoundedDate = @FoundedDate, RulerId = @RulerId,
                        CapitalCityId = @CapitalCityId, Population = @Population, Wealth = @Wealth,
                        MilitaryStrength = @MilitaryStrength, GovernmentType = @GovernmentType
                    WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", country.Id);
                cmd.Parameters.AddWithValue("@Name", country.Name);
                cmd.Parameters.AddWithValue("@FoundedDate", country.FoundedDate);
                cmd.Parameters.AddWithValue("@RulerId", (object?)country.RulerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CapitalCityId", (object?)country.CapitalCityId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Population", country.Population);
                cmd.Parameters.AddWithValue("@Wealth", country.Wealth);
                cmd.Parameters.AddWithValue("@MilitaryStrength", country.MilitaryStrength);
                cmd.Parameters.AddWithValue("@GovernmentType", country.GovernmentType);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveReligions(List<Religion> religions)
    {
        if (!religions.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var religion in religions)
        {
            if (religion.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Religions (Name, FoundedDate, FounderId, Followers, Beliefs, AllowsPolygamy)
                    VALUES (@Name, @FoundedDate, @FounderId, @Followers, @Beliefs, @AllowsPolygamy);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", religion.Name);
                cmd.Parameters.AddWithValue("@FoundedDate", religion.FoundedDate);
                cmd.Parameters.AddWithValue("@FounderId", (object?)religion.FounderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Followers", religion.Followers);
                cmd.Parameters.AddWithValue("@Beliefs", religion.Beliefs);
                cmd.Parameters.AddWithValue("@AllowsPolygamy", religion.AllowsPolygamy);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                religion.Id = newId;
            }
            else
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Religions SET Name = @Name, FoundedDate = @FoundedDate, FounderId = @FounderId,
                        Followers = @Followers, Beliefs = @Beliefs, AllowsPolygamy = @AllowsPolygamy
                    WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", religion.Id);
                cmd.Parameters.AddWithValue("@Name", religion.Name);
                cmd.Parameters.AddWithValue("@FoundedDate", religion.FoundedDate);
                cmd.Parameters.AddWithValue("@FounderId", (object?)religion.FounderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Followers", religion.Followers);
                cmd.Parameters.AddWithValue("@Beliefs", religion.Beliefs);
                cmd.Parameters.AddWithValue("@AllowsPolygamy", religion.AllowsPolygamy);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveJobs(List<Job> jobs)
    {
        if (!jobs.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var job in jobs)
        {
            if (job.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Jobs (Name, MinIntelligence, MinStrength, MinAge, BaseSalary, SocialStatusBonus, DeathRiskModifier, RequiresInvention, RequiredInventionId)
                    VALUES (@Name, @MinIntelligence, @MinStrength, @MinAge, @BaseSalary, @SocialStatusBonus, @DeathRiskModifier, @RequiresInvention, @RequiredInventionId);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", job.Name);
                cmd.Parameters.AddWithValue("@MinIntelligence", job.MinIntelligence);
                cmd.Parameters.AddWithValue("@MinStrength", job.MinStrength);
                cmd.Parameters.AddWithValue("@MinAge", job.MinAge);
                cmd.Parameters.AddWithValue("@BaseSalary", job.BaseSalary);
                cmd.Parameters.AddWithValue("@SocialStatusBonus", job.SocialStatusBonus);
                cmd.Parameters.AddWithValue("@DeathRiskModifier", job.DeathRiskModifier);
                cmd.Parameters.AddWithValue("@RequiresInvention", job.RequiresInvention);
                cmd.Parameters.AddWithValue("@RequiredInventionId", (object?)job.RequiredInventionId ?? DBNull.Value);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                job.Id = newId;
            }
            else
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Jobs SET Name = @Name, MinIntelligence = @MinIntelligence, MinStrength = @MinStrength,
                        MinAge = @MinAge, BaseSalary = @BaseSalary, SocialStatusBonus = @SocialStatusBonus,
                        DeathRiskModifier = @DeathRiskModifier, RequiresInvention = @RequiresInvention, RequiredInventionId = @RequiredInventionId
                    WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", job.Id);
                cmd.Parameters.AddWithValue("@Name", job.Name);
                cmd.Parameters.AddWithValue("@MinIntelligence", job.MinIntelligence);
                cmd.Parameters.AddWithValue("@MinStrength", job.MinStrength);
                cmd.Parameters.AddWithValue("@MinAge", job.MinAge);
                cmd.Parameters.AddWithValue("@BaseSalary", job.BaseSalary);
                cmd.Parameters.AddWithValue("@SocialStatusBonus", job.SocialStatusBonus);
                cmd.Parameters.AddWithValue("@DeathRiskModifier", job.DeathRiskModifier);
                cmd.Parameters.AddWithValue("@RequiresInvention", job.RequiresInvention);
                cmd.Parameters.AddWithValue("@RequiredInventionId", (object?)job.RequiredInventionId ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveInventions(List<Invention> inventions)
    {
        if (!inventions.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var invention in inventions)
        {
            if (invention.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Inventions (Name, Description, DiscoveredDate, InventorId, RequiredIntelligence, Category, HealthBonus, LifespanBonus)
                    VALUES (@Name, @Description, @DiscoveredDate, @InventorId, @RequiredIntelligence, @Category, @HealthBonus, @LifespanBonus);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", invention.Name);
                cmd.Parameters.AddWithValue("@Description", invention.Description);
                cmd.Parameters.AddWithValue("@DiscoveredDate", invention.DiscoveredDate);
                cmd.Parameters.AddWithValue("@InventorId", (object?)invention.InventorId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RequiredIntelligence", invention.RequiredIntelligence);
                cmd.Parameters.AddWithValue("@Category", invention.Category);
                cmd.Parameters.AddWithValue("@HealthBonus", invention.HealthBonus);
                cmd.Parameters.AddWithValue("@LifespanBonus", invention.LifespanBonus);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                invention.Id = newId;
            }
        }
    }

    public void SaveWars(List<War> wars)
    {
        if (!wars.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var war in wars)
        {
            if (war.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Wars (Name, StartDate, EndDate, AttackerCountryId, DefenderCountryId, WinnerCountryId, Casualties, IsActive)
                    VALUES (@Name, @StartDate, @EndDate, @AttackerCountryId, @DefenderCountryId, @WinnerCountryId, @Casualties, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", war.Name);
                cmd.Parameters.AddWithValue("@StartDate", war.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object?)war.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttackerCountryId", war.AttackerCountryId);
                cmd.Parameters.AddWithValue("@DefenderCountryId", war.DefenderCountryId);
                cmd.Parameters.AddWithValue("@WinnerCountryId", (object?)war.WinnerCountryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Casualties", war.Casualties);
                cmd.Parameters.AddWithValue("@IsActive", war.IsActive);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                war.Id = newId;
            }
            else
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Wars SET Name = @Name, StartDate = @StartDate, EndDate = @EndDate,
                        AttackerCountryId = @AttackerCountryId, DefenderCountryId = @DefenderCountryId,
                        WinnerCountryId = @WinnerCountryId, Casualties = @Casualties, IsActive = @IsActive
                    WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", war.Id);
                cmd.Parameters.AddWithValue("@Name", war.Name);
                cmd.Parameters.AddWithValue("@StartDate", war.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object?)war.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttackerCountryId", war.AttackerCountryId);
                cmd.Parameters.AddWithValue("@DefenderCountryId", war.DefenderCountryId);
                cmd.Parameters.AddWithValue("@WinnerCountryId", (object?)war.WinnerCountryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Casualties", war.Casualties);
                cmd.Parameters.AddWithValue("@IsActive", war.IsActive);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveDynasties(List<Dynasty> dynasties)
    {
        if (!dynasties.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var dynasty in dynasties)
        {
            if (dynasty.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Dynasties (Name, FounderId, FoundedDate, CurrentRulerId, MemberCount)
                    VALUES (@Name, @FounderId, @FoundedDate, @CurrentRulerId, @MemberCount);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Name", dynasty.Name);
                cmd.Parameters.AddWithValue("@FounderId", dynasty.FounderId);
                cmd.Parameters.AddWithValue("@FoundedDate", dynasty.FoundedDate);
                cmd.Parameters.AddWithValue("@CurrentRulerId", (object?)dynasty.CurrentRulerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MemberCount", dynasty.MemberCount);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                dynasty.Id = newId;
            }
            else
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Dynasties SET Name = @Name, FounderId = @FounderId, FoundedDate = @FoundedDate,
                        CurrentRulerId = @CurrentRulerId, MemberCount = @MemberCount
                    WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", dynasty.Id);
                cmd.Parameters.AddWithValue("@Name", dynasty.Name);
                cmd.Parameters.AddWithValue("@FounderId", dynasty.FounderId);
                cmd.Parameters.AddWithValue("@FoundedDate", dynasty.FoundedDate);
                cmd.Parameters.AddWithValue("@CurrentRulerId", (object?)dynasty.CurrentRulerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MemberCount", dynasty.MemberCount);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveEvents(List<Event> events)
    {
        if (!events.Any()) return;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var evt in events)
        {
            if (evt.Id < 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Events (Date, EventType, Description, PersonId, CityId, CountryId, ReligionId, WarId, InventionId)
                    VALUES (@Date, @EventType, @Description, @PersonId, @CityId, @CountryId, @ReligionId, @WarId, @InventionId);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                cmd.Parameters.AddWithValue("@Date", evt.Date);
                cmd.Parameters.AddWithValue("@EventType", evt.EventType);
                cmd.Parameters.AddWithValue("@Description", evt.Description);
                cmd.Parameters.AddWithValue("@PersonId", (object?)evt.PersonId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CityId", (object?)evt.CityId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryId", (object?)evt.CountryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ReligionId", (object?)evt.ReligionId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WarId", (object?)evt.WarId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InventionId", (object?)evt.InventionId ?? DBNull.Value);

                var newId = (long)(decimal)cmd.ExecuteScalar();
                evt.Id = newId;
            }
        }
    }
}
