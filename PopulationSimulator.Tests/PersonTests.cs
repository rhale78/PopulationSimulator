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
            BirthDate = new DateTime(2000, 1, 1),
            IsAlive = true
        };
        var currentDate = new DateTime(2020, 1, 1);
        
        // Act
        var age = person.GetAge(currentDate);
        
        // Assert
        Assert.Equal(20, age);
    }
    
    [Fact]
    public void GetAge_ReturnsCorrectAge_ForDeceasedPerson()
    {
        // Arrange
        var person = new Person
        {
            BirthDate = new DateTime(2000, 1, 1),
            DeathDate = new DateTime(2015, 1, 1),
            IsAlive = false
        };
        var currentDate = new DateTime(2020, 1, 1);
        
        // Act
        var age = person.GetAge(currentDate);
        
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
            BirthDate = DateTime.Now.AddYears(-age),
            IsPregnant = false
        };
        
        // Act
        var result = person.CanHaveChildren(DateTime.Now);
        
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
            BirthDate = DateTime.Now.AddYears(-age)
        };
        
        // Act
        var result = person.IsEligibleForMarriage(DateTime.Now);
        
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
            BirthDate = DateTime.Now.AddYears(-age)
        };
        
        // Act
        var result = person.IsEligibleForJob(DateTime.Now);
        
        // Assert
        Assert.Equal(expected, result);
    }
}
