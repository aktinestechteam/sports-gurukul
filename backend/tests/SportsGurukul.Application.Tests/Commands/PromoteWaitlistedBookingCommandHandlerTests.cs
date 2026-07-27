using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.PromoteWaitlistedBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class PromoteWaitlistedBookingCommandHandlerTests
{
    private readonly Mock<IWaitlistRepository> _waitlistRepositoryMock = TestMocks.CreateWaitlistRepository();
    private readonly Mock<IWaitlistService> _waitlistServiceMock = new();
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<PromoteWaitlistedBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<PromoteWaitlistedBookingCommandHandler>();
    private readonly PromoteWaitlistedBookingCommandHandler _handler;

    public PromoteWaitlistedBookingCommandHandlerTests()
    {
        _handler = new PromoteWaitlistedBookingCommandHandler(
            _waitlistRepositoryMock.Object,
            _waitlistServiceMock.Object,
            _bookingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_EntryNotFound_ReturnsFailure()
    {
        _waitlistRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookingWaitlist?)null);

        var result = await _handler.Handle(new PromoteWaitlistedBookingCommand
        {
            WaitlistEntryId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Waitlist entry not found.");
    }

    [Fact]
    public async Task Handle_NotActive_ReturnsFailure()
    {
        var entry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            Status = WaitlistStatus.Promoted
        };
        _waitlistRepositoryMock.Setup(r => r.GetByIdAsync(entry.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);

        var result = await _handler.Handle(new PromoteWaitlistedBookingCommand
        {
            WaitlistEntryId = entry.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only active waitlist entries can be promoted.");
    }

    [Fact]
    public async Task Handle_PromotionFails_ReturnsFailure()
    {
        var entry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            Status = WaitlistStatus.Active,
            BookingId = Guid.NewGuid()
        };
        _waitlistRepositoryMock.Setup(r => r.GetByIdAsync(entry.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _waitlistServiceMock.Setup(s => s.PromoteWaitlistedBookingAsync(entry, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new PromoteWaitlistedBookingCommand
        {
            WaitlistEntryId = entry.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to promote waitlist entry.");
    }

    [Fact]
    public async Task Handle_ValidPromotion_PromotesSuccessfully()
    {
        var entry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            Status = WaitlistStatus.Active,
            BookingId = Guid.NewGuid()
        };
        var booking = new Booking
        {
            Id = entry.BookingId,
            Status = BookingStatus.Confirmed,
            BookingNumber = "BK-TEST-001",
            AcademyId = Guid.NewGuid()
        };
        _waitlistRepositoryMock.Setup(r => r.GetByIdAsync(entry.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _waitlistServiceMock.Setup(s => s.PromoteWaitlistedBookingAsync(entry, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(entry.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new PromoteWaitlistedBookingCommand
        {
            WaitlistEntryId = entry.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BookingNumber.Should().Be("BK-TEST-001");
    }
}
