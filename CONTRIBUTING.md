# Contributing to Advanced Population Simulator

Thank you for your interest in contributing to the Advanced Population Simulator! This document provides guidelines and information for contributors.

## Table of Contents
1. [Getting Started](#getting-started)
2. [Development Setup](#development-setup)
3. [Project Structure](#project-structure)
4. [How to Contribute](#how-to-contribute)
5. [Coding Standards](#coding-standards)
6. [Testing](#testing)
7. [Feature Ideas](#feature-ideas)

## Getting Started

Before contributing, please:
1. Read the [README.md](README.md) to understand the project
2. Review the [TECHNICAL.md](TECHNICAL.md) for architecture details
3. Check existing issues and pull requests to avoid duplication

## Development Setup

### Prerequisites
- .NET 9.0 SDK or later
- Git
- Code editor (VS Code, Visual Studio, Rider, etc.)

### Setup Steps
```bash
# Clone the repository
git clone https://github.com/rhale78/PopulationSimulator.git
cd PopulationSimulator

# Build the project
dotnet build

# Run the simulator
cd PopulationSimulator
dotnet run
```

### Development Tools
Recommended extensions for VS Code:
- C# Dev Kit
- .NET Extension Pack
- SQLite Viewer

## Project Structure

```
PopulationSimulator/
├── PopulationSimulator/         # Main project
│   ├── Models/                  # Data models
│   │   ├── Person.cs
│   │   ├── City.cs
│   │   └── ...
│   ├── Core/                    # Core logic
│   │   ├── Simulator.cs
│   │   └── NameGenerator.cs
│   ├── Data/                    # Data access
│   │   └── DataAccessLayer.cs
│   ├── UI/                      # User interface
│   │   └── ConsoleUI.cs
│   └── Program.cs               # Entry point
├── README.md
├── TECHNICAL.md
└── ...
```

## How to Contribute

### Reporting Bugs
1. Check if the bug is already reported in Issues
2. Create a new issue with:
   - Clear description
   - Steps to reproduce
   - Expected vs actual behavior
   - System information (.NET version, OS)

### Suggesting Features
1. Open an issue with the "enhancement" label
2. Describe the feature and its benefits
3. Discuss implementation approach if possible

### Submitting Changes
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Test thoroughly
5. Commit with clear messages (`git commit -m 'Add amazing feature'`)
6. Push to your fork (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Pull Request Guidelines
- Keep changes focused and atomic
- Update documentation if needed
- Add examples for new features
- Ensure code builds without errors
- Follow existing code style

## Coding Standards

### C# Style Guide
```csharp
// Use PascalCase for classes, methods, properties
public class Person { }
public void ProcessBirth() { }
public string FirstName { get; set; }

// Use camelCase for local variables and parameters
var personAge = 25;
void CalculateAge(DateTime birthDate) { }

// Use _camelCase for private fields
private readonly Random _random;

// Use meaningful names
// Good: CalculateDeathChance
// Bad: CalcDC

// Add comments for complex logic
// Calculate death chance with age, health, and job risk factors
double deathChance = CalculateDeathChance(person, age);
```

### Organization
- Keep classes focused and single-responsibility
- Group related functionality
- Use namespaces appropriately
- Limit file size (prefer splitting large files)

### Performance Considerations
- Use dictionaries for O(1) lookups
- Avoid unnecessary LINQ queries in hot paths
- Consider memory allocation in loops
- Profile before optimizing

## Testing

### Manual Testing
1. Run the simulator for various durations
2. Check database integrity
3. Verify UI displays correctly
4. Test edge cases (very small/large populations)

### Test Checklist
- [ ] Application builds without errors
- [ ] Simulation starts correctly
- [ ] Population grows as expected
- [ ] Events are logged properly
- [ ] Database saves successfully
- [ ] UI updates correctly
- [ ] Speed controls work
- [ ] Application exits cleanly

### Future: Automated Tests
We welcome contributions to add:
- Unit tests for core logic
- Integration tests for database operations
- Performance benchmarks

## Feature Ideas

### High Priority
- [ ] Configuration file for simulation parameters
- [ ] Save/load simulation state
- [ ] Export data to CSV/JSON
- [ ] More detailed statistics view
- [ ] Pause/resume functionality

### Medium Priority
- [ ] Disease and epidemic simulation
- [ ] Migration between cities
- [ ] Trade and economy systems
- [ ] Educational institutions
- [ ] Technology prerequisites
- [ ] Diplomatic relations

### Low Priority
- [ ] Web-based UI
- [ ] 3D visualization
- [ ] Climate/weather simulation
- [ ] Cultural traditions system
- [ ] Language evolution
- [ ] Art and literature generation

### Technical Improvements
- [ ] Unit test coverage
- [ ] Performance profiling
- [ ] Configuration system
- [ ] Logging framework
- [ ] Plugin architecture
- [ ] API for data access

## Code Review Process

Pull requests will be reviewed for:
1. **Functionality**: Does it work as intended?
2. **Code Quality**: Is it readable and maintainable?
3. **Performance**: Does it impact simulation speed?
4. **Documentation**: Are changes documented?
5. **Testing**: Has it been tested adequately?

## Community Guidelines

- Be respectful and constructive
- Help others learn and improve
- Give credit where due
- Focus on the code, not the person
- Assume good intentions

## Questions?

- Open an issue for questions
- Check existing documentation first
- Be specific about what you need help with

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be recognized in:
- CHANGELOG.md
- GitHub contributors page
- Release notes (for significant features)

Thank you for contributing to the Advanced Population Simulator!
