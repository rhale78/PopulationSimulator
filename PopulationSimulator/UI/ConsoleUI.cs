using PopulationSimulator.Core;

namespace PopulationSimulator.UI;

public class ConsoleUI
{
    private int _lastEventCount = 0;
    private DateTime _lastUpdate = DateTime.Now;
    private DateTime _lastClear = DateTime.Now;
    private readonly List<string> _screenBuffer = new(); // Buffer for proper screen redraw
    
    public void Initialize()
    {
        Console.Clear();
        Console.Title = "Advanced Population Simulator";
        Console.CursorVisible = false;
        
        // Try to disable console scrolling (Windows only)
        try
        {
            Console.BufferHeight = Console.WindowHeight;
        }
        catch
        {
            // Ignore if not supported on this platform
        }
    }
    
    public void Update(SimulationStats stats, int simulationSpeed)
    {
        // Only update UI every 100ms to avoid flickering
        if ((DateTime.Now - _lastUpdate).TotalMilliseconds < 100)
            return;
        
        _lastUpdate = DateTime.Now;
        
        // Build screen content in buffer
        _screenBuffer.Clear();
        
        // Header
        _screenBuffer.Add("╔════════════════════════════════════════════════════════════════════════════╗");
        _screenBuffer.Add("║              ADVANCED POPULATION SIMULATOR - Living World                  ║");
        _screenBuffer.Add("╚════════════════════════════════════════════════════════════════════════════╝");
        _screenBuffer.Add("");
        _screenBuffer.Add($"  Current Date: Year {stats.CurrentDate.Year}, Day {stats.CurrentDate.DayOfYear}");
        _screenBuffer.Add($"  Generation: {stats.GenerationNumber}");
        
        // Population Statistics
        _screenBuffer.Add("");
        _screenBuffer.Add("╔══════════════════════════════════════════════════════════════════════════╗");
        _screenBuffer.Add("║                         POPULATION STATISTICS                            ║");
        _screenBuffer.Add("╚══════════════════════════════════════════════════════════════════════════╝");
        _screenBuffer.Add($"  Living Population:    {stats.LivingPopulation,8}");
        _screenBuffer.Add($"  Total Births:         {stats.TotalBirths,8}");
        _screenBuffer.Add($"  Total Deaths:         {stats.TotalDeaths,8}");
        _screenBuffer.Add($"  Total Marriages:      {stats.TotalMarriages,8}");
        
        // Top Jobs
        if (stats.TopJobs.Any())
        {
            _screenBuffer.Add("");
            _screenBuffer.Add("╔══════════════════════════════════════════════════════════════════════════╗");
            _screenBuffer.Add("║                            TOP OCCUPATIONS                               ║");
            _screenBuffer.Add("╚══════════════════════════════════════════════════════════════════════════╝");
            
            foreach (var job in stats.TopJobs)
            {
                _screenBuffer.Add($"  {job.JobName,-25} {job.Count,5} people");
            }
        }
        
        // Civilization Progress
        _screenBuffer.Add("");
        _screenBuffer.Add("╔══════════════════════════════════════════════════════════════════════════╗");
        _screenBuffer.Add("║                       CIVILIZATION PROGRESS                              ║");
        _screenBuffer.Add("╚══════════════════════════════════════════════════════════════════════════╝");
        _screenBuffer.Add($"  Cities:               {stats.TotalCities,8}");
        _screenBuffer.Add($"  Countries:            {stats.TotalCountries,8}");
        _screenBuffer.Add($"  Religions:            {stats.TotalReligions,8}");
        _screenBuffer.Add($"  Inventions:           {stats.TotalInventions,8}");
        _screenBuffer.Add($"  Wars:                 {stats.TotalWars,8}");
        
        // Family Trees
        if (stats.FamilyTrees.Any())
        {
            _screenBuffer.Add("");
            _screenBuffer.Add("╔══════════════════════════════════════════════════════════════════════════╗");
            _screenBuffer.Add("║                          ACTIVE FAMILY TREES                             ║");
            _screenBuffer.Add("╚══════════════════════════════════════════════════════════════════════════╝");
            
            foreach (var tree in stats.FamilyTrees.Take(1)) // Show max 1 tree
            {
                BuildFamilyTreeBuffer(tree, 0);
            }
        }
        
        // Recent Events - show last 10
        _screenBuffer.Add("");
        _screenBuffer.Add("╔══════════════════════════════════════════════════════════════════════════╗");
        _screenBuffer.Add("║                            RECENT EVENTS                                 ║");
        _screenBuffer.Add("╚══════════════════════════════════════════════════════════════════════════╝");
        
        if (stats.RecentEvents.Any())
        {
            foreach (var evt in stats.RecentEvents.TakeLast(10)) // Show last 10 events
            {
                string eventLine = $"  [{evt.EventType,-12}] {evt.Description}";
                
                if (eventLine.Length > 78)
                {
                    eventLine = eventLine.Substring(0, 78);
                }
                else
                {
                    eventLine = eventLine.PadRight(78);
                }
                
                _screenBuffer.Add(eventLine);
            }
        }
        else
        {
            _screenBuffer.Add("  No events yet...".PadRight(78));
        }
        
        // Clear remaining lines
        for (int i = stats.RecentEvents.Count; i < 10; i++)
        {
            _screenBuffer.Add(new string(' ', 78));
        }
        
        // Controls
        _screenBuffer.Add("");
        _screenBuffer.Add("╔══════════════════════════════════════════════════════════════════════════╗");
        _screenBuffer.Add("║                               CONTROLS                                   ║");
        _screenBuffer.Add("╚══════════════════════════════════════════════════════════════════════════╝");
        _screenBuffer.Add($"  Speed: {simulationSpeed}x | Press [+] to speed up, [-] to slow down, [Q] to quit");
        
        // Add blank lines to ensure screen is fully cleared
        for (int i = 0; i < 2; i++)
        {
            _screenBuffer.Add(new string(' ', 80));
        }
        
        // Redraw screen from buffer
        Console.SetCursorPosition(0, 0);
        foreach (var line in _screenBuffer)
        {
            Console.WriteLine(line);
        }
        
        _lastEventCount = stats.RecentEvents.Count;
    }
    
    private void BuildFamilyTreeBuffer(FamilyTreeNode node, int depth, string prefix = "")
    {
        if (depth > 3) return; // Limit depth to prevent overwhelming display
        
        string indent = new string(' ', depth * 2);
        string marker = depth == 0 ? "■ " : "└─";
        
        string status = node.IsAlive ? $"Age {node.Age}" : "†";
        string spouse = !string.IsNullOrEmpty(node.SpouseName) ? $" ♥ {node.SpouseName}" : "";
        
        // Add gender marker: ♂ for male, ♀ for female
        string genderMarker = node.Gender == "Male" ? "♂" : "♀";
        
        string line = $"  {indent}{marker}{genderMarker} {node.FirstName} {node.LastName} ({status}){spouse}";
        
        if (line.Length > 78)
            line = line.Substring(0, 75) + "...";
        
        _screenBuffer.Add(line.PadRight(78));
        
        // Show only first 10 living children to avoid clutter
        foreach (var child in node.Children.Take(10))
        {
            BuildFamilyTreeBuffer(child, depth + 1);
        }
        
        if (node.Children.Count > 10)
        {
            _screenBuffer.Add($"  {new string(' ', (depth + 1) * 2)}... and {node.Children.Count - 10} more".PadRight(78));
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
