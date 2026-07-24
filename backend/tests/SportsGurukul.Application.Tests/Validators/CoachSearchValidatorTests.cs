using FluentAssertions;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteSavedCoachSearch;
using SportsGurukul.Application.Features.CoachManagement.Commands.RecordCoachRecentSearch;
using SportsGurukul.Application.Features.CoachManagement.Commands.SaveCoachSearch;

namespace SportsGurukul.Application.Tests.Validators;

public class CoachSearchValidatorTests
{
    [Fact]
    public void SaveCoachSearch_EmptyUserId_Fails()
    {
        var validator = new SaveCoachSearchCommandValidator();
        var result = validator.Validate(new SaveCoachSearchCommand { UserId = Guid.Empty, Name = "Test", FiltersJson = "{}" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void SaveCoachSearch_EmptyName_Fails()
    {
        var validator = new SaveCoachSearchCommandValidator();
        var result = validator.Validate(new SaveCoachSearchCommand { UserId = Guid.NewGuid(), Name = "", FiltersJson = "{}" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void SaveCoachSearch_ValidData_Passes()
    {
        var validator = new SaveCoachSearchCommandValidator();
        var result = validator.Validate(new SaveCoachSearchCommand
        {
            UserId = Guid.NewGuid(),
            Name = "My Search",
            FiltersJson = "{\"city\":\"Mumbai\"}"
        });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteSavedCoachSearch_EmptyId_Fails()
    {
        var validator = new DeleteSavedCoachSearchCommandValidator();
        var result = validator.Validate(new DeleteSavedCoachSearchCommand { Id = Guid.Empty, UserId = Guid.NewGuid() });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void DeleteSavedCoachSearch_EmptyUserId_Fails()
    {
        var validator = new DeleteSavedCoachSearchCommandValidator();
        var result = validator.Validate(new DeleteSavedCoachSearchCommand { Id = Guid.NewGuid(), UserId = Guid.Empty });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void DeleteSavedCoachSearch_ValidData_Passes()
    {
        var validator = new DeleteSavedCoachSearchCommandValidator();
        var result = validator.Validate(new DeleteSavedCoachSearchCommand { Id = Guid.NewGuid(), UserId = Guid.NewGuid() });
        result.IsValid.Should().BeTrue();
    }
}
