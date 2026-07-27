using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.UpdateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
    private readonly Mock<IConflictDetectionService> _conflictServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateBookingCommandHandler>();
    private readonly UpdateBookingCommandHandler _handler;

    public UpdateBookingCommandHandlerTests()
    {
        _handler = new UpdateBookingCommandHandler(
            _bookingRepoMock.Object,
            _availabilityServiceMock.Object,
            _conflictServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ReturnsFailure()
    {
        _bookingRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = Guid.NewGuid(),
            Title = "Updated Title"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Booking not found.");
    }

    [Theory]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    [InlineData(BookingStatus.Rejected)]
    [InlineData(BookingStatus.Expired)]
    public async Task Handle_InvalidStatus_ReturnsFailure(BookingStatus status)
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: status);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            Title = "Updated"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only draft or pending bookings can be updated.");
    }

    [Theory]
    [InlineData(BookingStatus.Draft)]
    [InlineData(BookingStatus.Pending)]
    public async Task Handle_AllowedStatuses_CanUpdate(BookingStatus status)
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: status);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            Title = "Updated Title"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Handle_StartTimeAfterEndTime_ReturnsFailure()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Draft);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Start time must be before end time.");
    }

    [Fact]
    public async Task Handle_StartTimeEqualsEndTime_ReturnsFailure()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Draft);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Start time must be before end time.");
    }

    [Fact]
    public async Task Handle_FacilityNotAvailable_ReturnsFailure()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(
            status: BookingStatus.Draft,
            facilityId: facilityId,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _availabilityServiceMock.Setup(s => s.IsFacilityAvailableAsync(
            facilityId, It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            FacilityId = facilityId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("facility is not available");
    }

    [Fact]
    public async Task Handle_CoachNotAvailable_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(
            status: BookingStatus.Pending,
            coachId: coachId,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _availabilityServiceMock.Setup(s => s.IsCoachAvailableAsync(
            coachId, It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("coach is not available");
    }

    [Fact]
    public async Task Handle_UpdateTitle_UpdatesSuccessfully()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Draft);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            Title = "New Title",
            Description = "New Description"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New Title");
        result.Value.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task Handle_UpdateTimes_RecalculatesDuration()
    {
        var booking = BookingTestDataBuilder.CreateBooking(
            status: BookingStatus.Draft,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            StartTime = TimeSpan.FromHours(14),
            EndTime = TimeSpan.FromHours(16)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Duration.Should().Be(120);
        booking.StartTime.Should().Be(TimeSpan.FromHours(14));
        booking.EndTime.Should().Be(TimeSpan.FromHours(16));
    }

    [Fact]
    public async Task Handle_UpdateFacility_ChecksAvailability()
    {
        var facilityId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(
            status: BookingStatus.Draft,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _availabilityServiceMock.Setup(s => s.IsFacilityAvailableAsync(
            facilityId, It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            FacilityId = facilityId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.FacilityId.Should().Be(facilityId);
    }

    [Fact]
    public async Task Handle_UpdateAthlete_UpdatesSuccessfully()
    {
        var athleteId = Guid.NewGuid();
        var booking = BookingTestDataBuilder.CreateBooking(
            status: BookingStatus.Draft,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.AthleteId.Should().Be(athleteId);
    }

    [Fact]
    public async Task Handle_UpdateWithoutTimeOrFacility_SkipsAvailabilityCheck()
    {
        var booking = BookingTestDataBuilder.CreateBooking(
            status: BookingStatus.Draft,
            startTime: TimeSpan.FromHours(9),
            endTime: TimeSpan.FromHours(10));
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            Title = "Just Title"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _availabilityServiceMock.Verify(s => s.IsFacilityAvailableAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
        _availabilityServiceMock.Verify(s => s.IsCoachAvailableAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_CallsUpdateAndSaveChanges()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Pending);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            Title = "Updated"
        }, CancellationToken.None);

        _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullTitleAndDescription_DoesNotOverwrite()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Draft);
        booking.Title = "Original Title";
        booking.Description = "Original Description";
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        booking.Title.Should().Be("Original Title");
        booking.Description.Should().Be("Original Description");
    }

    [Fact]
    public async Task Handle_CrossMidnightTime_ReturnsFailureBecauseStartAfterEnd()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Draft);
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            StartTime = new TimeSpan(22, 0, 0),
            EndTime = new TimeSpan(1, 0, 0)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Start time must be before end time.");
    }

    [Fact]
    public async Task Handle_UpdateSetsUpdatedAtToUtcNow()
    {
        var booking = BookingTestDataBuilder.CreateBooking(status: BookingStatus.Draft);
        var before = DateTime.UtcNow;
        _bookingRepoMock.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new UpdateBookingCommand
        {
            BookingId = booking.Id,
            Title = "Updated"
        }, CancellationToken.None);

        booking.UpdatedAt.Should().BeOnOrAfter(before);
    }
}
