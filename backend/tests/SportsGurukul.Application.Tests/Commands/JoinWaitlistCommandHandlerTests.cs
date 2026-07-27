using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.JoinWaitlist;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class JoinWaitlistCommandHandlerTests
{
    private readonly Mock<IWaitlistRepository> _waitlistRepositoryMock = TestMocks.CreateWaitlistRepository();
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IWaitlistService> _waitlistServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<JoinWaitlistCommandHandler>> _loggerMock = TestMocks.CreateLogger<JoinWaitlistCommandHandler>();
    private readonly JoinWaitlistCommandHandler _handler;

    public JoinWaitlistCommandHandlerTests()
    {
        _handler = new JoinWaitlistCommandHandler(
            _waitlistRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _waitlistServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new JoinWaitlistCommand
        {
            BookingId = Guid.NewGuid(),
            WaitlistUserId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_CancelledBooking_ReturnsFailure()
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Cancelled };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new JoinWaitlistCommand
        {
            BookingId = booking.Id,
            WaitlistUserId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cancelled booking");
    }

    [Fact]
    public async Task Handle_AlreadyOnWaitlist_ReturnsFailure()
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed };
        var userId = Guid.NewGuid();
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _waitlistRepositoryMock.Setup(r => r.GetByBookingAndUserAsync(booking.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingWaitlist());

        var result = await _handler.Handle(new JoinWaitlistCommand
        {
            BookingId = booking.Id,
            WaitlistUserId = userId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already on the waitlist");
    }

    [Fact]
    public async Task Handle_ValidJoin_AddsToWaitlistAndReturnsSuccess()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Confirmed,
            BookingNumber = "BK-TEST-001"
        };
        var userId = Guid.NewGuid();
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _waitlistRepositoryMock.Setup(r => r.GetByBookingAndUserAsync(booking.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookingWaitlist?)null);
        _waitlistServiceMock.Setup(s => s.GetNextPriorityAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new JoinWaitlistCommand
        {
            BookingId = booking.Id,
            WaitlistUserId = userId,
            Notes = "Interested"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Priority.Should().Be(1);
        result.Value.Status.Should().Be("Active");
        _waitlistRepositoryMock.Verify(r => r.AddAsync(It.IsAny<BookingWaitlist>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
