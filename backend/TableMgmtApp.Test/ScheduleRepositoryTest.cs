using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TableMgmtApp.Test;

namespace TableMgmtApp.Persistence;

[TestFixture]
public class ScheduleRepositoryTest {
    private IScheduleRepository _repository;
    private TableMgmtAppDbContext _dbContext;
    private SqliteConnection _connection;

    [SetUp]
    public void Setup() {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TableMgmtAppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new TableMgmtAppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new ScheduleSQLRepository(_dbContext);
    }

    [TearDown]
    public void TearDown() {
        _dbContext.Dispose();
        _connection.Close();
    }

    [Test]
    public async Task Can_Retrieve_By_Schedule_Name() {
        var schedule1 = new Schedule();
        schedule1.Name = "daytime";
        var schedule2 = new Schedule();
        schedule2.Name = "nighttime";

        await _repository.AddAsync(schedule1);
        await _repository.AddAsync(schedule2);

        await _repository.SaveAsync();

        var schedulesFromRepo = await _repository.GetBySchedulesName("daytime");

        Assert.That(schedulesFromRepo[0].Name, Is.EqualTo("daytime"));
    }

    [Test]
    public async Task Can_Retrieve_All_Schedules() {
        var schedule1 = new Schedule();
        schedule1.Name = "daytime";
        var schedule2 = new Schedule();
        schedule2.Name = "nighttime";

        await _repository.AddAsync(schedule1);
        await _repository.AddAsync(schedule2);

        await _repository.SaveAsync();

        var schedulesFromRepo = await _repository.GetAllAsync();

        Assert.That(schedulesFromRepo.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Get_Real_Schedule_Check_Rate() {
        var schedule = ScheduleTest.GetSchedule();

        await _repository.AddAsync(schedule);

        await _repository.SaveAsync();

        var schedulesFromRepo = await _repository.GetBySchedulesName("Default");

        Assert.That(schedulesFromRepo[0].WeeklyRates[DayOfWeek.Wednesday][1].End, Is.EqualTo(new TimeSpan(23, 59, 59)));
    }
}
