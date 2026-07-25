using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteExperienceCommandHandlerTests
{
    private readonly Mock<IRepository<CoachExperience>> _experienceRepositoryMock = TestMocks.CreateCoachExperienceRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteExperienceCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteExperienceCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly DeleteExperienceCommandHandler _handler;

    public DeleteExperienceCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _handler = new DeleteExperienceCommandHandler(
            _experienceRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ExperienceNotFound_ReturnsFailure()
    {
        _experienceRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachExperience?)null);

        var result = await _handler.Handle(new DeleteExperienceCommand
        {
            ExperienceId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Experience not found.");
    }

    [Fact]
    public async Task Handle_ValidDelete_RemovesAndReturnsSuccess()
    {
        var experienceId = Guid.NewGuid();
        var experience = TestDataBuilder.CreateCoachExperience();
        experience.Id = experienceId;

        _experienceRepositoryMock.Setup(r => r.GetByIdAsync(experienceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(experience);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteExperienceCommand
        {
            ExperienceId = experienceId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _experienceRepositoryMock.Verify(r => r.Remove(experience), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
