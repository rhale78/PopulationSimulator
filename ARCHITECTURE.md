# Architecture Diagram

## Advanced Population Simulator - System Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           APPLICATION ENTRY POINT                            │
│                                Program.cs                                    │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │  • Initializes Simulator and UI                                       │  │
│  │  • Spawns background thread for simulation                            │  │
│  │  • Handles user input (speed control, quit)                           │  │
│  │  • Coordinates simulation loop and UI updates                         │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
└──────────────────────────────┬──────────────────────────────────────────────┘
                               │
                ┌──────────────┴───────────────┐
                │                              │
                ▼                              ▼
┌───────────────────────────────┐  ┌──────────────────────────────────────────┐
│     UI LAYER (ConsoleUI)      │  │     CORE SIMULATION LAYER                │
│───────────────────────────────│  │──────────────────────────────────────────│
│  • Welcome Screen             │  │  ┌────────────────────────────────────┐  │
│  • Statistics Display         │◄─┤  │         Simulator.cs               │  │
│  • Event Feed                 │  │  │  • Daily Loop Processing           │  │
│  • Color-Coded Output         │  │  │    - Deaths                        │  │
│  • Controls Display           │  │  │    - Job Assignments               │  │
│  • Shutdown Screen            │  │  │    - Marriages                     │  │
│  • Throttled Rendering        │  │  │    - Pregnancies                   │  │
└───────────────────────────────┘  │  │    - Births                        │  │
                                   │  │  • Yearly Events                   │  │
                                   │  │    - Found Cities                  │  │
                                   │  │    - Found Countries               │  │
                                   │  │    - Found Religions               │  │
                                   │  │    - Discover Inventions           │  │
                                   │  │    - Start Wars                    │  │
                                   │  │  • State Management                │  │
                                   │  │  • Event Logging                   │  │
                                   │  │  • Database Synchronization        │  │
                                   │  └────────────────────────────────────┘  │
                                   │                                          │
                                   │  ┌────────────────────────────────────┐  │
                                   │  │      NameGenerator.cs              │  │
                                   │  │  • Patronymic Names                │  │
                                   │  │  • City-Based Names                │  │
                                   │  │  • Occupation Names                │  │
                                   │  │  • Cultural Evolution              │  │
                                   │  └────────────────────────────────────┘  │
                                   └────────────┬─────────────────────────────┘
                                                │
                                                ▼
                            ┌─────────────────────────────────────────┐
                            │       DATA MODELS LAYER                 │
                            │─────────────────────────────────────────│
                            │  Person      City       Country         │
                            │  Religion    Job        Invention       │
                            │  War         Event      Dynasty         │
                            │  Law                                    │
                            │                                         │
                            │  • Entity Definitions                   │
                            │  • Relationships                        │
                            │  • Helper Methods                       │
                            │  • Properties & Attributes              │
                            └───────────────┬─────────────────────────┘
                                            │
                                            ▼
                            ┌─────────────────────────────────────────┐
                            │    DATA ACCESS LAYER                    │
                            │─────────────────────────────────────────│
                            │      DataAccessLayer.cs                 │
                            │  • Database Initialization              │
                            │  • Schema Creation                      │
                            │  • CRUD Operations                      │
                            │  • Transaction Management               │
                            │  • SQLite Connection                    │
                            └───────────────┬─────────────────────────┘
                                            │
                                            ▼
                            ┌─────────────────────────────────────────┐
                            │         SQLite Database                 │
                            │─────────────────────────────────────────│
                            │  population.db                          │
                            │                                         │
                            │  Tables:                                │
                            │  • People (38 columns)                  │
                            │  • Cities (7 columns)                   │
                            │  • Countries (9 columns)                │
                            │  • Religions (7 columns)                │
                            │  • Jobs (10 columns)                    │
                            │  • Inventions (7 columns)               │
                            │  • Wars (9 columns)                     │
                            │  • Events (10 columns)                  │
                            │  • Dynasties (6 columns)                │
                            │  • Laws (7 columns)                     │
                            └─────────────────────────────────────────┘
