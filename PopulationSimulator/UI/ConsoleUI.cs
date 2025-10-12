using PopulationSimulator.Core;

namespace PopulationSimulator.UI;

public class ConsoleUI
{
    private int _lastEventCount = 0;
    private DateTime _lastUpdate = DateTime.Now;
    private DateTime _lastClear = DateTime.Now;
    
    public void Initialize()
    {
        Console.Clear();
        Console.Title = "Advanced Population Simulator";
        Console.CursorVisible = false;
    }
    
    public void Update(SimulationStats stats, int simulationSpeed)
    {
        // Only update UI every 100ms to avoid flickering
        if ((DateTime.Now - _lastUpdate).TotalMilliseconds < 100)
            return;
        
        _lastUpdate = DateTime.Now;
        
        // Periodically clear and redraw entire screen (every 3 seconds) to fix corruption
        if ((DateTime.Now - _lastClear).TotalSeconds >= 3)
        {
            Console.Clear();
            _lastClear = DateTime.Now;
        }
        
        Console.SetCursorPosition(0, 0);
        
        // Header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              ADVANCED POPULATION SIMULATOR - Living World                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        // Current Date
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  Current Date: Year {stats.CurrentDate.Year}, Day {stats.CurrentDate.DayOfYear}");
        Console.WriteLine($"  Generation: {stats.GenerationNumber}");
        Console.ResetColor();
        
        // Population Statistics
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                         POPULATION STATISTICS                            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.WriteLine($"  Living Population:    {stats.LivingPopulation,8}");
        Console.WriteLine($"  Total Births:         {stats.TotalBirths,8}");
        Console.WriteLine($"  Total Deaths:         {stats.TotalDeaths,8}");
        Console.WriteLine($"  Total Marriages:      {stats.TotalMarriages,8}");
        
        // Civilization Progress
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                       CIVILIZATION PROGRESS                              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.WriteLine($"  Cities:               {stats.TotalCities,8}");
        Console.WriteLine($"  Countries:            {stats.TotalCountries,8}");
        Console.WriteLine($"  Religions:            {stats.TotalReligions,8}");
        Console.WriteLine($"  Inventions:           {stats.TotalInventions,8}");
        Console.WriteLine($"  Wars:                 {stats.TotalWars,8}");
        
        // Recent Events
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                            RECENT EVENTS                                 ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        if (stats.RecentEvents.Any())
        {
            foreach (var evt in stats.RecentEvents)
            {
                var color = evt.EventType switch
                {
                    "Birth" => ConsoleColor.Green,
                    "Death" => ConsoleColor.Red,
                    "Marriage" => ConsoleColor.Yellow,
                    "City" => ConsoleColor.Cyan,
                    "Country" => ConsoleColor.Magenta,
                    "Religion" => ConsoleColor.Blue,
                    "Invention" => ConsoleColor.Yellow,
                    "War" => ConsoleColor.DarkRed,
                    _ => ConsoleColor.Gray
                };
                
                Console.ForegroundColor = color;
                string eventLine = $"  [{evt.EventType,-12}] {evt.Description}";
                
                // Wrap long event text instead of truncating
                if (eventLine.Length > 78)
                {
                    Console.WriteLine(eventLine.Substring(0, 78));
                    // Print continuation on next line
                    string continuation = "                     " + eventLine.Substring(78);
                    if (continuation.Length > 78)
                        continuation = continuation.Substring(0, 75) + "...";
                    Console.WriteLine(continuation.PadRight(78));
                }
                else
                {
                    Console.WriteLine(eventLine.PadRight(78));
                }
            }
        }
        else
        {
            Console.WriteLine("  No events yet...");
        }
        
        // Clear remaining lines
        for (int i = stats.RecentEvents.Count; i < 10; i++)
        {
            Console.WriteLine(new string(' ', 78));
        }
        
        Console.ResetColor();
        
        // Controls
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                               CONTROLS                                   ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"  Speed: {simulationSpeed}x | Press [+] to speed up, [-] to slow down, [Q] to quit");
        Console.ResetColor();
        
        // Add blank lines to ensure screen is fully cleared
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine(new string(' ', 80));
        }
        
        _lastEventCount = stats.RecentEvents.Count;
    }
    
    public void ShowWelcome()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔════════════════════════════════════════════════════════════════════════════╗
║                                                                            ║
║              ADVANCED POPULATION SIMULATOR                                 ║
║              A Living, Evolving World                                      ║
║                                                                            ║
╚════════════════════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(@"
  Starting with Adam and Eve, watch as humanity grows from a single couple
  into a complex civilization with cities, countries, religions, inventions,
  and wars. Each person has unique traits, genetics, and life events.
  
  The simulation runs autonomously, with societies and cultures emerging
  organically from individual interactions and decisions.
");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n  Press any key to begin the simulation...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
    
    public void ShowShutdown()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n\n  Simulation ended. Database has been saved.");
        Console.WriteLine("  Thank you for using the Advanced Population Simulator!");
        Console.ResetColor();
        Console.WriteLine("\n  Press any key to exit...");
        Console.ReadKey(true);
    }
}
