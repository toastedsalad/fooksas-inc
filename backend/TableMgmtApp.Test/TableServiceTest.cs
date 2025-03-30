using Moq;
using TableMgmtApp.Persistence;

namespace TableMgmtApp.Test;

public class TableServiceTest {
    [Test]
    public void TableServiceCanGetATableById() {
        var tableService = new TableService();
        tableService.TableManagers = new List<TableManager>();
        var timeProvider = new FakeTimeProvider();
        var timer = new FakeTimer();
        
        var table1 = new PoolTable(1);
        var table2 = new PoolTable(2);
        var table3 = new PoolTable(3);
        var table4 = new PoolTable(4);

        var mockPSRepo = new Mock<IPlaySessionRepository>(); 

        tableService.TableManagers.Add(new TableManager(table1, timeProvider, timer, mockPSRepo.Object, 15));
        tableService.TableManagers.Add(new TableManager(table2, timeProvider, timer, mockPSRepo.Object, 15));
        tableService.TableManagers.Add(new TableManager(table3, timeProvider, timer, mockPSRepo.Object, 15));
        tableService.TableManagers.Add(new TableManager(table4, timeProvider, timer, mockPSRepo.Object, 15));

        var table1Result = tableService.GetTable(2);

        Assert.That(table1Result.Value.TableNumber, Is.EqualTo(2));
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

        var table1 = new PoolTable(1);
        var table2 = new PoolTable(2);
        var table3 = new PoolTable(3);
        var table4 = new PoolTable(4);

        var mockPSRepo = new Mock<IPlaySessionRepository>(); 
        tableService.TableManagers.Add(new TableManager(table1, timeProvider, timer, mockPSRepo.Object, 15));
        tableService.TableManagers.Add(new TableManager(table2, timeProvider, timer, mockPSRepo.Object, 15));
        tableService.TableManagers.Add(new TableManager(table3, timeProvider, timer, mockPSRepo.Object, 15));
        tableService.TableManagers.Add(new TableManager(table4, timeProvider, timer, mockPSRepo.Object, 15));

        var customSwitch = new VirtualSwitch();

        tableService.SwitchTable(1, customSwitch, SwitchState.On);

        var table = tableService.GetTable(1);

        Assert.That(table.Value.State, Is.EqualTo(TableState.Play));
    }
};
