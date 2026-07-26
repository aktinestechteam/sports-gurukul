using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateMembershipPlan;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class UpdateMembershipPlanCommandHandlerTests
{
    private readonly Mock<IAcademyMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateMembershipPlanCommandHandler>> _loggerMock;
    private readonly UpdateMembershipPlanCommandHandler _handler;

    public UpdateMembershipPlanCommandHandlerTests()
    {
        _membershipRepositoryMock = new Mock<IAcademyMembershipRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateMembershipPlanCommandHandler>>();
        _handler = new UpdateMembershipPlanCommandHandler(
            _membershipRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var membership = CreateMembership(membershipId, academyId, "Basic Plan", isDeleted: false);

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateMembershipPlanCommand
        {
            MembershipId = membershipId,
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 99.99m,
            Duration = 12
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MembershipName.Should().Be("Premium Plan");
        result.Value.Price.Should().Be(99.99m);
        result.Value.Duration.Should().Be(12);
        _membershipRepositoryMock.Verify(r => r.Update(It.IsAny<AcademyMembership>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PlanNotFound_ReturnsFailure()
    {
        var membershipId = Guid.NewGuid();
        var academyId = Guid.NewGuid();

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyMembership?)null);

        var command = new UpdateMembershipPlanCommand
        {
            MembershipId = membershipId,
            AcademyId = academyId,
            MembershipName = "Premium Plan"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_DeletedPlan_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var membership = CreateMembership(membershipId, academyId, "Basic Plan", isDeleted: true);

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);

        var command = new UpdateMembershipPlanCommand
        {
            MembershipId = membershipId,
            AcademyId = academyId,
            MembershipName = "Premium Plan"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deleted");
    }

    private static AcademyMembership CreateMembership(Guid id, Guid academyId, string name, bool isDeleted) => new()
    {
        Id = id,
        AcademyId = academyId,
        MembershipName = name,
        Price = 49.99m,
        Duration = 6,
        Status = AcademyMembershipStatus.Active,
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
