using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class ScheduleCampaignCommandValidatorTests
{
    private readonly ScheduleCampaignCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCampaignIdIsEmpty_ShouldHaveError()
    {
        var command = new ScheduleCampaignCommand(Guid.Empty, DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CampaignId);
    }

    [Fact]
    public void Validate_WhenCampaignIdIsValid_ShouldNotHaveErrorForCampaignId()
    {
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CampaignId);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInFuture_ShouldNotHaveError()
    {
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInPast_ShouldHaveError()
    {
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), DateTime.UtcNow.AddHours(-1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveErrors()
    {
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
