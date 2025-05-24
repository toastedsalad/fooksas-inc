namespace TableMgmtApp;

public class ScopedServiceWrapper<T> : IDisposable where T : class {
    private IServiceScope _scope;
    public T Service { get; }

    public ScopedServiceWrapper(IServiceScope scope, T service) {
        _scope = scope;
        Service = service;
    }

    // Dispose both the scope and repository
    public void Dispose() {
        _scope?.Dispose();
    }
}

public interface IScheduleServiceFactory {
    ScopedServiceWrapper<IScheduleService> CreateService();
}

public class ScheduleServiceFactory : IScheduleServiceFactory {
    private IServiceScopeFactory _scopeFactory;

    public ScheduleServiceFactory(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    public ScopedServiceWrapper<IScheduleService> CreateService() {
        var scope = _scopeFactory.CreateScope(); // Create a new scope
        var service = scope.ServiceProvider.GetRequiredService<IScheduleService>();
        return new ScopedServiceWrapper<IScheduleService>(scope, service);
    }
}
