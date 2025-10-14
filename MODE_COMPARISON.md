# Console vs Web Mode Comparison

## Feature Comparison

| Feature | Console Mode | Web Mode |
|---------|--------------|----------|
| **Platform** | Terminal/Console | Web Browser |
| **Framework** | RazorConsole + Spectre.Console | Blazor Server |
| **UI Style** | Text-based with ANSI colors | HTML/CSS with Bootstrap |
| **Real-time Updates** | ✅ Yes (500ms) | ✅ Yes (500ms via SignalR) |
| **Speed Controls** | ✅ Yes (Tab + Enter) | ✅ Yes (Button clicks) |
| **Population Stats** | ✅ Yes | ✅ Yes |
| **Civilization Progress** | ✅ Yes | ✅ Yes |
| **Top Jobs** | ✅ Yes | ✅ Yes |
| **Family Tree** | ✅ Yes | ✅ Yes |
| **Recent Events** | ✅ Yes | ✅ Yes |
| **Responsive Design** | N/A | ✅ Yes |
| **Mobile Support** | ❌ No | ✅ Yes |
| **Accessibility** | Screen reader limited | ✅ Better screen reader support |
| **Database** | ✅ SQLite | ✅ SQLite (shared) |
| **Simulation Engine** | ✅ Shared | ✅ Shared |

## When to Use Each Mode

### Console Mode
- **Best for:**
  - Developers and power users
  - Server environments without GUI
  - Quick testing and debugging
  - Scripting and automation
  - Low resource usage
  - Terminal enthusiasts

- **Example use cases:**
  - Running on a headless server
  - SSH sessions
  - CI/CD environments
  - Terminal multiplexer setups (tmux, screen)

### Web Mode
- **Best for:**
  - End users and non-technical users
  - Demonstrations and presentations
  - Remote access via browser
  - Mobile device access
  - Sharing with others (via URL)
  - Better visual experience

- **Example use cases:**
  - Showing simulation to stakeholders
  - Accessing from any device
  - Running on a remote server and accessing via browser
  - Embedding in presentations
  - Educational purposes

## Starting Each Mode

### Console Mode
```bash
cd PopulationSimulator.Console
dotnet run
```

### Web Mode
```bash
cd PopulationSimulator.Web
dotnet run
```

Then open browser to `https://localhost:5001`

## Shared Components

Both modes use the **exact same simulation engine** from `PopulationSimulator.Shared`:

- `Simulator.cs` - Core simulation logic
- `SimulatorService.cs` - Service layer for lifecycle management
- `DataAccessLayer.cs` - Database operations
- All Models (Person, City, Country, etc.)
- `NameGenerator.cs` - Cultural name generation

This ensures:
- Consistent behavior between modes
- No duplicate code
- Single source of truth
- Easy maintenance

## Performance

Both modes have similar performance characteristics:
- Simulation runs at 50ms tick rate
- UI updates throttled to 500ms
- Support for 1x to 100x speed multiplier
- In-memory data structures for fast access
- Periodic database syncing

## Database

Both modes share the same database (`population.db`):
- **Warning**: Don't run both modes simultaneously on the same database
- Each mode can read databases created by the other
- Database contains full history and state
- SQLite format for portability

## Future Enhancements

Possible additions for either mode:
- Pause/Resume functionality
- Save/Load simulation state
- Export data to CSV/JSON
- Multiple simulation profiles
- Multiplayer/shared simulations (web mode)
- API endpoint for external tools
- WebSocket API for custom clients
