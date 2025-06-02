namespace TableMgmtApp;

public class Player {
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public Guid? DiscountId { get; set; }

    // Navigation properties. 
    // Can have one discount:
    public Discount? Discount { get; set; }
    // Can have maney sessions:
    public ICollection<PlaySession> PlaySessions { get; set; } = new List<PlaySession>();

    public Player(string name, string surname, string email) {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
        Name = name;
        Surname = surname;
        Email = email;
    }
}

public record PlayerDTO {
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public string? Email { get; init; }
    public Guid DiscountId { get; init; }
    public string? DiscountType { get; init; }
    public string? DiscountName { get; init; }
    public int? DiscountRate { get; init; }
}
