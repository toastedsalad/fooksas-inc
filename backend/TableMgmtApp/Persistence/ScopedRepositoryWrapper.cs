namespace TableMgmtApp.Persistence;

public interface IPlaySessionRepositoryFactory {
    IPlaySessionRepository CreateRepository();
}

public class PlaySessionRepositoryFactory : IPlaySessionRepositoryFactory {
    private readonly IServiceScopeFactory _scopeFactory;

    public PlaySessionRepositoryFactory(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    public IPlaySessionRepository CreateRepository() {
        var scope = _scopeFactory.CreateScope(); // Creates a new DI scope
        return scope.ServiceProvider.GetRequiredService<IPlaySessionRepository>();
    }
}

