using System.Timers;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public class PlaySessionManager {
    public PlaySession Session { get; private set;} = new PlaySession();
    public TimeSpan TimedSessionSpan { get; }
    public bool IsStopActive { get; private set; }
    public TableManager TableManager {get; private set; }

    private ITimeProvider _timeProvider;
    private ICustomTimer _timer;
    private bool _isTimedSession;
    private TimeSpan _remainingTime;
    private Schedule _schedule;
    private readonly IPlaySessionRepositoryFactory _playSessionRepoFactory;

    public PlaySessionManager(Schedule schedule, TableManager tableManager) {
        _schedule = schedule;
        TableManager = tableManager;
        _playSessionRepoFactory = tableManager.PlaySessionRepoFactory;
        _timeProvider = tableManager.TimeProvider;
        _timer = tableManager.Timer;
    }

    public PlaySessionManager(Schedule schedule, TableManager tableManager,
                              TimeSpan timedSessionSpan) {
        _schedule = schedule;
        TableManager = tableManager;
        _playSessionRepoFactory = tableManager.PlaySessionRepoFactory;
        _timeProvider = tableManager.TimeProvider;
        _timer = tableManager.Timer;
        TimedSessionSpan = timedSessionSpan;
        _isTimedSession = true;
        _remainingTime = new TimeSpan();
    }

    public TimeSpan GetPlayTime(bool setTime = true) {
        return Session.PlayTime;
    }

    public TimeSpan GetRemainingPlayTime() {
        _remainingTime = TimedSessionSpan.Subtract(Session.PlayTime);

        if (_remainingTime.TotalSeconds <= 0) {
            TableManager.SetStandby();
            Stop();
        }

        return _remainingTime;
    }

    public decimal GetSessionPrice() {
        return Math.Round(Session.Price, 2, MidpointRounding.ToEven);
    }

    private void TimedEvent(Object? source, ElapsedEventArgs args) {
        Session.PlayTime += TimeSpan.FromSeconds(1);
        Session.Price += ScheduleService.GetCurrentRate(_schedule, _timeProvider) / 60 / 60;
    }

    public void Start() {
        Session.StartTime = _timeProvider.Now;
        Session.TableNumber = TableManager.TableNumber;
        IsStopActive = false;
        _timer.Elapsed += TimedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    public void Stop() {
        if (IsStopActive) {
            return;
        }

        // but second time I don't get here cause stop is already on.
        IsStopActive = true;
        _timer.Stop();
    }

    public async void Shutdown() {
        if (IsStopActive) {
            var repo = _playSessionRepoFactory.CreateRepository();
            await repo.AddAsync(Session);
            await repo.SaveAsync();
            _timer.Elapsed -= TimedEvent;
            _timer.Dispose();
            _timer = null!;
        }
    }

    public void Resume() {
        if (!IsStopActive) {
            return;
        }

        IsStopActive = false;
        _timer.Start();
    }
}


