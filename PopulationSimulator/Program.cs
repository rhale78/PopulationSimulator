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
        // Show welcome screen
        AnsiConsole.Clear();
        await AppHost.RunAsync<Welcome>(null, builder =>
        {
            builder.Configure(options =>
            {
                options.AutoClearConsole = false;
            });
        });
        
        Console.ReadKey(true);
        
        // Run main application
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
        AnsiConsole.MarkupLine("Press any key to exit...");
        Console.ReadKey(true);
    }
}
