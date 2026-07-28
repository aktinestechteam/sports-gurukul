using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CheckOutParticipantCommandHandlerTests
{
    private readonly Mock<IEventAttendanceRepository> _attendanceRepo;
    private readonly Mock<IEventAttendanceService> _attendanceService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CheckOutParticipantCommandHandler>> _logger;
    private readonly CheckOutParticipantCommandHandler _handler;

    public CheckOutParticipantCommandHandlerTests()
    {
        _attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        _attendanceService = EventMockFactory.CreateAttendanceService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<CheckOutParticipantCommandHandler>();
        _handler = new CheckOutParticipantCommandHandler(_attendanceRepo.Object, _attendanceService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_AttendanceNotFound_ReturnsFailure()
    {
        _attendanceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventAttendance?)null);

        var result = await _handler.Handle(new CheckOutParticipantCommand { AttendanceId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Attendance record not found");
    }

    [Fact]
    public async Task Handle_NoCheckIn_ReturnsFailure()
    {
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckInTime = null;
        attendance.Participant = EventDataFixture.CreateParticipant();
        _attendanceRepo.Setup(x => x.GetByIdAsync(attendance.Id, It.IsAny<CancellationToken>())).ReturnsAsync(attendance);

        var result = await _handler.Handle(new CheckOutParticipantCommand { AttendanceId = attendance.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("has not checked in");
    }

    [Fact]
    public async Task Handle_AlreadyCheckedOut_ReturnsFailure()
    {
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckOutTime = DateTime.UtcNow;
        attendance.Participant = EventDataFixture.CreateParticipant();
        _attendanceRepo.Setup(x => x.GetByIdAsync(attendance.Id, It.IsAny<CancellationToken>())).ReturnsAsync(attendance);

        var result = await _handler.Handle(new CheckOutParticipantCommand { AttendanceId = attendance.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already checked out");
    }

    [Fact]
    public async Task Handle_ValidCheckOut_UpdatesAttendance()
    {
        var participant = EventDataFixture.CreateParticipant();
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckInTime = DateTime.UtcNow.AddHours(-1);
        attendance.CheckOutTime = null;
        attendance.Participant = participant;
        _attendanceRepo.Setup(x => x.GetByIdAsync(attendance.Id, It.IsAny<CancellationToken>())).ReturnsAsync(attendance);
        _attendanceService.Setup(x => x.CanCheckOutAsync(participant, attendance, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(new CheckOutParticipantCommand { AttendanceId = attendance.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        attendance.CheckOutTime.Should().NotBeNull();
    }
}
