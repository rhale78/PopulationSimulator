# Population Simulator - User Guide

Complete guide to using all features of the Advanced Population Simulator.

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Basic Controls](#basic-controls)
3. [Save/Load System](#saveload-system)
4. [Search & Filter](#search--filter)
5. [Education System](#education-system)
6. [Charts & Visualization](#charts--visualization)
7. [World Map](#world-map)
8. [Export Data](#export-data)
9. [Tips & Tricks](#tips--tricks)
10. [Troubleshooting](#troubleshooting)

---

## Getting Started

### Running the Web Simulator

1. Open a terminal in the project directory
2. Navigate to the web project:
   ```bash
   cd PopulationSimulator.Web
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open your browser to the URL shown (typically `https://localhost:5001`)

### First Launch

When you first open the simulator:
- Adam and Eve are created in Year 21 (born in Year 1, age 20)
- They have perfect traits (100 in all attributes)
- The simulation starts automatically
- Statistics update every 500ms

---

## Basic Controls

### Speed Control
- **Speed Slider**: Adjust from 1x to 100x
- **Keyboard Shortcut**:
  - Press `+` to increase speed
  - Press `-` to decrease speed
- Higher speeds skip days faster but may miss some visual updates

### Pause/Resume
- **Pause Button**: Click to pause the simulation
- **Keyboard Shortcut**: Press `Space` to toggle pause/resume
- Useful for examining statistics or preparing exports

### Restart
- **Restart Button**: Starts a completely new simulation
- Warning: This will clear all current data
- Create a save file first if you want to preserve the current state

### Dark Mode
- **Toggle Button**: Switch between light and dark themes
- Preference is saved in browser localStorage
- Charts automatically adapt to dark mode

---

## Save/Load System

### Saving a Simulation

1. **Locate the Save/Load Section** (green card)
2. Click **"💾 Save Simulation"** button
3. Your browser will download a JSON file:
   - Filename format: `simulation_save_YYYY-MM-DD_HHmmss.json`
   - Contains complete simulation state
   - Includes all people, cities, schools, universities, inventions, etc.

**What's Saved:**
- All living and dead people with full genetics
- All cities with geography and climate
- All schools and universities
- All countries, religions, inventions
- All businesses, wars, disasters
- Complete event history
- Adam/Eve IDs and generation tracking
- Current simulation date

### Loading a Simulation

1. **Locate the Save/Load Section**
2. Click **"Choose File"** under "Load Saved Simulation"
3. Select your previously saved `.json` file
4. Click to upload (maximum 100MB)
5. Wait for confirmation message

**After Loading:**
- Simulation pauses automatically
- All statistics update to reflect loaded state
- Charts refresh with historical data
- Map shows loaded cities
- You can resume from exactly where you left off

**Important Notes:**
- Save files from older versions (before education system) are not compatible
- Always save before major updates
- Keep backups of important simulations

---

## Search & Filter

### Using the Search System

1. **Locate the Search & Filter Section** (blue "info" card)
2. **Select Search Type** from dropdown:
   - People
   - Cities
   - Countries
   - Inventions

### Searching People

**Available Filters:**
1. **Name Search**: Enter any part of first or last name
2. **Gender**: Male, Female, or All
3. **Status**: Alive, Dead, or All
4. **Age Range**:
   - Min Age: Minimum age (e.g., 18)
   - Max Age: Maximum age (e.g., 65)

**Example Searches:**
- "All women over 30": Gender=Female, Min Age=30
- "Living Smiths": Query="Smith", Status=Alive
- "Children": Min Age=0, Max Age=12

**Results Show:**
- Full name
- Gender
- Current age
- Status (✅ Alive or ⚰️ Dead)
- Current city

### Searching Cities

**Available Filters:**
1. **Name Search**: City name
2. **Geography**: Coastal, Mountain, Plains, Desert, Forest, RiverValley, Island
3. **Climate**: Tropical, Temperate, Arid, Arctic, Mediterranean
4. **Minimum Population**: Only cities with at least X people

**Example Searches:**
- "Coastal cities": Geography=Coastal
- "Large cities": Min Population=500
- "Tropical islands": Geography=Island, Climate=Tropical

**Results Show:**
- City name
- Geography type
- Climate type
- Current population
- Founding year

### Searching Countries

**Features:**
- Name search only
- Shows country name, capital city, ruler, population, military power

### Searching Inventions

**Features:**
- Name search
- Shows invention name, category, discovery date, inventor, health/lifespan bonuses

### Clearing Results

Click **"Clear"** button to reset all filters and remove results.

---

## Education System

### How Education Works

Education progresses through 4 levels:
1. **None**: No formal education
2. **Primary**: Basic education (ages 6-12)
3. **Secondary**: Advanced schooling (ages 12-18)
4. **University**: Higher education (ages 18-30)

### Schools

**Founding:**
- Automatically created when population > 100
- Founded in larger cities
- Can have multiple per city (up to 3× number of cities)

**Characteristics:**
- **Type**: Primary or Secondary
- **Capacity**: 50-200 students
- **Quality Rating**: 40-90 (affects intelligence gains)
- **Teachers**: 3-15 teachers
- **Founder**: Literate, intelligent person from the city

**Effects:**
- Students gain intelligence based on school quality
- Literacy is achieved (can read and write)
- Better schools = smarter students
- Education level progresses with age

### Universities

**Founding:**
- Requires population > 1000
- Requires at least 2 cities
- Requires at least 5 schools
- Automatically created in large, prosperous cities

**Characteristics:**
- **Capacity**: 100-500 students
- **Prestige Rating**: 50-85 (affects outcomes)
- **Primary Field**: General, Science, Arts, Medicine, Engineering, Philosophy
- **Research Output**: Contributes to invention discovery
- **Professors**: 10-50 faculty

**Admission Requirements:**
- Age 18-30
- Secondary education completed
- Intelligence ≥ 70

**Effects:**
- Major intelligence boost (+10 to +25)
- Creativity and wisdom increase
- Can discover inventions through research
- Graduates from prestigious universities become notable

### Knowledge Transfer

**Parent-to-Child Education:**
- Literate parents provide small intelligence boost annually
- University-educated parents provide larger boost
- Applied on child's birthday each year
- Compounds throughout childhood
- Simulates home education and mentorship

### Literacy

**Gaining Literacy:**
- Automatically achieved in school (chance based on quality)
- Once achieved, permanent
- Affects job eligibility (some jobs require literacy)

**Literacy Rate:**
- Displayed as percentage of living population
- Shown in Civilization Progress section
- Updated in real-time

### Viewing Education Statistics

**Main Statistics:**
- Total Schools
- Total Universities
- Current Students (in schools)
- Current University Students
- Literacy Rate (percentage and count)

**Education Distribution Chart:**
- Doughnut chart showing breakdown by level
- Real-time updates as education progresses

---

## Charts & Visualization

### Chart Types

The simulator includes 4 real-time charts in the "Simulation Analytics" section:

#### 1. Population Over Time
- **Type**: Line chart with filled area
- **Shows**: Living population across simulation history
- **X-Axis**: Year number
- **Y-Axis**: Population count
- **Color**: Teal/green
- **Updates**: Automatically every 500ms

#### 2. Birth & Death Rates
- **Type**: Dual-line chart
- **Shows**: Births (blue) vs Deaths (red) over time
- **X-Axis**: Year number
- **Y-Axis**: Count per time period
- **Purpose**: Visualize population growth vs decline
- **Updates**: Real-time

#### 3. Invention Timeline
- **Type**: Bar chart
- **Shows**: Inventions discovered by decade
- **X-Axis**: Decades (e.g., "50s", "100s")
- **Y-Axis**: Number of inventions
- **Color**: Yellow/gold
- **Purpose**: See technological progress rate
- **Data**: Groups last 20 decades

#### 4. Education Distribution
- **Type**: Doughnut chart
- **Shows**: Breakdown of education levels
- **Segments**: None, Primary, Secondary, University
- **Colors**: Red, Blue, Yellow, Teal
- **Purpose**: See society's education progress
- **Updates**: Live as people get educated

### Chart Interactions

- **Hover**: Show exact values for data points
- **Legend**: Click to toggle datasets
- **Responsive**: Charts resize with window
- **Performance**: Uses no-animation mode for smooth updates

### Reading the Charts

**Population Trends:**
- Steady rise = healthy growth
- Flat line = stable population
- Decline = deaths exceeding births

**Birth/Death Balance:**
- Blue above red = population growing
- Red above blue = population declining
- Crossing lines = demographic shifts

**Invention Progress:**
- Tall bars = periods of rapid discovery
- Multiple universities boost discovery rates
- Intelligence and education drive inventions

**Education Progress:**
- Growing blue/yellow/teal = society advancing
- Shrinking red = fewer uneducated people
- Large teal segment = highly educated society

---

## World Map

### Map Overview

The interactive world map shows all cities visually positioned on a gradient background representing sky and land.

**Location:** Below Education Statistics, above Charts

### Visualization Modes

Click mode buttons to switch between:

#### 1. Geography Mode (Default)
**Colors by Terrain Type:**
- 🟦 **Royal Blue**: Coastal cities
- 🟫 **Saddle Brown**: Mountain cities
- 🟩 **Lime Green**: Plains cities
- 🟨 **Sandy Brown**: Desert cities
- 🟢 **Forest Green**: Forest cities
- 🔵 **Steel Blue**: River Valley cities
- 🏝️ **Light Sea Green**: Island cities

**Use Case:** See terrain distribution of your civilization

#### 2. Climate Mode
**Colors by Climate Type:**
- 🌴 **Tomato Red**: Tropical climate
- 🌤️ **Light Green**: Temperate climate
- 🏜️ **Burly Wood**: Arid climate
- ❄️ **Powder Blue**: Arctic climate
- 🏖️ **Gold**: Mediterranean climate

**Use Case:** Visualize climate diversity

#### 3. Population Mode
**Heat Map:**
- **Green**: Low population (0-200)
- **Yellow-Green**: Medium population (200-500)
- **Orange**: High population (500-800)
- **Red**: Very high population (800+)

**Use Case:** Identify major population centers

### City Markers

**Visual Elements:**
- **Circle Size**: Proportional to population (8px to 30px)
- **Border**: Black 2px border for visibility
- **Shadow**: Drop shadow for depth
- **Name Label**: City name below marker
- **Text Outline**: White text with black shadow for readability

**Hover Tooltip:**
Shows:
- City name
- Current population
- Geography type
- Climate type

### Map Layout

- **Grid-Based**: Cities arranged in 10×10 grid
- **Supports**: Up to 100 cities
- **Position**: Based on founding order (index)
- **Background**: Gradient from sky blue to green

### Legend

Below the map shows:
- **Geography Mode**: Terrain type emoji legend
- **Climate Mode**: Climate type emoji legend
- **Population Mode**: "Larger circles = Higher population"

---

## Export Data

### CSV Exports

Located in the "📥 Export Data" section (right panel).

**Available Exports:**
1. **Population (CSV)**: All people with full details
   - Genetics, education, jobs, family relationships
   - Good for analysis in Excel or data science tools

2. **Cities (CSV)**: All cities
   - Geography, climate, population, wealth, founders

3. **Countries (CSV)**: All nations
   - Rulers, military, territory, capitals

4. **Inventions (CSV)**: All discoveries
   - Inventors, dates, categories, bonuses

5. **Disasters (CSV)**: All natural disasters
   - Types, dates, casualties, locations

6. **Businesses (CSV)**: All companies
   - Owners, revenue, employees, status

7. **Schools (CSV)**: All educational institutions
   - Types, capacity, quality, enrollment

8. **Universities (CSV)**: All universities
   - Fields, prestige, research output, students

9. **Events (CSV)**: Complete event log
   - All significant events with timestamps

### JSON Export

**"📦 Export All (JSON)"** button:
- Exports complete simulation state
- Same format as save files
- Can be used for:
  - Data analysis
  - Custom visualizations
  - Integration with other tools
  - Backup purposes

### Using Exported Data

**CSV Files:**
- Open in Excel, Google Sheets, or Numbers
- Import into data analysis tools (Python pandas, R)
- Create custom charts and reports
- Filter and sort as needed

**JSON Files:**
- Parse with programming languages
- Create custom visualizations
- Extract specific data
- Analyze relationships

---

## Tips & Tricks

### Optimal Simulation Speeds

- **1-5x**: Best for watching events unfold in detail
- **10-20x**: Good balance of speed and visibility
- **50-100x**: Fast-forward through early years
- **Recommended**: Start at 10x, increase after population > 500

### Education Strategy

**To Maximize Literacy:**
1. Let population grow to 100+ quickly
2. First school will found automatically
3. Schools accept ages 6-18
4. Literate parents boost children's intelligence

**To Get Universities:**
1. Reach 1000+ population
2. Ensure at least 2 cities exist
3. Wait for 5+ schools to be founded
4. Universities will appear automatically

### Finding Specific People

**Best Search Approaches:**
1. **Notable People**: Use name search with their title (e.g., "of" for city-based names)
2. **Family Lines**: Search by surname (later generations)
3. **Generation Tracking**: Use age ranges (Generation 0 = oldest)

### Monitoring Civilization Health

**Key Indicators:**
- **Population Growth**: Should be steady upward trend
- **Literacy Rate**: Aim for > 50% for advanced society
- **Universities**: More universities = more inventions
- **Birth > Death**: Green line above red in chart

### Performance Tips

**For Large Simulations (5000+ people):**
1. Reduce speed to 10-20x
2. Minimize browser tab switches
3. Close other applications
4. Save regularly
5. Export data periodically

### Data Analysis Workflow

1. **Let simulation run** to desired year/population
2. **Pause** the simulation
3. **Create save file** for backup
4. **Export relevant CSVs** for analysis
5. **Resume or restart** as needed

---

## Troubleshooting

### Charts Not Displaying

**Issue**: Charts appear blank or don't update
**Solutions:**
- Ensure JavaScript is enabled
- Refresh the page (Ctrl+R or Cmd+R)
- Check browser console for errors (F12)
- Try a different modern browser (Chrome, Firefox, Edge)
- Clear browser cache

### Load File Fails

**Issue**: Cannot load saved simulation
**Possible Causes:**
1. **File too large**: Reduce to < 100MB
2. **Corrupted file**: File was edited or damaged
3. **Version mismatch**: File from old version without education system
4. **Invalid JSON**: File format is incorrect

**Solutions:**
- Check file size (right-click → Properties)
- Don't edit save files manually
- Use files from same version
- Verify JSON syntax with validator

### Slow Performance

**Issue**: Simulation runs slowly at high speeds
**Causes:**
- Very large population (10,000+)
- Too many active processes
- Browser resource constraints

**Solutions:**
- Lower simulation speed
- Close other browser tabs
- Restart browser
- Save and restart simulation
- Export and analyze data offline

### Missing Cities on Map

**Issue**: Cities exist but don't show on map
**Possible Causes:**
- More than 100 cities (grid limit)
- Browser rendering issue

**Solutions:**
- Refresh page
- Check if cities show in search results
- Export cities CSV to verify existence
- Consider restarting for new simulation

### Search Returns No Results

**Issue**: Search filters but no results appear
**Causes:**
- Filters too restrictive
- No entities match criteria
- Simulation too early (no cities/inventions yet)

**Solutions:**
- Broaden filter criteria
- Clear and try again
- Check total counts in statistics
- Wait for more entities to be created

### Education System Not Working

**Issue**: No schools/universities appearing
**Causes:**
- Population too low
- Not enough cities
- Not enough schools (for universities)

**Requirements:**
- **Schools**: Population > 100
- **Universities**: Population > 1000, Cities > 2, Schools > 5

**Solutions:**
- Wait for population growth
- Check current statistics
- Ensure cities are being founded
- Be patient - universities take time

---

## Keyboard Shortcuts Reference

| Key | Action |
|-----|--------|
| `Space` | Pause/Resume simulation |
| `+` | Increase speed |
| `-` | Decrease speed |

---

## Statistics Glossary

### Population Metrics
- **Total Population**: Everyone ever born
- **Living Population**: Currently alive people
- **Total Births**: Same as Total Population
- **Total Deaths**: Dead people count
- **Total Marriages**: Married couples count

### Civilization Metrics
- **Cities**: Settlements founded
- **Countries**: Nations established
- **Religions**: Belief systems created
- **Inventions**: Technologies discovered
- **Wars**: Conflicts between nations

### Education Metrics
- **Schools**: Educational institutions (primary/secondary)
- **Universities**: Higher education centers
- **Students**: Current school enrollment
- **University Students**: Current university enrollment
- **Literacy Rate**: Percentage who can read/write
- **Literate Population**: Number of literate people

### Business Metrics
- **Total Businesses**: Companies created
- **Total Disasters**: Natural catastrophes occurred

---

## Advanced Topics

### Understanding Genetics

- **DNA Sequence**: 32-character genetic code
- **Blood Types**: Follow real genetic inheritance (A, B, AB, O, +/-)
- **Hereditary Conditions**: Can pass from parents to children
- **Trait Inheritance**: Blend of parental traits with mutation
- **Longevity**: Genetic predisposition to long life

### Disaster Mechanics

Different geography types have specific disaster risks:
- **Coastal**: Tsunamis, hurricanes
- **Mountain**: Earthquakes, volcanic eruptions
- **River Valley**: Floods
- **Island**: Volcanoes, tsunamis
- **Desert**: Droughts, sandstorms
- **Tropical**: Hurricanes, floods

Inventions can mitigate disasters (e.g., "Earthquake-Resistant Buildings" reduces earthquake damage).

### Economic System

- **Wealth**: Accumulated by cities and individuals
- **Jobs**: Provide income and social status
- **Businesses**: Generate wealth, employ people, can innovate
- **Trade**: (Future feature - not yet implemented)

---

## Getting Help

**Common Questions:**
1. Check this User Guide
2. Read README.md for technical details
3. Check PR_DESCRIPTION.md for feature specifics
4. Review commit messages for implementation details

**Reporting Issues:**
- Create GitHub issue with:
  - Browser and OS version
  - Steps to reproduce
  - Expected vs actual behavior
  - Save file (if relevant)
  - Screenshots

---

## Conclusion

The Population Simulator is a complex system with emergent behaviors. Experiment with different strategies, observe patterns, and enjoy watching civilizations rise and fall!

**Happy Simulating! 🎉**
