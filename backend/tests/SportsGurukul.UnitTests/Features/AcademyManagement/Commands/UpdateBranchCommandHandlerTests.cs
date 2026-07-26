using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateBranch;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class UpdateBranchCommandHandlerTests
{
    private readonly Mock<IAcademyBranchRepository> _branchRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateBranchCommandHandler>> _loggerMock;
    private readonly UpdateBranchCommandHandler _handler;

    public UpdateBranchCommandHandlerTests()
    {
        _branchRepositoryMock = new Mock<IAcademyBranchRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateBranchCommandHandler>>();
        _handler = new UpdateBranchCommandHandler(
            _branchRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, academyId, "Main Branch");

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateBranchCommand
        {
            BranchId = branchId,
            AcademyId = academyId,
            BranchName = "Updated Branch",
            City = "Mumbai"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.BranchName.Should().Be("Updated Branch");
        result.Value.City.Should().Be("Mumbai");
        _branchRepositoryMock.Verify(r => r.Update(It.IsAny<AcademyBranch>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BranchNotFound_ReturnsFailure()
    {
        var branchId = Guid.NewGuid();
        var academyId = Guid.NewGuid();

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyBranch?)null);

        var command = new UpdateBranchCommand
        {
            BranchId = branchId,
            AcademyId = academyId,
            BranchName = "Updated Branch"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_BranchBelongsToDifferentAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var otherAcademyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, otherAcademyId, "Main Branch");

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);

        var command = new UpdateBranchCommand
        {
            BranchId = branchId,
            AcademyId = academyId,
            BranchName = "Updated Branch"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("does not belong");
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, academyId, "Main Branch");

        var existingBranch = CreateBranch(Guid.NewGuid(), academyId, "New Name");

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _branchRepositoryMock
            .Setup(r => r.GetByAcademyIdAndNameAsync(academyId, "New Name", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBranch);

        var command = new UpdateBranchCommand
        {
            BranchId = branchId,
            AcademyId = academyId,
            BranchName = "New Name"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    private static AcademyBranch CreateBranch(Guid id, Guid academyId, string name) => new()
    {
        Id = id,
        AcademyId = academyId,
        BranchName = name,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
