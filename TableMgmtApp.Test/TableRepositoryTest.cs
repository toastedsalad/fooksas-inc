using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace TableMgmtApp.Persistence;

[TestFixture]
public class TableRepositoryTests {
    private IPlayerRepository _repository;
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

        // _repository = new TableRepository(_dbContext);
    }

    [TearDown]
    public void TearDown() {
        _dbContext.Dispose();
        _connection.Close();
    }

    [Test]
    public async Task Can_Retrieve_Players_By_Surname() {
        var fakeTimerProvider = new FakeTimeProvider();
        var fakeTimer = new FakeTimer();
        //var table1 = new Table();

        var playersWithDoeSurname = await _repository.GetBySurnameAsync("Doe");

        Assert.That(playersWithDoeSurname.Count, Is.EqualTo(2));
        Assert.That(playersWithDoeSurname.All(p => p.Surname == "Doe"));
    }
}

