using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteEducationCommandHandlerTests
{
    private readonly Mock<IRepository<CoachEducation>> _educationRepositoryMock = TestMocks.CreateCoachEducationRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteEducationCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteEducationCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly DeleteEducationCommandHandler _handler;

    public DeleteEducationCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _handler = new DeleteEducationCommandHandler(
            _educationRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_EducationNotFound_ReturnsFailure()
    {
        _educationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachEducation?)null);

        var result = await _handler.Handle(new DeleteEducationCommand
        {
            EducationId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Education not found.");
    }

    [Fact]
    public async Task Handle_ValidDelete_RemovesAndReturnsSuccess()
    {
        var educationId = Guid.NewGuid();
        var education = TestDataBuilder.CreateCoachEducation();
        education.Id = educationId;

        _educationRepositoryMock.Setup(r => r.GetByIdAsync(educationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(education);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteEducationCommand
        {
            EducationId = educationId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _educationRepositoryMock.Verify(r => r.Remove(education), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
