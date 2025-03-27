namespace TableMgmtApp.Test;

public class TableServiceTest {
    [Test]
    public void TableServiceCanGetATableById() {
        var tableService = new TableService();
        tableService.TableManagers = new List<TableManager>();
        var timeProvider = new FakeTimeProvider();
        var timer = new FakeTimer();
        
        var table1 = new Table(1);
        var table2 = new Table(2);
        var table3 = new Table(3);
        var table4 = new Table(4);

        tableService.TableManagers.Add(new TableManager(table1, timeProvider, timer, 15));
        tableService.TableManagers.Add(new TableManager(table2, timeProvider, timer, 15));
        tableService.TableManagers.Add(new TableManager(table3, timeProvider, timer, 15));
        tableService.TableManagers.Add(new TableManager(table4, timeProvider, timer, 15));

        var table1Result = tableService.GetTable(2);

        Assert.That(table1Result.Value.Id, Is.EqualTo(2));
        Assert.That(table1Result.IsSuccess, Is.True);

        var tableFailResult = tableService.GetTable(5);

        Assert.That(tableFailResult.IsFailure, Is.True);
        Assert.That(tableFailResult.Error, Does.Contain("with id 5"));
    }

    [Test]
    public void TableServiceCanSwitchTablesOnAndOff() {
        var tableService = new TableService();
        tableService.TableManagers = new List<TableManager>();
        var timeProvider = new FakeTimeProvider();
        var timer = new FakeTimer();

        var table1 = new Table(1);
        var table2 = new Table(2);
        var table3 = new Table(3);
        var table4 = new Table(4);

        tableService.TableManagers.Add(new TableManager(table1, timeProvider, timer, 15));
        tableService.TableManagers.Add(new TableManager(table2, timeProvider, timer, 15));
        tableService.TableManagers.Add(new TableManager(table3, timeProvider, timer, 15));
        tableService.TableManagers.Add(new TableManager(table4, timeProvider, timer, 15));

        var customSwitch = new VirtualSwitch();

        tableService.SwitchTable(1, customSwitch, SwitchState.On);

        var table = tableService.GetTable(1);

        Assert.That(table.Value.State, Is.EqualTo(TableState.Play));
    }
};
