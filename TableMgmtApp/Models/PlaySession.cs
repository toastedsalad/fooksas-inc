namespace TableMgmtApp;

public class PlaySession {
    public Guid Id { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan PlayTime { get; private set; }
    public TimeSpan TimedSessionSpan { get; }
    public bool IsStopActive {get; private set; }

    private ITimeProvider _timeProvider;
    private DateTime _pauseStart;
    private bool _isTimedSession;

    public PlaySession(ITimeProvider timeProvider) {
        _timeProvider = timeProvider;
    }

    public PlaySession(ITimeProvider timeProvider, TimeSpan timedSessionSpan) {
        _timeProvider = timeProvider;
        _isTimedSession = true;
        TimedSessionSpan = timedSessionSpan;
    }

    public TimeSpan GetPlayTime() {
        if (IsStopActive) {
            return PlayTime;
        }

        PlayTime += _timeProvider.Now - StartTime;
        return PlayTime;
    }

    public TimeSpan GetRemainingPlayTime() {
        var remainingTime = new TimeSpan();

        if (IsStopActive) {
            remainingTime = TimedSessionSpan.Subtract(PlayTime);
        }
        else {
            remainingTime = TimedSessionSpan.Subtract(_timeProvider.Now - StartTime);
        }

        if (remainingTime.TotalSeconds <= 0) {
            Stop();
        }
        return remainingTime;
    }

    public void Start() {
        Id = Guid.NewGuid();
        StartTime = _timeProvider.Now;
        IsStopActive = false;
    }

    public void Stop() {
        if (IsStopActive) {
            return;
        }

        _pauseStart = _timeProvider.Now;
        IsStopActive = true;
        PlayTime += _pauseStart - StartTime;
    }

    public void Resume() {
        if (!IsStopActive) {
            return;
        }

        IsStopActive = false;
        StartTime = _timeProvider.Now;
    }
}


