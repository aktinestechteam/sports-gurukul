using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands.Knowledge;

public class CreateKnowledgeBaseCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateKnowledgeBaseCommandHandler _handler;

    public CreateKnowledgeBaseCommandHandlerTests()
    {
        _handler = new CreateKnowledgeBaseCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsKnowledgeBaseDto()
    {
        var kb = new KnowledgeBase
        {
            Id = Guid.NewGuid(),
            Name = "Cricket Rules",
            Status = KnowledgeBaseStatus.Published,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.CreateBaseAsync(
                It.IsAny<CreateKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Success(kb));

        var result = await _handler.Handle(
            new CreateKnowledgeBaseCommand("Cricket Rules", null, KnowledgeBaseVisibility.Public, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(kb.Id);
        dto.Name.Should().Be("Cricket Rules");
        dto.Status.Should().Be(KnowledgeBaseStatus.Published);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.CreateBaseAsync(
                It.IsAny<CreateKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Failure("Knowledge base creation failed"));

        var result = await _handler.Handle(
            new CreateKnowledgeBaseCommand("x", null, KnowledgeBaseVisibility.Private, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base creation failed");
    }
}

public class UpdateKnowledgeBaseCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateKnowledgeBaseCommandHandler _handler;

    public UpdateKnowledgeBaseCommandHandlerTests()
    {
        _handler = new UpdateKnowledgeBaseCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsKnowledgeBaseDto()
    {
        var kb = new KnowledgeBase
        {
            Id = Guid.NewGuid(),
            Name = "Updated",
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.UpdateBaseAsync(
                It.IsAny<UpdateKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Success(kb));

        var result = await _handler.Handle(
            new UpdateKnowledgeBaseCommand(kb.Id, "Updated", null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(kb.Id);
        result.Value.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.UpdateBaseAsync(
                It.IsAny<UpdateKnowledgeBaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<KnowledgeBase>.Failure("Knowledge base not found"));

        var result = await _handler.Handle(
            new UpdateKnowledgeBaseCommand(Guid.NewGuid(), "x", null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }
}

public class AttachDocumentCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AttachDocumentCommandHandler _handler;

    public AttachDocumentCommandHandlerTests()
    {
        _handler = new AttachDocumentCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessDefault()
    {
        _service.Setup(s => s.AttachDocumentAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(
            new AttachDocumentCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.AttachDocumentAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Knowledge base not found"));

        var result = await _handler.Handle(
            new AttachDocumentCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }
}

public class DetachDocumentCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DetachDocumentCommandHandler _handler;

    public DetachDocumentCommandHandlerTests()
    {
        _handler = new DetachDocumentCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessDefault()
    {
        _service.Setup(s => s.DetachDocumentAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(
            new DetachDocumentCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.DetachDocumentAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Knowledge base not found"));

        var result = await _handler.Handle(
            new DetachDocumentCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }
}

public class RebuildKnowledgeIndexCommandHandlerTests
{
    private readonly Mock<IKnowledgeService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RebuildKnowledgeIndexCommandHandler _handler;

    public RebuildKnowledgeIndexCommandHandlerTests()
    {
        _handler = new RebuildKnowledgeIndexCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessDefault()
    {
        _service.Setup(s => s.RebuildIndexAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(
            new RebuildKnowledgeIndexCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.RebuildIndexAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Knowledge base not found"));

        var result = await _handler.Handle(
            new RebuildKnowledgeIndexCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Knowledge base not found");
    }
}
