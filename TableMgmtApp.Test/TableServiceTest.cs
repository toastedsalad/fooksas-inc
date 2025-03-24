namespace TableMgmtApp.Test;

public class TableServiceTest {
    [Test]
    public void TableServiceCanGetATableById() {
        var tableService = new TableService();
        tableService.Tables = new List<Table>();
        var timeProvider = new FakeTimeProvider();
        var timer = new FakeTimer();

        tableService.Tables.Add(new Table(1, timeProvider, timer, 15));
        tableService.Tables.Add(new Table(2, timeProvider, timer, 15));
        tableService.Tables.Add(new Table(3, timeProvider, timer, 15));
        tableService.Tables.Add(new Table(4, timeProvider, timer, 15));

        var table1Result = tableService.GetTable(2);

        Assert.That(table1Result.Value.Id, Is.EqualTo(2));
        Assert.That(table1Result.IsSuccess, Is.True);

        var tableFailResult = tableService.GetTable(5);

        Assert.That(tableFailResult.IsFailure, Is.True);
        Assert.That(tableFailResult.Error, Does.Contain("with id 5"));
    }
};
