# Advanced Population Simulator

A comprehensive .NET 9 application that simulates the emergence and evolution of human society from a single pair of individuals (Adam and Eve) through multiple generations. Available in both **console mode** (terminal-based) and **web mode** (browser-based with advanced visualizations).

## Available Modes

### 🖥️ Console Mode
Terminal-based interface using RazorConsole and Spectre.Console for a rich console experience.

### 🌐 Web Mode ⭐ **NEW!**
Modern web-based interface using Blazor Server with real-time updates, interactive charts, world map visualization, and comprehensive data management.

Both modes provide the complete simulation experience with all features!

---

## 🚀 New Features (Latest Update)

### 💾 **Save/Load System**
- **Complete State Persistence**: Save and load entire simulations
- **JSON Format**: Human-readable save files with all simulation data
- **100MB File Support**: Handle large, complex civilizations
- **One-Click Export/Import**: Simple file download and upload
- **Preserve Everything**: People, cities, schools, universities, inventions, businesses, and more

### 🔍 **Search & Filter System**
- **Multi-Entity Search**: Search people, cities, countries, and inventions
- **Advanced Filters**:
  - **People**: Filter by gender, alive/dead status, age range
  - **Cities**: Filter by geography type, climate, minimum population
  - **Countries & Inventions**: Quick name search
- **Live Results**: Real-time filtering as simulation runs
- **Export Results**: CSV export of filtered data

### 🎓 **Education System**
- **Schools & Universities**: Dynamically founded based on population
- **Education Progression**: None → Primary → Secondary → University
- **Literacy System**: Reading/writing skills affect job opportunities
- **Knowledge Transfer**: Educated parents boost children's intelligence
- **School Quality**: Better schools = smarter students
- **University Research**: Universities contribute to invention discovery
- **Notable Graduates**: Prestigious universities create famous individuals
- **Statistics**: Track literacy rate, enrollment, and education distribution

### 📊 **Advanced Charts (Web Only)**
Four interactive real-time charts:
- **Population Over Time**: Line chart tracking civilization growth
- **Birth & Death Rates**: Dual-line comparison chart
- **Invention Timeline**: Bar chart showing discoveries by decade
- **Education Distribution**: Doughnut chart of education levels
- **Real-Time Updates**: Charts refresh automatically every 500ms
- **Powered by Chart.js**: Professional, responsive visualizations

### 🗺️ **Interactive World Map (Web Only)**
- **Visual City Display**: See all cities on an interactive map
- **Three Visualization Modes**:
  - **Geography**: Color-coded by terrain (Coastal, Mountain, Plains, Desert, Forest, etc.)
  - **Climate**: Color-coded by climate type (Tropical, Temperate, Arid, Arctic, Mediterranean)
  - **Population**: Heat map showing population density (green to red)
- **Dynamic Sizing**: City markers scale with population
- **Hover Details**: Tooltips show city name, population, geography, and climate
- **Visual Legends**: Clear explanations for each mode

---

## Core Features

### Individual Life Simulation
- **Unique Traits**: Each person has 10+ inheritable attributes
- **Advanced Genetics**: DNA sequences, blood types, hereditary conditions
- **Physical Traits**: Eye color, hair color, skin tone, height, weight, build type
- **Disease Resistance & Longevity**: Genetic predispositions
- **Life Events**: Birth, death, marriage, pregnancy, job assignments, education

### Trait Inheritance
- **Genetic Blending**: Children inherit traits from both parents
- **Mutations**: Random variations introduce diversity
- **Dominant/Recessive**: Blood type inheritance follows genetic rules
- **Environmental Factors**: Education and upbringing affect intelligence

### Societal Structures
- **Cities** (7 Geography Types, 5 Climate Types):
  - Coastal, Mountain, Plains, Desert, Forest, River Valley, Island
  - Tropical, Temperate, Arid, Arctic, Mediterranean climates
  - Population tracking, wealth accumulation, founder recognition

- **Countries**:
  - Rulers and dynasties with succession rules
  - Military power and territorial control
  - Government structures

