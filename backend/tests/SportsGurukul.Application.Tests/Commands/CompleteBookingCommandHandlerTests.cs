using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CompleteBooking;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class CompleteBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<CompleteBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<CompleteBookingCommandHandler>();
    private readonly CompleteBookingCommandHandler _handler;

    public CompleteBookingCommandHandlerTests()
    {
        _handler = new CompleteBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new CompleteBookingCommand { BookingId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_NotConfirmed_ReturnsFailure()
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Pending };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new CompleteBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only confirmed bookings can be completed.");
    }

    [Fact]
    public async Task Handle_ConfirmedBooking_CompletesSuccessfully()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Confirmed,
            BookingNumber = "BK-TEST-001"
        };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CompleteBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Completed);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
