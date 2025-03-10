namespace TableMgmtApp;

public enum TableState {
    PlayOn,
    Off,
    Paused,
    Standby
}

public class PlaySession {
    public Guid Id { get; private set; }
    public bool IsSessionActive { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan PlayTime { get; private set; }

    private ITimeProvider _timeProvider;
    private DateTime _pauseStart;

    public PlaySession(ITimeProvider timeProvider) {
        _timeProvider = timeProvider;
    }

    public TimeSpan GetPlayTime() {
        return PlayTime;
    }

    public void Start() {
        Id = Guid.NewGuid();
        StartTime = _timeProvider.Now;
        IsSessionActive = true;
    }

    public void Pause() {
        _pauseStart = _timeProvider.Now;
        PlayTime += _pauseStart - StartTime;
    }

    // We need methods to pause and to resume session.
    // Time when paused should not count in PlayTime.
    // I also need a better way to manage time...
    // Create an interface and inject fake time perhaps.
}

public class Table {
    public int Id { get; private set; }
    public TableState State { get; private set; } = TableState.Off;
    public int PauseTimer { get; private set; }
    public PlaySession Session { get; private set; }
    // Keep a record of last three sessions.
    
    public Table(int id, ITimeProvider timeProvider, int pauseTimer = 1) {
        Id = id;
        PauseTimer = pauseTimer;
        Session = new PlaySession(timeProvider);
    }

    public void SetState(TableState newState) {
        if (State == TableState.Off && newState == TableState.PlayOn) {
            State = newState;
            Session.Start();
        }

        if (State == TableState.PlayOn && newState == TableState.Off) {
            State = TableState.Paused;
            StartPauseTimer();
        }

        if (State == TableState.Paused && newState == TableState.PlayOn) {
            State = TableState.PlayOn;
        }

        if (State == TableState.Standby && newState == TableState.Off) {
            State = TableState.Off;
        }
    }

    private async void StartPauseTimer() {
        await Task.Delay(PauseTimer * 1000);
        if (State == TableState.Paused) {
            State = TableState.Standby;
        }
    }
}

