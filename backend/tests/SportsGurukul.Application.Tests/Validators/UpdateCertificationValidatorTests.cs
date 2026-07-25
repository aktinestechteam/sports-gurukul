using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCertification;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateCertificationValidatorTests
{
    private readonly UpdateCertificationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            CertificationName = "Test Certification",
            IssuingAuthority = "Test Authority",
            CertificateNumber = "CERT-001",
            IssueDate = new DateTime(2023, 1, 1),
            ExpiryDate = new DateTime(2025, 12, 31)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCertificationId_ShouldHaveValidationError()
    {
        var command = new UpdateCertificationCommand
        {
            CertificationId = Guid.Empty,
            CertificationName = "Test Certification"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CertificationId)
            .WithErrorMessage("Certification ID is required.");
    }

    [Fact]
    public async Task CertificationNameExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            CertificationName = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CertificationName)
            .WithErrorMessage("Certification name must not exceed 200 characters.");
    }

    [Fact]
    public async Task ExpiryDateBeforeIssueDate_ShouldHaveValidationError()
    {
        var command = new UpdateCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            CertificationName = "Test Certification",
            IssueDate = new DateTime(2025, 12, 31),
            ExpiryDate = new DateTime(2023, 1, 1)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpiryDate)
            .WithErrorMessage("Expiry date must be after issue date.");
    }
}
