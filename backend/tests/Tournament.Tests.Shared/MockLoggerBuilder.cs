using Moq;
using Microsoft.Extensions.Logging;

namespace TournamentTestShared;

public static class MockLoggerBuilder
{
    public static Mock<ILogger<T>> Create<T>()
    {
        return new Mock<ILogger<T>>();
    }
}
