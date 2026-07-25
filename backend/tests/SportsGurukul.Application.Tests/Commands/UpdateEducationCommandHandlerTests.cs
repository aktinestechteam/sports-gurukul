using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateEducation;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateEducationCommandHandlerTests
{
    private readonly Mock<IRepository<CoachEducation>> _educationRepositoryMock = TestMocks.CreateCoachEducationRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateEducationCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateEducationCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly UpdateEducationCommandHandler _handler;

    public UpdateEducationCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _handler = new UpdateEducationCommandHandler(
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

        var result = await _handler.Handle(new UpdateEducationCommand
        {
            EducationId = Guid.NewGuid(),
            Degree = "MPEd"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Education not found.");
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesAndReturnsSuccess()
    {
        var educationId = Guid.NewGuid();
        var education = TestDataBuilder.CreateCoachEducation();
        education.Id = educationId;
        education.Degree = "BPEd";
        education.Institution = "NIS";

        _educationRepositoryMock.Setup(r => r.GetByIdAsync(educationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(education);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateEducationCommand
        {
            EducationId = educationId,
            Degree = "MPEd",
            Institution = "LNU",
            FieldOfStudy = "Sports Psychology",
            YearCompleted = 2022
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Degree.Should().Be("MPEd");
        result.Value.Institution.Should().Be("LNU");
        result.Value.FieldOfStudy.Should().Be("Sports Psychology");
        result.Value.YearCompleted.Should().Be(2022);
        _educationRepositoryMock.Verify(r => r.Update(education), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFieldsDoNotOverwriteExistingValues_ReturnsSuccess()
    {
        var educationId = Guid.NewGuid();
        var education = TestDataBuilder.CreateCoachEducation();
        education.Id = educationId;
        education.Degree = "Original Degree";
        education.Institution = "Original Institution";
        education.FieldOfStudy = "Original Field";

        _educationRepositoryMock.Setup(r => r.GetByIdAsync(educationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(education);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateEducationCommand
        {
            EducationId = educationId,
            Degree = null,
            Institution = null,
            FieldOfStudy = null,
            YearCompleted = null
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Degree.Should().Be("Original Degree");
        result.Value.Institution.Should().Be("Original Institution");
        result.Value.FieldOfStudy.Should().Be("Original Field");
    }
}
