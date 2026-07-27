using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingConflicts;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetBookingConflictsQueryHandlerTests
{
    private readonly Mock<IConflictRepository> _conflictRepositoryMock = TestMocks.CreateConflictRepository();
    private readonly Mock<ILogger<GetBookingConflictsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetBookingConflictsQueryHandler>();
    private readonly GetBookingConflictsQueryHandler _handler;

    public GetBookingConflictsQueryHandlerTests()
    {
        _handler = new GetBookingConflictsQueryHandler(
            _conflictRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsConflictsForBooking()
    {
        var bookingId = Guid.NewGuid();
        var conflicts = new List<BookingConflict>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                ConflictingBookingId = Guid.NewGuid(),
                ConflictType = BookingConflictType.FacilityOverlap,
                Description = "Overlap with booking BK-002",
                IsResolved = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        _conflictRepositoryMock.Setup(r => r.GetByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conflicts);

        var result = await _handler.Handle(new GetBookingConflictsQuery
        {
            BookingId = bookingId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].ConflictType.Should().Be("FacilityOverlap");
        result.Value[0].IsResolved.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NoConflicts_ReturnsEmptyList()
    {
        _conflictRepositoryMock.Setup(r => r.GetByBookingIdAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict>());

        var result = await _handler.Handle(new GetBookingConflictsQuery
        {
            BookingId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
