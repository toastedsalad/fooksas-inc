namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class PlaysessionModelTest {
    [Test]
    public void WhenSessionStartItStarts() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);
        session.Start();

        Assert.That(session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
        Assert.That(session.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(session.IsStopActive, Is.False);
    }

    [Test]
    public void SessionElapsedTimeDisplayHowMuchTimeWasPlayed() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);
        session.Start();
        fakeTimer.TriggerElapsed();
        var gameTime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gameTime, Is.EqualTo(1));
    }

    [Test]
    public void PlaySessionTakesInCustomTimer() {
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 12, 28);
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);
        session.Start();

        Assert.That(session.StartTime.Day, Is.EqualTo(28));
    }

    [Test]
    public void WhenTimeIncreasesWithFakerTimerPlayTimeIncreasesAsWell() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenSessionIsPausedGameTimeDoesNotIncrease() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        // TODO: Can we somehow check timer state?
        session.Stop();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(0));
    }

    [Test]
    public void SesionCanBeResumedFromPause() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenSessionIsPausedReturnLastPlayTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenSessionIsPausedAndResumedReturnPlayTimeWithoutPause() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(6));
    }

    [Test]
    public void WhenSessionIsPausedAndResumedAndStoppedReturnPlayTimeWithoutPauses() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(9));
    }

    [Test]
    public void WhenSessionIsResumedMultipleTimesAndStoppedAtTheEndItReturnsPlayTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(9));
    }

    [Test]
    public void WhenPausingMultipleTimesItCountsAsOneTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Stop();
        session.Stop();
        session.Stop();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenResumingMultipleTimesItCountsAsOneTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(20));
    }

    [Test]
    public void PlaySessionHasTimedSessionSpan() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);
        
        Assert.That(session.TimedSessionSpan.TotalSeconds, Is.EqualTo(5));
    }

    [Test]
    public void WhenTimeProceedsRemainingTimesGoesDown() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);
        session.Start();

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
    
        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
    }

    [Test]
    public void WhenTimedSessionExpiresItIsStopped() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);
        session.Start();

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
    
        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(5));
        Assert.That(session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTimerIsStoppedTimedSessionCounterDoesntGoDown() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(7));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(3));
        Assert.That(session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTimedSessionIsStoppedAndResumedPausedTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(session.IsStopActive, Is.False);
    }

    [Test]
    public void WhenTimedSessionIsStoppedAndResumedAndStoppedPausedTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenSessionsIsStoppedAndResumedMultipleTimesPausedTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        session.Stop();
        session.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(1));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(9));
        Assert.That(session.IsStopActive, Is.False);
    }

    [Test]
    public void TimedSessionTracksProperlyWhenTimePasses() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new Table(1, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, table);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(7));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(3));
        Assert.That(session.IsStopActive, Is.False);

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)session.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(session.IsStopActive, Is.False);
    }
    
    [Test]
    public void WhenSessionIsArchivedTheTimersAreDisposed() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer);

        session.Start();
        fakeTimer.TriggerElapsed();

        session.Stop(true);
        Assert.That(session.IsStopActive, Is.True);
        Assert.That(session.Timer, Is.EqualTo(null));
    }
}








