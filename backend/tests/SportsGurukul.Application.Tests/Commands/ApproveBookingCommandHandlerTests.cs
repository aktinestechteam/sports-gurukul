using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ApproveBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class ApproveBookingCommandHandlerTests
{
    private readonly Mock<IBookingApprovalService> _approvalServiceMock = new();
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<ILogger<ApproveBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<ApproveBookingCommandHandler>();
    private readonly ApproveBookingCommandHandler _handler;

    public ApproveBookingCommandHandlerTests()
    {
        _handler = new ApproveBookingCommandHandler(
            _approvalServiceMock.Object,
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ApprovalServiceReturnsFalse_ReturnsFailure()
    {
        _approvalServiceMock.Setup(s => s.ProcessApprovalAsync(
            It.IsAny<Guid>(), BookingApprovalStatus.Approved,
            It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new ApproveBookingCommand
        {
            BookingId = Guid.NewGuid(),
            ApproverUserId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found or cannot be approved");
    }

    [Fact]
    public async Task Handle_ValidApproval_ApprovesAndReturnsBooking()
    {
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            Status = BookingStatus.Pending,
            BookingNumber = "BK-TEST-001",
            AcademyId = Guid.NewGuid()
        };
        _approvalServiceMock.Setup(s => s.ProcessApprovalAsync(
            bookingId, BookingApprovalStatus.Approved,
            It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new ApproveBookingCommand
        {
            BookingId = bookingId,
            ApproverUserId = Guid.NewGuid(),
            Comments = "Looks good"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BookingNumber.Should().Be("BK-TEST-001");
    }
}
