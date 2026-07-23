using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Behaviors;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;

namespace SportsGurukul.Application.Tests.Services;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<DeleteAthleteCommand, Unit>(
            Enumerable.Empty<IValidator<DeleteAthleteCommand>>());

        var request = new DeleteAthleteCommand { AthleteId = Guid.NewGuid() };
        var nextCalled = false;
        RequestHandlerDelegate<Unit> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(Unit.Value);
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_ValidValidation_CallsNext()
    {
        var validator = new Mock<IValidator<DeleteAthleteCommand>>();
        validator.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<DeleteAthleteCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<DeleteAthleteCommand, Unit>(
            new[] { validator.Object });

        var request = new DeleteAthleteCommand { AthleteId = Guid.NewGuid() };
        RequestHandlerDelegate<Unit> next = () => Task.FromResult(Unit.Value);

        var result = await behavior.Handle(request, next, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_InvalidValidation_ThrowsValidationException()
    {
        var failure = new ValidationFailure("AthleteId", "Athlete ID is required.");
        var validator = new Mock<IValidator<DeleteAthleteCommand>>();
        validator.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<DeleteAthleteCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure }));

        var behavior = new ValidationBehavior<DeleteAthleteCommand, Unit>(
            new[] { validator.Object });

        var request = new DeleteAthleteCommand { AthleteId = Guid.NewGuid() };
        RequestHandlerDelegate<Unit> next = () => Task.FromResult(Unit.Value);

        var act = () => behavior.Handle(request, next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_MultipleValidators_CollectsAllFailures()
    {
        var failure1 = new ValidationFailure("AthleteId", "Required");
        var failure2 = new ValidationFailure("AthleteId", "Invalid");

        var validator1 = new Mock<IValidator<DeleteAthleteCommand>>();
        validator1.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<DeleteAthleteCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure1 }));

        var validator2 = new Mock<IValidator<DeleteAthleteCommand>>();
        validator2.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<DeleteAthleteCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure2 }));

        var behavior = new ValidationBehavior<DeleteAthleteCommand, Unit>(
            new[] { validator1.Object, validator2.Object });

        var request = new DeleteAthleteCommand { AthleteId = Guid.NewGuid() };
        RequestHandlerDelegate<Unit> next = () => Task.FromResult(Unit.Value);

        var act = () => behavior.Handle(request, next, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_NoFailures_DoesNotThrow()
    {
        var validator = new Mock<IValidator<DeleteAthleteCommand>>();
        validator.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<DeleteAthleteCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<DeleteAthleteCommand, Unit>(
            new[] { validator.Object });

        var act = () => behavior.Handle(
            new DeleteAthleteCommand { AthleteId = Guid.NewGuid() },
            () => Task.FromResult(Unit.Value),
            CancellationToken.None);

        await act.Should().NotThrowAsync();
    }
}
