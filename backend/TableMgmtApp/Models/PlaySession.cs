namespace TableMgmtApp;

public class PlaySession {
    public Guid Id  { get; set; } = Guid.NewGuid();
    public DateTime StartTime { get; set; }
    public TimeSpan PlayTime { get; set; }
    public decimal Price { get; set; } 
    public string TableName { get; set; } = "None";
    public int TableNumber { get; set; }
    public Guid? PlayerId { get; set; }
    public Guid? DiscountId { get; set; }

    public Player? Player { get; set; } // Navigation property
    public Discount? Discount { get; set; } // Navigation property
}

public record PlaySessionDTO {
    public Guid Id { get; init; }
    public DateTime StartTime { get; init; }
    public TimeSpan PlayTime { get; init; }
    public decimal Price { get; init; }
    public string? TableName { get; init; }
    public int TableNumber { get; init; }
    public Guid PlayerId { get; init; }
    public string? PlayerName { get; init; }
    public string? PlayerSurname { get; init; }
    public Guid? DiscountId { get; init; }
    public string? DiscountType { get; init; }
    public string? DiscountName { get; init; }
    public int? DiscountRate { get; init; }
}

