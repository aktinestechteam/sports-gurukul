using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddEducation;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class AddEducationCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IRepository<CoachEducation>> _educationRepositoryMock = TestMocks.CreateCoachEducationRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AddEducationCommandHandler>> _loggerMock = TestMocks.CreateLogger<AddEducationCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly AddEducationCommandHandler _handler;

    public AddEducationCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new AddEducationCommandHandler(
            _coachRepositoryMock.Object,
            _educationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = "BPEd"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_ValidEducation_AddsAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _educationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachEducation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachEducation());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AddEducationCommand
        {
            CoachId = coachId,
            Degree = "MPEd",
            Institution = "NIS",
            FieldOfStudy = "Sports Science",
            YearCompleted = 2020
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Degree.Should().Be("MPEd");
        result.Value.Institution.Should().Be("NIS");
        result.Value.FieldOfStudy.Should().Be("Sports Science");
        result.Value.YearCompleted.Should().Be(2020);
        _educationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachEducation>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
