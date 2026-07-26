using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyById;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Queries;

public class GetAcademyByIdQueryHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IRepository<AcademySocialLink>> _socialLinkRepositoryMock;
    private readonly Mock<ILogger<GetAcademyByIdQueryHandler>> _loggerMock;
    private readonly GetAcademyByIdQueryHandler _handler;

    public GetAcademyByIdQueryHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _socialLinkRepositoryMock = new Mock<IRepository<AcademySocialLink>>();
        _loggerMock = new Mock<ILogger<GetAcademyByIdQueryHandler>>();
        _handler = new GetAcademyByIdQueryHandler(
            _academyRepositoryMock.Object,
            _socialLinkRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AcademyExists_ReturnsAcademyDto()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateFullAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _socialLinkRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AcademySocialLink, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademySocialLink>());

        var query = new GetAcademyByIdQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Test Academy");
        result.Value.Email.Should().Be("test@test.com");
        result.Value.Phone.Should().Be("1234567890");
        result.Value.Contact.Should().NotBeNull();
        result.Value.Contact!.City.Should().Be("Mumbai");
        result.Value.OperatingHours.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var query = new GetAcademyByIdQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ReturnsCorrectBranchesAndFacilities()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateFullAcademy(academyId);

        academy.Branches = new List<AcademyBranch>
        {
            new() { Id = Guid.NewGuid(), AcademyId = academyId, BranchName = "Branch 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), AcademyId = academyId, BranchName = "Branch 2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        academy.Facilities = new List<AcademyFacility>
        {
            new() { Id = Guid.NewGuid(), AcademyId = academyId, FacilityName = "Court 1", FacilityType = AcademyFacilityType.Court, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), AcademyId = academyId, FacilityName = "Gym 1", FacilityType = AcademyFacilityType.Gym, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), AcademyId = academyId, FacilityName = "Pool 1", FacilityType = AcademyFacilityType.Pool, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _socialLinkRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AcademySocialLink, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademySocialLink>());

        var query = new GetAcademyByIdQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Branches.Should().HaveCount(2);
        result.Value.Facilities.Should().HaveCount(3);
        result.Value.Branches.Should().Contain(b => b.BranchName == "Branch 1");
        result.Value.Facilities.Should().Contain(f => f.FacilityName == "Court 1");
    }

    private static Academy CreateFullAcademy(Guid id) => new()
    {
        Id = id,
        AcademyCode = "ACAD-20260725-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            AcademyId = id,
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        OperatingHours = new AcademyOperatingHours
        {
            Id = Guid.NewGuid(),
            AcademyId = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        Verification = new AcademyVerification
        {
            Id = Guid.NewGuid(),
            AcademyId = id,
            VerificationStatus = VerificationStatus.Verified,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
