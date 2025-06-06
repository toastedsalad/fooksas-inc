using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace TableMgmtApp.Persistence;

[TestFixture]
public class TableRepositoryTests {
    private ITableRepository _repository;
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

        _repository = new TableSQLRepository(_dbContext);
    }

    [TearDown]
    public void TearDown() {
        _dbContext.Dispose();
        _connection.Close();
    }

    [Test]
    public async Task Can_Retrieve_All_Tables() {
        var table1 = new PoolTable(1);
        var table2 = new PoolTable(2);
        var table3 = new PoolTable(3);

        await _repository.AddAsync(table1);
        await _repository.AddAsync(table2);
        await _repository.AddAsync(table3);
        await _repository.SaveAsync();

        var allTables = await _repository.GetAllAsync();

        Assert.That(allTables.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task Can_Delete_A_Table() {
        var table1 = new PoolTable(1);
        var table2 = new PoolTable(2);

        await _repository.AddAsync(table1);
        await _repository.AddAsync(table2);
        await _repository.SaveAsync();

        var allTables = await _repository.GetAllAsync();

        Assert.That(allTables.Count, Is.EqualTo(2));

        _repository.Remove(table1);
        await _repository.SaveAsync();

        allTables = await _repository.GetAllAsync();
        Assert.That(allTables.Count, Is.EqualTo(1));
    }
}

