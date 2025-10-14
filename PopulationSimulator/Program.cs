using PopulationSimulator.Components;
using PopulationSimulator.Services;
using Microsoft.Extensions.DependencyInjection;
using RazorConsole.Core;
using Spectre.Console;

namespace PopulationSimulator;

class Program
{
    static async Task Main(string[] args)
    {
        // Show welcome message
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("PopSim").Centered().Color(Spectre.Console.Color.Cyan1));
        AnsiConsole.MarkupLine("[yellow]Advanced Population Simulator - A Living, Evolving World[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[white]Starting with Adam and Eve, watch humanity grow into a complex civilization.[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to begin...[/]");
        
        if (Console.IsInputRedirected)
        {
            await Task.Delay(2000); // Auto-start for testing
        }
        else
        {
            Console.ReadKey(true);
        }
        
        // Run main application
        AnsiConsole.Clear();
        await AppHost.RunAsync<App>(null, builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<SimulatorService>();
            });
            
            builder.Configure(options =>
            {
                options.AutoClearConsole = true;
            });
        });
        
        // Shutdown
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Simulation ended. Database has been saved.[/]");
        AnsiConsole.MarkupLine("[cyan]Thank you for using the Advanced Population Simulator![/]");
        AnsiConsole.WriteLine();
        
        if (!Console.IsInputRedirected)
        {
            AnsiConsole.MarkupLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
