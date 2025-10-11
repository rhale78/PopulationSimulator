# Quick Start Guide

## Getting Started with Advanced Population Simulator

### Prerequisites
- .NET 9.0 SDK installed on your system
- Terminal or command prompt

### Running the Simulator

1. **Navigate to the project directory:**
   ```bash
   cd PopulationSimulator
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Welcome Screen:**
   - You'll see a welcome message explaining the simulation
   - Press any key to begin

4. **Simulation Running:**
   - The console will display real-time statistics
   - Watch as Adam and Eve's descendants grow into a civilization

### Understanding the Display

The simulation display shows:

#### Population Statistics
- **Living Population**: Current number of living individuals
- **Total Births**: All births since simulation start
- **Total Deaths**: All deaths that have occurred
- **Total Marriages**: Number of married couples

#### Civilization Progress
- **Cities**: Number of founded cities
- **Countries**: Number of established nations
- **Religions**: Number of different religions
- **Inventions**: Technological discoveries
- **Wars**: Conflicts between countries

#### Recent Events
- Live feed of significant events as they happen
- Color-coded by event type:
  - Green: Births
  - Red: Deaths
  - Yellow: Marriages, Inventions
  - Cyan: New Cities
  - Magenta: New Countries
  - Blue: New Religions
  - Dark Red: Wars

### Controls During Simulation

- **`+` or `=`**: Increase simulation speed (1x to 100x)
- **`-`**: Decrease simulation speed
- **`Q`**: Quit and save to database

### Simulation Phases

1. **Early Population (0-50 people)**
   - High birth rates to ensure survival
   - Relaxed marriage rules
   - Protection from early deaths

2. **Growing Population (50-100 people)**
   - Moderate birth rates
   - Cities begin to form
   - Job assignments increase

3. **Established Civilization (100+ people)**
   - Normal birth/death rates
   - Countries and religions emerge
   - Wars and inventions occur
   - Complex family trees develop

### What to Expect

#### First Minutes
- Adam and Eve marry
- First children are born
- Names follow patronymic pattern (e.g., "Seth ben Adam")
- Population grows exponentially

#### After 10-20 Minutes (Simulated Time: Years)
- Multiple generations exist
- First city founded
- Job diversity increases
- Family trees become complex

#### After 30+ Minutes (Simulated Time: Decades)
- Countries established
- Religions founded
- Inventions discovered
- Wars between nations
- Dynasties formed
- Surnames evolve to occupation-based

### Database

The simulation automatically saves to `population.db` every 100 simulated days.

You can inspect this SQLite database with any SQLite viewer to see:
- Complete family trees
- Historical events
- City/country information
- All person details and traits

### Tips for Interesting Simulations

1. **Watch for Bottlenecks**: Early population can hit marriage bottlenecks if siblings avoid each other
2. **Speed Control**: Start slow (1x) to watch early events, then speed up (10x+) as population grows
3. **Trait Inheritance**: Notice how traits like intelligence cluster in certain family lines
4. **Dynasty Formation**: High-leadership individuals often found countries and dynasties
5. **War Impact**: Wars can significantly affect population numbers

### Troubleshooting

**Issue**: Simulation seems stuck at low population
- **Solution**: This is by design for early protection. Population will grow exponentially once established.

**Issue**: No countries or cities appearing
- **Solution**: These require sufficient population (100+ for cities, multiple cities for countries)

**Issue**: Console display flickering
- **Solution**: This is normal due to rapid updates. Increase simulation speed to reduce update frequency.

### Next Steps

After running your first simulation:
1. Examine the database file to see detailed data
2. Experiment with different simulation speeds
3. Watch how different traits affect outcomes
4. Observe the evolution of naming conventions
5. Track specific family lineages through the data

### Customization

To modify simulation parameters, edit these files:
- `Core/Simulator.cs` - Main simulation logic, rates, and thresholds
- `Core/NameGenerator.cs` - Names and cultural elements
- `Models/` - Data structures and properties

Enjoy watching your civilization grow from Adam and Eve to a complex, thriving society!
