using PopulationSimulator.Core;
using PopulationSimulator.UI;

namespace PopulationSimulator;

class Program
{
    static void Main(string[] args)
    {
        var ui = new ConsoleUI();
        ui.ShowWelcome();
        
        var simulator = new Simulator();
        simulator.Initialize();
        
        ui.Initialize();
        
        int simulationSpeed = 1;
        bool running = true;
        
        // Main simulation loop
        Task.Run(() =>
        {
            while (running)
            {
                for (int i = 0; i < simulationSpeed; i++)
                {
                    simulator.SimulateDay();
                }
                
                var stats = simulator.GetStats();
                ui.Update(stats, simulationSpeed);
                
                Thread.Sleep(50); // Update rate
            }
        });
        
        // Input handling
        while (running)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                
                switch (key.Key)
                {
                    case ConsoleKey.Q:
                        running = false;
                        break;
                    case ConsoleKey.OemPlus:
                    case ConsoleKey.Add:
                        simulationSpeed = Math.Min(simulationSpeed + 1, 100);
                        break;
                    case ConsoleKey.OemMinus:
                    case ConsoleKey.Subtract:
                        simulationSpeed = Math.Max(simulationSpeed - 1, 1);
                        break;
                    case ConsoleKey.R:
                        ui.ForceRedraw();
                        break;
                }
            }
            
            Thread.Sleep(50);
        }
        
        ui.ShowShutdown();
    }
}
