# Population Simulator - Complete Redesign

## Overview

This is a comprehensive redesign of the Population Simulator with significantly enhanced genetics, improved population growth mechanics, SQL Server integration, and better visibility into the simulation.

## Major Enhancements

### 1. Advanced Genetics System

#### DNA Sequences
- **32-character DNA sequences** using realistic bases (A, T, G, C)
- **Mendelian inheritance** from both parents with crossover recombination
- **2% mutation rate** per gene for evolutionary diversity
- DNA quality affects longevity and disease resistance

#### Blood Types
- Full **ABO blood type system** (A, B, AB, O) with Rh factors (+/-)
- **Realistic Mendelian genetics** for blood type inheritance
- A and B are codominant, O is recessive
- Rh+ is dominant over Rh-

#### Genetic Markers
- **8 genetic markers** including HLA-A, HLA-B, HLA-C, BRCA1, BRCA2, APOE, CCR5, MC1R
- Inherited and generated randomly from parent markers
- Used for genetic diversity tracking

#### Hereditary Conditions
- **7 hereditary conditions**: Hemophilia, Color Blindness, Sickle Cell Trait, Thalassemia, Lactose Intolerance, G6PD Deficiency, Cystic Fibrosis Carrier
- **10% inheritance chance** from each parent
- **3% chance of new genetic condition**
- Some conditions marked as diseases (affect health and lifespan)

#### Disease Resistance & Longevity
- **Disease Resistance** (0-100): Inherited from parents with DNA-based bonuses
- **Longevity** (0-100): Genetic predisposition to long life
- Both traits affect pregnancy chances and death rates
- DNA sequence quality impacts these traits (G-C bonds are stronger)

#### Physical Traits
- **Build Types**: Slim, Average, Muscular, Heavy (based on strength and weight)
- **Skin Tone**: Inherited and tracked
- **Weight**: Inherited from parents with variation
- Enhanced eye color and hair color inheritance

### 2. Enhanced Person Model

New fields added to Person class:
- `DNASequence` - 32-character genetic sequence
- `BloodType` - ABO + Rh blood type
- `GeneticMarkers` - Comma-separated genetic markers
- `HereditaryConditions` - Comma-separated conditions
- `HasHereditaryDisease` - Boolean flag
- `DiseaseResistance` - 0-100 scale
- `Longevity` - 0-100 genetic lifespan predisposition
- `SkinTone` - Physical appearance
- `Weight` - In kilograms
- `BuildType` - Slim/Average/Muscular/Heavy
- `CauseOfDeath` - Detailed death reason
- `TotalChildren` - Total children born
- `GenerationNumber` - Generation from Adam/Eve (0, 1, 2, ...)
- `IsNotable` - Marked as notable person
- `NotableFor` - Reason for being notable
- `ChildrenBorn` - Counter for children produced
- `DescendantCount` - Total living descendants

### 3. Improved Population Growth Mechanics

