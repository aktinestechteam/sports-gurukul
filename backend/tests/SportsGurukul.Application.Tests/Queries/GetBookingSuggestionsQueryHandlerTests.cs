using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetBookingSuggestions;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class GetBookingSuggestionsQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetBookingSuggestionsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetBookingSuggestionsQueryHandler>();
    private readonly GetBookingSuggestionsQueryHandler _handler;

    public GetBookingSuggestionsQueryHandlerTests()
    {
        _handler = new GetBookingSuggestionsQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShortPrefix_ReturnsEmptyList()
    {
        var result = await _handler.Handle(new GetBookingSuggestionsQuery
        {
            Prefix = "A",
            Limit = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ValidPrefix_ReturnsSuggestions()
    {
        var bookings = new List<Booking>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookingNumber = "BK-001",
                Title = "Morning Training",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
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

        var result = await _handler.Handle(new GetBookingSuggestionsQuery
        {
            Prefix = "BK-0",
            Limit = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value!.First().Text.Should().Be("BK-001");
        result.Value.First().Category.Should().Be("Booking Number");
    }

    [Fact]
    public async Task Handle_EmptyPrefix_ReturnsEmpty()
    {
        var result = await _handler.Handle(new GetBookingSuggestionsQuery
        {
            Prefix = "",
            Limit = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
