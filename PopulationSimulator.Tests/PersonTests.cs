using PopulationSimulator.Core;
using PopulationSimulator.Models;
using Xunit;

namespace PopulationSimulator.Tests;

public class PersonTests
{
    [Fact]
    public void GetAge_ReturnsCorrectAge_ForLivingPerson()
    {
        // Arrange
        var person = new Person
        {
            BirthDay = 0, // Day 0
            IsAlive = true
        };
        var currentDay = 20 * 365; // 20 years later
        
        // Act
        var age = person.GetAge(currentDay);
        
        // Assert
        Assert.Equal(20, age);
    }
    
    [Fact]
    public void GetAge_ReturnsCorrectAge_ForDeceasedPerson()
    {
        // Arrange
        var person = new Person
        {
            BirthDay = 0, // Day 0
            DeathDay = 15 * 365, // Died at year 15
            IsAlive = false
        };
        var currentDay = 20 * 365; // 20 years later
        
        // Act
        var age = person.GetAge(currentDay);
        
        // Assert
        Assert.Equal(15, age);
    }
    
    [Theory]
    [InlineData("Female", true, true, 25, true)]
    [InlineData("Male", true, true, 25, false)]
    [InlineData("Female", false, true, 25, false)]
    [InlineData("Female", true, false, 25, false)]
    [InlineData("Female", true, true, 13, false)]
    [InlineData("Female", true, true, 51, false)]
    public void CanHaveChildren_ReturnsExpectedResult(string gender, bool isAlive, bool isMarried, int age, bool expected)
    {
        // Arrange
        var person = new Person
        {
            Gender = gender,
            IsAlive = isAlive,
            SpouseId = isMarried ? 100L : null,
            BirthDay = -age * 365, // Negative days = born in the past
            IsPregnant = false
        };
        
        // Act
        var result = person.CanHaveChildren(0);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(true, false, 20, true)]
    [InlineData(false, false, 20, false)]
    [InlineData(true, true, 20, false)]
    [InlineData(true, false, 13, false)]
    public void IsEligibleForMarriage_ReturnsExpectedResult(bool isAlive, bool isMarried, int age, bool expected)
    {
        // Arrange
        var person = new Person
        {
            IsAlive = isAlive,
            SpouseId = isMarried ? 100L : null,
            BirthDay = -age * 365 // Negative days = born in the past
        };
        
        // Act
        var result = person.IsEligibleForMarriage(0);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(true, false, 15, true)]
    [InlineData(false, false, 15, false)]
    [InlineData(true, true, 15, false)]
    [InlineData(true, false, 11, false)]
    public void IsEligibleForJob_ReturnsExpectedResult(bool isAlive, bool hasJob, int age, bool expected)
    {
        // Arrange
        var person = new Person
        {
            IsAlive = isAlive,
            JobId = hasJob ? 1L : null,
            BirthDay = -age * 365 // Negative days = born in the past
        };
        
        // Act
        var result = person.IsEligibleForJob(0);
        
        // Assert
        Assert.Equal(expected, result);
    }
}