- **Religions**:
  - Founded by charismatic individuals
  - Spread through populations
  - Moral codes and traditions

- **Jobs** (75+ Occupations):
  - Gender and age-specific roles
  - Intelligence, strength, and education requirements
  - Risk factors (warriors, miners, sailors have higher death rates)

- **Inventions** (120+ Discoveries):
  - Discovered by intelligent individuals and universities
  - Health bonuses and lifespan improvements
  - Technology progression

- **Wars**:
  - Conflicts between countries
  - Population casualties
  - Power shifts

- **Businesses**:
  - Dynamic founding and management
  - Employee tracking
  - Innovation and invention discovery
  - Rise and fall based on performance

- **Natural Disasters** (Geography-Aware):
  - Earthquakes (Mountain regions)
  - Tsunamis (Coastal cities)
  - Hurricanes (Tropical climates)
  - Floods (River Valleys)
  - Volcanoes (Island/Mountain)
  - Droughts (Desert/Arid)
  - Invention-based mitigation (earthquake-resistant buildings, levees, etc.)

### Cultural Evolution
- **Dynamic Naming System**:
  - Diverse first names (500+ options)
  - Early generations: Patronymic names (e.g., "ben Adam")
  - Middle generations: City-based names (e.g., "of Cyrus")
  - Later generations: Hereditary surnames from occupations (e.g., "Smith")

- **Dynasties**:
  - Royal families with succession rules
  - Noble lineages

- **Social Status**:
  - Affected by jobs, wealth, and leadership positions
  - Notable people tracking (inventors, rulers, etc.)

### Genius System
- **Exceptional Individuals**:
  - Born with enhanced intelligence (120-150 IQ)
  - Major contributors to inventions and discoveries
  - Leadership roles in society

---

## Technical Features

### Performance
- **In-Memory Optimization**: Fast simulation using dictionaries and efficient data structures
- **O(1) Lookups**: Quick entity access via ID dictionaries
- **Cached Living People**: Avoid repeated filtering
- **Periodic Database Sync**: SQLite persistence every 100 days
- **Chart Performance**: No-animation updates for smooth real-time visualization
- **Efficient LINQ**: Minimized allocations, direct iterations where possible

### Data Management
- **SQLite Database**: Long-term persistence with 10+ tables
- **JSON Save/Load**: Human-readable simulation snapshots
- **CSV Export**: Export any entity type for analysis
- **Full State Preservation**: Every detail saved and restorable

### User Interface (Web Mode)
- **Dark Mode**: Toggle between light and dark themes
- **Keyboard Shortcuts**:
  - Space: Pause/Resume
  - +/-: Adjust speed
- **Tooltips**: Hover explanations on all statistics
- **Toast Notifications**: Major events (population milestones, first city, inventions)
- **Responsive Design**: Works on desktop and mobile
- **Real-Time Updates**: 500ms refresh for smooth statistics

### User Interface (Console Mode)
- **RazorConsole**: Component-based terminal UI
- **Spectre.Console**: Rich formatting and colors
- **Interactive Controls**: Speed adjustment, pause/resume, restart
- **Live Statistics**: Population, cities, inventions, events
- **Family Trees**: Visual ancestor trees with Unicode characters

---

## Requirements

- .NET 9.0 SDK or later
- Windows, Linux, or macOS
- Modern web browser (for web mode - Chrome, Firefox, Edge recommended)

---

## Installation

1. Clone the repository:
```bash
git clone https://github.com/rhale78/PopulationSimulator.git
cd PopulationSimulator
```

2. Build the project:
```bash
dotnet build
```

---

## Usage

### 🌐 Web Mode (Recommended)

Run the web-based simulator:
```bash
cd PopulationSimulator.Web
dotnet run
```

Then open your browser to `https://localhost:5001` (or the URL shown in console).

**Web Features:**
- ✅ Real-time interactive charts
- ✅ World map visualization with 3 modes
- ✅ Save/load simulations
- ✅ Advanced search and filtering
- ✅ Education system statistics
- ✅ Dark mode toggle
- ✅ Keyboard shortcuts
- ✅ Export data to CSV/JSON
- ✅ Responsive design

