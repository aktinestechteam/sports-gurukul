using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class SoftDeleteTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteAcademyCommandHandler>> _deleteLoggerMock;
    private readonly Mock<ILogger<RestoreAcademyCommandHandler>> _restoreLoggerMock;
    private readonly DeleteAcademyCommandHandler _deleteHandler;
    private readonly RestoreAcademyCommandHandler _restoreHandler;

    public SoftDeleteTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _deleteLoggerMock = new Mock<ILogger<DeleteAcademyCommandHandler>>();
        _restoreLoggerMock = new Mock<ILogger<RestoreAcademyCommandHandler>>();
        _deleteHandler = new DeleteAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _deleteLoggerMock.Object);
        _restoreHandler = new RestoreAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _restoreLoggerMock.Object);
    }

    [Fact]
    public async Task SoftDelete_SetsIsDeletedTrue()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _deleteHandler.Handle(new DeleteAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDelete_SetsStatusToInactive()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _deleteHandler.Handle(new DeleteAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.Status.Should().Be(AcademyStatus.Inactive);
    }

    [Fact]
    public async Task SoftDelete_PreservesOriginalData()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var originalName = academy.Name;
        var originalEmail = academy.Email;
        var originalPhone = academy.Phone;
        var originalCode = academy.AcademyCode;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _deleteHandler.Handle(new DeleteAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.Name.Should().Be(originalName);
        academy.Email.Should().Be(originalEmail);
        academy.Phone.Should().Be(originalPhone);
        academy.AcademyCode.Should().Be(originalCode);
    }

    [Fact]
    public async Task Restore_SetsIsDeletedFalse()
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

        await _restoreHandler.Handle(new RestoreAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task Restore_SetsStatusToActive()
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

        await _restoreHandler.Handle(new RestoreAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.Status.Should().Be(AcademyStatus.Active);
    }

    [Fact]
    public async Task Restore_PreservesOriginalData()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.IsDeleted = true;
        academy.Status = AcademyStatus.Inactive;
        var originalName = academy.Name;
        var originalEmail = academy.Email;
        var originalPhone = academy.Phone;
        var originalCode = academy.AcademyCode;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _restoreHandler.Handle(new RestoreAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.Name.Should().Be(originalName);
        academy.Email.Should().Be(originalEmail);
        academy.Phone.Should().Be(originalPhone);
        academy.AcademyCode.Should().Be(originalCode);
    }

    [Fact]
    public async Task Delete_ThenRestore_ReturnsToOriginalState()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var originalName = academy.Name;
        var originalEmail = academy.Email;
        var originalPhone = academy.Phone;
        var originalCode = academy.AcademyCode;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _deleteHandler.Handle(new DeleteAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.IsDeleted.Should().BeTrue();
        academy.Status.Should().Be(AcademyStatus.Inactive);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _restoreHandler.Handle(new RestoreAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.IsDeleted.Should().BeFalse();
        academy.Status.Should().Be(AcademyStatus.Active);
        academy.Name.Should().Be(originalName);
        academy.Email.Should().Be(originalEmail);
        academy.Phone.Should().Be(originalPhone);
        academy.AcademyCode.Should().Be(originalCode);
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
