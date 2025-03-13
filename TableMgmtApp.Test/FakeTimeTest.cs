namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class FakeTimeTest {
    [Test]
    public void RegularClockReturnsImplementationReturnsNow() {
        var systemTime = new SystemTimeProvider();
        Assert.That(systemTime.Now.Minute, Is.EqualTo(DateTime.Now.Minute));
    }

    [Test]
    public void MockedTimeReturnsTimeFromInput() {
        var fakeTime = new FakeTimeProvider();
        fakeTime.Now = new DateTime(2025, 10, 28);
        Assert.That(fakeTime.Now.Year, Is.EqualTo(2025));
        Assert.That(fakeTime.Now.Month, Is.EqualTo(10));
        Assert.That(fakeTime.Now.Day, Is.EqualTo(28));
    }

    [Test]
    public void MockedTimerCanSimulateDelay() {
        var fakeTime = new FakeTimeProvider();
        var delay1 = fakeTime.DelayAsync(2000);
        fakeTime.AdvanceTimeBySeconds(1);

        Assert.That(delay1.IsCompleted, Is.False);

        var delay2 = fakeTime.DelayAsync(4000);
        fakeTime.AdvanceTimeBySeconds(1);

        Assert.That(delay1.IsCompleted, Is.True);
        Assert.That(delay2.IsCompleted, Is.False);

        fakeTime.AdvanceTimeBySeconds(4);

        Assert.That(delay2.IsCompleted, Is.True);
    }
}

