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
    public PlaySession Session { get; private set; } = default!;
    public DateTime PauseStart { get; private set; }
    public RingBuffer<PlaySession> LatestSessions { get; private set; } = 
        new RingBuffer<PlaySession>(3);

    ITimeProvider _timeProvider;

    public Table(int id, ITimeProvider timeProvider, int pauseTimer = 1) {
        Id = id;
        PauseTimer = pauseTimer;
        _timeProvider = timeProvider;
    }

    public void SetPlay() {
        if (State == TableState.Off) {
            Play();
        }
        else {
            State = TableState.Play;
            Session.Resume();
        }
    }

    public void SetStanby() {
        if (State == TableState.Play || State == TableState.Paused) {
            Standby();
        }
    }

    public void SetOff() {
        if (State == TableState.Standby) {
            Off();
        }
    }

    public void SetStateBySwitch(TableState newState) {
        if (State == TableState.Off && newState == TableState.Play) {
            Play();
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
            Off();
        }

        if (State == TableState.Standby && newState == TableState.Play) {
            State = TableState.Play;
            Session.Resume();
        }
    }

    private void Play() {
        State = TableState.Play;
        Session = new PlaySession(_timeProvider);
        Session.Start();
    }

    private void Standby() {
        State = TableState.Standby;
        Session.Stop();
    }

    private void Off() {
        State = TableState.Off;
        LatestSessions.EnQueue(Session);
        Session.Stop();
    }

    public async void StartPauseTimer() {
        await _timeProvider.DelayAsync(PauseTimer * 1000);
        if (State == TableState.Paused) {
            Standby();
        }
    }

}

