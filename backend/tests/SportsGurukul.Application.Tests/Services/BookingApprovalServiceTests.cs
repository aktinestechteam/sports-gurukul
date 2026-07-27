using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Services;

public class BookingApprovalServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<BookingApprovalService>> _loggerMock = TestMocks.CreateLogger<BookingApprovalService>();
    private readonly BookingApprovalService _service;

    public BookingApprovalServiceTests()
    {
        _service = new BookingApprovalService(
            _bookingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessApprovalAsync_BookingNotFound_ReturnsFalse()
    {
        _bookingRepositoryMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _service.ProcessApprovalAsync(
            Guid.NewGuid(), BookingApprovalStatus.Approved, Guid.NewGuid(),
            null, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessApprovalAsync_ValidBooking_UpdatesApprovalStatus()
    {
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            Status = BookingStatus.Pending,
            ApprovalStatus = BookingApprovalStatus.Pending
        };
        _bookingRepositoryMock.Setup(r => r.GetWithDetailsAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _service.ProcessApprovalAsync(
            bookingId, BookingApprovalStatus.Approved, Guid.NewGuid(),
            "Approved", CancellationToken.None);

        result.Should().BeTrue();
        booking.ApprovalStatus.Should().Be(BookingApprovalStatus.Approved);
    }

    [Fact]
    public void CreateApprovalRequestAsync_ReturnsApprovalEntity()
    {
        var bookingId = Guid.NewGuid();

        var result = _service.CreateApprovalRequestAsync(
            bookingId, BookingApprovalStatus.Pending, "Needs review").Result;

        result.Should().NotBeNull();
        result.BookingId.Should().Be(bookingId);
        result.ApprovalStatus.Should().Be(BookingApprovalStatus.Pending);
        result.Comments.Should().Be("Needs review");
        result.EscalationLevel.Should().Be(0);
    }
}
