using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using DomainSettlementStatus = SportsGurukul.Domain.Enums.Finance.SettlementStatus;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class CreateSettlementBatchCommandHandlerTests
{
    private readonly Mock<ISettlementService> _serviceMock;
    private readonly CreateSettlementBatchCommandHandler _handler;

    public CreateSettlementBatchCommandHandlerTests()
    {
        _serviceMock = new Mock<ISettlementService>();
        _handler = new CreateSettlementBatchCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var paymentIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var batchId = Guid.NewGuid();
        var command = new CreateSettlementBatchCommand(paymentIds);
        var expected = Result<SettlementDto>.Success(new SettlementDto(batchId, "BATCH-001", 1000m, 2, DomainSettlementStatus.Pending, null, null, DateTime.UtcNow, new List<SettlementItemDto>()));
        _serviceMock.Setup(s => s.CreateSettlementBatchAsync(It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CreateSettlementBatchAsync(It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CreateSettlementBatchCommand(Array.Empty<Guid>());
        _serviceMock.Setup(s => s.CreateSettlementBatchAsync(It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("Batch creation failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Batch creation failed");
    }
}

public class CompleteSettlementCommandHandlerTests
{
    private readonly Mock<ISettlementService> _serviceMock;
    private readonly CompleteSettlementCommandHandler _handler;

    public CompleteSettlementCommandHandlerTests()
    {
        _serviceMock = new Mock<ISettlementService>();
        _handler = new CompleteSettlementCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var batchId = Guid.NewGuid();
        var command = new CompleteSettlementCommand(batchId, "GW-SETTLE-001");
        var expected = Result<SettlementDto>.Success(new SettlementDto(batchId, "BATCH-001", 1000m, 2, DomainSettlementStatus.Completed, "GW-SETTLE-001", DateTime.UtcNow, DateTime.UtcNow, new List<SettlementItemDto>()));
        _serviceMock.Setup(s => s.CompleteSettlementAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CompleteSettlementAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CompleteSettlementCommand(Guid.NewGuid(), null);
        _serviceMock.Setup(s => s.CompleteSettlementAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("Settlement completion failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Settlement completion failed");
    }
}
