namespace TableMgmtApp;

public class PoolTable {
    public int Number { get; private set; }
    public Guid Id { get; private set; }

    public PoolTable(int number) {
        Number = number;
        Id = Guid.NewGuid();
    }
}

