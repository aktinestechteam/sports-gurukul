using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;
using SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.EdgeCases;

public class EdgeCaseTests
{
    [Fact]
    public async Task RegisterParticipant_MaxCapacity_Waitlists()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.RegistrationType = EventRegistrationType.Waitlist;
        evt.MaxParticipants = 1;

        var eventRepo = EventMockFactory.CreateEventRepository();
        var regRepo = EventMockFactory.CreateRegistrationRepository();
        var regService = EventMockFactory.CreateRegistrationService();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<RegisterParticipantCommandHandler>();
        var handler = new RegisterParticipantCommandHandler(eventRepo.Object, regRepo.Object, regService.Object, unitOfWork.Object, logger.Object);

        eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        regService.Setup(x => x.IsRegistrationAllowedAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        regService.Setup(x => x.IsDuplicateRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        regService.Setup(x => x.DetermineInitialStatusAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(EventRegistrationStatus.Waitlisted);
        regService.Setup(x => x.GenerateRegistrationNumberAsync(It.IsAny<CancellationToken>())).ReturnsAsync("REG-001");

        var result = await handler.Handle(new RegisterParticipantCommand { EventId = evt.Id, AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(EventRegistrationStatus.Waitlisted.ToString());
    }

    [Fact]
    public async Task ApproveRegistration_AlreadyApproved_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateApprovedRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();

        var regRepo = EventMockFactory.CreateRegistrationRepository();
        var regService = EventMockFactory.CreateRegistrationService();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<ApproveRegistrationCommandHandler>();
        var handler = new ApproveRegistrationCommandHandler(regRepo.Object, regService.Object, unitOfWork.Object, logger.Object);

        regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await handler.Handle(new ApproveRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only pending or waitlisted");
    }

    [Fact]
    public async Task CheckIn_AlreadyCheckedIn_ReturnsNewAttendance()
    {
        var participantId = Guid.NewGuid();
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        var participant = EventDataFixture.CreateParticipant(participantId, evt.Id, EventAttendanceStatus.CheckedIn);
        evt.Participants = new List<Domain.Entities.EventParticipant> { participant };

        var eventRepo = EventMockFactory.CreateEventRepository();
        var attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        var attendanceService = EventMockFactory.CreateAttendanceService();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<CheckInParticipantCommandHandler>();
        var handler = new CheckInParticipantCommandHandler(attendanceRepo.Object, eventRepo.Object, attendanceService.Object, unitOfWork.Object, logger.Object);

        eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        attendanceService.Setup(x => x.CanCheckInAsync(participant, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await handler.Handle(new CheckInParticipantCommand { EventId = evt.Id, ParticipantId = participantId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not eligible");
    }

    [Fact]
    public async Task CheckOut_NotCheckedIn_ReturnsFailure()
    {
        var attendance = EventDataFixture.CreateAttendance();
        attendance.CheckInTime = null;
        attendance.Participant = EventDataFixture.CreateParticipant();

        var attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        var attendanceService = EventMockFactory.CreateAttendanceService();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<CheckOutParticipantCommandHandler>();
        var handler = new CheckOutParticipantCommandHandler(attendanceRepo.Object, attendanceService.Object, unitOfWork.Object, logger.Object);

        attendanceRepo.Setup(x => x.GetByIdAsync(attendance.Id, It.IsAny<CancellationToken>())).ReturnsAsync(attendance);

        var result = await handler.Handle(new CheckOutParticipantCommand { AttendanceId = attendance.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("has not checked in");
    }

    [Fact]
    public async Task MoveFromWaitlist_AlreadyApproved_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateApprovedRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();

        var regRepo = EventMockFactory.CreateRegistrationRepository();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<MoveFromWaitlistCommandHandler>();
        var handler = new MoveFromWaitlistCommandHandler(regRepo.Object, unitOfWork.Object, logger.Object);

        regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await handler.Handle(new MoveFromWaitlistCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only waitlisted");
    }

    [Fact]
    public async Task GenerateCertificates_EventNotCompleted_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateInProgressEvent();
        var eventRepo = EventMockFactory.CreateEventRepository();
        var certService = EventMockFactory.CreateCertificateService();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<GenerateCertificatesCommandHandler>();

        eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var handler = new SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates.GenerateCertificatesCommandHandler(
            eventRepo.Object, certService.Object, unitOfWork.Object, logger.Object);

        var result = await handler.Handle(new SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates.GenerateCertificatesCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("can only be generated for completed events");
    }
}
