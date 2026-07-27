using Moq;
using SportsGurukul.Application.Common.Interfaces;

namespace TournamentTestShared;

public static class MockUnitOfWorkBuilder
{
    public static Mock<IUnitOfWork> Create(int saveChangesResult = 1)
    {
        var mock = new Mock<IUnitOfWork>();
        mock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(saveChangesResult);
        return mock;
    }
}
