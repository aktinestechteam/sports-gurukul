using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class RestoreAcademyCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RestoreAcademyCommandHandler>> _loggerMock;
    private readonly RestoreAcademyCommandHandler _handler;

    public RestoreAcademyCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RestoreAcademyCommandHandler>>();
        _handler = new RestoreAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_RestoresAcademy()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.IsDeleted = true;
        academy.Status = AcademyStatus.Inactive;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RestoreAcademyCommand { AcademyId = academyId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        academy.IsDeleted.Should().BeFalse();
        academy.Status.Should().Be(AcademyStatus.Active);
        _academyRepositoryMock.Verify(r => r.Update(It.IsAny<Academy>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new RestoreAcademyCommand { AcademyId = academyId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_NotDeleted_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.IsDeleted = false;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new RestoreAcademyCommand { AcademyId = academyId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy is not deleted.");
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
