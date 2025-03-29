namespace TableMgmtApp;

public class PlaySession {
    public Guid Id  { get; set; } = Guid.NewGuid();
    // TODO Do we need endtime here?
    public DateTime StartTime { get; set; }
    public TimeSpan PlayTime { get; set; }
    public decimal Price { get; set; } 
    public int TableNumber { get; set; }
    public Guid PlayerId { get; set; }
}

