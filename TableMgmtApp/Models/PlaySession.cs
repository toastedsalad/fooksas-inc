using System.Timers;

namespace TableMgmtApp;

public class PlaySession {
    public Guid Id { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan PlayTime { get; private set; } = TimeSpan.Zero;
    public TimeSpan TimedSessionSpan { get; }
    public bool IsStopActive {get; private set; }

    private ITimeProvider _timeProvider;
    private bool _isTimedSession;
    private TimeSpan _remainingTime;
    private ITimer _timer;

    public PlaySession(ITimeProvider timeProvider, ITimer timer) {
        _timeProvider = timeProvider;
        _timer = timer;
    }

    public PlaySession(ITimeProvider timeProvider, TimeSpan timedSessionSpan,
                       ITimer timer) {
        _timeProvider = timeProvider;
        _isTimedSession = true;
        _remainingTime = new TimeSpan();
        TimedSessionSpan = timedSessionSpan;
        _timer = timer;
    }

    public TimeSpan GetPlayTime(bool setTime = true) {
        return PlayTime;
    }

    public TimeSpan GetRemainingPlayTime() {
        _remainingTime = TimedSessionSpan.Subtract(PlayTime);

        if (_remainingTime.TotalSeconds <= 0) {
            Stop();
        }

        return _remainingTime;
    }

    private void TimedEvent(Object source, ElapsedEventArgs args) {
        PlayTime += TimeSpan.FromSeconds(1);
    }

    public void Start() {
        Id = Guid.NewGuid();
        StartTime = _timeProvider.Now;
        IsStopActive = false;
        _timer.Elapsed += TimedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    public void Stop() {
        if (IsStopActive) {
            return;
        }

        IsStopActive = true;
        // TODO: When to dispose of the timer?
        _timer.Stop();
    }

    public void Resume() {
        if (!IsStopActive) {
            return;
        }

        IsStopActive = false;
        _timer.Start();
    }
}


