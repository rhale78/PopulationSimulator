# Population Simulator - Web Mode

This document describes how to use the web-based version of the Population Simulator.

## Project Structure

The solution now consists of three projects:

1. **PopulationSimulator.Shared** - Shared library containing:
   - Core simulation logic (`Simulator.cs`)
   - Data access layer (`DataAccessLayer.cs`)
   - Models (Person, City, Country, Religion, etc.)
   - Services (`SimulatorService.cs`)
   - Name generator

2. **PopulationSimulator.Console** - Console application using RazorConsole
   - Terminal-based UI with Spectre.Console
   - Interactive console components
   - Same simulation features as web

3. **PopulationSimulator.Web** - Blazor Server web application
   - Modern web-based UI
   - Real-time updates via SignalR
   - Responsive design with Bootstrap
   - All simulation features

## Running the Web Application

### Prerequisites
- .NET 9.0 SDK or later
- Modern web browser (Chrome, Firefox, Edge, Safari)

### Starting the Web Server

From the solution root:
```bash
cd PopulationSimulator.Web
dotnet run
```

The application will start on `https://localhost:5001` (or `http://localhost:5000`).

### Using the Web Interface

1. **Open your browser** to the URL shown in the console (typically https://localhost:5001)

2. **The simulation starts automatically** when the page loads

3. **Monitor the simulation** through multiple panels:
   - **Population Statistics** - Total population, births, deaths, marriages
   - **Civilization Progress** - Cities, countries, religions, inventions, wars
   - **Top Occupations** - Most common jobs in the population
   - **Family Tree** - Adam & Eve's lineage showing up to 5 generations
   - **Recent Events** - Last 10 significant events

4. **Control the simulation speed**:
   - Click "➕ Speed Up" to increase simulation speed (up to 100x)
   - Click "➖ Slow Down" to decrease simulation speed (minimum 1x)

5. **The UI updates automatically** every 500ms with the latest statistics

## Running the Console Application

From the solution root:
```bash
cd PopulationSimulator.Console
dotnet run
```

The console version provides the same simulation in a terminal-based interface using RazorConsole.

## Features

Both console and web modes provide:

- **Starting with Adam and Eve** at age 20 with perfect genetic traits
- **Realistic population dynamics**:
  - Age-based marriage and reproduction
  - Genetic trait inheritance (intelligence, strength, health, fertility, etc.)
  - Age and health-based mortality
  - Family relationships tracking

- **Civilization development**:
  - City founding when population reaches thresholds
  - Country formation from multiple cities
  - Religion emergence and spread
  - Technology/invention discovery
  - Job system with requirements
  - Wars between countries
  - Law enactment

- **Detailed tracking**:
  - Every person with full genealogy
  - Event logging (births, deaths, marriages, etc.)
  - Family tree visualization
  - Statistical summaries

## Database

Both applications save simulation data to a SQLite database (`population.db`) in the application directory. The database contains:
- People (with all traits and relationships)
- Cities, Countries, Religions
- Jobs and Inventions
- Wars and Dynasties
- Events and Laws

## Customization

To modify the simulation parameters, edit the `Simulator.cs` file in the `PopulationSimulator.Shared` project:

- Adjust pregnancy chances in `ProcessPregnancies()`
- Modify death rates in `CalculateDeathChance()`
- Add new jobs in `SeedJobs()`
- Change city/country formation thresholds
- Customize invention discovery rates

## Development

### Building the Solution
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Project References
- The Console and Web projects both reference the Shared library
- The Shared library has no external dependencies except SQLite
- The Console project uses RazorConsole and Spectre.Console
- The Web project uses Blazor Server with Bootstrap

## Troubleshooting

### Port Already in Use
If you get an error that the port is already in use, specify a different port:
```bash
dotnet run --urls "http://localhost:5005"
```

### Database Locked
If the database is locked, make sure no other instance of the application is running.

### UI Not Updating
If the web UI stops updating:
1. Check the browser console for errors
2. Refresh the page
3. Restart the application

## Performance

- The simulation runs at 50ms tick rate
- UI updates are throttled to 500ms to prevent excessive rendering
- Web mode uses SignalR for real-time updates
- Both modes support speeds from 1x to 100x

## License

MIT License - See LICENSE file for details
