using PopulationSimulator.Core;

namespace PopulationSimulator.Services;

public class SimulatorService
{
    private readonly Simulator _simulator;
    private int _simulationSpeed = 1;
    private bool _running = true;
    
    public event Action<SimulationStats>? OnStatsUpdated;
    public event Action<int>? OnSpeedChanged;
    
    public SimulatorService()
    {
        _simulator = new Simulator();
        _simulator.Initialize();
    }
    
    public void Start()
    {
        Task.Run(async () =>
        {
            while (_running)
            {
                for (int i = 0; i < _simulationSpeed; i++)
                {
                    _simulator.SimulateDay();
                }
                
                var stats = _simulator.GetStats();
                OnStatsUpdated?.Invoke(stats);
                
                await Task.Delay(50); // Update rate
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
