using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateFacility;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class CreateFacilityCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IAcademyFacilityRepository> _academyFacilityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateFacilityCommandHandler>> _loggerMock;
    private readonly CreateFacilityCommandHandler _handler;

    public CreateFacilityCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _academyFacilityRepositoryMock = new Mock<IAcademyFacilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateFacilityCommandHandler>>();
        _handler = new CreateFacilityCommandHandler(
            _academyRepositoryMock.Object,
            _academyFacilityRepositoryMock.Object,
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
        _academyFacilityRepositoryMock
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
            Available = true,
            Description = "A well-maintained indoor court"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.FacilityName.Should().Be("Indoor Court");
        result.Value.AcademyId.Should().Be(academyId);
        _academyFacilityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new CreateFacilityCommand
        {
            AcademyId = academyId,
            FacilityName = "Indoor Court",
            FacilityType = AcademyFacilityType.Court
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
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
