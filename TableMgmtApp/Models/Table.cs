namespace TableMgmtApp;

public class Table {
    public int Number { get; private set; }
    public Guid Id { get; private set; }

    public Table(int number) {
        Number = number;
        Id = Guid.NewGuid();
    }
}

