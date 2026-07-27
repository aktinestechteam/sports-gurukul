using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingStatistics;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetBookingStatisticsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetBookingStatisticsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetBookingStatisticsQueryHandler>();
    private readonly GetBookingStatisticsQueryHandler _handler;

    public GetBookingStatisticsQueryHandlerTests()
    {
        _handler = new GetBookingStatisticsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsStatistics()
    {
        var academyId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed, BookingDate = DateTime.UtcNow, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) },
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Completed, BookingDate = DateTime.UtcNow, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11) },
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Cancelled, BookingDate = DateTime.UtcNow, StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15) }
        };
        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new GetBookingStatisticsQuery
        {
            AcademyId = academyId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalBookings.Should().Be(3);
        result.Value.CancellationRate.Should().BeApproximately(33.33m, 0.01m);
    }

    [Fact]
    public async Task Handle_NoBookings_ReturnsZeroStatistics()
    {
        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new GetBookingStatisticsQuery
        {
            AcademyId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalBookings.Should().Be(0);
        result.Value.CancellationRate.Should().Be(0);
        result.Value.FacilityUtilizationPercent.Should().Be(0);
    }
}
