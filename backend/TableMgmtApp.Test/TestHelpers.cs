using Moq;
using TableMgmtApp.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace TableMgmtApp.Test;

public class TestHelpers {
    public static Mock<IPlaySessionRepositoryFactory> GetRepoFactoryMock() {
        var mockPSRepo = new Mock<IPlaySessionRepository>(); 
        var mockRepoWrapper = new Mock<ScopedRepositoryWrapper<IPlaySessionRepository>>(It.IsAny<IServiceScope>(), mockPSRepo.Object);
        var mockFactory = new Mock<IPlaySessionRepositoryFactory>();
        mockFactory.Setup(f => f.CreateRepository()).Returns(mockRepoWrapper.Object);

        return mockFactory;
    }

    public static Mock<IScheduleServiceFactory> GetServiceFactoryMock() {
        var mockService = new Mock<IScheduleService>(); 
        var mockServiceWrapper = new Mock<ScopedServiceWrapper<IScheduleService>>(It.IsAny<IServiceScope>(), mockService.Object);
        var mockFactory = new Mock<IScheduleServiceFactory>();
        mockFactory.Setup(f => f.CreateService()).Returns(mockServiceWrapper.Object);

        return mockFactory;
    }
}
