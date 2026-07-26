using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateFacility;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class FacilityUniquenessTests
{
    private readonly Mock<IAcademyRepository> _academyRepoMock;
    private readonly Mock<IAcademyFacilityRepository> _facilityRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateFacilityCommandHandler>> _loggerMock;
    private readonly CreateFacilityCommandHandler _handler;

    public FacilityUniquenessTests()
    {
        _academyRepoMock = new Mock<IAcademyRepository>();
        _facilityRepoMock = new Mock<IAcademyFacilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateFacilityCommandHandler>>();
        _handler = new CreateFacilityCommandHandler(
            _academyRepoMock.Object,
            _facilityRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task FacilityName_UniqueWithinAcademy()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _facilityRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyFacility f, CancellationToken _) => f);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateFacilityCommand
        {
            AcademyId = academyId,
            FacilityName = "Indoor Court",
            FacilityType = AcademyFacilityType.Court,
            IndoorOutdoor = "Indoor",
            Capacity = 50,
            Available = true
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.FacilityName.Should().Be("Indoor Court");
        _facilityRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FacilityName_DuplicateWithinAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var existingFacilities = new List<AcademyFacility>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AcademyId = academyId,
                FacilityName = "Indoor Court",
                FacilityType = AcademyFacilityType.Court
            }
        };

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _facilityRepoMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFacilities);

        var command = new CreateFacilityCommand
        {
            AcademyId = academyId,
            FacilityName = "Indoor Court",
            FacilityType = AcademyFacilityType.Court
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _facilityRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FacilityName_SameNameDifferentAcademy_Allowed()
    {
        var academyId1 = Guid.NewGuid();
        var academyId2 = Guid.NewGuid();
        var academy1 = CreateTestAcademy(academyId1);
        var academy2 = CreateTestAcademy(academyId2);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy2);
        _facilityRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyFacility f, CancellationToken _) => f);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateFacilityCommand
        {
            AcademyId = academyId2,
            FacilityName = "Indoor Court",
            FacilityType = AcademyFacilityType.Court
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _facilityRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()), Times.Once);
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
