using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddCertification;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class AddCertificationValidatorTests
{
    private readonly AddCertificationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
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
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.Empty,
            CertificationName = "Test Certification"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task EmptyCertificationName_ShouldHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = string.Empty
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CertificationName)
            .WithErrorMessage("Certification name is required.");
    }

    [Fact]
    public async Task CertificationNameExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CertificationName)
            .WithErrorMessage("Certification name must not exceed 200 characters.");
    }

    [Fact]
    public async Task IssuingAuthorityExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = "Test Certification",
            IssuingAuthority = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.IssuingAuthority)
            .WithErrorMessage("Issuing authority must not exceed 200 characters.");
    }

    [Fact]
    public async Task CertificateNumberExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = "Test Certification",
            CertificateNumber = new string('x', 101)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CertificateNumber)
            .WithErrorMessage("Certificate number must not exceed 100 characters.");
    }

    [Fact]
    public async Task ExpiryDateBeforeIssueDate_ShouldHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = "Test Certification",
            IssueDate = new DateTime(2025, 12, 31),
            ExpiryDate = new DateTime(2023, 1, 1)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpiryDate)
            .WithErrorMessage("Expiry date must be after issue date.");
    }

    [Fact]
    public async Task ExpiryDateWithoutIssueDate_ShouldNotHaveValidationError()
    {
        var command = new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = "Test Certification",
            IssueDate = null,
            ExpiryDate = new DateTime(2025, 12, 31)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ExpiryDate);
    }
}
