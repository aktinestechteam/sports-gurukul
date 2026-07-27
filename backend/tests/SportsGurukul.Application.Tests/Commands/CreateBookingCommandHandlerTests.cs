using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class CreateBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = TestMocks.CreateBookingRepository();
    private readonly Mock<IConflictDetectionService> _conflictDetectionServiceMock = new();
    private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
    private readonly Mock<ISchedulingEngine> _schedulingEngineMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<CreateBookingCommandHandler>> _loggerMock = TestMocks.CreateLogger<CreateBookingCommandHandler>();
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests()
    {
        _handler = new CreateBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _conflictDetectionServiceMock.Object,
            _availabilityServiceMock.Object,
            _schedulingEngineMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidBookingType_ReturnsFailure()
    {
        var result = await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "InvalidType",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Title = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid booking type");
    }

    [Fact]
    public async Task Handle_StartTimeAfterEndTime_ReturnsFailure()
    {
        var result = await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "TrainingSession",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(9),
            Title = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Start time must be before end time.");
    }

    [Fact]
    public async Task Handle_PastDate_ReturnsFailure()
    {
        var result = await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "TrainingSession",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.AddDays(-1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Title = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("past dates");
    }

    [Fact]
    public async Task Handle_FacilityNotAvailable_ReturnsFailure()
    {
        var facilityId = Guid.NewGuid();
        _availabilityServiceMock.Setup(s => s.IsFacilityAvailableAsync(
            facilityId, It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "TrainingSession",
            AcademyId = Guid.NewGuid(),
            FacilityId = facilityId,
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Title = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not available");
    }

    [Fact]
    public async Task Handle_CoachNotAvailable_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        _availabilityServiceMock.Setup(s => s.IsFacilityAvailableAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _availabilityServiceMock.Setup(s => s.IsCoachAvailableAsync(
            coachId, It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "TrainingSession",
            AcademyId = Guid.NewGuid(),
            CoachId = coachId,
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Title = "Test"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("coach");
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesBookingAndReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        _schedulingEngineMock.Setup(s => s.GenerateBookingNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("BK-20260727-ABCD");
        _conflictDetectionServiceMock.Setup(s => s.DetectConflictsAsync(
            It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict>());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "TrainingSession",
            Title = "Morning Session",
            AcademyId = academyId,
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BookingNumber.Should().Be("BK-20260727-ABCD");
        result.Value.Title.Should().Be("Morning Session");
        result.Value.AcademyId.Should().Be(academyId);
        result.Value.Duration.Should().Be(60);
        _bookingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_DetectsConflicts()
    {
        var bookingId = Guid.NewGuid();
        Booking? capturedBooking = null;
        _schedulingEngineMock.Setup(s => s.GenerateBookingNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("BK-20260727-ABCD");
        _availabilityServiceMock.Setup(s => s.IsFacilityAvailableAsync(
            It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(),
            null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((b, _) => { capturedBooking = b; b.Id = bookingId; })
            .ReturnsAsync(new Booking());
        _conflictDetectionServiceMock.Setup(s => s.DetectConflictsAsync(
            It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookingConflict>
            {
                new() { Id = Guid.NewGuid(), ConflictType = BookingConflictType.FacilityOverlap }
            });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new CreateBookingCommand
        {
            BookingType = "TrainingSession",
            Title = "Conflict Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        }, CancellationToken.None);

        _conflictDetectionServiceMock.Verify(s => s.DetectConflictsAsync(
            It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
