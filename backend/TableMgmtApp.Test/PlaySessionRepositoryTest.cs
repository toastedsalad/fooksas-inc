using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace TableMgmtApp.Persistence;

[TestFixture]
public class PlaySessionRepositoryTests {
    private IPlaySessionRepository _repository;
    private IPlayerRepository _playerRepository;
    private TableMgmtAppDbContext _dbContext;
    private SqliteConnection _connection;
    private Player _player;

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
        _playerRepository = new PlayerSQLRepository(_dbContext);

        _player = new Player("None", "None", "None", 10);
        _playerRepository.AddAsync(_player);
        _playerRepository.SaveAsync();
    }

    [TearDown]
    public void TearDown() {
        _dbContext.Dispose();
        _connection.Close();
    }

    [Test]
    public async Task Can_Retrieve_Session_By_Table_Number() {
        var session1 = new PlaySession();
        session1.StartTime = DateTime.Now;
        session1.PlayTime = TimeSpan.Zero;
        session1.Price = 0.0m;
        session1.TableNumber = 2;
        session1.PlayerId = _player.Id;

        var session2 = new PlaySession();
        session2.StartTime = DateTime.Now;
        session2.PlayTime = TimeSpan.Zero;
        session2.Price = 0.0m;
        session2.TableNumber = 2;
        session2.PlayerId = _player.Id;

        var session3 = new PlaySession();
        session3.StartTime = DateTime.Now;
        session3.PlayTime = TimeSpan.Zero;
        session3.Price = 0.0m;
        session3.TableNumber = 3;
        session3.PlayerId = _player.Id;

        await _repository.AddAsync(session1);
        await _repository.AddAsync(session2);
        await _repository.AddAsync(session3);

        await _repository.SaveAsync();

        var sessionsFromTable2 = await _repository.GetByTableNumber(2);

        Assert.That(sessionsFromTable2.Count, Is.EqualTo(2));
        Assert.That(sessionsFromTable2.All(t => t.TableNumber == 2));
    }

    [Test]
    public async Task Can_Retrieve_All_Sessions() {
        var session1 = new PlaySession();
        session1.StartTime = DateTime.Now;
        session1.PlayTime = TimeSpan.Zero;
        session1.Price = 0.0m;
        session1.TableNumber = 2;
        session1.PlayerId = _player.Id;

        var session2 = new PlaySession();
        session2.StartTime = DateTime.Now;
        session2.PlayTime = TimeSpan.Zero;
        session2.Price = 0.0m;
        session2.TableNumber = 2;
        session2.PlayerId = _player.Id;

        var session3 = new PlaySession();
        session3.StartTime = DateTime.Now;
        session3.PlayTime = TimeSpan.Zero;
        session3.Price = 0.0m;
        session3.TableNumber = 3;
        session3.PlayerId = _player.Id;

        await _repository.AddAsync(session1);
        await _repository.AddAsync(session2);
        await _repository.AddAsync(session3);

        await _repository.SaveAsync();

        var sessionsFromTable2 = await _repository.GetAllAsync();

        Assert.That(sessionsFromTable2.Count, Is.EqualTo(3));
    }
}

