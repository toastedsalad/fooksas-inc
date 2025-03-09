namespace TableMgmtApp;

public enum TableState {
    PlayOn,
    Off,
    Paused,
    Standby
}

public class PlaySession {
    public Guid Id { get; private set; }
    public DateTime StartTime { get; private set; }
    public bool IsSessionActive { get; private set; }
    public TimeSpan PlayTime { get; private set; }

    public TimeSpan GetPlayTime() {
        PlayTime = DateTime.Now - StartTime;

        return PlayTime;
    }

    public void Start() {
        Id = Guid.NewGuid();
        StartTime = DateTime.Now;
        IsSessionActive = true;
    }

    // We need methods to pause and to resume session.
    // Time when paused should not count in PlayTime.
}

public class Table {
    public int Id { get; private set; }
    public TableState State { get; private set; } = TableState.Off;
    public int PauseTimer { get; private set; }
    public PlaySession Session { get; private set; } = new PlaySession();

    public Table(int id, int pauseTimer = 1) {
        Id = id;
        PauseTimer = pauseTimer;
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


