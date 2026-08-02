using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Application.Features.AIManagement.Validators;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Validators;

public class AddMessageCommandValidatorTests
{
    private readonly AddMessageCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(
            new AddMessageCommand(Guid.NewGuid(), MessageRole.User, "hello", null));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyConversationId_Fails()
    {
        var result = _validator.TestValidate(
            new AddMessageCommand(Guid.Empty, MessageRole.User, "hello", null));
        result.ShouldHaveValidationErrorFor(x => x.ConversationId);
    }

    [Fact]
    public void EmptyContent_Fails()
    {
        var result = _validator.TestValidate(
            new AddMessageCommand(Guid.NewGuid(), MessageRole.User, "", null));
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void ContentTooLong_Fails()
    {
        var result = _validator.TestValidate(
            new AddMessageCommand(Guid.NewGuid(), MessageRole.User, new string('x', 100001), null));
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void InvalidRole_Fails()
    {
        var result = _validator.TestValidate(
            new AddMessageCommand(Guid.NewGuid(), (MessageRole)999, "hello", null));
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}

public class ClonePromptCommandValidatorTests
{
    private readonly ClonePromptCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(new ClonePromptCommand(Guid.NewGuid(), "copy"));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyNewName_Fails()
    {
        var result = _validator.TestValidate(new ClonePromptCommand(Guid.NewGuid(), ""));
        result.ShouldHaveValidationErrorFor(x => x.NewName);
    }

    [Fact]
    public void NewNameTooLong_Fails()
    {
        var result = _validator.TestValidate(new ClonePromptCommand(Guid.NewGuid(), new string('n', 201)));
        result.ShouldHaveValidationErrorFor(x => x.NewName);
    }
}

public class CreateAgentCommandValidatorTests
{
    private readonly CreateAgentCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(
            new CreateAgentCommand("Agent", null, null, null, null, null, null, 5, false));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_Fails()
    {
        var result = _validator.TestValidate(
            new CreateAgentCommand("", null, null, null, null, null, null, null, null));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameTooLong_Fails()
    {
        var result = _validator.TestValidate(
            new CreateAgentCommand(new string('a', 201), null, null, null, null, null, null, null, null));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}

public class CreateAssistantCommandValidatorTests
{
    private readonly CreateAssistantCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(
            new CreateAssistantCommand("Coach", null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, null, null, false));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_Fails()
    {
        var result = _validator.TestValidate(
            new CreateAssistantCommand("", null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, null, null, false));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameTooLong_Fails()
    {
        var result = _validator.TestValidate(
            new CreateAssistantCommand(new string('c', 201), null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, null, null, false));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void SystemPromptTooLong_Fails()
    {
        var result = _validator.TestValidate(
            new CreateAssistantCommand("Coach", null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, new string('p', 8001), null, false));
        result.ShouldHaveValidationErrorFor(x => x.SystemPrompt);
    }

    [Fact]
    public void NullSystemPrompt_Passes()
    {
        var result = _validator.TestValidate(
            new CreateAssistantCommand("Coach", null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, null, null, false));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateConversationCommandValidatorTests
{
    private readonly CreateConversationCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(new CreateConversationCommand("Hello", null, null));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NullTitle_Passes()
    {
        var result = _validator.TestValidate(new CreateConversationCommand(null, null, null));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void TitleTooLong_Fails()
    {
        var result = _validator.TestValidate(new CreateConversationCommand(new string('t', 201), null, null));
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}

public class CreateKnowledgeBaseCommandValidatorTests
{
    private readonly CreateKnowledgeBaseCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(
            new CreateKnowledgeBaseCommand("KB", null, KnowledgeBaseVisibility.Public, null, null));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_Fails()
    {
        var result = _validator.TestValidate(
            new CreateKnowledgeBaseCommand("", null, KnowledgeBaseVisibility.Private, null, null));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameTooLong_Fails()
    {
        var result = _validator.TestValidate(
            new CreateKnowledgeBaseCommand(new string('k', 201), null, KnowledgeBaseVisibility.Public, null, null));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}

public class CreatePromptTemplateCommandValidatorTests
{
    private readonly CreatePromptTemplateCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(
            new CreatePromptTemplateCommand("Prompt", null, PromptType.System, "content", null, null, null));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_Fails()
    {
        var result = _validator.TestValidate(
            new CreatePromptTemplateCommand("", null, PromptType.System, "content", null, null, null));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptyTemplateContent_Fails()
    {
        var result = _validator.TestValidate(
            new CreatePromptTemplateCommand("Prompt", null, PromptType.System, "", null, null, null));
        result.ShouldHaveValidationErrorFor(x => x.TemplateContent);
    }
}

public class RenameConversationCommandValidatorTests
{
    private readonly RenameConversationCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(new RenameConversationCommand(Guid.NewGuid(), "New Title"));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyId_Fails()
    {
        var result = _validator.TestValidate(new RenameConversationCommand(Guid.Empty, "New Title"));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void EmptyTitle_Fails()
    {
        var result = _validator.TestValidate(new RenameConversationCommand(Guid.NewGuid(), ""));
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleTooLong_Fails()
    {
        var result = _validator.TestValidate(new RenameConversationCommand(Guid.NewGuid(), new string('n', 201)));
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}
