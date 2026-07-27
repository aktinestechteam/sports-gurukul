using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.AdvancedSearchBookings;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class AdvancedSearchBookingsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<AdvancedSearchBookingsQueryHandler>> _loggerMock = TestMocks.CreateLogger<AdvancedSearchBookingsQueryHandler>();
    private readonly AdvancedSearchBookingsQueryHandler _handler;

    public AdvancedSearchBookingsQueryHandlerTests()
    {
        _handler = new AdvancedSearchBookingsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsBookingsWithPagination()
    {
        var bookings = new List<Booking>
        {
            CreateBooking("BK-001", "Morning Session", BookingType.TrainingSession, BookingStatus.Confirmed),
            CreateBooking("BK-002", "Evening Practice", BookingType.FacilityReservation, BookingStatus.Pending)
        };

        SetupSearch(bookings, 2);

        var result = await _handler.Handle(new AdvancedSearchBookingsQuery
        {
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(2);
        result.Value.TotalPages.Should().Be(1);
        result.Value.CurrentPage.Should().Be(1);
        result.Value.SearchTimeMs.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task Handle_FiltersByBookingType()
    {
        var bookings = new List<Booking>
        {
            CreateBooking("BK-001", "Session", BookingType.TrainingSession, BookingStatus.Confirmed)
        };

        _bookingRepositoryMock.Setup(r => r.SearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<BookingType?>(), It.IsAny<BookingStatus?>(),
            It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);
        _bookingRepositoryMock.Setup(r => r.CountSearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<BookingType?>(), It.IsAny<BookingStatus?>(),
            It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AdvancedSearchBookingsQuery
        {
            BookingType = "TrainingSession",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_SortByDate_ReturnsOrderedByDate()
    {
        var bookings = new List<Booking>
        {
            CreateBooking("BK-002", "Later", BookingType.TrainingSession, BookingStatus.Confirmed, DateTime.UtcNow.AddDays(2)),
            CreateBooking("BK-001", "Earlier", BookingType.TrainingSession, BookingStatus.Confirmed, DateTime.UtcNow.AddDays(1))
        };

        SetupSearch(bookings, 2);

        var result = await _handler.Handle(new AdvancedSearchBookingsQuery
        {
            SortBy = "date",
            SortDescending = false,
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items[0].BookingNumber.Should().Be("BK-001");
    }

    [Fact]
    public async Task Handle_FiltersByDateRange()
    {
        var bookings = new List<Booking>
        {
            CreateBooking("BK-001", "In Range", BookingType.TrainingSession, BookingStatus.Confirmed, DateTime.UtcNow.AddDays(5))
        };

        SetupSearch(bookings, 1);

        var result = await _handler.Handle(new AdvancedSearchBookingsQuery
        {
            DateFrom = DateTime.UtcNow.AddDays(1),
            DateTo = DateTime.UtcNow.AddDays(10),
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_BookingNumberFilter_FiltersCorrectly()
    {
        var allBookings = new List<Booking>
        {
            CreateBooking("BK-001", "First", BookingType.TrainingSession, BookingStatus.Confirmed),
            CreateBooking("BK-002", "Second", BookingType.TrainingSession, BookingStatus.Confirmed),
            CreateBooking("BK-100", "Third", BookingType.TrainingSession, BookingStatus.Confirmed)
        };

        SetupSearch(allBookings, 3);

        var result = await _handler.Handle(new AdvancedSearchBookingsQuery
        {
            BookingNumber = "BK-001",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].BookingNumber.Should().Be("BK-001");
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsEmptyPage()
    {
        SetupSearch(new List<Booking>(), 0);

        var result = await _handler.Handle(new AdvancedSearchBookingsQuery
        {
            SearchTerm = "NonExistent",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.HasNext.Should().BeFalse();
        result.Value.HasPrevious.Should().BeFalse();
    }

    private void SetupSearch(IReadOnlyList<Booking> bookings, int totalCount)
    {
        _bookingRepositoryMock.Setup(r => r.SearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<BookingType?>(), It.IsAny<BookingStatus?>(),
            It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);
        _bookingRepositoryMock.Setup(r => r.CountSearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<BookingType?>(), It.IsAny<BookingStatus?>(),
            It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);
    }

    private static Booking CreateBooking(
        string number, string title, BookingType type, BookingStatus status,
        DateTime? date = null)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = number,
            BookingType = type,
            Status = status,
            Title = title,
            AcademyId = Guid.NewGuid(),
            BookingDate = date ?? DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Duration = 60,
            ApprovalStatus = BookingApprovalStatus.Approved,
            CreatedAt = DateTime.UtcNow
        };
    }
}
