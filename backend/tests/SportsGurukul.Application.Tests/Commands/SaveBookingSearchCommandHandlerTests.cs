using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.SaveBookingSearch;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class SaveBookingSearchCommandHandlerTests
{
    private readonly Mock<ISavedSearchRepository> _savedSearchRepositoryMock = TestMocks.CreateSavedSearchRepository();
    private readonly Mock<ILogger<SaveBookingSearchCommandHandler>> _loggerMock = TestMocks.CreateLogger<SaveBookingSearchCommandHandler>();
    private readonly SaveBookingSearchCommandHandler _handler;

    public SaveBookingSearchCommandHandlerTests()
    {
        _handler = new SaveBookingSearchCommandHandler(
            _savedSearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsSavedSearch()
    {
        SavedSearch? captured = null;
        _savedSearchRepositoryMock.Setup(r => r.AddAsync(
            It.IsAny<SavedSearch>(), It.IsAny<CancellationToken>()))
            .Callback<SavedSearch, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync((SavedSearch s, CancellationToken _) =>
            {
                s.Id = Guid.NewGuid();
                s.CreatedAt = DateTime.UtcNow;
                return s;
            });

        var result = await _handler.Handle(new SaveBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            Name = "My Training Search",
            SearchTerm = "training",
            AcademyId = Guid.NewGuid(),
            BookingType = "TrainingSession"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("My Training Search");
        result.Value.Filters.SearchTerm.Should().Be("training");
        result.Value.Filters.BookingType.Should().Be("TrainingSession");
        captured.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_SetsFiltersCorrectly()
    {
        _savedSearchRepositoryMock.Setup(r => r.AddAsync(
            It.IsAny<SavedSearch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedSearch s, CancellationToken _) =>
            {
                s.Id = Guid.NewGuid();
                s.CreatedAt = DateTime.UtcNow;
                return s;
            });

        var result = await _handler.Handle(new SaveBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Date Range Search",
            DateFrom = new DateTime(2025, 1, 1),
            DateTo = new DateTime(2025, 12, 31),
            Status = "Confirmed"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Filters.DateFrom.Should().Be(new DateTime(2025, 1, 1));
        result.Value.Filters.DateTo.Should().Be(new DateTime(2025, 12, 31));
        result.Value.Filters.Status.Should().Be("Confirmed");
    }
}
