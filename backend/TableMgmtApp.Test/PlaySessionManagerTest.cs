namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class PlaySessionManagerTest {
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
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);
        sessionManager.Start();

        Assert.That(sessionManager.Session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
        Assert.That(sessionManager.Session.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(sessionManager.IsStopActive, Is.False);
    }

    [Test]
    public void SessionElapsedTimeDisplayHowMuchTimeWasPlayed() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);
        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        var gameTime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gameTime, Is.EqualTo(1));
    }

    [Test]
    public void PlaySessionTakesInCustomTimer() {
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.Now = new DateTime(2025, 12, 28);
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);
        sessionManager.Start();

        Assert.That(sessionManager.Session.StartTime.Day, Is.EqualTo(28));
    }

    [Test]
    public void WhenTimeIncreasesWithFakerTimerPlayTimeIncreasesAsWell() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenSessionIsPausedGameTimeDoesNotIncrease() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        // TODO: Can we somehow check timer state?
        sessionManager.Stop();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(0));
    }

    [Test]
    public void SesionCanBeResumedFromPause() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenSessionIsPausedReturnLastPlayTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);

        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenSessionIsPausedAndResumedReturnPlayTimeWithoutPause() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(6));
    }

    [Test]
    public void WhenSessionIsPausedAndResumedAndStoppedReturnPlayTimeWithoutPauses() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(9));
    }

    [Test]
    public void WhenSessionIsResumedMultipleTimesAndStoppedAtTheEndItReturnsPlayTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(9));
    }

    [Test]
    public void WhenPausingMultipleTimesItCountsAsOneTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Stop();
        sessionManager.Stop();
        sessionManager.Stop();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(3));
    }

    [Test]
    public void WhenResumingMultipleTimesItCountsAsOneTime() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        var gametime = sessionManager.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(20));
    }

    [Test]
    public void PlaySessionHasTimedSessionSpan() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);
        
        Assert.That(sessionManager.TimedSessionSpan.TotalSeconds, Is.EqualTo(5));
    }

    [Test]
    public void WhenTimeProceedsRemainingTimesGoesDown() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 5);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var session = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);
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
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);
        sessionManager.Start();

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
    
        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(5));
        Assert.That(sessionManager.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTimerIsStoppedTimedSessionCounterDoesntGoDown() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();

        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(7));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(3));
        Assert.That(sessionManager.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTimedSessionIsStoppedAndResumedPausedTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(sessionManager.IsStopActive, Is.False);
    }

    [Test]
    public void WhenTimedSessionIsStoppedAndResumedAndStoppedPausedTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();

        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(sessionManager.IsStopActive, Is.True);
    }

    [Test]
    public void WhenSessionsIsStoppedAndResumedMultipleTimesPausedTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        sessionManager.Stop();
        sessionManager.Resume();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(1));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(9));
        Assert.That(sessionManager.IsStopActive, Is.False);
    }

    [Test]
    public void TimedSessionTracksProperlyWhenTimePasses() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var timedSessionSpan = new TimeSpan(0, 0, 10);
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, timedSessionSpan, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(7));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(3));
        Assert.That(sessionManager.IsStopActive, Is.False);

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)sessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)sessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(sessionManager.IsStopActive, Is.False);
    }
    
    [Test]
    public void WhenSessionIsArchivedTheTimersAreDisposed() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();

        sessionManager.Stop();
        sessionManager.Shutdown();
        Assert.That(sessionManager.IsStopActive, Is.True);
    }

    [Test]
    public void SessionHoldsSessionPrice() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);
        sessionManager.Start();

        Assert.That(sessionManager.GetSessionPrice(), Is.EqualTo(0.00m));
    }

    [Test]
    public void WhenTicksSessionPriceIncreases() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var sessionManager = new PlaySessionManager(_schedule, tableManager, fakeTimer);
        fakeTimeProvider.Now = new DateTime(2025, 03, 24, 10, 0, 0);

        sessionManager.Start();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That(sessionManager.GetSessionPrice(), Is.EqualTo(0.02m));
    }

    [Test]
    public void PlaySessionManagerAppliesDiscounts() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var schedule = new Schedule();
        schedule.DefaultRate = 6;
        var sessionManager = new PlaySessionManager(schedule, tableManager, fakeTimer);

        var discount = new Discount("player", "default", 75);
        sessionManager.Start();
        sessionManager.Session.Discount = discount;

        for (var i = 0; i < 600; i++) {
            fakeTimer.TriggerElapsed();
        }

        Assert.That(sessionManager.GetSessionPrice(), Is.EqualTo(0.25m));
    }

    [Test]
    public void FullDiscountGivesNoPrice() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var schedule = new Schedule();
        schedule.DefaultRate = 6;
        var sessionManager = new PlaySessionManager(schedule, tableManager, fakeTimer);

        var discount = new Discount("player", "default", 100);
        sessionManager.Start();
        sessionManager.Session.Discount = discount;

        for (var i = 0; i < 600; i++) {
            fakeTimer.TriggerElapsed();
        }

        Assert.That(sessionManager.GetSessionPrice(), Is.EqualTo(0));
    }

    [Test]
    public void ZeroDiscountGivesFullPrice() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
         
        var tableManager = new TableManager(table, fakeTimeProvider, TestHelpers.GetRepoFactoryMock().Object, TestHelpers.GetServiceFactoryMock().Object);
        var schedule = new Schedule();
        schedule.DefaultRate = 6;
        var sessionManager = new PlaySessionManager(schedule, tableManager, fakeTimer);

        var discount = new Discount("player", "default", 0);
        sessionManager.Start();
        sessionManager.Session.Discount = discount;

        for (var i = 0; i < 600; i++) {
            fakeTimer.TriggerElapsed();
        }

        Assert.That(sessionManager.GetSessionPrice(), Is.EqualTo(1));
    }
}








