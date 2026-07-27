using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.SearchBookings;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class SearchBookingsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<SearchBookingsQueryHandler>> _loggerMock = TestMocks.CreateLogger<SearchBookingsQueryHandler>();
    private readonly SearchBookingsQueryHandler _handler;

    public SearchBookingsQueryHandlerTests()
    {
        _handler = new SearchBookingsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsMatchingBookings()
    {
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
                Title = "Morning Session",
                AcademyId = Guid.NewGuid(),
                BookingDate = DateTime.UtcNow.AddDays(1),
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(10),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };
        _bookingRepositoryMock.Setup(r => r.SearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<BookingType?>(),
            It.IsAny<BookingStatus?>(), It.IsAny<string?>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);
        _bookingRepositoryMock.Setup(r => r.CountSearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<BookingType?>(),
            It.IsAny<BookingStatus?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new SearchBookingsQuery
        {
            SearchTerm = "Morning",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
        result.Value.Items[0].BookingNumber.Should().Be("BK-001");
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsEmptyList()
    {
        _bookingRepositoryMock.Setup(r => r.SearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<BookingType?>(),
            It.IsAny<BookingStatus?>(), It.IsAny<string?>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());
        _bookingRepositoryMock.Setup(r => r.CountSearchAsync(
            It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<BookingType?>(),
            It.IsAny<BookingStatus?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new SearchBookingsQuery
        {
            SearchTerm = "NonExistent",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}
