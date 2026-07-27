using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetCoachBookings;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachBookingsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetCoachBookingsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachBookingsQueryHandler>();
    private readonly GetCoachBookingsQueryHandler _handler;

    public GetCoachBookingsQueryHandlerTests()
    {
        _handler = new GetCoachBookingsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsBookingsForCoach()
    {
        var coachId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.PrivateCoaching,
                Status = BookingStatus.Confirmed,
                Title = "Private Coaching",
                AcademyId = Guid.NewGuid(),
                CoachId = coachId,
                BookingDate = date,
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(11),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new GetCoachBookingsQuery
        {
            CoachId = coachId,
            Date = date
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_NoBookings_ReturnsEmptyList()
    {
        _bookingRepositoryMock.Setup(r => r.GetByCoachIdAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new GetCoachBookingsQuery
        {
            CoachId = Guid.NewGuid(),
            Date = DateTime.UtcNow.Date
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
