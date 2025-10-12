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
        
        // Top Jobs
        if (stats.TopJobs.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                            TOP OCCUPATIONS                               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            foreach (var job in stats.TopJobs)
            {
                Console.WriteLine($"  {job.JobName,-25} {job.Count,5} people");
            }
        }
        
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
        
        // Family Trees
        if (stats.FamilyTrees.Any())
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                          ACTIVE FAMILY TREES                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            foreach (var tree in stats.FamilyTrees.Take(2)) // Show max 2 trees to fit screen
            {
                DisplayFamilyTree(tree, 0);
                Console.WriteLine(); // Blank line between trees
            }
        }
        
        // Recent Events
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                            RECENT EVENTS                                 ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        if (stats.RecentEvents.Any())
        {
            foreach (var evt in stats.RecentEvents.Take(5)) // Show fewer events to fit screen
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
        for (int i = stats.RecentEvents.Count; i < 5; i++)
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
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine(new string(' ', 80));
        }
        
        _lastEventCount = stats.RecentEvents.Count;
    }
    
    private void DisplayFamilyTree(FamilyTreeNode node, int depth, string prefix = "")
    {
        if (depth > 3) return; // Limit depth to prevent overwhelming display
        
        string indent = new string(' ', depth * 2);
        string marker = depth == 0 ? "■ " : "└─";
        
        var color = node.IsAlive ? ConsoleColor.Green : ConsoleColor.DarkGray;
        Console.ForegroundColor = color;
        
        string status = node.IsAlive ? $"Age {node.Age}" : "†";
        string spouse = !string.IsNullOrEmpty(node.SpouseName) ? $" ♥ {node.SpouseName}" : "";
        string line = $"  {indent}{marker}{node.FirstName} {node.LastName} ({status}){spouse}";
        
        if (line.Length > 78)
            line = line.Substring(0, 75) + "...";
        
        Console.WriteLine(line.PadRight(78));
        
        Console.ResetColor();
        
        // Show only first few living children to avoid clutter
        foreach (var child in node.Children.Take(5))
        {
            DisplayFamilyTree(child, depth + 1);
        }
        
        if (node.Children.Count > 5)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {new string(' ', (depth + 1) * 2)}... and {node.Children.Count - 5} more");
            Console.ResetColor();
        }
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
