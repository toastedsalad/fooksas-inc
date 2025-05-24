namespace TableMgmtApp;

public class PoolTable {
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Number { get; private set; }
    public Guid ScheduleId { get; internal set; }

    public PoolTable(int number, string name = "Pool") {
        Number = number;
        Name = name;
        Id = Guid.NewGuid();
        ScheduleId = Guid.Empty;
    }
}

