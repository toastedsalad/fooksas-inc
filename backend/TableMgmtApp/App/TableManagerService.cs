using System.Collections.Concurrent;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public class TableManagerService {
    private readonly ITimeProvider _timeProvider;
    private readonly IPlaySessionRepositoryFactory _repo;

    // In-memory dictionary to track active TableManager instances
    private readonly ConcurrentDictionary<int, TableManager> _tableManagers = new();

    public TableManagerService(ITimeProvider timeProvider, IPlaySessionRepositoryFactory repo) {
        _timeProvider = timeProvider;
        _repo = repo;
    }

    public void CreateAllTableManagersAsync(List<PoolTable> tables) {
        foreach (var table in tables) {
            if (!_tableManagers.ContainsKey(table.Number)) {
                var manager = new TableManager(table, _timeProvider, _repo);
                _tableManagers.TryAdd(table.Number, manager);
            }
        }
    }

    public List<object> GetAllTableManagersWithSessions() {
        return _tableManagers.Values
            .Select(manager => new {
                TableId = manager.TableNumber,
                TableStatus = manager.State.ToString(),
                PlayTime = manager.SessionManager?.GetPlayTime(),
                RemainingTime = manager.SessionManager?.GetRemainingPlayTime(),
                Price = manager.SessionManager?.GetSessionPrice()
            })
            .ToList<object>();
    }

    public TableManager? GetTableManager(int tableId) {
        _tableManagers.TryGetValue(tableId, out var manager);
        return manager;
    }
}

