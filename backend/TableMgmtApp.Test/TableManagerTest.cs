using Moq;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class TableManagerTest {
    [Test]
    public void WhenNewTableInitializedItHasAttributes() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, systemTimeProvider, systemTimer, mockPSRepo.Object);
        Assert.That(tableManager.TableNumber, Is.EqualTo(1));
    }

    [Test]
    public void WhenNewTableInitializedFourStatesAreAllowed() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, systemTimeProvider, systemTimer, mockPSRepo.Object);

        tableManager.SetStateBySwitch(TableState.Play);
        Assert.That(Enum.IsDefined(typeof(TableState), tableManager.State));

        tableManager.SetStateBySwitch(TableState.Off);
        Assert.That(Enum.IsDefined(typeof(TableState), tableManager.State));

        tableManager.SetStateBySwitch(TableState.Paused);
        Assert.That(Enum.IsDefined(typeof(TableState), tableManager.State));

        tableManager.SetStateBySwitch(TableState.Standby);
        Assert.That(Enum.IsDefined(typeof(TableState), tableManager.State));
    }

    [Test]
    public void InValidStateCannotBeSet() {
        int invalidState = 999; 
        bool isValid = Enum.IsDefined(typeof(TableState), invalidState);

        Assert.That(isValid, Is.False);
    }

    [Test]
    public void WhenTableIsInPlayOnItCanOnlyBeSetToPausedFirstWhenOffIsSent() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, systemTimeProvider, systemTimer, mockPSRepo.Object);

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Paused));
    }

    [Test]
    public void TableHasAPauseTimer () {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, systemTimeProvider, systemTimer, 
                                            mockPSRepo.Object, 5);

        Assert.That(tableManager.PauseTimer, Is.EqualTo(5));
    }

    [Test]
    public void IfTableIsInPauseItCanTransitionToStandByAfterPauseTimerExpires() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, 
                                            mockPSRepo.Object, 1); // Pause timer expires.
        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
    }
    
    [Test]
    public void WhenTableIsInPauseSendingPlayWithinTimerWillSetItToPlay() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, 
                                            mockPSRepo.Object, 2);
        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Paused));

        fakeTimer.TriggerElapsed();
        tableManager.SetStateBySwitch(TableState.Play);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
    }

    [Test]
    public void WhenTableStateIsInStandbySendingOffWillOffTheTable() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, 
                                            mockPSRepo.Object, 1);

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));
    }

    [Test]
    public void WhenTableIsOffSettingOnSetsAGuid() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, systemTimeProvider, systemTimer, 
                                            mockPSRepo.Object, 1);
        tableManager.SetStateBySwitch(TableState.Play);

        Assert.That(tableManager.SessionManager.Session.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void WhenTableIsOffSettingOnSetsASessionTimer() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, systemTimeProvider, systemTimer, mockPSRepo.Object, 1);
        tableManager.SetStateBySwitch(TableState.Play);

        Assert.That(tableManager.SessionManager.Session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
    }

    [Test]
    public void WhenTableIsInStandBySettingPlayOnWillContinueTheGame() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);
        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));

        tableManager.SetStateBySwitch(TableState.Play);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
    }

    [Test]
    public void TableHoldsLatestThreeSessions() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        Assert.That(tableManager.LatestSessions.circularArray.Length, Is.EqualTo(3));
        Assert.That(tableManager.LatestSessions.IsEmpty, Is.True);
    }

    [Test]
    public void WhenTableSessionIsStoppedItIsAddedToRingBuffer() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);
        
        Assert.That(tableManager.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));

        Assert.That(tableManager.LatestSessions.IsEmpty, Is.False);
    }

    [Test]
    public void OneTableCanCreateManySesssions() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));

        Assert.That(tableManager.LatestSessions.IsEmpty, Is.False);
        Assert.That(tableManager.LatestSessions.usageCount, Is.EqualTo(1));

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));
        Assert.That(tableManager.LatestSessions.usageCount, Is.EqualTo(2));
    }

    [Test]
    public void WhenTableIsSetToPlayOnNewSessionStartsOrItDoesNoting() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetPlay();

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));

        tableManager.SetPlay();

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
        Assert.That(tableManager.LatestSessions.IsEmpty, Is.True);
    }

    [Test]
    public void WhenTableIsSetToStandByTheSessionIsStopped() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetPlay();
        tableManager.SetStandby();

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        Assert.That(tableManager.SessionManager.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTableIsSetToOffSessionIsStoppedAndMovedToArchive() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetPlay();
        tableManager.SetStandby();
        tableManager.SetOff();

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));
        Assert.That(tableManager.SessionManager, Is.EqualTo(null));
    }

    [Test]
    public void WhenStoppedAndStartedANewUniqueSessionIsCreated() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));

        Assert.That(tableManager.LatestSessions.IsEmpty, Is.False);
        Assert.That(tableManager.LatestSessions.usageCount, Is.EqualTo(1));

        tableManager.SetStateBySwitch(TableState.Play);
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
        
        tableManager.SetStateBySwitch(TableState.Off);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Off));
        Assert.That(tableManager.LatestSessions.usageCount, Is.EqualTo(2));
        Assert.That(tableManager.LatestSessions.circularArray[0].Id, 
                Is.Not.EqualTo(tableManager.LatestSessions.circularArray[1].Id));
    }

    [Test]
    public void TableCanRunAndTrackATimedSession() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetPlay(10);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
        Assert.That((int)tableManager.SessionManager.TimedSessionSpan.TotalSeconds, Is.EqualTo(10));
        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(10));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));
    }

    [Test]
    public void WhenTableStopsTimedSessionPlayTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetPlay(10);

        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
        Assert.That((int)tableManager.SessionManager.TimedSessionSpan.TotalSeconds, Is.EqualTo(10));
        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(10));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));

        tableManager.SetStandby();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));
        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));

        tableManager.SetPlay();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That((int)tableManager.SessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
        
        tableManager.SetStandby();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That((int)tableManager.SessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));

        tableManager.SetPlay();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)tableManager.SessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));
    }

    [Test]
    public void WhenTimedSessionExpiresTableGoesToStandby() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new PoolTable(1);
        var mockPSRepo = new Mock<IPlaySessionRepository>();
        var tableManager = new TableManager(table, fakeTimeProvider, fakeTimer, mockPSRepo.Object, 1);

        tableManager.SetPlay(10);
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));
        Assert.That((int)tableManager.SessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(2));
        Assert.That(tableManager.State, Is.EqualTo(TableState.Play));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)tableManager.SessionManager.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
        Assert.That((int)tableManager.SessionManager.GetPlayTime().TotalSeconds, Is.EqualTo(10));
        Assert.That(tableManager.State, Is.EqualTo(TableState.Standby));
    }
}










