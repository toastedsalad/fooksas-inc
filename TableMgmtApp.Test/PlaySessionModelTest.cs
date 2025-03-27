namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class PlaysessionModelTest {
    private Schedule _schedule = new Schedule {
        WeeklyRates = new Dictionary<DayOfWeek, List<TimeRate>> {
            { 
                DayOfWeek.Monday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            },
            { 
                DayOfWeek.Tuesday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            },
            { 
                DayOfWeek.Wednesday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            },
            { 
                DayOfWeek.Thursday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            },
            { 
                DayOfWeek.Friday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            },
            { 
                DayOfWeek.Saturday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            },
            { 
                DayOfWeek.Sunday, new List<TimeRate> {
                    new TimeRate(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10.00m)
                }
            }
        }
    };

    [Test]
    public void WhenSessionStartItStarts() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);
        session.Start();

        Assert.That(session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
        Assert.That(session.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(session.IsStopActive, Is.False);
    }

    [Test]
    public void SessionElapsedTimeDisplayHowMuchTimeWasPlayed() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);
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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);
        session.Start();

        Assert.That(session.StartTime.Day, Is.EqualTo(28));
    }

    [Test]
    public void WhenTimeIncreasesWithFakerTimerPlayTimeIncreasesAsWell() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);
        
        Assert.That(session.TimedSessionSpan.TotalSeconds, Is.EqualTo(5));
    }

    [Test]
    public void WhenTimeProceedsRemainingTimesGoesDown() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);
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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);
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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);

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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);

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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);

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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);

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
        var table = new Table(1);
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer);
        var session = new PlaySession(fakeTimeProvider, timedSessionSpan, fakeTimer, tableManager, _schedule);

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
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

        session.Start();
        fakeTimer.TriggerElapsed();

        session.Stop(true);
        Assert.That(session.IsStopActive, Is.True);
        Assert.That(session.Timer, Is.EqualTo(null));
    }

    [Test]
    public void SessionHoldsSessionPrice() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);

        Assert.That(session.GetSessionPrice(), Is.EqualTo(0.00m));
    }

    [Test]
    public void WhenTicksSessionPriceIncreases() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var session = new PlaySession(fakeTimeProvider, fakeTimer, _schedule);
        fakeTimeProvider.Now = new DateTime(2025, 03, 24, 10, 0, 0);

        session.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That(session.GetSessionPrice(), Is.EqualTo(0.02m));
    }
}








