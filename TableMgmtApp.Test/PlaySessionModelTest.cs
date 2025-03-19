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
        Assert.That(session.IsStopActive, Is.False);
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
    public void WhenSessionIsPausedAndResumedReturnPlayTimeWithoutPause() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(6));
    }

    [Test]
    public void WhenSessionIsPausedAndResumedAndStoppedReturnPlayTimeWithoutPauses() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(9));
    }

    [Test]
    public void WhenSessionIsResumedMultipleTimesAndStoppedAtTheEndItReturnsPlayTime() {
        var fakeTimer = new FakeTimeProvider();
        var session = new PlaySession(fakeTimer);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(9));
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

    [Test]
    public void PlaySessionHasTimedSessionSpan() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var session = new PlaySession(fakeTimer, timedSessionSpan);
        
        Assert.That(session.TimedSessionSpan.TotalSeconds, Is.EqualTo(5));
    }

    [Test]
    public void WhenTimeProceedsRemainingTimesGoesDown() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var session = new PlaySession(fakeTimer, timedSessionSpan);
        session.Start();

        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
    
        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
    }

    [Test]
    public void WhenTimedSessionExpiresItIsStopped() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var session = new PlaySession(fakeTimer, timedSessionSpan);
        session.Start();

        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
    
        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(5));
        Assert.That(session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTimerIsStoppedTimedSessionCounterDoesntGoDown() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var session = new PlaySession(fakeTimer, timedSessionSpan);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(7));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(3));
        Assert.That(session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTimedSessionIsStoppedAndResumedPausedTimeDoesntCount() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var session = new PlaySession(fakeTimer, timedSessionSpan);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(session.IsStopActive, Is.False);
    }

    [Test]
    public void WhenTimedSessionIsStoppedAndResumedAndStoppedPausedTimeDoesntCount() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var session = new PlaySession(fakeTimer, timedSessionSpan);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenSessionsIsStoppedAndResumedMultipleTimesPausedTimeDoesntCount() {
        var fakeTimer = new FakeTimeProvider();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var session = new PlaySession(fakeTimer, timedSessionSpan);

        session.Start();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);
        session.Stop();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);
        session.Resume();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(3);

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(1));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(9));
        Assert.That(session.IsStopActive, Is.False);
    }
}








