using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteMembershipPlan;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class DeleteMembershipPlanCommandHandlerTests
{
    private readonly Mock<IAcademyMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteMembershipPlanCommandHandler>> _loggerMock;
    private readonly DeleteMembershipPlanCommandHandler _handler;

    public DeleteMembershipPlanCommandHandlerTests()
    {
        _membershipRepositoryMock = new Mock<IAcademyMembershipRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteMembershipPlanCommandHandler>>();
        _handler = new DeleteMembershipPlanCommandHandler(
            _membershipRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesPlan()
    {
        var membershipId = Guid.NewGuid();
        var membership = CreateMembership(membershipId, isDeleted: false);

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteMembershipPlanCommand { MembershipId = membershipId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _membershipRepositoryMock.Verify(r => r.Remove(membership), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PlanNotFound_ReturnsFailure()
    {
        var membershipId = Guid.NewGuid();

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyMembership?)null);

        var command = new DeleteMembershipPlanCommand { MembershipId = membershipId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _membershipRepositoryMock.Verify(r => r.Remove(It.IsAny<AcademyMembership>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AlreadyDeleted_ReturnsFailure()
    {
        var membershipId = Guid.NewGuid();
        var membership = CreateMembership(membershipId, isDeleted: true);

        _membershipRepositoryMock
            .Setup(r => r.GetByIdAsync(membershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membership);

        var command = new DeleteMembershipPlanCommand { MembershipId = membershipId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already deleted");
        _membershipRepositoryMock.Verify(r => r.Remove(It.IsAny<AcademyMembership>()), Times.Never);
    }

    private static AcademyMembership CreateMembership(Guid id, bool isDeleted) => new()
    {
        Id = id,
        AcademyId = Guid.NewGuid(),
        MembershipName = "Test Plan",
        Price = 49.99m,
        Duration = 6,
        Status = AcademyMembershipStatus.Active,
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
