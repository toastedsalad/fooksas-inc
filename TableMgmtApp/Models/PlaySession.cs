using System.Timers;

namespace TableMgmtApp;

public class PlaySession {
    public Guid Id { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan PlayTime { get; private set; } = TimeSpan.Zero;
    public TimeSpan TimedSessionSpan { get; }
    public bool IsStopActive { get; private set; }
    public ITimer Timer { get; private set; }

    private ITimeProvider _timeProvider;
    private bool _isTimedSession;
    private TimeSpan _remainingTime;
    private Table _table;

    public PlaySession(ITimeProvider timeProvider, ITimer timer) {
        _timeProvider = timeProvider;
        Timer = timer;
        _table = null!;
    }

    public PlaySession(ITimeProvider timeProvider, TimeSpan timedSessionSpan,
                       ITimer timer, Table table) {
        _timeProvider = timeProvider;
        _isTimedSession = true;
        _remainingTime = new TimeSpan();
        TimedSessionSpan = timedSessionSpan;
        Timer = timer;
        _table = table;
    }

    public TimeSpan GetPlayTime(bool setTime = true) {
        return PlayTime;
    }

    public TimeSpan GetRemainingPlayTime() {
        _remainingTime = TimedSessionSpan.Subtract(PlayTime);

        if (_remainingTime.TotalSeconds <= 0) {
            _table.SetStandby();
            Stop();
        }

        return _remainingTime;
    }

    private void TimedEvent(Object? source, ElapsedEventArgs args) {
        PlayTime += TimeSpan.FromSeconds(1);
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


