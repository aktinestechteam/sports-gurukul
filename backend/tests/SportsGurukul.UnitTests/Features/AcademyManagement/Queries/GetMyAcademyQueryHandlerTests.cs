using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetMyAcademy;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Queries;

public class GetMyAcademyQueryHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<ILogger<GetMyAcademyQueryHandler>> _loggerMock;
    private readonly GetMyAcademyQueryHandler _handler;

    public GetMyAcademyQueryHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _loggerMock = new Mock<ILogger<GetMyAcademyQueryHandler>>();
        _handler = new GetMyAcademyQueryHandler(
            _academyRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserOwnsAcademy_ReturnsAcademyDto()
    {
        var userId = Guid.NewGuid();
        var academy = CreateAcademy(userId);

        _academyRepositoryMock
            .Setup(r => r.GetByOwnerUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var result = await _handler.Handle(
            new GetMyAcademyQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("My Test Academy");
        result.Value.AcademyCode.Should().Be("ACAD-20260725-MINE");
        result.Value.LogoUrl.Should().Be("https://example.com/logo.png");
        result.Value.Email.Should().Be("academy@test.com");
        result.Value.Contact.Should().NotBeNull();
        result.Value.Contact!.PrimaryContactName.Should().Be("Aarav Sharma");
        result.Value.Contact.City.Should().Be("Mumbai");
        result.Value.Contact.Country.Should().Be("India");
    }

    [Fact]
    public async Task Handle_UserOwnsAcademy_ReturnsBranchesAndSports()
    {
        var userId = Guid.NewGuid();
        var academy = CreateAcademy(userId);

        var branchId = Guid.NewGuid();
        academy.Branches = new List<AcademyBranch>
        {
            new() { Id = branchId, AcademyId = academy.Id, BranchName = "Main Branch", Address = "MG Road", Country = "India", State = "Maharashtra", City = "Pune", PostalCode = "411001", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var sportId = Guid.NewGuid();
        academy.AcademySports = new List<AcademySport>
        {
            new() { Id = Guid.NewGuid(), AcademyId = academy.Id, SportId = sportId, IsPrimarySport = true, Sport = new Sport { Id = sportId, Name = "Cricket" }, JoinedDate = DateTime.UtcNow }
        };

        _academyRepositoryMock
            .Setup(r => r.GetByOwnerUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var result = await _handler.Handle(
            new GetMyAcademyQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Branches.Should().HaveCount(1);
        result.Value.Branches[0].BranchName.Should().Be("Main Branch");
        result.Value.Branches[0].City.Should().Be("Pune");
        result.Value.Sports.Should().HaveCount(1);
        result.Value.Sports[0].Name.Should().Be("Cricket");
        result.Value.Sports[0].IsPrimarySport.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserOwnsNoAcademy_ReturnsFailure()
    {
        var userId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByOwnerUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var result = await _handler.Handle(
            new GetMyAcademyQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    private static Academy CreateAcademy(Guid userId) => new()
    {
        Id = Guid.NewGuid(),
        AcademyCode = "ACAD-20260725-MINE",
        Name = "My Test Academy",
        Email = "academy@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        OwnedByUserId = userId,
        LogoUrl = "https://example.com/logo.png",
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            PrimaryContactName = "Aarav Sharma",
            Address = "MG Road",
            Country = "India",
            State = "Maharashtra",
            City = "Mumbai",
            PostalCode = "400001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
