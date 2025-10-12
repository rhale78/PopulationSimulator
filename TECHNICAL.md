# Technical Design Document

## Advanced Population Simulator

### Overview
The Advanced Population Simulator is a complex system that models human civilization growth from two individuals through multiple generations, incorporating genetics, social structures, and cultural evolution.

---

## Architecture

### Layer Structure

```
┌─────────────────────────────────────┐
│         Program.cs (Entry)          │
│  - Main loop                        │
│  - Input handling                   │
└────────────┬────────────────────────┘
             │
    ┌────────┴─────────┐
    │                  │
┌───▼────────┐   ┌────▼──────────┐
│     UI     │   │     Core      │
│  Layer     │◄──┤   (Simulator) │
└────────────┘   └───────┬───────┘
                         │
              ┌──────────┴──────────┐
              │                     │
         ┌────▼─────┐         ┌────▼─────┐
         │  Models  │         │   Data   │
         │  Layer   │◄────────┤  Layer   │
         └──────────┘         └──────────┘
```

### Component Responsibilities

#### Program.cs
- Application entry point
- Main simulation loop coordination
- User input processing
- Thread management for async simulation

#### UI Layer (ConsoleUI.cs)
- Console rendering and display
- Statistics visualization
- Event feed management
- Welcome/shutdown screens
- Differential rendering for performance

#### Core Layer
**Simulator.cs**
- Main simulation orchestration
- Daily event processing
- Yearly event processing
- Population management
- In-memory state management
- Database synchronization

**NameGenerator.cs**
- Cultural name generation
- Context-aware naming (patronymic, city-based, occupation-based)
- Random name selection from predefined pools

#### Models Layer
Data classes representing:
- Person (primary entity)
- City, Country, Religion
- Job, Invention, War
- Event, Dynasty, Law

#### Data Layer (DataAccessLayer.cs)
- Database schema creation
- CRUD operations for all entities
- SQLite connection management
- Data persistence and retrieval

---

## Data Flow

### Simulation Loop
```
Initialize
    │
    ├─► Seed initial data (Adam & Eve, Jobs)
    │
    ▼
Daily Loop
    │
    ├─► Process Deaths
    │   └─► Calculate death chances
    │       └─► Handle ruler succession
    │
    ├─► Assign Jobs
    │   └─► Match people to eligible jobs
    │       └─► Update social status
    │
    ├─► Process Marriages
    │   └─► Find compatible pairs
    │       └─► Apply cultural rules
    │
    ├─► Process Pregnancies
    │   └─► Calculate pregnancy chance
    │       └─► Handle twins/triplets
    │
    ├─► Process Births
    │   └─► Inherit traits from parents
    │       └─► Mutate traits
    │           └─► Generate names
    │
    ├─► Yearly Events? (Day 1 of year)
    │   ├─► Found Cities
    │   ├─► Found Countries
    │   ├─► Found Religions
    │   ├─► Discover Inventions
    │   └─► Start Wars
    │
    ├─► Database Sync? (Every 100 days)
    │   └─► Save all entities
    │
    └─► Update UI
        └─► Display statistics and events
```

---

## Key Algorithms

### Trait Inheritance
```csharp
InheritTrait(trait1, trait2):
    average = (trait1 + trait2) / 2
    variation = Random(-10, +10)
    mutation = Random(0, 99) < 5 ? Random(-20, +20) : 0
    result = Clamp(average + variation + mutation, 0, 100)
    return result
```

### Death Chance Calculation
```csharp
CalculateDeathChance(person, age):
    if (populationSmall && age < 50) return veryLow
    
    baseChance = ageBasedChance(age)
    healthModifier = 1.0 - (health / 200.0)
    jobRisk = getJobRiskModifier(person.job)
    
    return baseChance * (1.0 + healthModifier) * jobRisk
```

### Marriage Compatibility
```csharp
IsCompatible(male, female):
    if (notEligible(male) || notEligible(female)) return false
    if (isDirectRelative(male, female)) return false
    if (earlyPopulation) return true  // Relaxed rules
    if (differentCountry(male, female)) return false
    if (differentReligion(male, female)) return false
    if (areSiblings(male, female)) return false
    return true
```

---

## Performance Optimizations

### In-Memory Storage
- **Lists**: Fast iteration for processing all entities
- **Dictionaries**: O(1) lookup by ID for relationships
- **Dual storage**: Each entity type stored in both list and dictionary

