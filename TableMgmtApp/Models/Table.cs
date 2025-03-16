
namespace TableMgmtApp;

public enum TableState {
    Play,
    Off,
    Paused,
    Standby
}

public class Table {
    public int Id { get; private set; }
    public TableState State { get; private set; } = TableState.Off;
    public int PauseTimer { get; private set; }
    public PlaySession Session { get; private set; }
    public DateTime PauseStart { get; private set; }
    public RingBuffer<PlaySession> LatestSessions { get; private set; } = 
        new RingBuffer<PlaySession>(3);

    ITimeProvider _timeProvider;

    public Table(int id, ITimeProvider timeProvider, int pauseTimer = 1) {
        Id = id;
        PauseTimer = pauseTimer;
        Session = new PlaySession(timeProvider);
        _timeProvider = timeProvider;
    }

    public void SetPlay() {
        if (State == TableState.Off) {
            State = TableState.Play;
            Session.Start();
        }
        else {
            Session.Resume();
        }
    }

    public void SetStanby() {
        if (State == TableState.Play || State == TableState.Paused) {
            State = TableState.Standby;
            Session.Stop();
        }
    }

    public void SetOff() {
        if (State == TableState.Standby) {
            State = TableState.Off;
            LatestSessions.EnQueue(Session);
            Session.Stop();
        }
    }

    // We are not creating new sesssions?
    public void SetStateBySwitch(TableState newState) {
        if (State == TableState.Off && newState == TableState.Play) {
            State = newState;
            Session.Start();
        }

        if (State == TableState.Play && newState == TableState.Off) {
            State = TableState.Paused;
            StartPauseTimer();
        }

        if (State == TableState.Paused && newState == TableState.Play) {
            State = TableState.Play;
            Session.Resume();
        }

        if (State == TableState.Standby && newState == TableState.Off) {
            State = TableState.Off;
            LatestSessions.EnQueue(Session);
            Session.Stop();
        }

        if (State == TableState.Standby && newState == TableState.Play) {
            State = TableState.Play;
            Session.Resume();
        }
    }

    public async void StartPauseTimer() {
        await _timeProvider.DelayAsync(PauseTimer * 1000);
        if (State == TableState.Paused) {
            State = TableState.Standby;
            Session.Stop();
        }
    }

}

