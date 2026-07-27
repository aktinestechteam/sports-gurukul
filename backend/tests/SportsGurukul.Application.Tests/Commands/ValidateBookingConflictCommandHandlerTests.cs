using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ValidateBookingConflict;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class ValidateBookingConflictCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IConflictDetectionService> _conflictDetectionServiceMock = new();
    private readonly Mock<ILogger<ValidateBookingConflictCommandHandler>> _loggerMock = TestMocks.CreateLogger<ValidateBookingConflictCommandHandler>();
    private readonly ValidateBookingConflictCommandHandler _handler;

    public ValidateBookingConflictCommandHandlerTests()
    {
        _handler = new ValidateBookingConflictCommandHandler(
            _bookingRepositoryMock.Object,
            _conflictDetectionServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new ValidateBookingConflictCommand
        {
            BookingId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_NoConflicts_ReturnsEmptyList()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-TEST-001",
            FacilityId = Guid.NewGuid()
        };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _conflictDetectionServiceMock.Setup(s => s.DetectConflictsAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict>());

        var result = await _handler.Handle(new ValidateBookingConflictCommand
        {
            BookingId = booking.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithConflicts_ReturnsConflictDtos()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-TEST-001",
            FacilityId = Guid.NewGuid()
        };
        var conflict = new BookingConflict
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            ConflictingBookingId = Guid.NewGuid(),
            ConflictType = BookingConflictType.FacilityOverlap,
            Description = "Overlap detected",
            IsResolved = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _conflictDetectionServiceMock.Setup(s => s.DetectConflictsAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict> { conflict });

        var result = await _handler.Handle(new ValidateBookingConflictCommand
        {
            BookingId = booking.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].ConflictType.Should().Be("FacilityOverlap");
        result.Value[0].Description.Should().Be("Overlap detected");
    }
}
