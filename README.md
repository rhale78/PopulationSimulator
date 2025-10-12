# Advanced Population Simulator

A comprehensive .NET 9 console application that simulates the emergence and evolution of human society from a single pair of individuals (Adam and Eve) through multiple generations.

## Features

### Core Simulation
- **Individual Life Simulation**: Each person has unique traits, genetics, and life events
- **Trait Inheritance**: Children inherit traits from parents with random variation and mutations
- **Life Events**: Birth, death, marriage, pregnancy, and job assignments
- **Genetic Diversity**: 10+ inheritable traits including intelligence, strength, health, fertility, charisma, creativity, leadership, aggression, wisdom, and beauty

### Societal Structures
- **Cities**: Founded when population thresholds are met
- **Countries**: Emerge from cities with rulers, dynasties, and governments
- **Religions**: Can be founded by charismatic individuals and spread through populations
- **Jobs**: 20+ different occupations with varying requirements and risks
- **Inventions**: Discovered by intelligent individuals, unlocking new opportunities
- **Wars**: Conflicts between countries affecting populations and power structures

### Cultural Evolution
- **Dynamic Naming**: 
  - Early generations use patronymic names (e.g., "ben Adam")
  - Middle generations use city-based names (e.g., "of Cyrus")
  - Later generations use occupation-based surnames (e.g., "Smith")
- **Dynasties**: Royal families with succession rules
- **Social Status**: Affected by jobs, wealth, and leadership positions

### Technical Features
- **In-Memory Performance**: Fast simulation using optimized data structures
- **Database Persistence**: SQLite database for long-term data storage
- **Real-Time Console UI**: Live statistics, events, and family trees
- **Adjustable Speed**: Control simulation speed from 1x to 100x
- **Event Logging**: Comprehensive tracking of all significant events

## Requirements

- .NET 9.0 SDK or later
- Windows, Linux, or macOS

## Installation

1. Clone the repository:
```bash
git clone https://github.com/rhale78/PopulationSimulator.git
cd PopulationSimulator
```

2. Build the project:
```bash
cd PopulationSimulator
dotnet build
```

## Usage

Run the simulator:
```bash
dotnet run
```

### Controls
- Press any key at the welcome screen to start
- `+` or `=` - Increase simulation speed
- `-` - Decrease simulation speed
- `Q` - Quit and save to database

## Architecture

### Project Structure
```
PopulationSimulator/
├── Models/          # Data models (Person, City, Country, Religion, etc.)
├── Core/           # Core simulation logic (Simulator, NameGenerator)
├── Data/           # Database access layer
├── UI/             # Console user interface
└── Program.cs      # Application entry point
```

### Key Classes

- **Person**: Represents an individual with traits, relationships, and life events
- **Simulator**: Main simulation engine orchestrating all game logic
- **NameGenerator**: Generates culturally appropriate names for people, places, and entities
- **DataAccessLayer**: Handles all database operations with SQLite
- **ConsoleUI**: Real-time console interface with statistics and events

### Simulation Flow

1. **Initialization**: Creates Adam and Eve with idealized traits
2. **Daily Loop**:
   - Process deaths based on age, health, and job risk
   - Assign jobs to eligible individuals
   - Process marriages between compatible individuals
   - Handle pregnancies and births
3. **Yearly Events**:
   - Found new cities when population grows
   - Establish countries from successful cities
   - Create religions through charismatic founders
   - Discover inventions through intelligent individuals
   - Trigger wars between countries
4. **Database Sync**: Periodic saving to SQLite database

## Database Schema

The simulator uses SQLite with the following main tables:
- People
- Cities
- Countries
- Religions
- Jobs
- Inventions
- Wars
- Events
- Dynasties
- Laws

## Customization

### Adding New Jobs
Edit the `SeedJobs()` method in `Simulator.cs` to add new occupations with custom requirements and risk factors.

### Adjusting Population Growth
Modify pregnancy chances in `ProcessPregnancies()` to control growth rates for different population phases.

### Changing Death Rates
Adjust the `CalculateDeathChance()` method to modify mortality rates based on age, health, and other factors.

## Future Enhancements

Potential features for future development:
- Disease and epidemic simulation
- Migration between cities and countries
- Trade and economic systems
- Educational institutions
- More detailed law and moral systems
- Graphical or web-based visualization
- Climate and environmental factors
- Technology trees and prerequisites
- Diplomatic relations
- Cultural traditions and customs

## Performance

The simulator is optimized for performance:
- In-memory data structures for fast processing
- Dictionary lookups for O(1) access to entities
- Periodic database synchronization (every 100 days)
- Event queue limited to last 1000 events
- Can simulate thousands of individuals efficiently

## License

This project is open source and available under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Author

Created as a comprehensive simulation engine for studying population dynamics, social evolution, and cultural change.
