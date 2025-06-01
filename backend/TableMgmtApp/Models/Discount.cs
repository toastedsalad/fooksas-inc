namespace TableMgmtApp;

public class Discount {
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public int Rate { get; set; }

    // Navigation property
    public ICollection<PlaySession> PlaySessions { get; set; } = new List<PlaySession>();

    public Discount(string type, string name, int rate){
        Id = Guid.NewGuid();
        Type = type;
        Name = name;
        Rate = rate;
    }
}
