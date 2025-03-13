namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class TableModelTest {
    [Test]
    public void WhenNewTableInitializedItHasAttributes() {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer);
        Assert.That(table.Id, Is.EqualTo(1));
    }

    [Test]
    public void WhenNewTableInitializedFourStatesAreAllowed() {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer);

        table.SetState(TableState.PlayOn);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));

        table.SetState(TableState.Off);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));

        table.SetState(TableState.Paused);
        Assert.That(Enum.IsDefined(typeof(TableState), table.State));

        table.SetState(TableState.Standby);
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
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));
    }

    [Test]
    public void TableHasAPauseTimer () {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer, 5);

        Assert.That(table.PauseTimer, Is.EqualTo(5));
    }

    [Test]
    public void IfTableIsInPauseItCanTransitionToStandByAfterPauseTimerExpires() {
        var fakeTimer = new FakeTimeProvider();
        var table = new Table(1, fakeTimer, 1); // Pause timer expires.
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimer.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
    }
    
    [Test]
    public void WhenTableIsInPauseSendingPlayWithinTimerWillSetItToPlay() {
        var fakeTimer = new FakeTimeProvider();
        var table = new Table(1, fakeTimer, 2);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        fakeTimer.AdvanceTimeBySeconds(1);
        table.SetState(TableState.PlayOn);

        Assert.That(table.State, Is.EqualTo(TableState.PlayOn));

        fakeTimer.AdvanceTimeBySeconds(3);

        Assert.That(table.State, Is.EqualTo(TableState.PlayOn));
    }

    [Test]
    public void WhenTableStateIsInStandbySendingOffWillOffTheTable() {
        var fakeTimer = new FakeTimeProvider();
        var table = new Table(1, fakeTimer, 1);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        fakeTimer.AdvanceTimeBySeconds(2);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
        
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Off));
    }

    [Test]
    public void WhenTableIsOffSettingOnSetsAGuid() {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer, 1);
        table.SetState(TableState.PlayOn);

        Assert.That(table.Session.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void WhenTableIsOffSettingOnSetsASessionTimer() {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer, 1);
        table.SetState(TableState.PlayOn);

        Assert.That(table.Session.StartTime.Minute, Is.EqualTo(DateTime.Now.Minute));
    }
}










