using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddExperience;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class AddExperienceCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IRepository<CoachExperience>> _coachExperienceRepositoryMock = TestMocks.CreateCoachExperienceRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AddExperienceCommandHandler>> _loggerMock = TestMocks.CreateLogger<AddExperienceCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly AddExperienceCommandHandler _handler;

    public AddExperienceCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new AddExperienceCommandHandler(
            _coachRepositoryMock.Object,
            _coachExperienceRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new AddExperienceCommand
        {
            CoachId = Guid.NewGuid(),
            Organization = "Test Org"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_ValidExperience_AddsAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var startDate = DateTime.UtcNow.AddYears(-3);
        var endDate = DateTime.UtcNow.AddYears(-1);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachExperienceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachExperience>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachExperience());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AddExperienceCommand
        {
            CoachId = coachId,
            Organization = "State Cricket Academy",
            Role = "Head Coach",
            Sport = "Cricket",
            StartDate = startDate,
            EndDate = endDate,
            Description = "Led the state team"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Organization.Should().Be("State Cricket Academy");
        result.Value.Role.Should().Be("Head Coach");
        result.Value.Sport.Should().Be("Cricket");
        result.Value.StartDate.Should().Be(startDate);
        result.Value.EndDate.Should().Be(endDate);
        result.Value.Description.Should().Be("Led the state team");
        _coachExperienceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachExperience>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