### 🖥️ Console Mode

Run the terminal-based simulator:
```bash
cd PopulationSimulator.Console
dotnet run
```

**Console Controls:**
- Press any key at welcome screen to start
- Tab: Switch focus between controls
- Enter: Activate focused button
- Ctrl+C: Quit and save to database

---

## Project Architecture

### Structure
```
PopulationSimulator/
├── PopulationSimulator.Shared/    # Shared library
│   ├── Models/                    # Data models (Person, City, School, University, etc.)
│   ├── Core/                      # Simulation logic (Simulator, GeneticsEngine, GeniusSystem)
│   ├── Data/                      # Database access (SQLite)
│   └── Services/                  # Simulation service with events
├── PopulationSimulator.Console/   # Console mode
│   ├── Components/                # RazorConsole components
│   └── Program.cs                 # Console entry point
└── PopulationSimulator.Web/       # Web mode
    ├── Components/                # Blazor components
    │   ├── Pages/                 # Home.razor (main UI)
    │   └── App.razor              # Root component
    ├── wwwroot/                   # Static files
    │   ├── app.js                 # Chart.js integration
    │   └── app.css                # Dark mode styles
    └── Program.cs                 # Web entry point
```

### Key Classes

#### Models
- **Person**: Individual with 50+ properties (genetics, education, jobs, relationships)
- **City**: Settlement with geography, climate, population, wealth
- **School**: Educational institution (Primary/Secondary)
- **University**: Advanced education with research and prestige
- **Country**: Nation with ruler, military, territory
- **Religion**: Belief system with founder and followers
- **Job**: Occupation with requirements and risk factors
- **Invention**: Discovery with health/lifespan bonuses
- **Business**: Economic entity with employees and innovation
- **NaturalDisaster**: Geography-aware catastrophic events

#### Core Systems
- **Simulator**: Main simulation engine with 2500+ lines of logic
- **GeneticsEngine**: DNA, blood types, inheritance, mutations
- **GeniusSystem**: Exceptional individuals with high intelligence
- **NameGenerator**: Culturally appropriate names for people and places
- **DataAccessLayer**: SQLite database operations

#### Services
- **SimulatorService**: Event-based simulation management
  - OnStatsUpdated: Real-time statistics
  - OnSpeedChanged: Speed control
  - OnPausedChanged: Pause state
  - OnSimulationRestarted: Reset notifications

### Simulation Flow

1. **Initialization**:
   - Creates Adam and Eve (born Year 1, age 20 at start)
   - Perfect traits (100 in all attributes)
   - Primordial DNA sequences
   - Generation 0

2. **Daily Loop**:
   - **Deaths**: Based on age, health, job risk, disasters
   - **Education**: Student assignment, graduation, knowledge transfer
   - **Jobs**: Assign eligible people to occupations
   - **Marriages**: Compatible individuals pair up
   - **Pregnancies**: Chance-based with twins/triplets
   - **Births**: New generation with inherited traits

3. **Yearly Events**:
   - **Cities**: Founded when population > 100
   - **Countries**: Established from cities when cities > 3
   - **Religions**: Founded by charismatic individuals
   - **Inventions**: Discovered by intelligent people and universities
   - **Schools**: Founded when population > 100
   - **Universities**: Founded when population > 1000, cities > 2, schools > 5
   - **Businesses**: Created and managed
   - **Disasters**: Geography-specific catastrophes
   - **Wars**: Conflicts between countries

4. **Database Sync**: Every 100 days to SQLite

---

## Database Schema

SQLite database with comprehensive tables:
- **People**: All individuals (living and dead)
- **Cities**: Settlements with geography/climate
- **Schools**: Educational institutions
- **Universities**: Advanced education centers
- **Countries**: Nations and territories
- **Religions**: Belief systems
- **Jobs**: Occupations
- **Inventions**: Technological discoveries
- **Wars**: Conflicts
- **Events**: Historical log
- **Dynasties**: Royal families
- **Businesses**: Economic entities
- **BusinessEmployees**: Employment relationships
- **NaturalDisasters**: Catastrophic events

