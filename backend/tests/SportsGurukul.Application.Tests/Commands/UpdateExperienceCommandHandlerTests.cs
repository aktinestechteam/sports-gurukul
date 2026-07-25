using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateExperience;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateExperienceCommandHandlerTests
{
    private readonly Mock<IRepository<CoachExperience>> _coachExperienceRepositoryMock = TestMocks.CreateCoachExperienceRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateExperienceCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateExperienceCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly UpdateExperienceCommandHandler _handler;

    public UpdateExperienceCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _handler = new UpdateExperienceCommandHandler(
            _coachExperienceRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ExperienceNotFound_ReturnsFailure()
    {
        _coachExperienceRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachExperience?)null);

        var result = await _handler.Handle(new UpdateExperienceCommand
        {
            ExperienceId = Guid.NewGuid(),
            Organization = "Updated Org"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Experience not found.");
    }

    [Fact]
    public async Task Handle_ExperienceIsDeleted_ReturnsFailure()
    {
        var experienceId = Guid.NewGuid();
        var experience = TestDataBuilder.CreateCoachExperience();
        experience.Id = experienceId;
        experience.IsDeleted = true;

        _coachExperienceRepositoryMock.Setup(r => r.GetByIdAsync(experienceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(experience);

        var result = await _handler.Handle(new UpdateExperienceCommand
        {
            ExperienceId = experienceId,
            Organization = "Updated Org"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Experience not found.");
    }

    [Fact]
    public async Task Handle_ValidUpdateWithAllFields_UpdatesAndReturnsSuccess()
    {
        var experienceId = Guid.NewGuid();
        var experience = TestDataBuilder.CreateCoachExperience();
        experience.Id = experienceId;
        experience.Organization = "Old Org";
        experience.Role = "Old Role";

        _coachExperienceRepositoryMock.Setup(r => r.GetByIdAsync(experienceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(experience);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var newStartDate = DateTime.UtcNow.AddYears(-5);
        var newEndDate = DateTime.UtcNow;
        var result = await _handler.Handle(new UpdateExperienceCommand
        {
            ExperienceId = experienceId,
            Organization = "New Org",
            Role = "New Role",
            Sport = "Football",
            StartDate = newStartDate,
            EndDate = newEndDate,
            Description = "Updated description"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Organization.Should().Be("New Org");
        result.Value.Role.Should().Be("New Role");
        result.Value.Sport.Should().Be("Football");
        result.Value.StartDate.Should().Be(newStartDate);
        result.Value.EndDate.Should().Be(newEndDate);
        result.Value.Description.Should().Be("Updated description");
        _coachExperienceRepositoryMock.Verify(r => r.Update(experience), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFieldsDoNotOverwriteExistingValues_ReturnsSuccess()
    {
        var experienceId = Guid.NewGuid();
        var experience = TestDataBuilder.CreateCoachExperience();
        experience.Id = experienceId;
        experience.Organization = "Original Org";
        experience.Role = "Original Role";
        experience.Sport = "Cricket";

        _coachExperienceRepositoryMock.Setup(r => r.GetByIdAsync(experienceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(experience);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateExperienceCommand
        {
            ExperienceId = experienceId,
            Organization = null,
            Role = null,
            Sport = null,
            StartDate = null,
            EndDate = null,
            Description = null
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Organization.Should().Be("Original Org");
        result.Value.Role.Should().Be("Original Role");
        result.Value.Sport.Should().Be("Cricket");
    }
}
