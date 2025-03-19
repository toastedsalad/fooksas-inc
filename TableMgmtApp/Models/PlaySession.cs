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
    private TimeSpan _remainingTime;

    public PlaySession(ITimeProvider timeProvider) {
        _timeProvider = timeProvider;
    }

    public PlaySession(ITimeProvider timeProvider, TimeSpan timedSessionSpan) {
        _timeProvider = timeProvider;
        _isTimedSession = true;
        _remainingTime = new TimeSpan();
        TimedSessionSpan = timedSessionSpan;
    }

    public TimeSpan GetPlayTime(bool setTime = true) {
        if (IsStopActive) {
            return PlayTime;
        }

        if (setTime) {
            PlayTime += _timeProvider.Now - StartTime;
            return PlayTime;
        }
        else {
            return _timeProvider.Now - StartTime + PlayTime;
        }
    }

    public TimeSpan GetRemainingPlayTime() {
        if (IsStopActive) {
            _remainingTime = TimedSessionSpan.Subtract(PlayTime);
        } 
        else {
            _remainingTime = TimedSessionSpan.Subtract(GetPlayTime(false));
        }

        if (_remainingTime.TotalSeconds <= 0) {
            Stop();
        }
        return _remainingTime;
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


