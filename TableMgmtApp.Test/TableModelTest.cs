namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class TableModelTest {
    [Test]
    public void WhenNewTableInitializedItHasAttributes() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new Table(1, systemTimeProvider, systemTimer);
        Assert.That(table.Id, Is.EqualTo(1));
    }

    [Test]
    public void WhenNewTableInitializedFourStatesAreAllowed() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new Table(1, systemTimeProvider, systemTimer);

        table.SetStateBySwitch(TableState.Play);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));

        table.SetStateBySwitch(TableState.Off);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));

        table.SetStateBySwitch(TableState.Paused);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));

        table.SetStateBySwitch(TableState.Standby);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));
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
        var table = new Table(1, systemTimeProvider, systemTimer);

        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));
    }

    [Test]
    public void TableHasAPauseTimer () {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new Table(1, systemTimeProvider, systemTimer, 5);

        Assert.That(table.PauseTimer, Is.EqualTo(5));
    }

    [Test]
    public void IfTableIsInPauseItCanTransitionToStandByAfterPauseTimerExpires() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1); // Pause timer expires.
        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
    }
    
    [Test]
    public void WhenTableIsInPauseSendingPlayWithinTimerWillSetItToPlay() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 2);
        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimer.TriggerElapsed();
        table.SetStateBySwitch(TableState.Play);

        Assert.That(table.State, Is.EqualTo(TableState.Play));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That(table.State, Is.EqualTo(TableState.Play));
    }

    [Test]
    public void WhenTableStateIsInStandbySendingOffWillOffTheTable() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);
        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));
    }

    [Test]
    public void WhenTableIsOffSettingOnSetsAGuid() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new Table(1, systemTimeProvider, systemTimer, 1);
        table.SetStateBySwitch(TableState.Play);

        Assert.That(table.Session.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void WhenTableIsOffSettingOnSetsASessionTimer() {
        var systemTimeProvider = new SystemTimeProvider();
        var systemTimer = new RealTimer(1000);
        var table = new Table(1, systemTimeProvider, systemTimer, 1);
        table.SetStateBySwitch(TableState.Play);

        Assert.That(table.Session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
    }

    [Test]
    public void WhenTableIsInStandBySettingPlayOnWillContinueTheGame() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);
        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));

        table.SetStateBySwitch(TableState.Play);

        Assert.That(table.State, Is.EqualTo(TableState.Play));
    }

    [Test]
    public void TableHoldsLatestThreeSessions() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        Assert.That(table.LatestSessions.circularArray.Length, Is.EqualTo(3));
        Assert.That(table.LatestSessions.IsEmpty, Is.True);
    }

    [Test]
    public void WhenTableSessionIsStoppedItIsAddedToRingBuffer() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);
        
        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));

        Assert.That(table.LatestSessions.IsEmpty, Is.False);
    }

    [Test]
    public void OneTableCanCreateManySesssions() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));

        Assert.That(table.LatestSessions.IsEmpty, Is.False);
        Assert.That(table.LatestSessions.usageCount, Is.EqualTo(1));

        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));
        Assert.That(table.LatestSessions.usageCount, Is.EqualTo(2));
    }

    [Test]
    public void WhenTableIsSetToPlayOnNewSessionStartsOrItDoesNoting() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetPlay();

        Assert.That(table.State, Is.EqualTo(TableState.Play));

        table.SetPlay();

        Assert.That(table.State, Is.EqualTo(TableState.Play));
        Assert.That(table.LatestSessions.IsEmpty, Is.True);
    }

    [Test]
    public void WhenTableIsSetToStandByTheSessionIsStopped() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetPlay();
        table.SetStandby();

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        Assert.That(table.Session.IsStopActive, Is.True);
    }

    [Test]
    public void WhenTableIsSetToOffSessionIsStoppedAndMovedToArchive() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetPlay();
        table.SetStandby();
        table.SetOff();

        Assert.That(table.State, Is.EqualTo(TableState.Off));
        Assert.That(table.Session, Is.EqualTo(null));
    }

    [Test]
    public void WhenStoppedAndStartedANewUniqueSessionIsCreated() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));

        Assert.That(table.LatestSessions.IsEmpty, Is.False);
        Assert.That(table.LatestSessions.usageCount, Is.EqualTo(1));

        table.SetStateBySwitch(TableState.Play);
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimeProvider.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetStateBySwitch(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));
        Assert.That(table.LatestSessions.usageCount, Is.EqualTo(2));
        Assert.That(table.LatestSessions.circularArray[0].Id, 
                Is.Not.EqualTo(table.LatestSessions.circularArray[1].Id));
    }

    [Test]
    public void TableCanRunAndTrackATimedSession() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetPlay(10);

        Assert.That(table.State, Is.EqualTo(TableState.Play));
        Assert.That((int)table.Session.TimedSessionSpan.TotalSeconds, Is.EqualTo(10));
        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(10));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));
    }

    [Test]
    public void WhenTableStopsTimedSessionPlayTimeDoesntCount() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetPlay(10);

        Assert.That(table.State, Is.EqualTo(TableState.Play));
        Assert.That((int)table.Session.TimedSessionSpan.TotalSeconds, Is.EqualTo(10));
        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(10));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));

        table.SetStandby();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));
        Assert.That(table.State, Is.EqualTo(TableState.Standby));

        table.SetPlay();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That((int)table.Session.GetPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That(table.State, Is.EqualTo(TableState.Play));
        
        table.SetStandby();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That((int)table.Session.GetPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That(table.State, Is.EqualTo(TableState.Standby));

        table.SetPlay();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(4));
        Assert.That((int)table.Session.GetPlayTime().TotalSeconds, Is.EqualTo(6));
        Assert.That(table.State, Is.EqualTo(TableState.Play));
    }

    [Test]
    public void WhenTimedSessionExpiresTableGoesToStandby() {
        var fakeTimeProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        var table = new Table(1, fakeTimeProvider, fakeTimer, 1);

        table.SetPlay(10);
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(8));
        Assert.That((int)table.Session.GetPlayTime().TotalSeconds, Is.EqualTo(2));
        Assert.That(table.State, Is.EqualTo(TableState.Play));

        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();
        fakeTimer.TriggerElapsed();

        Assert.That((int)table.Session.GetRemainingPlayTime().TotalSeconds, Is.EqualTo(0));
        Assert.That((int)table.Session.GetPlayTime().TotalSeconds, Is.EqualTo(10));
        Assert.That(table.State, Is.EqualTo(TableState.Standby));
    }
}










