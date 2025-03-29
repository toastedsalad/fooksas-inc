using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public enum TableState {
    Play,
    Off,
    Paused,
    Standby
}

// This should become a TableManager
// A smaller object should Table will be stored and retrieved from a db
public class TableManager {
    public int TableNumber { get; private set; }
    public Table Table { get; private set; }
    public TableState State { get; private set; } = TableState.Off;
    public int PauseTimer { get; private set; }
    public PlaySessionManager SessionManager { get; private set; } = default!;
    public DateTime PauseStart { get; private set; }
    public RingBuffer<PlaySession> LatestSessions { get; private set; } = 
        new RingBuffer<PlaySession>(3);
    // Do I really want this to be a prop?
    // Maybe we can have something that gets the schedule...
    public Schedule Schedule { get; set; } = new Schedule();
    public ITimeProvider TimeProvider { get; private set; }
    public ITimer Timer { get; private set; }
    public IPlaySessionRepository PlaySessionRepository { get; private set; }

    public TableManager(Table table, ITimeProvider timeProvider, ITimer timer, 
                        IPlaySessionRepository playSessionRepository, int pauseTimer = 1) {
        TableNumber = table.Number;
        Table = table;
        TimeProvider = timeProvider;
        Timer = timer;
        PlaySessionRepository = playSessionRepository;
        PauseTimer = pauseTimer;
    }

    public void SetPlay(int timedSeconds = 0) {
        if (State == TableState.Off) {
            Play(timedSeconds);
        } else {
            State = TableState.Play;
            SessionManager.Resume();
        }
    }

    public void SetStandby() {
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
            Play(0);
        }

        if (State == TableState.Play && newState == TableState.Off) {
            State = TableState.Paused;
            StartPauseTimer();
        }

        if (State == TableState.Paused && newState == TableState.Play) {
            State = TableState.Play;
            SessionManager.Resume();
        }

        if (State == TableState.Standby && newState == TableState.Off) {
            Off();
        }

        if (State == TableState.Standby && newState == TableState.Play) {
            State = TableState.Play;
            SessionManager.Resume();
        }
    }

    private void Play(int timedSeconds) {
        State = TableState.Play;
        if (timedSeconds == 0) {
            SessionManager = new PlaySessionManager(Schedule, this);
        } else {
            SessionManager = new PlaySessionManager(Schedule, this, 
                                                    new TimeSpan(0, 0, timedSeconds));
        }
        SessionManager.Start();
    }

    private void Standby() {
        State = TableState.Standby;
        SessionManager.Stop();
    }

    private void Off() {
        State = TableState.Off;
        LatestSessions.EnQueue(SessionManager.Session);
        SessionManager.Stop(true);
        SessionManager = null!;
    }

    public async void StartPauseTimer() {
        await TimeProvider.DelayAsync(PauseTimer * 1000);
        if (State == TableState.Paused) {
            Standby();
        }
    }
}

