using System.Collections.Concurrent;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public class TableManagerService {
    private readonly ITimeProvider _timeProvider;
    private readonly IPlaySessionRepositoryFactory _sessionRepo;
    private readonly ITableRepositoryFactory _tableRepo;

    // In-memory dictionary to track active TableManager instances
    private readonly ConcurrentDictionary<int, TableManager> _tableManagers = new();
    private readonly ConcurrentBag<TableManager> _tm = new();

    public TableManagerService(ITimeProvider timeProvider,
                               IPlaySessionRepositoryFactory repo,
                               ITableRepositoryFactory tableRepo) {
        _timeProvider = timeProvider;
        _sessionRepo = repo;
        _tableRepo = tableRepo;
    }

    public void CreateAllTableManagersAsync(List<PoolTable> tables) {
        foreach (var table in tables) {
            var manager = new TableManager(table, _timeProvider, _sessionRepo);
            _tm.Add(manager);
        }
    }

    public async Task UpdateTableManagers() {
        // Get all table tables from repo.
        // Then for each table from repo 
        // if tablefrom repo is in table managers 
        //    continue
        // else 
        //    create new table manager with new table
        //    add it the list of table managers
        using var repoWrapper = _tableRepo.CreateRepository();
        var repo = repoWrapper.Repository;
        var tables = await repo.GetAllAsync();

        // Then for each table from table managers
        // if tablemanager is in repo
        //    continure
        // else 
        //    remove table manager from table managers
    }

    public List<object> GetAllTableManagersWithSessions() {
        return _tm.Select(manager => new {
                          TableId = manager.TableNumber,
                          TableStatus = manager.State.ToString(),
                          PlayTime = manager.SessionManager?.GetPlayTime(),
                          RemainingTime = manager.SessionManager?.GetRemainingPlayTime(),
                          Price = manager.SessionManager?.GetSessionPrice()}).ToList<object>();}

    public TableManager? GetTableManager(int tableId) {
        return _tm.FirstOrDefault(tm => tm.TableNumber == tableId);
    }
}

