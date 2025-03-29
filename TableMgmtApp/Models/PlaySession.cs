using System.Timers;

namespace TableMgmtApp;

public class PlaySession {
    public Guid Id { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan PlayTime { get; private set; } = TimeSpan.Zero;
    public TimeSpan TimedSessionSpan { get; }
    public TableManager TableManager {get; private set; }
    public bool IsStopActive { get; private set; }
    public ITimer Timer { get; private set; }

    private decimal _price = 0.00m;
    private ITimeProvider _timeProvider;
    private bool _isTimedSession;
    private TimeSpan _remainingTime;
    private Schedule _schedule;

    public PlaySession(ITimeProvider timeProvider, ITimer timer, Schedule schedule,
                       TableManager tableManager) {
        _timeProvider = timeProvider;
        Timer = timer;
        _schedule = schedule;
        TableManager = tableManager;
    }

    public PlaySession(ITimeProvider timeProvider, TimeSpan timedSessionSpan,
                       ITimer timer, TableManager tableManager, Schedule schedule) {
        _timeProvider = timeProvider;
        _isTimedSession = true;
        _remainingTime = new TimeSpan();
        TimedSessionSpan = timedSessionSpan;
        Timer = timer;
        TableManager = tableManager;
        _schedule = schedule;
    }

    public TimeSpan GetPlayTime(bool setTime = true) {
        return PlayTime;
    }

    public TimeSpan GetRemainingPlayTime() {
        _remainingTime = TimedSessionSpan.Subtract(PlayTime);

        if (_remainingTime.TotalSeconds <= 0) {
            TableManager.SetStandby();
            Stop();
        }

        return _remainingTime;
    }

    public decimal GetSessionPrice() {
        return Math.Round(_price, 2, MidpointRounding.ToEven);
    }

    private void TimedEvent(Object? source, ElapsedEventArgs args) {
        PlayTime += TimeSpan.FromSeconds(1);
        _price += ScheduleService.GetCurrentRate(_schedule, _timeProvider) / 60 / 60;
    }

    public void Start() {
        Id = Guid.NewGuid();
        StartTime = _timeProvider.Now;
        IsStopActive = false;
        Timer.Elapsed += TimedEvent;
        Timer.AutoReset = true;
        Timer.Enabled = true;
    }

    public void Stop(bool disposeTimers = false) {
        if (IsStopActive) {
            return;
        }

        IsStopActive = true;
        Timer.Stop();

        if (disposeTimers) {
            Timer.Elapsed -= TimedEvent;
            Timer.Dispose();
            Timer = null!;
        }
    }

    public void Resume() {
        if (!IsStopActive) {
            return;
        }

        IsStopActive = false;
        Timer.Start();
    }
}


