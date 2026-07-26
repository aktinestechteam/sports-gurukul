using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateBranch;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateFacility;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class UniqueConstraintTests
{
    private readonly Mock<IAcademyRepository> _academyRepoMock;
    private readonly Mock<IAcademyBranchRepository> _branchRepoMock;
    private readonly Mock<IAcademyFacilityRepository> _facilityRepoMock;
    private readonly Mock<IAcademyMembershipRepository> _membershipRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateAcademyCommandHandler>> _createAcademyLoggerMock;
    private readonly Mock<ILogger<CreateBranchCommandHandler>> _createBranchLoggerMock;
    private readonly Mock<ILogger<CreateFacilityCommandHandler>> _createFacilityLoggerMock;
    private readonly Mock<ILogger<CreateMembershipPlanCommandHandler>> _createMembershipLoggerMock;
    private readonly CreateAcademyCommandHandler _createAcademyHandler;
    private readonly CreateBranchCommandHandler _createBranchHandler;
    private readonly CreateFacilityCommandHandler _createFacilityHandler;
    private readonly CreateMembershipPlanCommandHandler _createMembershipHandler;

    public UniqueConstraintTests()
    {
        _academyRepoMock = new Mock<IAcademyRepository>();
        _branchRepoMock = new Mock<IAcademyBranchRepository>();
        _facilityRepoMock = new Mock<IAcademyFacilityRepository>();
        _membershipRepoMock = new Mock<IAcademyMembershipRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _createAcademyLoggerMock = new Mock<ILogger<CreateAcademyCommandHandler>>();
        _createBranchLoggerMock = new Mock<ILogger<CreateBranchCommandHandler>>();
        _createFacilityLoggerMock = new Mock<ILogger<CreateFacilityCommandHandler>>();
        _createMembershipLoggerMock = new Mock<ILogger<CreateMembershipPlanCommandHandler>>();

        _createAcademyHandler = new CreateAcademyCommandHandler(
            _academyRepoMock.Object,
            _unitOfWorkMock.Object,
            _createAcademyLoggerMock.Object);

        _createBranchHandler = new CreateBranchCommandHandler(
            _academyRepoMock.Object,
            _branchRepoMock.Object,
            _unitOfWorkMock.Object,
            _createBranchLoggerMock.Object);

        _createFacilityHandler = new CreateFacilityCommandHandler(
            _academyRepoMock.Object,
            _facilityRepoMock.Object,
            _unitOfWorkMock.Object,
            _createFacilityLoggerMock.Object);

        _createMembershipHandler = new CreateMembershipPlanCommandHandler(
            _academyRepoMock.Object,
            _membershipRepoMock.Object,
            _unitOfWorkMock.Object,
            _createMembershipLoggerMock.Object);
    }

    [Fact]
    public async Task DuplicateAcademyName_ReturnsError()
    {
        var existingAcademy = CreateTestAcademy();
        existingAcademy.Name = "Elite Academy";

        _academyRepoMock
            .Setup(r => r.GetByEmailAsync("duplicate@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAcademy);

        var command = new CreateAcademyCommand
        {
            Name = "Elite Academy",
            Email = "duplicate@test.com",
            Phone = "1234567890"
        };

        var result = await _createAcademyHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("email");
        _academyRepoMock.Verify(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DuplicateAcademyCode_ReturnsError()
    {
        var existingAcademy = CreateTestAcademy();

        _academyRepoMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);
        _academyRepoMock
            .Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Academy, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateAcademyCommand
        {
            Name = "New Academy",
            Email = "new@test.com",
            Phone = "1234567890"
        };

        var result = await _createAcademyHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DuplicateEmail_ReturnsError()
    {
        var existingAcademy = CreateTestAcademy();

        _academyRepoMock
            .Setup(r => r.GetByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAcademy);

        var command = new CreateAcademyCommand
        {
            Name = "Another Academy",
            Email = "existing@test.com",
            Phone = "9876543210"
        };

        var result = await _createAcademyHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("email");
        _academyRepoMock.Verify(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DuplicateBranchName_ReturnsError()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var existingBranch = new AcademyBranch
        {
            Id = Guid.NewGuid(),
            AcademyId = academyId,
            BranchName = "Main Branch"
        };

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _branchRepoMock
            .Setup(r => r.GetByAcademyIdAndNameAsync(academyId, "Main Branch", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBranch);

        var command = new CreateBranchCommand
        {
            AcademyId = academyId,
            BranchName = "Main Branch"
        };

        var result = await _createBranchHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
        _branchRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyBranch>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DuplicateFacilityName_ReturnsError()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new CreateFacilityCommand
        {
            AcademyId = academyId,
            FacilityName = "Indoor Court",
            FacilityType = AcademyFacilityType.Court
        };

        var result = await _createFacilityHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _facilityRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyFacility>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DuplicateMembershipPlanName_ReturnsError()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var existingPlan = new AcademyMembership
        {
            Id = Guid.NewGuid(),
            AcademyId = academyId,
            MembershipName = "Premium Plan"
        };

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _membershipRepoMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyMembership> { existingPlan });

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 100,
            Duration = 30
        };

        var result = await _createMembershipHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("same name");
        _membershipRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()), Times.Never);
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