### Database Strategy
- **Batch operations**: All saves done in transactions
- **Periodic sync**: Every 100 days instead of real-time
- **Temporary IDs**: Negative IDs for in-memory entities before DB sync

### UI Rendering
- **Throttling**: Max 10 updates per second
- **Differential rendering**: Only update changed console areas
- **Event buffer**: Keep only last 1000 events in memory

---

## Configuration Parameters

### Population Growth
- Early population: < 50 people (10% pregnancy chance)
- Growing: 50-100 people (5% pregnancy chance)
- Established: 100+ people (2% pregnancy chance)

### City/Country Thresholds
- City: 100+ population, 1 city per 100 people
- Country: 3+ cities, 1 country per 3 cities

### Marriage Rules
- Minimum age: 14 years
- Early population: Relaxed sibling restrictions
- Later: Same country, religion, no siblings

### Job Requirements
- Minimum age: 12 years
- Trait requirements vary by job
- Risk modifiers: 0.6 (Scholar) to 3.0 (Warrior)

---

## Database Schema

### Primary Entities
- **People**: 38 columns including traits, relationships, status
- **Cities**: 7 columns including founder, population
- **Countries**: 9 columns including ruler, military strength
- **Religions**: 7 columns including beliefs, followers
- **Jobs**: 10 columns including requirements, risks
- **Inventions**: 7 columns including inventor, category
- **Wars**: 9 columns including combatants, casualties
- **Events**: 10 columns including type, description
- **Dynasties**: 6 columns including founder, members
- **Laws**: 7 columns including country/religion association

### Relationships
- Parent-child: FatherId, MotherId in Person
- Marriage: SpouseId, SecondarySpouseId in Person
- Location: CityId, CountryId in Person
- Religion: ReligionId in Person
- Dynasty: DynastyId in Person
- Job: JobId in Person

---

## Extensibility Points

### Adding New Entity Types
1. Create model class in `Models/`
2. Add database schema in `DataAccessLayer.InitializeDatabase()`
3. Add save method in `DataAccessLayer`
4. Add in-memory storage in `Simulator`
5. Add processing logic in simulation loop

### Adding New Events
1. Add event type constant
2. Add trigger logic in appropriate simulation phase
3. Add event logging call
4. Add color coding in UI (optional)

### Adding New Traits
1. Add property to `Person` model
2. Add column to database schema
3. Add inheritance logic in `CreatePerson()`
4. Use trait in relevant calculations

### Adding New Jobs
1. Add entry to `SeedJobs()` method
2. Specify requirements and risk factors
3. Optional: Add invention prerequisite

---

## Thread Safety

### Concurrent Operations
- Simulation runs on background thread
- UI updates on background thread
- Main thread handles input only

### Synchronization
- No shared mutable state between threads
- Simulator state read-only during UI updates
- Stats copied for UI rendering

---

## Future Architecture Considerations

### Scalability
- Current design: Single-process, in-memory
- Future: Distributed simulation for very large populations
- Consideration: Partition by geography or time period

### Persistence
- Current: SQLite file
- Future options: PostgreSQL, NoSQL for scale
- Consideration: Event sourcing pattern

### Visualization
- Current: Console text UI
- Future: Web dashboard, 3D visualization
- Consideration: API layer for data access

---

## Testing Strategy

### Unit Tests (Not Yet Implemented)
- Trait inheritance calculations
- Death chance calculations
- Marriage compatibility rules
- Name generation logic

### Integration Tests (Not Yet Implemented)
- Database operations
- Full simulation cycles
- Event generation

### Manual Testing
- Run simulation for various durations
- Verify population growth curves
- Check data integrity in database
- Monitor UI responsiveness

---

## Known Limitations

1. **Early population**: Sibling marriages needed for growth
2. **Performance**: Single-threaded simulation limits scale
3. **Randomness**: No seed control for reproducibility
4. **UI**: Console-only, limited by terminal capabilities
5. **Names**: Limited name pool, eventual repetition

---

## Dependencies

### NuGet Packages
- Microsoft.Data.Sqlite (9.0.0) - Database operations
- System.Data.SQLite.Core (1.0.118) - SQLite runtime

### .NET Features Used
- C# 13 language features
- .NET 9.0 runtime
- Threading and async/await
- LINQ for data queries
- Collections (List, Dictionary)

---

## Version History

**1.0.0** - Initial release
- Complete simulation engine
- All core features implemented
- Documentation complete
