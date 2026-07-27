using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetAthleteBookings;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteBookingsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetAthleteBookingsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteBookingsQueryHandler>();
    private readonly GetAthleteBookingsQueryHandler _handler;

    public GetAthleteBookingsQueryHandlerTests()
    {
        _handler = new GetAthleteBookingsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsBookingsForAthlete()
    {
        var athleteId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
                Title = "Training Session",
                AcademyId = Guid.NewGuid(),
                AthleteId = athleteId,
                BookingDate = DateTime.UtcNow.AddDays(1),
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByAthleteIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new GetAthleteBookingsQuery
        {
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].BookingNumber.Should().Be("BK-001");
    }

    [Fact]
    public async Task Handle_NoBookings_ReturnsEmptyList()
    {
        _bookingRepositoryMock.Setup(r => r.GetByAthleteIdAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new GetAthleteBookingsQuery
        {
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
