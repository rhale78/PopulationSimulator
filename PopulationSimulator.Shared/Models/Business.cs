namespace PopulationSimulator.Models;

public class Business
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Farm, Workshop, Merchant House, etc.
    public long OwnerId { get; set; } // Person who owns the business
    public long? CityId { get; set; }
    public DateTime FoundedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal Wealth { get; set; }
    public int EmployeeCount { get; set; }
    public int MaxEmployees { get; set; } = 10;
    public string GoodsProduced { get; set; } = string.Empty; // Comma-separated
    public decimal AnnualRevenue { get; set; }
    public decimal AnnualCosts { get; set; }
    public int Reputation { get; set; } // 0-100

    // Innovation tracking
    public bool CanInnovate { get; set; } = true;
    public List<long> InventionsCreated { get; set; } = new();
    public int InnovationPoints { get; set; } // Accumulates over time

    // Rise and fall
    public string Status { get; set; } = "Growing"; // Growing, Stable, Declining, Failing
    public int YearsInBusiness { get; set; }
    public decimal PeakWealth { get; set; }
}

public class BusinessEmployee
{
    public long Id { get; set; }
    public long BusinessId { get; set; }
    public long PersonId { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public string Role { get; set; } = string.Empty; // Worker, Manager, Specialist
}
