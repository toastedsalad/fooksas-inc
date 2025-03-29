using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace TableMgmtApp.Persistence;

[TestFixture]
public class PlaySessionRepositoryTests {
    private IPlaySessionRepository _repository;
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

        _repository = new PlaySessionSQLRepository(_dbContext);
    }

    [TearDown]
    public void TearDown() {
        _dbContext.Dispose();
        _connection.Close();
    }

    [Test]
    public async Task Can_Retrieve_Players_By_Surname() {
        var session1 = new PlaySession();
        session1.StartTime = DateTime.Now;
        session1.PlayTime = TimeSpan.Zero;
        session1.Price = 0.0m;
        session1.TableNumber = 2;
        session1.PlayerId = Guid.NewGuid();

        var session2 = new PlaySession();
        session2.StartTime = DateTime.Now;
        session2.PlayTime = TimeSpan.Zero;
        session2.Price = 0.0m;
        session2.TableNumber = 2;
        session2.PlayerId = Guid.NewGuid();

        var session3 = new PlaySession();
        session3.StartTime = DateTime.Now;
        session3.PlayTime = TimeSpan.Zero;
        session3.Price = 0.0m;
        session3.TableNumber = 3;
        session3.PlayerId = Guid.NewGuid();

        await _repository.AddAsync(session1);
        await _repository.AddAsync(session2);
        await _repository.AddAsync(session3);

        await _repository.SaveAsync();

        var sessionsFromTable2 = await _repository.GetByTableNumber(2);

        Assert.That(sessionsFromTable2.Count, Is.EqualTo(2));
        Assert.That(sessionsFromTable2.All(t => t.TableNumber == 2));
    }
}

