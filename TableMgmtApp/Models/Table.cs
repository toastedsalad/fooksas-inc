namespace TableMgmtApp;

public enum TableState {
    PlayOn,
    Off,
    Paused,
    Standby
}

public class Table {
    public int Id { get; private set; }
    public TableState State { get; private set; } = TableState.Off;
    public int PauseTimer { get; private set; }
    public PlaySession Session { get; private set; }
    // TODO: Keep a record of last three sessions.
    
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

    // TODO:
    // This is something I don't like.
    // Let's change it instead to Task.Deelay we do time maths 
    // so that we can use our fake timer.
    // Paused table state is to account for accidental switch on and off.
    // During this time the session should be live.
    private async void StartPauseTimer() {
        await Task.Delay(PauseTimer * 1000);
        if (State == TableState.Paused) {
            State = TableState.Standby;
        }
    }
}

