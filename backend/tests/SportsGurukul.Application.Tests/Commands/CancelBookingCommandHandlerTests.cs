using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class CancelBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IWaitlistService> _waitlistServiceMock = new();
    private readonly Mock<IWaitlistRepository> _waitlistRepositoryMock = TestMocks.CreateWaitlistRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<CancelBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<CancelBookingCommandHandler>();
    private readonly CancelBookingCommandHandler _handler;

    public CancelBookingCommandHandlerTests()
    {
        _handler = new CancelBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _waitlistServiceMock.Object,
            _waitlistRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new CancelBookingCommand
        {
            BookingId = Guid.NewGuid(),
            Reason = "No longer needed"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ReturnsFailure()
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Cancelled };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new CancelBookingCommand
        {
            BookingId = booking.Id,
            Reason = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking is already cancelled.");
    }

    [Fact]
    public async Task Handle_CompletedBooking_ReturnsFailure()
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Completed };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new CancelBookingCommand
        {
            BookingId = booking.Id,
            Reason = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a completed booking.");
    }

    [Fact]
    public async Task Handle_ValidCancellation_SetsStatusAndReturnsSuccess()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Confirmed,
            BookingNumber = "BK-TEST-001",
            BookingCreatorId = Guid.NewGuid()
        };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CancelBookingCommand
        {
            BookingId = booking.Id,
            Reason = "Schedule conflict",
            Notes = "Will rebook later"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _bookingRepositoryMock.Verify(r => r.Update(booking), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
