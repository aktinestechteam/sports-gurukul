using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingHistory;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetBookingHistoryQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetBookingHistoryQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetBookingHistoryQueryHandler>();
    private readonly GetBookingHistoryQueryHandler _handler;

    public GetBookingHistoryQueryHandlerTests()
    {
        _handler = new GetBookingHistoryQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new GetBookingHistoryQuery
        {
            BookingId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_BookingExists_ReturnsHistory()
    {
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            BookingNumber = "BK-TEST-001"
        };
        booking.History.Add(new BookingHistory
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Action = "Created",
            PerformedBy = Guid.NewGuid().ToString(),
            PerformedOn = DateTime.UtcNow.AddHours(-2),
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            UpdatedAt = DateTime.UtcNow.AddHours(-2)
        });
        booking.History.Add(new BookingHistory
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Action = "Confirmed",
            PerformedBy = Guid.NewGuid().ToString(),
            PerformedOn = DateTime.UtcNow.AddHours(-1),
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        });
        _bookingRepositoryMock.Setup(r => r.GetWithDetailsAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new GetBookingHistoryQuery
        {
            BookingId = bookingId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Action.Should().Be("Confirmed");
        result.Value[1].Action.Should().Be("Created");
    }
}
