using System.Timers;

namespace TableMgmtApp;

public interface ICustomTimer : IDisposable {
    event ElapsedEventHandler Elapsed;
    void Start();
    void Stop();
    bool Enabled { get; set; }
    bool AutoReset { get; set; }
}

public class RealTimer : ICustomTimer {
    private readonly System.Timers.Timer _timer;

    public RealTimer(double interval) {
        _timer = new System.Timers.Timer(interval);
    }

    public event ElapsedEventHandler Elapsed {
        add { _timer.Elapsed += value; }
        remove { _timer.Elapsed -= value; }
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    public bool Enabled {
        get => _timer.Enabled;
        set => _timer.Enabled = value;
    }

    public bool AutoReset {
        get => _timer.AutoReset;
        set => _timer.AutoReset = value;
    }

    public void Dispose() {
        _timer.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class FakeTimer : ICustomTimer {
    public event ElapsedEventHandler Elapsed;
    public bool Enabled { get; set; }
    public bool AutoReset { get; set; } = true;
    private bool _disposed = false;

    public void Start() => Enabled = true;
    public void Stop() => Enabled = false;

    public void TriggerElapsed() {
        Elapsed?.Invoke(this, null);
        if (!AutoReset) Stop();
    }

    public void Dispose() {
        if (!_disposed) {
            Elapsed = null; // Remove all event subscriptions
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
