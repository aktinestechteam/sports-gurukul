using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetUpcomingBookings;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetUpcomingBookingsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetUpcomingBookingsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetUpcomingBookingsQueryHandler>();
    private readonly GetUpcomingBookingsQueryHandler _handler;

    public GetUpcomingBookingsQueryHandlerTests()
    {
        _handler = new GetUpcomingBookingsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUpcomingBookings()
    {
        var academyId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
                Title = "Upcoming Session",
                AcademyId = academyId,
                BookingDate = DateTime.UtcNow.AddDays(2),
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new GetUpcomingBookingsQuery
        {
            AcademyId = academyId,
            DaysAhead = 7
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ExcludesCancelledBookings()
    {
        var academyId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Status = BookingStatus.Cancelled,
                BookingDate = DateTime.UtcNow.AddDays(2),
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Status = BookingStatus.Confirmed,
                BookingDate = DateTime.UtcNow.AddDays(3),
                StartTime = TimeSpan.FromHours(14),
                EndTime = TimeSpan.FromHours(15)
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new GetUpcomingBookingsQuery
        {
            AcademyId = academyId,
            DaysAhead = 7
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}
