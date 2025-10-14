# RazorConsole Integration

This document describes the integration of RazorConsole into the PopulationSimulator project.

## Overview

The PopulationSimulator has been fully reworked to use RazorConsole instead of the traditional System.Console for UI rendering. RazorConsole provides a modern, component-based approach to building console applications using Razor components and Spectre.Console.

## Changes Made

### 1. Project Configuration
- Updated `PopulationSimulator.csproj` to use `Microsoft.NET.Sdk.Razor` SDK
- Added `RazorConsole.Core` NuGet package (v0.0.3-alpha.4657e6)
- Added `Spectre.Console` NuGet package (v0.51.1) for rich console rendering

### 2. Component Architecture

Created the following Razor components in `/Components` directory:

- **App.razor**: Main application component that orchestrates all UI panels
- **PopulationStatsPanel.razor**: Displays population statistics (births, deaths, marriages)
- **TopJobsPanel.razor**: Shows the top 5 occupations in the simulation
- **CivilizationPanel.razor**: Displays civilization progress (cities, countries, religions, etc.)
- **FamilyTreePanel.razor**: Renders family tree with proper overflow handling and depth limiting
- **RecentEventsPanel.razor**: Shows the last 10 simulation events with color coding
- **Welcome.razor**: Welcome screen component (currently not in main flow)

### 3. Services

Created **SimulatorService.cs** in `/Services` directory to:
- Manage the simulation lifecycle
- Provide event-based communication between the simulator and UI components
- Handle simulation speed changes
- Emit stats updates for UI rendering

### 4. Text Overflow Handling

All text regions now properly handle overflow:
- **Family Tree**: Limited to max width of 75 characters with truncation ("...")
- **Recent Events**: Limited to max width of 75 characters with truncation
- **Panel Heights**: Fixed heights prevent screen overflow (Family Tree: 20 lines, Events: 15 lines)
- **Depth Limiting**: Family tree limited to 3 generations deep
- **Child Limiting**: Only first 10 children shown per person, with "... and X more" indicator

### 5. Interactive Controls

Added interactive buttons using RazorConsole's `TextButton` component:
- **[+] Speed Up**: Increases simulation speed (max 100x)
- **[-] Slow Down**: Decreases simulation speed (min 1x)
- Focus navigation using Tab key
- Button activation using Enter key
- Ctrl+C to quit the application

### 6. Screen Redraw

RazorConsole automatically handles:
- Double-buffered rendering to prevent flicker
- Component-based updates
- Automatic screen clearing and redrawing
- Proper state management through React-style component lifecycle

### 7. Removed Files

- **UI/ConsoleUI.cs**: Completely replaced by Razor components
- Old manual buffering and console manipulation code removed

## Benefits of RazorConsole

1. **Component-Based UI**: Modular, reusable UI components
2. **Automatic Redraw**: No manual screen clearing or cursor positioning needed
3. **Rich Formatting**: Spectre.Console provides beautiful terminal UI elements
4. **Proper Text Handling**: Built-in overflow and text wrapping support
5. **Interactive Elements**: Buttons, text inputs with focus management
6. **State Management**: React-style component lifecycle and state updates
7. **Maintainability**: Cleaner separation of concerns and easier to test

## Running the Application

```bash
dotnet run --project PopulationSimulator
```

The application will show a welcome screen, then automatically (or after keypress) launch into the main simulation UI. Use Tab to navigate between controls, Enter to activate buttons, and Ctrl+C to exit.

## Technical Notes

- RazorConsole uses a virtual DOM approach similar to web frameworks
- All rendering is done through Spectre.Console for rich terminal output
- Components automatically update when their parameters change
- The simulation runs in a background task and updates UI via events
- UI update rate is throttled to 50ms (20 FPS) to avoid performance issues
