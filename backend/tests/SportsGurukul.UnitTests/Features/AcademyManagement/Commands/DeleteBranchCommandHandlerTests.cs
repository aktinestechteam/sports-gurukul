using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteBranch;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class DeleteBranchCommandHandlerTests
{
    private readonly Mock<IAcademyBranchRepository> _branchRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteBranchCommandHandler>> _loggerMock;
    private readonly DeleteBranchCommandHandler _handler;

    public DeleteBranchCommandHandlerTests()
    {
        _branchRepositoryMock = new Mock<IAcademyBranchRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteBranchCommandHandler>>();
        _handler = new DeleteBranchCommandHandler(
            _branchRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesBranch()
    {
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, isDeleted: false);

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteBranchCommand { BranchId = branchId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _branchRepositoryMock.Verify(r => r.Remove(branch), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BranchNotFound_ReturnsFailure()
    {
        var branchId = Guid.NewGuid();

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyBranch?)null);

        var command = new DeleteBranchCommand { BranchId = branchId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _branchRepositoryMock.Verify(r => r.Remove(It.IsAny<AcademyBranch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AlreadyDeleted_ReturnsFailure()
    {
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, isDeleted: true);

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);

        var command = new DeleteBranchCommand { BranchId = branchId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already deleted");
        _branchRepositoryMock.Verify(r => r.Remove(It.IsAny<AcademyBranch>()), Times.Never);
    }

    private static AcademyBranch CreateBranch(Guid id, bool isDeleted) => new()
    {
        Id = id,
        AcademyId = Guid.NewGuid(),
        BranchName = "Test Branch",
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
