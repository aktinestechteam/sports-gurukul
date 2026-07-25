using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachProfile;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateCoachProfileCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateCoachProfileCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateCoachProfileCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly UpdateCoachProfileCommandHandler _handler;

    public UpdateCoachProfileCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new UpdateCoachProfileCommandHandler(
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new UpdateCoachProfileCommand { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_UpdateFields_UpdatesAndReturnsSuccess()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails(userId: _testUserId);
        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateCoachProfileCommand
        {
            CoachId = coach.Id,
            Biography = "Updated biography",
            YearsOfExperience = 12,
            CurrentOrganization = "Updated Academy",
            HighestQualification = "PhD",
            PreferredLanguage = "Tamil",
            CoachingLevel = CoachingLevel.National
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        coach.Biography.Should().Be("Updated biography");
        coach.YearsOfExperience.Should().Be(12);
        coach.CurrentOrganization.Should().Be("Updated Academy");
        coach.HighestQualification.Should().Be("PhD");
        coach.PreferredLanguage.Should().Be("Tamil");
        coach.CoachingLevel.Should().Be(CoachingLevel.National);
        coach.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _coachRepositoryMock.Verify(r => r.Update(coach), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFields_DoesNotOverwrite()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails(userId: _testUserId);
        var originalBiography = coach.Biography;
        var originalExperience = coach.YearsOfExperience;
        var originalOrganization = coach.CurrentOrganization;
        var originalQualification = coach.HighestQualification;
        var originalLanguage = coach.PreferredLanguage;
        var originalLevel = coach.CoachingLevel;

        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateCoachProfileCommand
        {
            CoachId = coach.Id,
            Biography = null,
            YearsOfExperience = null,
            CurrentOrganization = null,
            HighestQualification = null,
            PreferredLanguage = null,
            CoachingLevel = null
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        coach.Biography.Should().Be(originalBiography);
        coach.YearsOfExperience.Should().Be(originalExperience);
        coach.CurrentOrganization.Should().Be(originalOrganization);
        coach.HighestQualification.Should().Be(originalQualification);
        coach.PreferredLanguage.Should().Be(originalLanguage);
        coach.CoachingLevel.Should().Be(originalLevel);
    }
}
