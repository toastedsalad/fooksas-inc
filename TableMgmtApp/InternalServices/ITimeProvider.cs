namespace TableMgmtApp;

public interface ITimeProvider {
    DateTime Now { get; }
}

public class SystemTimeProvider : ITimeProvider {
    public DateTime Now => DateTime.Now;
}

public class FakeTimeProvider : ITimeProvider {
    public DateTime Now { get; set; } = DateTime.Now;
}

