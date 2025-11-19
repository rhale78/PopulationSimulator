using PopulationSimulator.Core;

namespace PopulationSimulator.Services;

public class SimulatorService
{
    private Simulator _simulator;
    private int _simulationSpeed = 1;
    private bool _running = true;
    private bool _paused = false;
    private DateTime _lastUIUpdate = DateTime.Now;
    private readonly object _updateLock = new();

    public event Action<SimulationStats>? OnStatsUpdated;
    public event Action<int>? OnSpeedChanged;
    public event Action? OnSimulationRestarted;
    public event Action<bool>? OnPausedChanged;

    public bool IsPaused => _paused;
    
    public SimulatorService()
    {
        _simulator = new Simulator();
        _simulator.Initialize();
    }
    
    public void Restart()
    {
        _running = false;
        Thread.Sleep(100); // Give the simulation loop time to stop
        
        // Create new simulator instance
        _simulator = new Simulator();
        _simulator.Initialize();
        
        // Reset speed
        _simulationSpeed = 1;
        _running = true;
        
        // Notify UI of restart
        OnSimulationRestarted?.Invoke();
        OnSpeedChanged?.Invoke(_simulationSpeed);
        
        // Restart the simulation loop
        Start();
    }
    
    public void Start()
    {
        Task.Run(async () =>
        {
            while (_running)
            {
                // Check if paused
                if (!_paused)
                {
                    // Process simulation days based on speed
                    for (int i = 0; i < _simulationSpeed; i++)
                    {
                        _simulator.SimulateDay();
                    }
                }

                // Always update UI even when paused (so we can see stats)
                // Throttle UI updates to prevent screen corruption
                // Only update UI every 500ms
                if ((DateTime.Now - _lastUIUpdate).TotalMilliseconds >= 500)
                {
                    lock (_updateLock)
                    {
                        var stats = _simulator.GetStats();
                        OnStatsUpdated?.Invoke(stats);
                        _lastUIUpdate = DateTime.Now;
                    }
                }

                // At speed 1, simulate 1 day per second (1000ms delay)
                // At higher speeds, delay decreases but never below 50ms
                int delay = Math.Max(50, 1000 / _simulationSpeed);
                await Task.Delay(delay);
            }
        });
    }
    
    public void Stop()
    {
        _running = false;
    }

    public void Pause()
    {
        _paused = true;
        OnPausedChanged?.Invoke(_paused);
    }

    public void Resume()
    {
        _paused = false;
        OnPausedChanged?.Invoke(_paused);
    }

    public void TogglePause()
    {
        _paused = !_paused;
        OnPausedChanged?.Invoke(_paused);
    }

    public void SetSpeed(int speed)
    {
        _simulationSpeed = Math.Clamp(speed, 1, 100);
        OnSpeedChanged?.Invoke(_simulationSpeed);
    }

    public void IncreaseSpeed()
    {
        _simulationSpeed = Math.Min(_simulationSpeed + 1, 100);
        OnSpeedChanged?.Invoke(_simulationSpeed);
    }

    public void DecreaseSpeed()
    {
        _simulationSpeed = Math.Max(_simulationSpeed - 1, 1);
        OnSpeedChanged?.Invoke(_simulationSpeed);
    }

    public SimulationStats GetCurrentStats()
    {
        return _simulator.GetStats();
    }

    // ============================================================================
    // EXPORT FUNCTIONALITY
    // ============================================================================

    public string ExportPopulationAsCSV()
    {
        return _simulator.ExportPopulationAsCSV();
    }

    public string ExportCitiesAsCSV()
    {
        return _simulator.ExportCitiesAsCSV();
    }

    public string ExportCountriesAsCSV()
    {
        return _simulator.ExportCountriesAsCSV();
    }

    public string ExportInventionsAsCSV()
    {
        return _simulator.ExportInventionsAsCSV();
    }

    public string ExportDisastersAsCSV()
    {
        return _simulator.ExportDisastersAsCSV();
    }

    public string ExportBusinessesAsCSV()
    {
        return _simulator.ExportBusinessesAsCSV();
    }

    public string ExportEventsAsCSV()
    {
        return _simulator.ExportEventsAsCSV();
    }

    public string ExportSchoolsAsCSV()
    {
        return _simulator.ExportSchoolsAsCSV();
    }

    public string ExportUniversitiesAsCSV()
    {
        return _simulator.ExportUniversitiesAsCSV();
    }

    public string ExportAllAsJSON()
    {
        return _simulator.ExportAllAsJSON();
    }

    // ============================================================================
    // SEARCH & FILTER ACCESS
    // ============================================================================

    public List<Person> GetAllPeople() => _simulator.GetAllPeople();
    public List<City> GetAllCities() => _simulator.GetAllCities();
    public List<Country> GetAllCountries() => _simulator.GetAllCountries();
    public List<Invention> GetAllInventions() => _simulator.GetAllInventions();
    public List<School> GetAllSchools() => _simulator.GetAllSchools();
    public List<University> GetAllUniversities() => _simulator.GetAllUniversities();
    public Dictionary<long, Person> GetPeopleById() => _simulator.GetPeopleById();
    public Dictionary<long, Person> GetDeadPeopleById() => _simulator.GetDeadPeopleById();
    public Dictionary<long, City> GetCitiesById() => _simulator.GetCitiesById();
    public Dictionary<long, Country> GetCountriesById() => _simulator.GetCountriesById();
    public Dictionary<long, School> GetSchoolsById() => _simulator.GetSchoolsById();
    public Dictionary<long, University> GetUniversitiesById() => _simulator.GetUniversitiesById();
    public DateTime GetCurrentDate() => _simulator.GetCurrentDate();

    // ============================================================================
    // SAVE/LOAD SYSTEM
    // ============================================================================

    public string SaveSimulation()
    {
        return _simulator.SaveSimulation();
    }

    public bool LoadSimulation(string jsonData)
    {
        // Pause the simulation during load
        bool wasPaused = _paused;
        _paused = true;

        bool success = _simulator.LoadSimulation(jsonData);

        if (success)
        {
            // Reset UI state
            _simulationSpeed = 1;
            OnSpeedChanged?.Invoke(_simulationSpeed);

            // Trigger stats update
            var stats = _simulator.GetStats();
            OnStatsUpdated?.Invoke(stats);
        }

        // Restore pause state
        _paused = wasPaused;
        OnPausedChanged?.Invoke(_paused);

        return success;
    }
}
