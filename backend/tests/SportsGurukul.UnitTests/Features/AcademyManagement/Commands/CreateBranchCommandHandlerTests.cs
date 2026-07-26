using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateBranch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class CreateBranchCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IAcademyBranchRepository> _academyBranchRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateBranchCommandHandler>> _loggerMock;
    private readonly CreateBranchCommandHandler _handler;

    public CreateBranchCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _academyBranchRepositoryMock = new Mock<IAcademyBranchRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateBranchCommandHandler>>();
        _handler = new CreateBranchCommandHandler(
            _academyRepositoryMock.Object,
            _academyBranchRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _academyBranchRepositoryMock
            .Setup(r => r.GetByAcademyIdAndNameAsync(academyId, "Main Branch", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyBranch?)null);
        _academyBranchRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AcademyBranch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyBranch b, CancellationToken _) => b);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateBranchCommand
        {
            AcademyId = academyId,
            BranchName = "Main Branch",
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BranchName.Should().Be("Main Branch");
        result.Value.City.Should().Be("Mumbai");
        result.Value.AcademyId.Should().Be(academyId);
        _academyBranchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AcademyBranch>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new CreateBranchCommand
        {
            AcademyId = academyId,
            BranchName = "Main Branch"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_DuplicateBranchName_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var existingBranch = new AcademyBranch
        {
            Id = Guid.NewGuid(),
            AcademyId = academyId,
            BranchName = "Main Branch"
        };

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _academyBranchRepositoryMock
            .Setup(r => r.GetByAcademyIdAndNameAsync(academyId, "Main Branch", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBranch);

        var command = new CreateBranchCommand
        {
            AcademyId = academyId,
            BranchName = "Main Branch"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Main Branch");
    }

    private static Academy CreateTestAcademy(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyCode = "ACAD-20260725-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>()
    };
}