```

## Data Flow Diagram

```
┌──────────────────────────────────────────────────────────────────────┐
│                        SIMULATION LIFECYCLE                          │
└──────────────────────────────────────────────────────────────────────┘

    INITIALIZATION
         │
         ├─► Seed Database Schema
         ├─► Seed Initial Jobs (20+)
         ├─► Create Adam (idealized traits)
         ├─► Create Eve (idealized traits)
         └─► Marry Adam & Eve
         │
         ▼
    DAILY LOOP (Background Thread)
         │
         ├─► DEATHS
         │   ├─ Calculate death chance (age + health + job risk)
         │   ├─ Early population protection
         │   └─ Handle ruler succession if needed
         │
         ├─► JOB ASSIGNMENTS
         │   ├─ Find eligible people (age 12+, no job)
         │   ├─ Match to job requirements (traits, age)
         │   └─ Update social status
         │
         ├─► MARRIAGES
         │   ├─ Find eligible singles (age 14+)
         │   ├─ Check compatibility (family, country, religion)
         │   ├─ Early population: relaxed rules
         │   └─ Random chance to marry (10% per day)
         │
         ├─► PREGNANCIES
         │   ├─ Find married, fertile females
         │   ├─ Calculate pregnancy chance (population phase)
         │   ├─ Fertility & health modifiers
         │   └─ Twins/triplets (1% / 4% chance)
         │
         ├─► BIRTHS
         │   ├─ Find pregnancies due today
         │   ├─ Inherit traits from parents
         │   ├─ Apply variation & mutation
         │   ├─ Generate culturally appropriate name
         │   └─ Add to population
         │
         ├─► YEARLY EVENTS (Day 1 of Year)
         │   ├─ Found Cities (threshold: 100+ population)
         │   ├─ Found Countries (threshold: 3+ cities)
         │   ├─ Found Religions (charismatic founders)
         │   ├─ Discover Inventions (intelligent individuals)
         │   └─ Start Wars (between countries)
         │
         ├─► DATABASE SYNC (Every 100 Days)
         │   ├─ Batch save all people
         │   ├─ Save cities, countries, religions
         │   ├─ Save inventions, wars, dynasties
         │   └─ Save events log
         │
         └─► UI UPDATE
             ├─ Calculate statistics
             ├─ Format recent events
             ├─ Update console display
             └─ Throttle to 10 FPS
         │
         ▼
    [LOOP CONTINUES]
         │
         ▼
    USER QUIT (Q key)
         │
         ├─► Final database sync
         ├─► Display shutdown screen
         └─► Exit application
```

## Memory Structure

```
┌──────────────────────────────────────────────────────────────────┐
│                     IN-MEMORY STATE                              │
│                                                                  │
│  Lists (for iteration):                                         │
│    • _people              List<Person>                          │
│    • _cities              List<City>                            │
│    • _countries           List<Country>                         │
│    • _religions           List<Religion>                        │
│    • _jobs                List<Job>                             │
│    • _inventions          List<Invention>                       │
│    • _wars                List<War>                             │
│    • _dynasties           List<Dynasty>                         │
│    • _recentEvents        List<Event> (max 1000)               │
│                                                                  │
│  Dictionaries (for O(1) lookup):                                │
│    • _peopleById          Dictionary<long, Person>             │
│    • _citiesById          Dictionary<long, City>               │
│    • _countriesById       Dictionary<long, Country>            │
│    • _religionsById       Dictionary<long, Religion>           │
│    • _jobsById            Dictionary<long, Job>                │
│    • _inventionsById      Dictionary<long, Invention>          │
│    • _dynastiesById       Dictionary<long, Dynasty>            │
│                                                                  │
│  Metadata:                                                       │
│    • _currentDate         DateTime                             │
│    • _nextTempId          long (negative IDs)                  │
│    • _generationNumber    int                                  │
│    • _syncCounter         int                                  │
└──────────────────────────────────────────────────────────────────┘
```

## Threading Model

```
┌─────────────────────────────────────────────────────────────┐
│                   MAIN THREAD                               │
│  • User input processing                                    │
│  • Speed control                                            │
│  • Quit handling                                            │
└────────────────────────────┬────────────────────────────────┘
                             │
                             │ spawns
                             ▼
┌─────────────────────────────────────────────────────────────┐
│               BACKGROUND THREAD                             │
│  • Simulation loop (daily & yearly events)                 │
│  • Statistics calculation                                   │
│  • UI updates                                               │
│                                                             │
│  Synchronized via:                                          │
│    • Read-only stats for UI                                │
│    • No shared mutable state                               │
└─────────────────────────────────────────────────────────────┘
```

## Key Algorithms Visualized

### Trait Inheritance
```
Parent 1 Trait: 70        Parent 2 Trait: 80
        │                          │
        └──────────┬───────────────┘
                   ▼
            Average: 75
                   │
    ┌──────────────┼──────────────┐
    │              │              │
    ▼              ▼              ▼
Variation      Mutation       Clamp
(-10 to +10)   (5% chance)   (0-100)
Random: +5     Random: 0     Result: 80
```

### Population Phase Effects
```
Population Size    Pregnancy Chance    Death Protection
     0-50         ────► 10%            ────► Very High
    50-100        ────► 5%             ────► High
   100-500        ────► 2%             ────► Medium
    500+          ────► 1%             ────► Normal
```

This architecture enables:
- ✅ Fast simulation (thousands of individuals)
- ✅ Real-time updates
- ✅ Persistent storage
- ✅ Emergent complexity
- ✅ Extensibility
