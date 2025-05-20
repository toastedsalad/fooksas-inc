using System.Collections.Concurrent;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public class TableManagerService {
    private readonly ITimeProvider _timeProvider;
    private readonly IPlaySessionRepositoryFactory _sessionRepo;
    private readonly ITableRepositoryFactory _tableRepo;

    // In-memory dictionary to track active TableManager instances
    private readonly ConcurrentDictionary<Guid, TableManager> _tableManagers = new();

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
            _tableManagers.TryAdd(table.Id, manager);
        }
    }

    public async Task UpdateTableManagers() {
        using var repoWrapper = _tableRepo.CreateRepository();
        var repo = repoWrapper.Repository;
        var tables = await repo.GetAllAsync();

        foreach(var table in tables) {
            var tm = GetTableManager(table.Id);
            if(tm == null) {
               var manager = new TableManager(table, _timeProvider, _sessionRepo);
               _tableManagers.TryAdd(table.Id, manager);
            }
        }

        foreach(var tm in _tableManagers) {
            var table = await repo.GetByIdAsync(tm.Value.Table.Id);
            if(table == null) {
                _tableManagers.Remove(tm.Value.Table.Id, out var tableManager);
            }
        }
    }

    public List<object> GetAllTableManagersWithSessions() {
        var tmData =  _tableManagers.Values.Select(manager => new {
                              TableId = manager.Table.Id,
                              TableNumber = manager.Table.Number,
                              TableName = manager.Table.Name,
                              TableStatus = manager.State.ToString(),
                              PlayTime = manager.SessionManager?.GetPlayTime(),
                              RemainingTime = manager.SessionManager?.GetRemainingPlayTime(),
                              Price = manager.SessionManager?.GetSessionPrice()})
                              .ToList();

        var sortedTmData = tmData
            .OrderBy(x => x.TableName)
            .ThenBy(x => x.TableNumber)
            .ToList();
        
        return sortedTmData.Cast<object>().ToList();
    }

    public TableManager? GetTableManager(Guid tableId) {
        _tableManagers.TryGetValue(tableId, out var tableManager);
        return tableManager;
    }
}

