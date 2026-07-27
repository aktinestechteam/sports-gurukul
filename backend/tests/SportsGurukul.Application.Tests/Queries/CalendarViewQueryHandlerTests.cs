using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.CalendarView;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class CalendarViewQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<CalendarViewQueryHandler>> _loggerMock = TestMocks.CreateLogger<CalendarViewQueryHandler>();
    private readonly CalendarViewQueryHandler _handler;

    public CalendarViewQueryHandlerTests()
    {
        _handler = new CalendarViewQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_MonthlyView_ReturnsCalendarEvents()
    {
        var academyId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
                Title = "Morning Session",
                AcademyId = academyId,
                BookingDate = now.Date,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };

        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Monthly,
            ViewDate = now
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Events.Should().HaveCount(1);
        result.Value.Events[0].BookingNumber.Should().Be("BK-001");
        result.Value.ViewType.Should().Be(CalendarViewType.Monthly);
    }

    [Fact]
    public async Task Handle_WeeklyView_CoversCorrectDateRange()
    {
        var academyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Weekly,
            ViewDate = now
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ViewStartDate.Should().Be(now.Date.AddDays(-(int)now.DayOfWeek));
        result.Value.Events.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DailyView_CoversSingleDay()
    {
        var academyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Daily,
            ViewDate = now
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var expectedStart = now.Date;
        var expectedEnd = now.Date.AddDays(1);
        result.Value!.ViewStartDate.Should().Be(expectedStart);
        result.Value.ViewEndDate.Should().Be(expectedEnd);
    }

    [Fact]
    public async Task Handle_ExcludesCancelledBookings()
    {
        var academyId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Cancelled,
                Title = "Cancelled",
                AcademyId = academyId,
                BookingDate = now.Date,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };

        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Monthly,
            ViewDate = now
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Events.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_FiltersByFacilityId()
    {
        var academyId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.FacilityReservation,
                Status = BookingStatus.Confirmed,
                Title = "Facility Booking",
                AcademyId = academyId,
                FacilityId = facilityId,
                BookingDate = now.Date,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-002",
                BookingType = BookingType.FacilityReservation,
                Status = BookingStatus.Confirmed,
                Title = "Other Facility",
                AcademyId = academyId,
                FacilityId = Guid.NewGuid(),
                BookingDate = now.Date,
                StartTime = TimeSpan.FromHours(11),
                EndTime = TimeSpan.FromHours(12),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };

        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var result = await _handler.Handle(new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Monthly,
            ViewDate = now,
            FacilityId = facilityId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Events.Should().HaveCount(1);
        result.Value.Events[0].BookingNumber.Should().Be("BK-001");
    }

    [Fact]
    public async Task Handle_AgendaView_Covers30Days()
    {
        var academyId = Guid.NewGuid();

        _bookingRepositoryMock.Setup(r => r.GetByDateRangeAsync(
            academyId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var result = await _handler.Handle(new CalendarViewQuery
        {
            AcademyId = academyId,
            ViewType = CalendarViewType.Agenda
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var span = result.Value!.ViewEndDate - result.Value.ViewStartDate;
        span.Days.Should().Be(30);
    }
}
