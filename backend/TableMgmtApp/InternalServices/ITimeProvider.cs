namespace TableMgmtApp;

public interface ITimeProvider {
    DateTime Now { get; }
    Task DelayAsync(int miliseconds);
}

public class SystemTimeProvider : ITimeProvider {
    public DateTime Now => DateTime.Now;

    public async Task DelayAsync(int miliseconds) {
        await Task.Delay(miliseconds);
    }
}

public class FakeTimeProvider : ITimeProvider {
    public DateTime Now { get; set; } = DateTime.Now;
    private readonly List<(DateTime targetTime, TaskCompletionSource<bool> tcs)> _pendingDelays = new();

    public Task DelayAsync(int milliseconds) {
        var tcs = new TaskCompletionSource<bool>();
        var targetTime = Now.AddMilliseconds(milliseconds);
        _pendingDelays.Add((targetTime, tcs));
        return tcs.Task;
    }

    public void AdvanceTimeBySeconds(int seconds) {
        Now = Now.AddSeconds(seconds);

        var expiredDelays = _pendingDelays.Where(d => d.targetTime <= Now).ToList();
        foreach (var delay in expiredDelays) {
            delay.tcs.SetResult(true);
            _pendingDelays.Remove(delay);
        }
    }
}

