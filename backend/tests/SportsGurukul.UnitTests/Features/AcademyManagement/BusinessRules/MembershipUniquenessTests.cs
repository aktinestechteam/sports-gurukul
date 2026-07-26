using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.ActivateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeactivateMembershipPlan;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class MembershipUniquenessTests
{
    private readonly Mock<IAcademyRepository> _academyRepoMock;
    private readonly Mock<IAcademyMembershipRepository> _membershipRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateMembershipPlanCommandHandler>> _createLoggerMock;
    private readonly Mock<ILogger<ActivateMembershipPlanCommandHandler>> _activateLoggerMock;
    private readonly Mock<ILogger<DeactivateMembershipPlanCommandHandler>> _deactivateLoggerMock;
    private readonly CreateMembershipPlanCommandHandler _createHandler;
    private readonly ActivateMembershipPlanCommandHandler _activateHandler;
    private readonly DeactivateMembershipPlanCommandHandler _deactivateHandler;

    public MembershipUniquenessTests()
    {
        _academyRepoMock = new Mock<IAcademyRepository>();
        _membershipRepoMock = new Mock<IAcademyMembershipRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _createLoggerMock = new Mock<ILogger<CreateMembershipPlanCommandHandler>>();
        _activateLoggerMock = new Mock<ILogger<ActivateMembershipPlanCommandHandler>>();
        _deactivateLoggerMock = new Mock<ILogger<DeactivateMembershipPlanCommandHandler>>();

        _createHandler = new CreateMembershipPlanCommandHandler(
            _academyRepoMock.Object,
            _membershipRepoMock.Object,
            _unitOfWorkMock.Object,
            _createLoggerMock.Object);

        _activateHandler = new ActivateMembershipPlanCommandHandler(
            _membershipRepoMock.Object,
            _unitOfWorkMock.Object,
            _activateLoggerMock.Object);

        _deactivateHandler = new DeactivateMembershipPlanCommandHandler(
            _membershipRepoMock.Object,
            _unitOfWorkMock.Object,
            _deactivateLoggerMock.Object);
    }

    [Fact]
    public async Task MembershipName_UniqueWithinAcademy()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _membershipRepoMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyMembership>());
        _membershipRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyMembership m, CancellationToken _) => m);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 100,
            Duration = 30
        };

        var result = await _createHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MembershipName.Should().Be("Premium Plan");
        _membershipRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MembershipName_DuplicateWithinAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var existingPlans = new List<AcademyMembership>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AcademyId = academyId,
                MembershipName = "Premium Plan",
                Status = AcademyMembershipStatus.Active
            }
        };

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _membershipRepoMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlans);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 100,
            Duration = 30
        };

        var result = await _createHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("same name");
        _membershipRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MembershipName_SameNameDifferentAcademy_Allowed()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _membershipRepoMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyMembership>());
        _membershipRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyMembership m, CancellationToken _) => m);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 100,
            Duration = 30
        };

        var result = await _createHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _membershipRepoMock.Verify(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivatePlan_AlreadyActive_NoEffect()
    {
        var membershipId = Guid.NewGuid();
        var membership = CreateTestMembership(membershipId);
        membership.Status = AcademyMembershipStatus.Active;

        _membershipRepoMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _activateHandler.Handle(new ActivateMembershipPlanCommand
        {
            MembershipId = membershipId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        membership.Status.Should().Be(AcademyMembershipStatus.Active);
        _membershipRepoMock.Verify(r => r.Update(It.IsAny<AcademyMembership>()), Times.Once);
    }

    [Fact]
    public async Task DeactivatePlan_AlreadyInactive_NoEffect()
    {
        var membershipId = Guid.NewGuid();
        var membership = CreateTestMembership(membershipId);
        membership.Status = AcademyMembershipStatus.Inactive;

        _membershipRepoMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _deactivateHandler.Handle(new DeactivateMembershipPlanCommand
        {
            MembershipId = membershipId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        membership.Status.Should().Be(AcademyMembershipStatus.Inactive);
        _membershipRepoMock.Verify(r => r.Update(It.IsAny<AcademyMembership>()), Times.Once);
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

    private static AcademyMembership CreateTestMembership(Guid? id = null, Guid? academyId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyId = academyId ?? Guid.NewGuid(),
        MembershipName = "Premium Plan",
        Description = "Premium membership plan",
        Price = 100,
        Duration = 30,
        Benefits = "All access",
        Status = AcademyMembershipStatus.Active,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
