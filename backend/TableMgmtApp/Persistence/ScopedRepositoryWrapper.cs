namespace TableMgmtApp.Persistence;

public class ScopedRepositoryWrapper<T> : IDisposable where T : class {
    private IServiceScope _scope;
    public T Repository { get; }

    public ScopedRepositoryWrapper(IServiceScope scope, T repository) {
        _scope = scope;
        Repository = repository;
    }

    // Dispose both the scope and repository
    public void Dispose() {
        _scope?.Dispose();
    }
}

public interface IPlaySessionRepositoryFactory {
    ScopedRepositoryWrapper<IPlaySessionRepository> CreateRepository();
}

public class PlaySessionRepositoryFactory : IPlaySessionRepositoryFactory {
    private IServiceScopeFactory _scopeFactory;

    public PlaySessionRepositoryFactory(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    public ScopedRepositoryWrapper<IPlaySessionRepository> CreateRepository() {
        var scope = _scopeFactory.CreateScope(); // Create a new scope
        var repository = scope.ServiceProvider.GetRequiredService<IPlaySessionRepository>();
        return new ScopedRepositoryWrapper<IPlaySessionRepository>(scope, repository);
    }
}

public interface ITableRepositoryFactory {
    ScopedRepositoryWrapper<ITableRepository> CreateRepository();
}

public class TableRepositoryFactory : ITableRepositoryFactory {
    private IServiceScopeFactory _scopeFactory;

    public TableRepositoryFactory(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    public ScopedRepositoryWrapper<ITableRepository> CreateRepository() {
        var scope = _scopeFactory.CreateScope(); // Create a new scope
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository>();
        return new ScopedRepositoryWrapper<ITableRepository>(scope, repository);
    }
}
