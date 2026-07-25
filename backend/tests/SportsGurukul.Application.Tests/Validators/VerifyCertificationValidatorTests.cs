using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCertification;
using SportsGurukul.Application.Features.CoachManagement.Validators;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class VerifyCertificationValidatorTests
{
    private readonly VerifyCertificationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new VerifyCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            Status = VerificationStatus.Verified
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCertificationId_ShouldHaveValidationError()
    {
        var command = new VerifyCertificationCommand
        {
            CertificationId = Guid.Empty,
            Status = VerificationStatus.Verified
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CertificationId)
            .WithErrorMessage("Certification ID is required.");
    }

    [Fact]
    public async Task InvalidStatus_ShouldHaveValidationError()
    {
        var command = new VerifyCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            Status = (VerificationStatus)999
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("A valid verification status is required.");
    }
}
