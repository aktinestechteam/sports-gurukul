using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeactivateMembershipPlan;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class DeactivateMembershipPlanCommandHandlerTests
{
    private readonly Mock<IAcademyMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeactivateMembershipPlanCommandHandler>> _loggerMock;
    private readonly DeactivateMembershipPlanCommandHandler _handler;

    public DeactivateMembershipPlanCommandHandlerTests()
    {
        _membershipRepositoryMock = new Mock<IAcademyMembershipRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeactivateMembershipPlanCommandHandler>>();
        _handler = new DeactivateMembershipPlanCommandHandler(
            _membershipRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeactivatesPlan()
    {
        var membershipId = Guid.NewGuid();
        var membership = CreateTestMembership(membershipId);
        membership.Status = AcademyMembershipStatus.Active;

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeactivateMembershipPlanCommand { MembershipId = membershipId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        membership.Status.Should().Be(AcademyMembershipStatus.Inactive);
        _membershipRepositoryMock.Verify(r => r.Update(It.IsAny<AcademyMembership>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PlanNotFound_ReturnsFailure()
    {
        var membershipId = Guid.NewGuid();

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyMembership?)null);

        var command = new DeactivateMembershipPlanCommand { MembershipId = membershipId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Membership plan not found.");
    }

    [Fact]
    public async Task Handle_DeletedPlan_ReturnsFailure()
    {
        var membershipId = Guid.NewGuid();
        var membership = CreateTestMembership(membershipId);
        membership.IsDeleted = true;

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);

        var command = new DeactivateMembershipPlanCommand { MembershipId = membershipId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Membership plan is deleted.");
    }

    private static AcademyMembership CreateTestMembership(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyId = Guid.NewGuid(),
        MembershipName = "Premium Plan",
        Description = "Premium membership plan",
        Price = 999.99m,
        Duration = 30,
        Benefits = "Access to all facilities",
        Status = AcademyMembershipStatus.Active,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
