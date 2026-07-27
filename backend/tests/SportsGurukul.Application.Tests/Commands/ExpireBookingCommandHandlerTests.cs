using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ExpireBooking;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class ExpireBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<ExpireBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<ExpireBookingCommandHandler>();
    private readonly ExpireBookingCommandHandler _handler;

    public ExpireBookingCommandHandlerTests()
    {
        _handler = new ExpireBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new ExpireBookingCommand { BookingId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Theory]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Rejected)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    public async Task Handle_InvalidStatus_ReturnsFailure(BookingStatus status)
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = status };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new ExpireBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only draft or pending bookings can be expired.");
    }

    [Theory]
    [InlineData(BookingStatus.Draft)]
    [InlineData(BookingStatus.Pending)]
    public async Task Handle_ValidStatus_ExpiresSuccessfully(BookingStatus status)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = status,
            BookingNumber = "BK-TEST-001"
        };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new ExpireBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Expired);
    }
}
