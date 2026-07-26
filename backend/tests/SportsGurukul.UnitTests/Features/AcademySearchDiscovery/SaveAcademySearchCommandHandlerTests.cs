using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.SaveAcademySearch;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class SaveAcademySearchCommandHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<SaveAcademySearchCommandHandler>> _loggerMock;
    private readonly SaveAcademySearchCommandHandler _handler;

    public SaveAcademySearchCommandHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<SaveAcademySearchCommandHandler>>();
        _handler = new SaveAcademySearchCommandHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_SavesSearch()
    {
        _academySearchRepositoryMock
            .Setup(r => r.SaveSearchAsync(It.IsAny<Domain.Entities.SavedAcademySearch>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(
            new SaveAcademySearchCommand
            {
                UserId = Guid.NewGuid(),
                SearchName = "My Cricket Search",
                SearchTerm = "cricket",
                City = "Mumbai",
                ResultCount = 10
            },
            CancellationToken.None);

        _academySearchRepositoryMock.Verify(
            r => r.SaveSearchAsync(It.IsAny<Domain.Entities.SavedAcademySearch>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsSavedSearchDto()
    {
        _academySearchRepositoryMock
            .Setup(r => r.SaveSearchAsync(It.IsAny<Domain.Entities.SavedAcademySearch>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new SaveAcademySearchCommand
            {
                UserId = Guid.NewGuid(),
                SearchName = "My Search",
                SearchTerm = "academy",
                City = "Delhi",
                State = "Delhi",
                SportName = "Cricket",
                VerifiedOnly = true,
                ResultCount = 5
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SearchName.Should().Be("My Search");
        result.Value.SearchTerm.Should().Be("academy");
        result.Value.City.Should().Be("Delhi");
        result.Value.State.Should().Be("Delhi");
        result.Value.SportName.Should().Be("Cricket");
        result.Value.VerifiedOnly.Should().BeTrue();
        result.Value.ResultCount.Should().Be(5);
    }
}
