using System.Collections.Concurrent;
using TableMgmtApp.Persistence;

namespace TableMgmtApp;

public class TableManagerService {
    private readonly ITimeProvider _timeProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // In-memory dictionary to track active TableManager instances
    private readonly ConcurrentDictionary<int, TableManager> _tableManagers = new();

    public TableManagerService(
        ITimeProvider timeProvider,
        IServiceScopeFactory serviceScopeFactory) {
        _timeProvider = timeProvider;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void CreateAllTableManagersAsync(List<PoolTable> tables) {
        foreach (var table in tables) {
            if (!_tableManagers.ContainsKey(table.Number)) {
                using var scope = _serviceScopeFactory.CreateScope();
                var playSessionRepository = scope.ServiceProvider.GetRequiredService<IPlaySessionRepository>();

                // TODO move the timer elsewhere:
                var timer = new RealTimer(1000);
                var manager = new TableManager(table, _timeProvider, timer, playSessionRepository);
                _tableManagers.TryAdd(table.Number, manager);
            }
        }
    }

    public List<object> GetAllTableManagersWithSessions() {
        return _tableManagers.Values
            .Select(manager => new {
                TableId = manager.TableNumber,
                PlayTime = manager.SessionManager?.GetPlayTime(),
                Price = manager.SessionManager?.GetSessionPrice()
            })
            .ToList<object>();
    }

    public TableManager? GetTableManager(int tableId) {
        _tableManagers.TryGetValue(tableId, out var manager);
        return manager;
    }
}

