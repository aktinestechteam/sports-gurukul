using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreBranch;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class RestoreBranchCommandHandlerTests
{
    private readonly Mock<IAcademyBranchRepository> _branchRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RestoreBranchCommandHandler>> _loggerMock;
    private readonly RestoreBranchCommandHandler _handler;

    public RestoreBranchCommandHandlerTests()
    {
        _branchRepositoryMock = new Mock<IAcademyBranchRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RestoreBranchCommandHandler>>();
        _handler = new RestoreBranchCommandHandler(
            _branchRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_RestoresBranch()
    {
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, isDeleted: true);

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RestoreBranchCommand { BranchId = branchId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(branchId);
        branch.IsDeleted.Should().BeFalse();
        _branchRepositoryMock.Verify(r => r.Update(branch), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BranchNotFound_ReturnsFailure()
    {
        var branchId = Guid.NewGuid();

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyBranch?)null);

        var command = new RestoreBranchCommand { BranchId = branchId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_NotDeleted_ReturnsFailure()
    {
        var branchId = Guid.NewGuid();
        var branch = CreateBranch(branchId, isDeleted: false);

        _branchRepositoryMock
            .Setup(r => r.GetByIdAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);

        var command = new RestoreBranchCommand { BranchId = branchId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not deleted");
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
