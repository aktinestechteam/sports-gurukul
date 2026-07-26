using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateAcademy;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class UpdateAcademyCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateAcademyCommandHandler>> _loggerMock;
    private readonly UpdateAcademyCommandHandler _handler;

    public UpdateAcademyCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateAcademyCommandHandler>>();
        _handler = new UpdateAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademyWithDetails(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateAcademyCommand
        {
            AcademyId = academyId,
            Name = "Updated Academy"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Updated Academy");
        _academyRepositoryMock.Verify(r => r.Update(It.IsAny<Academy>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new UpdateAcademyCommand
        {
            AcademyId = academyId,
            Name = "Updated Academy"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_DeletedAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademyWithDetails(academyId);
        academy.IsDeleted = true;

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new UpdateAcademyCommand
        {
            AcademyId = academyId,
            Name = "Updated Academy"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy has been deleted.");
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesNonNullFields()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademyWithDetails(academyId);
        academy.Name = "Original Name";
        academy.Phone = "1111111111";

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateAcademyCommand
        {
            AcademyId = academyId,
            Name = "New Name"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        academy.Name.Should().Be("New Name");
        academy.Phone.Should().Be("1111111111");
    }

    private static Academy CreateTestAcademyWithDetails(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyCode = "ACAD-20260725-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Verification = new AcademyVerification
        {
            Id = Guid.NewGuid(),
            VerificationStatus = VerificationStatus.Verified,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        OperatingHours = new AcademyOperatingHours
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>()
    };
}
