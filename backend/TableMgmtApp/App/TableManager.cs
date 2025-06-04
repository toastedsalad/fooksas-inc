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
    public PoolTable Table { get; private set; }
    public TableState State { get; private set; } = TableState.Off;
    public int PauseTimer { get; private set; }
    public PlaySessionManager SessionManager { get; private set; } = default!;
    public DateTime PauseStart { get; private set; }
    public RingBuffer<PlaySession> LatestSessions { get; private set; } = 
        new RingBuffer<PlaySession>(3);
    public ITimeProvider TimeProvider { get; private set; }
    public IPlaySessionRepositoryFactory PlaySessionRepoFactory { get; private set; }

    private IScheduleServiceFactory _scheduleServiceFactory;
    private ICustomTimer _timer;

    public TableManager(PoolTable table, ITimeProvider timeProvider, 
                        IPlaySessionRepositoryFactory playSessionRepoFactory, 
                        IScheduleServiceFactory scheduleServiceFactory,
                        int pauseTimer = 1, ICustomTimer timer = null!) {
        Table = table;
        TimeProvider = timeProvider;
        PlaySessionRepoFactory = playSessionRepoFactory;
        _scheduleServiceFactory = scheduleServiceFactory;
        PauseTimer = pauseTimer;
        _timer = timer;
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

    private async Task Play(int timedSeconds) {
        using var serviceWrapper = _scheduleServiceFactory.CreateService();
        var service = serviceWrapper.Service;
        Schedule schedule = null!;

        var scheduleDTO = await service.GetById(Table.ScheduleId);
        if (scheduleDTO != null) {
            schedule = ScheduleService.FromScheduleDTO(scheduleDTO);
        } else {
            schedule = new Schedule();
        }

        serviceWrapper.Dispose();

        State = TableState.Play;
        // We need to somehow pass the discount here.
        // OR handle it on the frontend as we have the APIs there already?
        // This will be interesting...
        //
        //
        // 1. Press button
        // 2. A list of discounts appear
        // 3. Select discount.
        // 4. From the table manager get table.
        // 5. From table get session.
        // 6. Edit session to add a discount.
        // 7. Post the update to in mem object.
        if (timedSeconds == 0) {
            // Or resolve the discount inside the session.
            SessionManager = new PlaySessionManager(schedule, this, _timer);
        } else {
            SessionManager = new PlaySessionManager(schedule, this, 
                                                    new TimeSpan(0, 0, timedSeconds),
                                                    _timer);
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
        SessionManager.Shutdown();
        SessionManager = null!;
    }

    public async Task StartPauseTimer() {
        await TimeProvider.DelayAsync(PauseTimer * 1000);
        if (State == TableState.Paused) {
            Standby();
        }
    }
}

