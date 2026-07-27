using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Services;

public class WaitlistServiceTests
{
    private readonly Mock<IWaitlistRepository> _waitlistRepositoryMock = TestMocks.CreateWaitlistRepository();
    private readonly Mock<ILogger<WaitlistService>> _loggerMock = TestMocks.CreateLogger<WaitlistService>();
    private readonly WaitlistService _service;

    public WaitlistServiceTests()
    {
        _service = new WaitlistService(_waitlistRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetNextInWaitlistAsync_ReturnsLowestPriority()
    {
        var bookingId = Guid.NewGuid();
        var entries = new List<BookingWaitlist>
        {
            new() { Id = Guid.NewGuid(), Priority = 3, RequestedOn = DateTime.UtcNow.AddHours(-1), Status = WaitlistStatus.Active },
            new() { Id = Guid.NewGuid(), Priority = 1, RequestedOn = DateTime.UtcNow.AddHours(-2), Status = WaitlistStatus.Active },
            new() { Id = Guid.NewGuid(), Priority = 2, RequestedOn = DateTime.UtcNow.AddHours(-3), Status = WaitlistStatus.Active }
        };
        _waitlistRepositoryMock.Setup(r => r.GetActiveByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entries);

        var result = await _service.GetNextInWaitlistAsync(bookingId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Priority.Should().Be(1);
    }

    [Fact]
    public async Task GetNextInWaitlistAsync_NoActiveEntries_ReturnsNull()
    {
        _waitlistRepositoryMock.Setup(r => r.GetActiveByBookingIdAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingWaitlist>());

        var result = await _service.GetNextInWaitlistAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task PromoteWaitlistedBookingAsync_ActiveEntry_PromotesSuccessfully()
    {
        var entry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            Status = WaitlistStatus.Active,
            Priority = 1
        };

        var result = await _service.PromoteWaitlistedBookingAsync(entry, CancellationToken.None);

        result.Should().BeTrue();
        entry.Status.Should().Be(WaitlistStatus.Promoted);
        entry.PromotionOrder.Should().Be(1);
    }

    [Fact]
    public async Task PromoteWaitlistedBookingAsync_NonActiveEntry_ReturnsFalse()
    {
        var entry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            Status = WaitlistStatus.Promoted
        };

        var result = await _service.PromoteWaitlistedBookingAsync(entry, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetNextPriorityAsync_ReturnsIncrementedPriority()
    {
        var bookingId = Guid.NewGuid();
        _waitlistRepositoryMock.Setup(r => r.GetMaxPriorityByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _service.GetNextPriorityAsync(bookingId, CancellationToken.None);

        result.Should().Be(4);
    }

    [Fact]
    public async Task GetNextPriorityAsync_NoEntries_ReturnsOne()
    {
        _waitlistRepositoryMock.Setup(r => r.GetMaxPriorityByBookingIdAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _service.GetNextPriorityAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().Be(1);
    }
}
