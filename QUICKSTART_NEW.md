# Quick Start Guide

Get the Population Simulator running in under 2 minutes!

## Prerequisites

- .NET 9.0 SDK ([Download here](https://dotnet.microsoft.com/download/dotnet/9.0))
- A terminal/command prompt
- A web browser (for web mode)

## Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/rhale78/PopulationSimulator.git
   cd PopulationSimulator
   ```

2. **Build everything**
   ```bash
   dotnet build
   ```

That's it! You're ready to run.

## Choose Your Mode

### Option 1: Web Mode (Easiest)

1. Start the web server:
   ```bash
   cd PopulationSimulator.Web
   dotnet run
   ```

2. Open your browser to the URL shown (typically `https://localhost:5001`)

3. Watch the simulation run! 
   - Use the "Speed Up" and "Slow Down" buttons to control speed
   - The page updates automatically every 500ms

4. Stop the simulation: Press `Ctrl+C` in the terminal

### Option 2: Console Mode

1. Start the console app:
   ```bash
   cd PopulationSimulator.Console
   dotnet run
   ```

2. Press any key at the welcome screen

3. Watch the simulation in your terminal!
   - Use Tab to switch between controls
   - Press Enter to activate buttons
   - Press Ctrl+C to quit

## What You'll See

The simulation starts with Adam and Eve (both age 20, perfect traits) and tracks:

- **Population growth** through births, deaths, and marriages
- **Family trees** showing genealogy
- **Civilization development** as cities, countries, and religions form
- **Technology progress** with inventions discovered
- **Job assignments** as the population grows
- **Recent events** showing births, deaths, marriages, wars, etc.

## Typical Timeline

- **Years 1-50**: Adam and Eve have children, first generation
- **Years 50-100**: Second generation, population reaches ~20-50
- **Years 100-200**: First cities founded, technology discoveries
- **Years 200-500**: Countries form, religions emerge
- **Years 500+**: Complex civilization with wars, dynasties, etc.

## Speed Control

- **1x**: Normal speed (good for watching details)
- **10x**: Fast forward through early years
- **50x**: Very fast, good for reaching civilization stage
- **100x**: Maximum speed

## Database

The simulation saves to `population.db` in the application directory:
- All people with full history
- All events, cities, countries, etc.
- Persists between runs
- Delete `population.db` to start fresh

## Tips

1. **Start with web mode** - it's easier to use and looks better
2. **Use higher speeds** (10x-50x) to get through early years quickly
3. **Watch the family tree** - it's fascinating to see lineages develop
4. **Check recent events** - they tell the story of what's happening
5. **Be patient** - complex civilizations take time (500+ years)

## Troubleshooting

### "Port already in use" (Web mode)
Run on a different port:
```bash
dotnet run --urls "http://localhost:5005"
```

### Build errors
Make sure you have .NET 9.0 SDK:
```bash
dotnet --version
```

### Database locked
Stop any other running instances of the application.

## Next Steps

- Read [WEB_README.md](WEB_README.md) for detailed documentation
- Check [MODE_COMPARISON.md](MODE_COMPARISON.md) to understand both modes
- Read the main [README.md](README.md) for full feature list
- Explore the code in `PopulationSimulator.Shared/Core/Simulator.cs`

## Need Help?

- Open an issue on GitHub
- Check existing documentation
- Look at the code comments

Enjoy watching humanity evolve! 🌍👨‍👩‍👧‍👦
