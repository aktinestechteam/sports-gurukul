using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Validators;

public class CreateConversationCommandValidatorTests
{
    private readonly CreateConversationCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyAssistantId_IsInvalid()
    {
        var command = new CreateConversationCommand(Guid.Empty, "Title", AIResourceOwnerType.Athlete, null, null, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AssistantId");
    }

    [Fact]
    public void Validate_EmptyTitle_IsInvalid()
    {
        var command = new CreateConversationCommand(Guid.NewGuid(), "", AIResourceOwnerType.Athlete, null, null, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var command = new CreateConversationCommand(Guid.NewGuid(), "My chat", AIResourceOwnerType.Athlete, null, null, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

public class CreateAssistantCommandValidatorTests
{
    private readonly CreateAssistantCommandValidator _validator = new();

    private static CreateAssistantCommand Build() => new(
        "Coach", "Coach", null, AIAssistantType.Coach, null, null, null, null,
        null, true, false, AIResourceOwnerType.Athlete, null, null, null, null);

    [Fact]
    public void Validate_EmptyName_IsInvalid()
    {
        var result = _validator.Validate(Build() with { Name = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_TemperatureOutOfRange_IsInvalid()
    {
        var result = _validator.Validate(Build() with { Temperature = 2.5 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Temperature");
    }

    [Fact]
    public void Validate_NonPositiveMaxTokens_IsInvalid()
    {
        var result = _validator.Validate(Build() with { MaxTokens = 0 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MaxTokens");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.Validate(Build());

        result.IsValid.Should().BeTrue();
    }
}

public class AssignKnowledgeBaseCommandValidatorTests
{
    private readonly AssignKnowledgeBaseCommandValidator _validator = new();

    [Fact]
    public void Validate_NullKnowledgeBaseIds_IsInvalid()
    {
        var command = new AssignKnowledgeBaseCommand(Guid.NewGuid(), null!, false);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "KnowledgeBaseIds");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var command = new AssignKnowledgeBaseCommand(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() }, false);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

public class CreatePromptTemplateCommandValidatorTests
{
    private readonly CreatePromptTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyTemplateText_IsInvalid()
    {
        var command = new CreatePromptTemplateCommand(
            Guid.NewGuid(), "Drill", null, AIPromptType.Template, "", null, null, null, false);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TemplateText");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var command = new CreatePromptTemplateCommand(
            Guid.NewGuid(), "Drill", null, AIPromptType.Template, "Explain {topic}", null, null, null, false);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

public class CreateKnowledgeBaseCommandValidatorTests
{
    private readonly CreateKnowledgeBaseCommandValidator _validator = new();

    private static CreateKnowledgeBaseCommand Build(int chunkSize = 1024, int chunkOverlap = 100) => new(
        "Drills", null, AIKnowledgeBaseType.Sports, AIResourceOwnerType.Academy, null, null, null,
        AIChunkingStrategy.FixedSize, chunkSize, chunkOverlap, null);

    [Fact]
    public void Validate_EmptyName_IsInvalid()
    {
        var result = _validator.Validate(Build() with { Name = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ChunkSizeTooSmall_IsInvalid()
    {
        var result = _validator.Validate(Build(chunkSize: 64));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChunkSize");
    }

    [Fact]
    public void Validate_NegativeChunkOverlap_IsInvalid()
    {
        var result = _validator.Validate(Build(chunkOverlap: -1));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChunkOverlap");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.Validate(Build());

        result.IsValid.Should().BeTrue();
    }
}

public class AttachDocumentCommandValidatorTests
{
    private readonly AttachDocumentCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyTitle_IsInvalid()
    {
        var command = new AttachDocumentCommand(Guid.NewGuid(), "", AIKnowledgeDocumentType.Text, null, null, null, null, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var command = new AttachDocumentCommand(Guid.NewGuid(), "Drill", AIKnowledgeDocumentType.Text, "content", null, null, null, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

public class CreateAgentCommandValidatorTests
{
    private readonly CreateAgentCommandValidator _validator = new();

    private static CreateAgentCommand Build() => new(
        null, null, "Scout", null, AIAgentType.Researcher, null, 0.5, 5, true, null, null);

    [Fact]
    public void Validate_EmptyName_IsInvalid()
    {
        var result = _validator.Validate(Build() with { Name = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NonPositiveMaxIterations_IsInvalid()
    {
        var result = _validator.Validate(Build() with { MaxIterations = 0 });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MaxIterations");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.Validate(Build());

        result.IsValid.Should().BeTrue();
    }
}

public class AssignWorkflowCommandValidatorTests
{
    private readonly AssignWorkflowCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyWorkflowId_IsInvalid()
    {
        var result = _validator.Validate(new AssignWorkflowCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "WorkflowId");
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.Validate(new AssignWorkflowCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }
}
