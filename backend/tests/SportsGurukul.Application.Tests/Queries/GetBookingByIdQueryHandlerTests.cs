using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingById;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Queries;

public class GetBookingByIdQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<GetBookingByIdQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetBookingByIdQueryHandler>();
    private readonly GetBookingByIdQueryHandler _handler;

    public GetBookingByIdQueryHandlerTests()
    {
        _handler = new GetBookingByIdQueryHandler(
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepositoryMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new GetBookingByIdQuery
        {
            BookingId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_BookingExists_ReturnsBookingDto()
    {
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            BookingNumber = "BK-TEST-001",
            BookingType = BookingType.TrainingSession,
            Status = BookingStatus.Confirmed,
            Title = "Morning Training",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Duration = 60,
            ApprovalStatus = BookingApprovalStatus.Approved,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _bookingRepositoryMock.Setup(r => r.GetWithDetailsAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new GetBookingByIdQuery
        {
            BookingId = bookingId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(bookingId);
        result.Value.BookingNumber.Should().Be("BK-TEST-001");
        result.Value.Title.Should().Be("Morning Training");
        result.Value.Duration.Should().Be(60);
    }
}
