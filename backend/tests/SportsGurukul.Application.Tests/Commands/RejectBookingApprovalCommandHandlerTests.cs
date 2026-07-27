using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBookingApproval;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class RejectBookingApprovalCommandHandlerTests
{
    private readonly Mock<IBookingApprovalService> _approvalServiceMock = new();
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<RejectBookingApprovalCommandHandler>> _loggerMock = TestMocks.CreateLogger<RejectBookingApprovalCommandHandler>();
    private readonly RejectBookingApprovalCommandHandler _handler;

    public RejectBookingApprovalCommandHandlerTests()
    {
        _handler = new RejectBookingApprovalCommandHandler(
            _approvalServiceMock.Object,
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ApprovalServiceReturnsFalse_ReturnsFailure()
    {
        _approvalServiceMock.Setup(s => s.ProcessApprovalAsync(
            It.IsAny<Guid>(), BookingApprovalStatus.Rejected,
            It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new RejectBookingApprovalCommand
        {
            BookingId = Guid.NewGuid(),
            ApproverUserId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ValidRejection_RejectsSuccessfully()
    {
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            Status = BookingStatus.Pending,
            BookingNumber = "BK-TEST-001"
        };
        _approvalServiceMock.Setup(s => s.ProcessApprovalAsync(
            bookingId, BookingApprovalStatus.Rejected,
            It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new RejectBookingApprovalCommand
        {
            BookingId = bookingId,
            ApproverUserId = Guid.NewGuid(),
            Comments = "Does not meet requirements"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Rejected);
    }
}
