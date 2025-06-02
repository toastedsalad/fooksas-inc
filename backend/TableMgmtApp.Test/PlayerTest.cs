namespace TableMgmtApp.Test;

public class PlayerTests {
    // TODO: Each session should have a user assigned if available.
    [Test]
    public void PlayerHasUniqueID() {
        var player = new Player("Jane", "Doe", "doe@doe.com");

        Assert.That(player.Name, Is.EqualTo("Jane"));
    }
}

