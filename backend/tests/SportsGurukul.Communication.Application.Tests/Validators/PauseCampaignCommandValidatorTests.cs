using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class PauseCampaignCommandValidatorTests
{
    private readonly PauseCampaignCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCampaignIdIsEmpty_ShouldHaveError()
    {
        var command = new PauseCampaignCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CampaignId);
    }

    [Fact]
    public void Validate_WhenCampaignIdIsValid_ShouldNotHaveErrors()
    {
        var command = new PauseCampaignCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
