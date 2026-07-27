using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RescheduleBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class RescheduleBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
    private readonly Mock<IConflictDetectionService> _conflictDetectionServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RescheduleBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<RescheduleBookingCommandHandler>();
    private readonly RescheduleBookingCommandHandler _handler;

    public RescheduleBookingCommandHandlerTests()
    {
        _handler = new RescheduleBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _availabilityServiceMock.Object,
            _conflictDetectionServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new RescheduleBookingCommand
        {
            BookingId = Guid.NewGuid(),
            NewDate = DateTime.UtcNow.AddDays(5),
            NewStartTime = TimeSpan.FromHours(9),
            NewEndTime = TimeSpan.FromHours(10)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Theory]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Rejected)]
    public async Task Handle_InvalidStatus_ReturnsFailure(BookingStatus status)
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = status };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new RescheduleBookingCommand
        {
            BookingId = booking.Id,
            NewDate = DateTime.UtcNow.AddDays(5),
            NewStartTime = TimeSpan.FromHours(9),
            NewEndTime = TimeSpan.FromHours(10)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only pending or confirmed");
    }

    [Fact]
    public async Task Handle_NewTimeStartAfterEnd_ReturnsFailure()
    {
        var booking = new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed };
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new RescheduleBookingCommand
        {
            BookingId = booking.Id,
            NewDate = DateTime.UtcNow.AddDays(5),
            NewStartTime = TimeSpan.FromHours(10),
            NewEndTime = TimeSpan.FromHours(9)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("New start time must be before new end time.");
    }

    [Fact]
    public async Task Handle_ValidReschedule_UpdatesBookingSuccessfully()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Confirmed,
            BookingNumber = "BK-TEST-001",
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Duration = 60
        };
        var newDate = DateTime.UtcNow.AddDays(5);
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RescheduleBookingCommand
        {
            BookingId = booking.Id,
            NewDate = newDate,
            NewStartTime = TimeSpan.FromHours(14),
            NewEndTime = TimeSpan.FromHours(16),
            Reason = "Coach unavailable"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.BookingDate.Date.Should().Be(newDate.Date);
        booking.StartTime.Should().Be(TimeSpan.FromHours(14));
        booking.EndTime.Should().Be(TimeSpan.FromHours(16));
        booking.Duration.Should().Be(120);
    }
}
