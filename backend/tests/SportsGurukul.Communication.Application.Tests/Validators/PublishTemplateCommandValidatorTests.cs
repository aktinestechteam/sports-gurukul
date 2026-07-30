using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class PublishTemplateCommandValidatorTests
{
    private readonly PublishTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError()
    {
        var command = new PublishTemplateCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveErrors()
    {
        var command = new PublishTemplateCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
