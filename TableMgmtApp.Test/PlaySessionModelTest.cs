namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class PlaysessionModelTest {
    [Test]
    public void WhenSessionStartItStarts() {
        var systemTimer = new SystemTimeProvider();
        var session = new PlaySession(systemTimer);
        session.Start();

        Assert.That(session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
        Assert.That(session.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(session.IsSessionActive, Is.True);
    }

    [Test]
    public void SessionElapsedTimeDisplayHowMuchTimeWasPlayed() {
        var systemTimer = new SystemTimeProvider();
        var session = new PlaySession(systemTimer);
        session.Start();
        Thread.Sleep(1 * 1000);
        var gameTime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gameTime, Is.EqualTo(1));
    }

    [Test]
    public void PlaySessionTakesInCustomTimer() {
        var fakeTimer = new FakeTimeProvider();
        fakeTimer.Now = new DateTime(2025, 12, 28);
        var session = new PlaySession(fakeTimer);
        session.Start();

        Assert.That(session.StartTime.Day, Is.EqualTo(28));
    }

    [Test]
    public void WhenTimeIncreasesWithFakerTimerPlayTimeIncreasesAsWell() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(5));
    }

    [Test]
    public void WhenSessionIsPausedGameTimeDoesNotIncrease() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(0));
    }

    [Test]
    public void SeesionCanBeResumedFromPause() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(5));
    }

    [Test]
    public void WhenSessionIsPausedReturnLastPlayTime() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenPausingMultipleTimesItCountsAsOneTime() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenResumingMultipleTimesItCountsAsOneTime() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(20));
    }
}










