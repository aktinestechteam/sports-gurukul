using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.SaveSearch;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class SaveSearchCommandTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ILogger<SaveSearchCommandHandler>> _loggerMock = new();
    private readonly SaveSearchCommandHandler _handler;

    public SaveSearchCommandTests()
    {
        _handler = new SaveSearchCommandHandler(_searchRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesSearchSuccessfully()
    {
        var command = new SaveSearchCommand
        {
            UserId = Guid.NewGuid(),
            SearchName = "My Cricket Search",
            SearchTerm = "cricket",
            City = "Mumbai",
            EventType = "Competition"
        };

        _searchRepositoryMock.Setup(r => r.SaveSearchAsync(
            It.IsAny<EventSavedSearch>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.SearchName.Should().Be("My Cricket Search");
        result.Value.SearchTerm.Should().Be("cricket");
        result.Value.City.Should().Be("Mumbai");
    }

    [Fact]
    public async Task Handle_WithAllFilters_SavesAllFields()
    {
        var command = new SaveSearchCommand
        {
            UserId = Guid.NewGuid(),
            SearchName = "Advanced Search",
            SearchTerm = "football",
            SportName = "Football",
            AcademyName = "Elite Academy",
            CoachName = "Coach Sharma",
            City = "Delhi",
            State = "Delhi",
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddDays(30),
            MinPrice = 100,
            MaxPrice = 5000,
            EventType = "Workshop",
            Category = "Training",
            SkillLevel = "Intermediate",
            AgeGroup = "16-18",
            Language = "Hindi",
            SortBy = "Popularity"
        };

        _searchRepositoryMock.Setup(r => r.SaveSearchAsync(
            It.IsAny<EventSavedSearch>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SportName.Should().Be("Football");
        result.Value.CoachName.Should().Be("Coach Sharma");
        result.Value.SkillLevel.Should().Be("Intermediate");
    }
}
