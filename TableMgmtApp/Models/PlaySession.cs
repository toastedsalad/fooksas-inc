namespace TableMgmtApp;

public class PlaySession {
    public Guid Id { get; private set; }
    public bool IsSessionActive { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan PlayTime { get; private set; }

    private ITimeProvider _timeProvider;
    private DateTime _pauseStart;
    private bool _isStopActive;

    public PlaySession(ITimeProvider timeProvider) {
        _timeProvider = timeProvider;
    }

    public TimeSpan GetPlayTime() {
        if (_isStopActive) {
            return PlayTime;
        }

        PlayTime = _timeProvider.Now - StartTime;
        return PlayTime;
    }

    public void Start() {
        Id = Guid.NewGuid();
        StartTime = _timeProvider.Now;
        IsSessionActive = true;
    }

    public void Stop() {
        if (_isStopActive) {
            return;
        }

        _pauseStart = _timeProvider.Now;
        _isStopActive = true;
        PlayTime += _pauseStart - StartTime;
    }

    public void Resume() {
        if (!_isStopActive) {
            return;
        }

        _isStopActive = false;
        StartTime = _timeProvider.Now;
    }
}


