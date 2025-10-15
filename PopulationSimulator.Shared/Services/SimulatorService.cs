using PopulationSimulator.Core;

namespace PopulationSimulator.Services;

public class SimulatorService
{
    private Simulator _simulator;
    private int _simulationSpeed = 1;
    private bool _running = true;
    private DateTime _lastUIUpdate = DateTime.Now;
    private readonly object _updateLock = new();
    
    public event Action<SimulationStats>? OnStatsUpdated;
    public event Action<int>? OnSpeedChanged;
    public event Action? OnSimulationRestarted;
    
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
            try
            {
                while (_running)
                {
                    // Process simulation days based on speed
                    for (int i = 0; i < _simulationSpeed; i++)
                    {
                        _simulator.SimulateDay();
                    }
                    
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Simulation error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                _running = false;
            }
        });
    }
    
    public void Stop()
    {
        _running = false;
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
}
