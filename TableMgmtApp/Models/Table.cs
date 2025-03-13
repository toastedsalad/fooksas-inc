
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
    public DateTime PauseStart { get; set; }

    ITimeProvider _timeProvider;
    // TODO: Keep a record of last three sessions.

    public Table(int id, ITimeProvider timeProvider, int pauseTimer = 1) {
        Id = id;
        PauseTimer = pauseTimer;
        Session = new PlaySession(timeProvider);
        _timeProvider = timeProvider;
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
            Session.Resume();
        }

        if (State == TableState.Standby && newState == TableState.Off) {
            State = TableState.Off;
            Session.Stop();
            // TODO: And add a completed session to the list of sessions.
        }

        // TODO: When on standby and play on is sent table is on play on.
    }

    public async void StartPauseTimer() {
        await _timeProvider.DelayAsync(PauseTimer * 1000);
        if (State == TableState.Paused) {
            State = TableState.Standby;
            Session.Stop();
        }
    }
}