#### Increased Pregnancy Rates
- **< 20 people: 30%** daily base chance (very high for Adam/Eve's children)
- **< 50 people: 25%** (high for early generations)
- **< 150 people: 18%** (moderate growth)
- **< 300 people: 12%** (good growth rate)
- **< 500 people: 8%** (slowing down)
- **< 1000 people: 4%** (controlled growth)
- **1000+ people: 2%** (steady state)

#### Genetic Bonuses
- **+20% bonus for Generation 0-2** (Adam/Eve and their children)
- **+10% bonus for Longevity > 70**
- **-20% penalty for Disease Resistance < 30**
- Higher fertility increases twin/triplet chances by 50%

#### Multiple Births
- **Triplets: 1%** base (1.5% with high fertility)
- **Twins: 6%** base (9% with high fertility)
- Up from previous 5% twins / 1% triplets

### 4. SQL Server Integration (ADO.NET Only)

#### Database Engine
- **Replaced SQLite** with **Microsoft SQL Server**
- Uses **LocalDB** by default: `(localdb)\MSSQLLocalDB`
- Database name: `PopulationSimulator`
- **Integrated Security** with trusted connection
- Auto-creates database if it doesn't exist

#### Schema Updates
- All tables updated for SQL Server syntax
- `IDENTITY(1,1)` instead of `AUTOINCREMENT`
- `BIGINT` instead of `INTEGER`
- `NVARCHAR` instead of `TEXT`
- `DATETIME2` for dates
- `BIT` for booleans
- `DECIMAL(18,2)` for currency

#### New Database Columns
All new genetics and tracking fields added to the `People` table:
- DNASequence, BloodType, GeneticMarkers
- HereditaryConditions, HasHereditaryDisease
- DiseaseResistance, Longevity
- SkinTone, Weight, BuildType
- CauseOfDeath, TotalChildren, GenerationNumber
- IsNotable, NotableFor, ChildrenBorn, DescendantCount

### 5. Cause of Death Tracking

Deaths now include detailed causes:
- **Infant mortality** (age < 1)
- **Childhood illness** (age < 5)
- **Combat** (warriors, guards)
- **Mining accident** (miners)
- **Shipwreck** (sailors)
- **Occupational hazard** (high-risk jobs)
- **Chronic illness** (health < 30)
- **Hereditary condition** (genetic diseases)
- **Old age** (age > 70)
- **Age-related illness** (age > 50)
- **Disease, Accident, Natural causes, Fever, Infection** (random)

### 6. Adam & Eve Initialization

Both start with perfect stats and genetics:
- **All traits at 100**: Intelligence, Strength, Health, Fertility, Charisma, Creativity, Leadership, Aggression, Wisdom, Beauty
- **Perfect genetics**: Disease Resistance 100, Longevity 100
- **Unique DNA sequences**: Randomly generated primordial DNA
- **Blood Type O+**: Universal donor type
- **Notable status**: "First Human - Father/Mother of Humanity"
- **Generation 0**: Root of the family tree
- **Can reproduce until age 100** (normal people until age 50 for females)

### 7. Enhanced Statistics & Visibility

#### New SimulationStats Fields
- `AlivePeople` - List of all living people with details
- `RecentDeaths` - Recent deaths with causes
- `RecentBirths` - Recent births with genetics info
- `NotablePeople` - Notable individuals and achievements
- `BloodTypeDistribution` - Breakdown of blood types in population
- `TotalWithHereditaryDiseases` - Count of people with genetic diseases
- `AverageLongevity` - Population average genetic longevity
- `AverageDiseaseResistance` - Population average disease resistance

#### PersonSummary Class
New summary class for displaying people:
- Full name, gender, age, alive status
- Blood type, job, city
- Cause of death (if dead)
- Children count, generation number
- Notable status and reason
- Hereditary disease info

### 8. Console UI Enhancements

The existing console UI (using Spectre.Console and RazorConsole) now displays:
- **Population statistics** with genetics info
- **Top occupations**
- **Civilization progress** (cities, countries, religions, inventions, wars)
- **Family tree visualization** (hierarchical, color-coded)
- **Recent events** (births, deaths, marriages, inventions)
- **Live updates** every 500ms
- **Speed controls** (1x to 100x)

## Architecture

### In-Memory First, Periodic Persistence
- All simulation runs **in-memory** for maximum performance
- Uses `Dictionary` lookups for O(1) access to people, cities, etc.
- **Syncs to SQL Server every 100 days** of simulation time
- Maintains separate dictionaries for living and dead people

### Genetics Engine
New `GeneticsEngine.cs` handles all genetic operations:
- DNA generation and inheritance
- Blood type Mendelian genetics
- Genetic marker generation
- Hereditary condition inheritance
- Disease resistance calculation
- Longevity calculation
- Trait inheritance with mutations

### Data Layer
- `DataAccessLayer.cs` completely rewritten for SQL Server
- Uses **ADO.NET only** (no Entity Framework or ORMs)
- Parametrized queries prevent SQL injection
- Handles temp IDs (negative) vs. database IDs (positive)
- Batch operations for performance

## Installation & Setup

### Prerequisites
- **.NET 9.0** SDK
- **SQL Server** or **SQL Server LocalDB** (comes with Visual Studio)
- Windows, Linux, or macOS

### Database Setup

#### Option 1: LocalDB (Default - Windows only)
LocalDB is automatically used if no connection string is provided. It comes with:
- Visual Studio 2017 or later
- SQL Server Express (separate download)

No manual setup required! The database is auto-created on first run.

#### Option 2: SQL Server
Update the connection string in `DataAccessLayer.cs`:

```csharp
_connectionString = @"Server=YOUR_SERVER;Database=PopulationSimulator;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;";
```

Or pass a connection string to the constructor:
```csharp
var dataAccess = new DataAccessLayer("YOUR_CONNECTION_STRING");
```

### Run the Simulation

#### Console Mode (Recommended)
```bash
cd PopulationSimulator.Console
dotnet run
```

#### Web Mode
```bash
cd PopulationSimulator.Web
dotnet run
```

Then navigate to: http://localhost:5000

## Key Features

### ✅ Realistic Genetics
- DNA, blood types, genetic markers, hereditary conditions
- Mendelian inheritance with mutations
- Disease resistance and longevity genes

### ✅ Sustainable Population Growth
- Starts with Adam & Eve (perfect genetics)
- High early-generation pregnancy rates ensure population doesn't die out
- Genetic bonuses for early generations
- Population grows steadily to thousands

### ✅ SQL Server Backend
- Enterprise-grade database (LocalDB for dev, full SQL Server for production)
- In-memory simulation with periodic persistence
- All data fully queryable via SQL

### ✅ Comprehensive Visibility
- Who's alive right now
- Recent deaths with causes
- Family tree visualization
- Notable people tracking
- Genetic diversity statistics
- Bloodline tracking

### ✅ Rich Simulation
- 90+ inventions across 10 categories
- 50+ occupations with requirements
- Cities, countries, religions, dynasties
- Wars and conflicts
- Cultural evolution (naming conventions)
- Social mobility

## Notable Improvements Over Original

| Feature | Original | Redesigned |
|---------|----------|------------|
| **Genetics** | Basic trait inheritance | Full DNA, blood types, hereditary diseases |
| **Database** | SQLite | SQL Server (LocalDB/Full) |
| **Pregnancy Rates** | Too low, population dies | Optimized for sustainable growth |
| **Cause of Death** | Not tracked | Detailed causes tracked |
| **Visibility** | Basic stats | Full population dashboard, notable people, bloodlines |
| **Generation Tracking** | Basic counter | Per-person generation numbers from Adam/Eve |
| **Physical Traits** | Eye/hair color only | Build type, skin tone, weight, height all inherited |
| **Notable People** | Not tracked | Automatic tracking with reasons |
| **Build Type** | N/A | Slim, Average, Muscular, Heavy (based on genetics) |

## Performance

- **In-memory simulation**: Millions of operations per second
- **Optimized lookups**: O(1) dictionary access
- **Cached living people**: Avoid repeated filtering
- **Periodic persistence**: Only syncs every 100 days
- **Event buffering**: Limited to last 1000 events
- **Efficient genetics**: Pre-calculated DNA quality and bonuses

## Database Schema

### People Table (38+ columns)
Complete person record including all genetics, relationships, physical traits, and achievements.

### Key Relationships
- Person → Person (Father, Mother, Spouse)
- Person → City, Country, Religion, Job, Dynasty
- City → Country
- Country → Person (Ruler)
- Invention → Person (Inventor)
- War → Country (Attacker, Defender, Winner)

## Future Enhancements

Possible additions:
- [ ] Migration patterns
- [ ] Trade routes between cities
- [ ] Disease epidemics with genetic resistance
- [ ] Natural disasters
- [ ] Genetic diseases that reduce lifespan
- [ ] Selective breeding / eugenics
- [ ] Environmental factors affecting genetics
- [ ] Chromosomal abnormalities
- [ ] More detailed hereditary disease impacts

## Technical Stack

- **.NET 9.0**: Latest LTS version
- **C# 12**: Modern language features
- **SQL Server**: Enterprise database
- **ADO.NET**: Direct database access (no ORM)
- **Spectre.Console**: Rich console UI
- **RazorConsole**: Console-based Razor components
- **Blazor Server**: Web UI mode

## Credits

Enhanced and redesigned by Claude (Anthropic AI Assistant)

Original concept: Population simulation with emergent civilization

## License

MIT License - Feel free to use, modify, and distribute
