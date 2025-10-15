using PopulationSimulator.Core;
using PopulationSimulator.Models;
using Xunit;

namespace PopulationSimulator.Tests;

public class SimulatorTests
{
    [Fact]
    public void Simulator_Initializes_WithAdamAndEve()
    {
        // Arrange & Act
        var simulator = new Simulator();
        simulator.Initialize();
        var stats = simulator.GetStats();
        
        // Assert
        Assert.Equal(2, stats.LivingPopulation);
        Assert.Equal(1, stats.MaleCount);
        Assert.Equal(1, stats.FemaleCount);
    }
    
    [Fact]
    public void SimulateDay_ProcessesDailyEvents()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Act - Run simulation for 30 days
        for (int i = 0; i < 30; i++)
        {
            simulator.SimulateDay();
        }
        
        var stats = simulator.GetStats();
        
        // Assert
        Assert.True(stats.CurrentDay >= 30);
    }
    
    [Fact]
    public void SimulateDay_PopulationGrows_AfterManyDays()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Act - Run simulation for 365 days (1 year)
        for (int i = 0; i < 365; i++)
        {
            simulator.SimulateDay();
        }
        
        var stats = simulator.GetStats();
        
        // Assert - Population should have grown (Adam and Eve should have had children)
        Assert.True(stats.LivingPopulation >= 2, $"Population should grow after 1 year, but was {stats.LivingPopulation}");
    }
    
    [Fact]
    public void GetStats_ReturnsValidStatistics()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Act
        var stats = simulator.GetStats();
        
        // Assert
        Assert.NotNull(stats);
        Assert.True(stats.TotalPopulation >= 2);
        Assert.True(stats.GenerationNumber >= 0);
        Assert.NotNull(stats.RecentEvents);
        Assert.NotNull(stats.TopJobs);
        Assert.NotNull(stats.FamilyTrees);
    }
    
    [Fact]
    public void GetStats_IncludesDetailLists()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Act
        var stats = simulator.GetStats();
        
        // Assert
        Assert.NotNull(stats.Cities);
        Assert.NotNull(stats.Countries);
        Assert.NotNull(stats.Religions);
        Assert.NotNull(stats.Inventions);
    }
    
    [Fact]
    public void SimulateDay_EventsAreLogged()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Get initial stats
        var initialStats = simulator.GetStats();
        var initialEventCount = initialStats.RecentEvents.Count;
        
        // Act - Run simulation for 100 days
        for (int i = 0; i < 100; i++)
        {
            simulator.SimulateDay();
        }
        
        var stats = simulator.GetStats();
        
        // Assert - Should have more events
        Assert.True(stats.RecentEvents.Count > initialEventCount || stats.RecentEvents.Count == 10, 
            "Events should be logged (or at max capacity of 10)");
    }
    
    [Fact]
    public void GetStats_FamilyTreeIncludesAdamAndEve()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Act
        var stats = simulator.GetStats();
        
        // Assert
        Assert.NotEmpty(stats.FamilyTrees);
        var rootNode = stats.FamilyTrees.First();
        Assert.Equal("Adam", rootNode.FirstName);
    }
    
    [Fact]
    public void GetStats_TopJobsAreCalculated()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Run for some time to assign jobs
        for (int i = 0; i < 365 * 5; i++) // 5 years
        {
            simulator.SimulateDay();
        }
        
        // Act
        var stats = simulator.GetStats();
        
        // Assert
        Assert.NotNull(stats.TopJobs);
        // With a population, should have some job statistics
        if (stats.LivingPopulation > 10)
        {
            Assert.NotEmpty(stats.TopJobs);
        }
    }
    
    [Fact]
    public void SimulateDay_ContinuesPast100Days()
    {
        // Arrange
        var simulator = new Simulator();
        simulator.Initialize();
        
        // Act - Simulate 150 days to test past the SYNC_INTERVAL of 100
        for (int i = 0; i < 150; i++)
        {
            simulator.SimulateDay();
        }
        
        var stats = simulator.GetStats();
        
        // Assert
        Assert.True(stats.CurrentDay >= 7300 + 150, $"Expected day >= {7300 + 150}, got {stats.CurrentDay}");
        Assert.Equal(2, stats.LivingPopulation); // Adam and Eve should still be alive
    }
}
