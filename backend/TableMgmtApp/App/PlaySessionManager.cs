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

    public PlaySessionManager(Schedule schedule, TableManager tableManager, 
                              ICustomTimer timer = null!) {
        _schedule = schedule;
        TableManager = tableManager;
        _playSessionRepoFactory = tableManager.PlaySessionRepoFactory;
        _timeProvider = tableManager.TimeProvider;
        _timer = timer;
    }

    public PlaySessionManager(Schedule schedule, TableManager tableManager,
                              TimeSpan timedSessionSpan, ICustomTimer timer = null!) {
        _schedule = schedule;
        TableManager = tableManager;
        _playSessionRepoFactory = tableManager.PlaySessionRepoFactory;
        _timeProvider = tableManager.TimeProvider;
        _timer = timer;
        TimedSessionSpan = timedSessionSpan;
        _isTimedSession = true;
        _remainingTime = timedSessionSpan;
    }

    public TimeSpan GetPlayTime(bool setTime = true) {
        return Session.PlayTime;
    }

    public TimeSpan GetRemainingPlayTime() {
        return _remainingTime;
    }

    public decimal GetSessionPrice() {
        return Math.Round(Session.Price, 2, MidpointRounding.ToEven);
    }

    private decimal ApplyDiscount(decimal rate) {
        if (Session.Discount == null) {
            return rate;
        }
        if (Session.Discount.Rate == 0) {
            return rate;
        }

        decimal discount = 1.0m - ((decimal)Session.Discount.Rate / 100);
        return rate * discount;
    }

    private void TimedEvent(Object? source, ElapsedEventArgs args) {
        Session.PlayTime += TimeSpan.FromSeconds(1);
        var currentTick = ApplyDiscount(ScheduleService.GetCurrentRate(_schedule, _timeProvider) / 60 / 60);
        Session.Price += currentTick;

        if (_isTimedSession) {
            _remainingTime = TimedSessionSpan.Subtract(Session.PlayTime);
        }

        if (_remainingTime.TotalSeconds <= 0 && _isTimedSession) {
            TableManager.SetStandby();
            Stop();
        }
    }

    public void Start() {
        if (_timer == null) {
            _timer = TimerFactory.CreateTimer();
        }
        Session.StartTime = _timeProvider.Now;
        Session.TableNumber = TableManager.Table.Number;
        Session.TableName = TableManager.Table.Name;
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
            using var repoWrapper = _playSessionRepoFactory.CreateRepository();
            var repo = repoWrapper.Repository;
            Session.Price = Math.Round(Session.Price, 2, MidpointRounding.ToEven);
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