---

## Customization

### Adding New Jobs
Edit `SeedJobs()` in `Simulator.cs`:
```csharp
_jobs.Add(new Job
{
    Name = "New Occupation",
    MinIntelligence = 50,
    MinStrength = 40,
    DeathRiskModifier = 1.2,
    RequiresEducation = true,
    MinimumEducation = "Primary"
});
```

### Adjusting Education Thresholds
Modify in `ProcessYearlyEvents()`:
```csharp
// School founding
if (population > 100 && _schools.Count < _cities.Count * 3)
    FoundSchool();

// University founding
if (population > 1000 && _cities.Count > 2 && _schools.Count > 5)
    FoundUniversity();
```

### Changing Death Rates
Adjust `CalculateDeathChance()` method for mortality curves.

### Adding Chart Types
Add new charts in `app.js` and update `Home.razor`:
```javascript
window.charts.createCustomChart = function(canvasId, data) {
    // Chart.js configuration
};
```

---

## Export Formats

### CSV Exports (All Entity Types)
- Population (with genetics, education, jobs)
- Cities (with geography, climate, wealth)
- Schools (with quality, enrollment, teachers)
- Universities (with prestige, research, students)
- Countries (with rulers, military, territory)
- Inventions (with discovery dates, bonuses)
- Disasters (with casualties, types, dates)
- Businesses (with owners, revenue, status)
- Events (full historical log)

### JSON Export
- Complete simulation state
- All entities and relationships
- Full restoration capability

---

## Performance Benchmarks

- **Small Simulation** (100-500 people): 100x speed achievable
- **Medium Simulation** (1000-5000 people): 50x speed typical
- **Large Simulation** (10,000+ people): 10-20x speed
- **Chart Updates**: Smooth at 500ms intervals
- **Database Sync**: ~50ms for 10,000 people every 100 days

---

## Known Limitations

- **Web Mode**: Chart.js requires modern browser (IE not supported)
- **Save Files**: Not backward compatible with pre-education system versions
- **Map Display**: Limited to 100 cities in grid layout
- **File Size**: 100MB upload limit (configurable)

---

## Roadmap

### Completed ✅
- ✅ Advanced genetics with DNA and blood types
- ✅ 120+ inventions with health bonuses
- ✅ 75+ jobs with gender/age requirements
- ✅ Geography-aware natural disasters
- ✅ Genius system for exceptional individuals
- ✅ Comprehensive naming evolution
- ✅ Businesses with innovation
- ✅ **Save/load system**
- ✅ **Search and filter**
- ✅ **Education system**
- ✅ **Real-time charts**
- ✅ **World map visualization**

### Potential Future Features
- [ ] Disease and epidemic simulation
- [ ] Migration between cities
- [ ] Trade routes and economics
- [ ] Diplomacy and alliances
- [ ] Cultural traditions
- [ ] Technology prerequisites/trees
- [ ] Climate change impacts
- [ ] More map modes (wars, trade routes, religions)
- [ ] 3D city visualization
- [ ] Multiplayer scenarios

---

## Contributing

Contributions are welcome! Areas for contribution:
- Additional chart types
- Map enhancements
- New disaster types
- Educational specializations
- Performance optimizations
- Unit tests
- Documentation improvements

Please submit pull requests or open issues for bugs and feature requests.

---

## License

This project is open source and available under the MIT License.

---

## Credits

- **Chart.js**: Data visualization (https://www.chartjs.org/)
- **Bootstrap**: UI framework
- **Blazor**: Web framework
- **Spectre.Console**: Terminal UI
- **RazorConsole**: Console rendering

---

## Author

Created as a comprehensive simulation engine for studying population dynamics, social evolution, cultural change, and the emergence of complex societies from simple beginnings.

**Latest Update**: Complete feature enhancement suite with save/load, search, education, charts, and map visualization.
