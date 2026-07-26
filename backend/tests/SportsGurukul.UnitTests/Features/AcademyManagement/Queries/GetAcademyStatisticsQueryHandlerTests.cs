using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyStatistics;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Queries;

public class GetAcademyStatisticsQueryHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IAcademyBranchRepository> _branchRepositoryMock;
    private readonly Mock<IAcademyFacilityRepository> _facilityRepositoryMock;
    private readonly Mock<IAcademyMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<ICoachAcademyRepository> _coachAcademyRepositoryMock;
    private readonly Mock<IAthleteAcademyRepository> _athleteAcademyRepositoryMock;
    private readonly Mock<ILogger<GetAcademyStatisticsQueryHandler>> _loggerMock;
    private readonly GetAcademyStatisticsQueryHandler _handler;

    public GetAcademyStatisticsQueryHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _branchRepositoryMock = new Mock<IAcademyBranchRepository>();
        _facilityRepositoryMock = new Mock<IAcademyFacilityRepository>();
        _membershipRepositoryMock = new Mock<IAcademyMembershipRepository>();
        _coachAcademyRepositoryMock = new Mock<ICoachAcademyRepository>();
        _athleteAcademyRepositoryMock = new Mock<IAthleteAcademyRepository>();
        _loggerMock = new Mock<ILogger<GetAcademyStatisticsQueryHandler>>();
        _handler = new GetAcademyStatisticsQueryHandler(
            _academyRepositoryMock.Object,
            _branchRepositoryMock.Object,
            _facilityRepositoryMock.Object,
            _membershipRepositoryMock.Object,
            _coachAcademyRepositoryMock.Object,
            _athleteAcademyRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AcademyExists_ReturnsStatistics()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _branchRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyBranch>
            {
                new() { Id = Guid.NewGuid(), AcademyId = academyId },
                new() { Id = Guid.NewGuid(), AcademyId = academyId }
            });
        _facilityRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyFacility>
            {
                new() { Id = Guid.NewGuid(), AcademyId = academyId }
            });
        _membershipRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyMembership>
            {
                new() { Id = Guid.NewGuid(), AcademyId = academyId },
                new() { Id = Guid.NewGuid(), AcademyId = academyId },
                new() { Id = Guid.NewGuid(), AcademyId = academyId }
            });
        _coachAcademyRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAcademy>
            {
                new() { Id = Guid.NewGuid(), AcademyId = academyId, IsActive = true },
                new() { Id = Guid.NewGuid(), AcademyId = academyId, IsActive = true },
                new() { Id = Guid.NewGuid(), AcademyId = academyId, IsActive = false }
            });
        _athleteAcademyRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAcademy>
            {
                new() { Id = Guid.NewGuid(), AcademyId = academyId, IsActive = true }
            });
        _academyRepositoryMock
            .Setup(r => r.GetDocumentsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyDocument>
            {
                new() { Id = Guid.NewGuid(), AcademyId = academyId },
                new() { Id = Guid.NewGuid(), AcademyId = academyId }
            });
        _academyRepositoryMock
            .Setup(r => r.GetGalleryImagesAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyGallery>());

        var query = new GetAcademyStatisticsQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AcademyId.Should().Be(academyId);
        result.Value.AcademyName.Should().Be("Test Academy");
        result.Value.TotalCoaches.Should().Be(2);
        result.Value.TotalAthletes.Should().Be(1);
        result.Value.TotalBranches.Should().Be(2);
        result.Value.TotalFacilities.Should().Be(1);
        result.Value.ActiveMemberships.Should().Be(3);
        result.Value.TotalDocuments.Should().Be(2);
        result.Value.TotalGalleryImages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var query = new GetAcademyStatisticsQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_EmptyAcademy_ReturnsZeroCounts()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _branchRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyBranch>());
        _facilityRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyFacility>());
        _membershipRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyMembership>());
        _coachAcademyRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAcademy>());
        _athleteAcademyRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAcademy>());
        _academyRepositoryMock
            .Setup(r => r.GetDocumentsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyDocument>());
        _academyRepositoryMock
            .Setup(r => r.GetGalleryImagesAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyGallery>());

        var query = new GetAcademyStatisticsQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCoaches.Should().Be(0);
        result.Value.TotalAthletes.Should().Be(0);
        result.Value.TotalBranches.Should().Be(0);
        result.Value.TotalFacilities.Should().Be(0);
        result.Value.ActiveMemberships.Should().Be(0);
        result.Value.SportsOffered.Should().Be(0);
        result.Value.TotalDocuments.Should().Be(0);
        result.Value.TotalGalleryImages.Should().Be(0);
    }

    private static Academy CreateAcademy(Guid id) => new()
    {
        Id = id,
        AcademyCode = "ACAD-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        AcademySports = new List<AcademySport>(),
        Branches = new List<AcademyBranch>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
