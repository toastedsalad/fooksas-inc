namespace TableMgmtApp.Test;

public class TableModetTest {
    [Test]
    public void WhenNewTableInitializedItHasAttributes() {
        var table = new Table(1);
        Assert.That(table.Id, Is.EqualTo(1));
    }

    [Test]
    public void WhenNewTableInitializedFourStatesAreAllowed() {
        var table = new Table(1);

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
        var table = new Table(1);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));
    }

    [Test]
    public void TableHasAPauseTimer () {
        var table = new Table(1, 5);

        Assert.That(table.PauseTimer, Is.EqualTo(5));
    }

    [Test]
    public void IfTableIsInPauseItCanTransitionToStandByAfterPauseTimerExpires() {
        var table = new Table(1);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        Thread.Sleep(2 * 1000);

        Assert.That(table.State, Is.EqualTo(TableState.Standby));
    }
    
    [Test]
    public void WhenTableIsInPauseSendingPlayOnWillSetItToPlay() {
        var table = new Table(1, 2);
        table.SetState(TableState.PlayOn);
        table.SetState(TableState.Off);

        Assert.That(table.State, Is.EqualTo(TableState.Paused));

        Thread.Sleep(1 * 1000);
        table.SetState(TableState.PlayOn);

        Assert.That(table.State, Is.EqualTo(TableState.PlayOn));
    }
}










