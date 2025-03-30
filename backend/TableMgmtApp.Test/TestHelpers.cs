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
}
