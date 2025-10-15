using PopulationSimulator.Core;
using Xunit;

namespace PopulationSimulator.Tests;

public class NameGeneratorTests
{
    private readonly NameGenerator _nameGenerator;
    
    public NameGeneratorTests()
    {
        _nameGenerator = new NameGenerator(new Random(42)); // Use fixed seed for reproducible tests
    }
    
    [Fact]
    public void GenerateMaleFirstName_ReturnsNonEmptyString()
    {
        // Act
        var name = _nameGenerator.GenerateMaleFirstName();
        
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(name));
    }
    
    [Fact]
    public void GenerateFemaleFirstName_ReturnsNonEmptyString()
    {
        // Act
        var name = _nameGenerator.GenerateFemaleFirstName();
        
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(name));
    }
    
    [Fact]
    public void GenerateCityName_ReturnsNonEmptyString()
    {
        // Act
        var name = _nameGenerator.GenerateCityName();
        
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(name));
    }
    
    [Fact]
    public void GenerateCountryName_ReturnsNonEmptyString()
    {
        // Act
        var name = _nameGenerator.GenerateCountryName();
        
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(name));
    }
    
    [Fact]
    public void GenerateReligionName_ReturnsNonEmptyString()
    {
        // Act
        var name = _nameGenerator.GenerateReligionName();
        
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(name));
    }
    
    [Fact]
    public void GenerateEyeColor_ReturnsValidColor()
    {
        // Act
        var color = _nameGenerator.GenerateEyeColor();
        
        // Assert
        var validColors = new[] { "Brown", "Blue", "Green", "Hazel", "Gray", "Amber" };
        Assert.Contains(color, validColors);
    }
    
    [Fact]
    public void GenerateHairColor_ReturnsValidColor()
    {
        // Act
        var color = _nameGenerator.GenerateHairColor();
        
        // Assert
        var validColors = new[] { "Black", "Brown", "Blonde", "Red", "Auburn", "Gray" };
        Assert.Contains(color, validColors);
    }
    
    [Fact]
    public void GenerateMaleFirstName_ReturnsDifferentNames_WithDifferentSeeds()
    {
        // Arrange
        var generator1 = new NameGenerator(new Random(1));
        var generator2 = new NameGenerator(new Random(2));
        
        // Act
        var name1 = generator1.GenerateMaleFirstName();
        var name2 = generator2.GenerateMaleFirstName();
        
        // We expect different names with different random seeds (may occasionally fail, but very unlikely)
        // Run this multiple times to be more confident
        var names1 = Enumerable.Range(0, 10).Select(_ => generator1.GenerateMaleFirstName()).ToList();
        var names2 = Enumerable.Range(0, 10).Select(_ => generator2.GenerateMaleFirstName()).ToList();
        
        // Assert - At least some names should be different
        Assert.NotEqual(names1, names2);
    }
    
    [Fact]
    public void GenerateLastName_ReturnsPatronymicFormat_ForEarlyGeneration()
    {
        // Arrange
        var fatherName = "Adam";
        var generationNumber = 5; // Early generation
        
        // Act
        var lastName = _nameGenerator.GenerateLastName(fatherName, null, null, generationNumber);
        
        // Assert
        Assert.Equal("ben Adam", lastName);
    }
    
    [Fact]
    public void GenerateLastName_ReturnsCityBasedFormat_ForMiddleGeneration()
    {
        // Arrange
        var cityName = "Babylon";
        var generationNumber = 25; // Middle generation
        
        // Act
        var lastName = _nameGenerator.GenerateLastName(null, cityName, null, generationNumber);
        
        // Assert
        Assert.Equal("of Babylon", lastName);
    }
    
    [Fact]
    public void GenerateDynastyName_ReturnsCorrectFormat()
    {
        // Arrange
        var founderName = "Alexander";
        
        // Act
        var dynastyName = _nameGenerator.GenerateDynastyName(founderName);
        
        // Assert
        Assert.Contains("Alexander", dynastyName);
    }
}
