using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.UpdateEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;
using SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;
using SportsGurukul.Application.Features.EventManagement.Commands.ScheduleEvent;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.RejectRegistration;
using SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;
using SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventById;
using SportsGurukul.Application.Features.EventManagement.Validators;

namespace SportsGurukul.Application.Tests.EventManagement.Validators;

public class EventValidatorTests
{
    [Fact]
    public async Task CreateEventValidator_EmptyEventName_Fails()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = string.Empty, AcademyId = Guid.NewGuid(), SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(37), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(25) };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.EventName);
    }

    [Fact]
    public async Task CreateEventValidator_EmptyAcademyId_Fails()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = "Test", AcademyId = Guid.Empty, SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(37), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(25) };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AcademyId);
    }

    [Fact]
    public async Task CreateEventValidator_EndDateBeforeStart_Fails()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = "Test", AcademyId = Guid.NewGuid(), SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(37), EndDate = DateTime.UtcNow.AddDays(30), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(25) };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public async Task CreateEventValidator_RegCloseAfterStart_Fails()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = "Test", AcademyId = Guid.NewGuid(), SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(17), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(15) };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.RegistrationCloseDate);
    }

    [Fact]
    public async Task CreateEventValidator_NegativeFee_Fails()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = "Test", AcademyId = Guid.NewGuid(), SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(37), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(25), RegistrationFee = -10 };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.RegistrationFee);
    }

    [Fact]
    public async Task CreateEventValidator_InvalidEmail_Fails()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = "Test", AcademyId = Guid.NewGuid(), SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(37), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(25), ContactEmail = "invalid-email" };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public async Task CreateEventValidator_ValidRequest_NoErrors()
    {
        var validator = new CreateEventValidator();
        var command = new CreateEventCommand { EventName = "Test", AcademyId = Guid.NewGuid(), SportId = Guid.NewGuid(), EventTypeId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(37), RegistrationOpenDate = DateTime.UtcNow.AddDays(1), RegistrationCloseDate = DateTime.UtcNow.AddDays(25) };
        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateEventValidator_EmptyEventId_Fails()
    {
        var validator = new UpdateEventValidator();
        var command = new UpdateEventCommand { EventId = Guid.Empty };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.EventId);
    }

    [Fact]
    public async Task UpdateEventValidator_ValidRequest_NoErrors()
    {
        var validator = new UpdateEventValidator();
        var command = new UpdateEventCommand { EventId = Guid.NewGuid(), EventName = "Test" };
        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CheckInParticipantValidator_EmptyIds_Fails()
    {
        var validator = new CheckInParticipantValidator();
        var command = new CheckInParticipantCommand { EventId = Guid.Empty, ParticipantId = Guid.Empty };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.EventId);
        result.ShouldHaveValidationErrorFor(x => x.ParticipantId);
    }

    [Fact]
    public async Task SubmitFeedbackValidator_RatingOutOfRange_Fails()
    {
        var validator = new SubmitFeedbackValidator();
        var command = new SubmitFeedbackCommand { EventId = Guid.NewGuid(), UserId = Guid.NewGuid(), OverallRating = 6 };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.OverallRating);
    }

    [Fact]
    public async Task SubmitFeedbackValidator_ValidFeedback_NoErrors()
    {
        var validator = new SubmitFeedbackValidator();
        var command = new SubmitFeedbackCommand { EventId = Guid.NewGuid(), UserId = Guid.NewGuid(), OverallRating = 4 };
        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ScheduleEventValidator_EmptyEventId_Fails()
    {
        var validator = new ScheduleEventValidator();
        var command = new ScheduleEventCommand { EventId = Guid.Empty };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.EventId);
    }

    [Fact]
    public async Task ApproveRegistrationValidator_EmptyRegistrationId_Fails()
    {
        var validator = new ApproveRegistrationValidator();
        var command = new ApproveRegistrationCommand { RegistrationId = Guid.Empty };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.RegistrationId);
    }

    [Fact]
    public async Task GetEventByIdValidator_EmptyEventId_Fails()
    {
        var validator = new GetEventByIdValidator();
        var query = new GetEventByIdQuery { EventId = Guid.Empty };
        var result = await validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.EventId);
    }

    [Fact]
    public async Task RegisterParticipantValidator_EmptyEventId_Fails()
    {
        var validator = new RegisterParticipantValidator();
        var command = new RegisterParticipantCommand { EventId = Guid.Empty };
        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.EventId);
    }
}
