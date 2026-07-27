using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Services;

public class ConflictDetectionServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IConflictRepository> _conflictRepositoryMock = TestMocks.CreateConflictRepository();
    private readonly Mock<ILogger<ConflictDetectionService>> _loggerMock = TestMocks.CreateLogger<ConflictDetectionService>();
    private readonly ConflictDetectionService _service;

    public ConflictDetectionServiceTests()
    {
        _service = new ConflictDetectionService(
            _bookingRepositoryMock.Object,
            _conflictRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task DetectConflictsAsync_FacilityOverlap_DetectsConflict()
    {
        var facilityId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-001",
            FacilityId = facilityId,
            BookingDate = DateTime.UtcNow.Date,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(11),
            Status = BookingStatus.Confirmed
        };
        var existingBookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-002",
                FacilityId = facilityId,
                BookingDate = DateTime.UtcNow.Date,
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(12),
                Status = BookingStatus.Confirmed
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facilityId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookings);

        var result = await _service.DetectConflictsAsync(booking, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].ConflictType.Should().Be(BookingConflictType.FacilityOverlap);
    }

    [Fact]
    public async Task DetectConflictsAsync_NoOverlap_ReturnsEmpty()
    {
        var facilityId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-001",
            FacilityId = facilityId,
            BookingDate = DateTime.UtcNow.Date,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Status = BookingStatus.Confirmed
        };
        _bookingRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facilityId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _service.DetectConflictsAsync(booking, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task DetectConflictsAsync_ExcludesCancelledBookings()
    {
        var facilityId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-001",
            FacilityId = facilityId,
            BookingDate = DateTime.UtcNow.Date,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Status = BookingStatus.Confirmed
        };
        var existingBookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-002",
                FacilityId = facilityId,
                BookingDate = DateTime.UtcNow.Date,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Status = BookingStatus.Cancelled
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facilityId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookings);

        var result = await _service.DetectConflictsAsync(booking, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HasConflictsAsync_UnresolvedConflicts_ReturnsTrue()
    {
        var bookingId = Guid.NewGuid();
        _conflictRepositoryMock.Setup(r => r.GetUnresolvedByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict> { new() { Id = Guid.NewGuid() } });

        var result = await _service.HasConflictsAsync(bookingId, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasConflictsAsync_NoUnresolvedConflicts_ReturnsFalse()
    {
        _conflictRepositoryMock.Setup(r => r.GetUnresolvedByBookingIdAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict>());

        var result = await _service.HasConflictsAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeFalse();
    }
}
