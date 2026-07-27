using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetFacilityBookings;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetFacilityBookingsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetFacilityBookingsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetFacilityBookingsQueryHandler>();
    private readonly GetFacilityBookingsQueryHandler _handler;

    public GetFacilityBookingsQueryHandlerTests()
    {
        _handler = new GetFacilityBookingsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsBookingsForFacility()
    {
        var facilityId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
                Title = "Court A Session",
                AcademyId = Guid.NewGuid(),
                FacilityId = facilityId,
                BookingDate = date,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };
        _bookingRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facilityId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new GetFacilityBookingsQuery
        {
            FacilityId = facilityId,
            Date = date
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].FacilityName.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NoBookings_ReturnsEmptyList()
    {
        _bookingRepositoryMock.Setup(r => r.GetByFacilityIdAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new GetFacilityBookingsQuery
        {
            FacilityId = Guid.NewGuid(),
            Date = DateTime.UtcNow.Date
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
