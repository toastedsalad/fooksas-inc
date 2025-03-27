using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace TableMgmtApp.Persistence;

[TestFixture]
public class PlayerRepositoryTests {
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

        _repository = new PlayerSQLRepository(_dbContext);
    }

    [TearDown]
    public void TearDown() {
        _dbContext.Dispose();
        _connection.Close();
    }

    [Test]
    public async Task Can_Retrieve_Players_By_Surname() {
        var player1 = new Player("John", "Doe", "john@doe.com", 10);
        var player2 = new Player("Jane", "Doe", "jane@doe.com", 15);
        var player3 = new Player("Jack", "Smith", "jack@smith.com", 5);

        await _repository.AddAsync(player1);
        await _repository.AddAsync(player2);
        await _repository.AddAsync(player3);
        await _repository.SaveAsync();

        var playersWithDoeSurname = await _repository.GetBySurnameAsync("Doe");

        Assert.That(playersWithDoeSurname.Count, Is.EqualTo(2));
        Assert.That(playersWithDoeSurname.All(p => p.Surname == "Doe"));
    }
}

