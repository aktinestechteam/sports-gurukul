using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class ResumeCampaignCommandValidatorTests
{
    private readonly ResumeCampaignCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCampaignIdIsEmpty_ShouldHaveError()
    {
        var command = new ResumeCampaignCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CampaignId);
    }

    [Fact]
    public void Validate_WhenCampaignIdIsValid_ShouldNotHaveErrors()
    {
        var command = new ResumeCampaignCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
