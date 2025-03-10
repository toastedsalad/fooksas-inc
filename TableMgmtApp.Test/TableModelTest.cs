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
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        Thread.Sleep(2 * 1000);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
    }
    
    [Test]
    public void WhenTableIsInPauseSendingPlayOnWillSetItToPlay() {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer, 2);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        Thread.Sleep(1 * 1000);
        table.SetState(TableState.PlayOn);

        Assert.That(table.State, Is.EqualTo(TableState.PlayOn));

        Thread.Sleep(3 * 1000);

        Assert.That(table.State, Is.EqualTo(TableState.PlayOn));
    }

    [Test]
    public void WhenTableStateIsInStandbySendingOffWillOffTheTable() {
        var systemTimer = new SystemTimeProvider();
        var table = new Table(1, systemTimer, 1);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);
        Thread.Sleep(1 * 1100);

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

    // Session Tests
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
        session.Pause();
        fakeTimer.Now = fakeTimer.Now + TimeSpan.FromSeconds(5);

        var gametime = session.GetPlayTime().TotalSeconds;

        Assert.That((int)gametime, Is.EqualTo(0));
    }
}










