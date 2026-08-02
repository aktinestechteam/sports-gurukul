using System.Reflection;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Domain.Tests.Enums;

public class AIEnumTests
{
    public static IEnumerable<object[]> EnumMemberCounts()
    {
        yield return [typeof(AgentExecutionStatus), 5];
        yield return [typeof(AgentStatus), 4];
        yield return [typeof(AIAssistantPersonality), 7];
        yield return [typeof(AIAssistantType), 9];
        yield return [typeof(AIModelCapability), 9];
        yield return [typeof(AIModelStatus), 4];
        yield return [typeof(AIProviderType), 8];
        yield return [typeof(AuditEventType), 8];
        yield return [typeof(AuditSeverity), 5];
        yield return [typeof(ConversationStatus), 4];
        yield return [typeof(DocumentType), 13];
        yield return [typeof(EmbeddingStatus), 4];
        yield return [typeof(KnowledgeBaseStatus), 3];
        yield return [typeof(KnowledgeBaseVisibility), 4];
        yield return [typeof(KnowledgeSourceType), 6];
        yield return [typeof(MemoryImportance), 4];
        yield return [typeof(MemoryType), 5];
        yield return [typeof(MessageRole), 5];
        yield return [typeof(MessageStatus), 6];
        yield return [typeof(PromptStatus), 4];
        yield return [typeof(PromptType), 4];
        yield return [typeof(RoutingStatus), 3];
        yield return [typeof(RoutingStrategy), 5];
        yield return [typeof(SemanticSearchStatus), 4];
        yield return [typeof(SourceStatus), 4];
        yield return [typeof(ToolStatus), 3];
        yield return [typeof(ToolType), 4];
        yield return [typeof(VectorIndexStatus), 4];
        yield return [typeof(WorkflowExecutionStatus), 5];
        yield return [typeof(WorkflowStatus), 4];
    }

    [Theory]
    [MemberData(nameof(EnumMemberCounts))]
    public void Enum_HasExpectedMemberCount(Type type, int expectedCount)
    {
        type.IsEnum.Should().BeTrue();
        Enum.GetNames(type).Length.Should().Be(expectedCount);
    }

    [Theory]
    [MemberData(nameof(EnumMemberCounts))]
    public void Enum_IsDefinedForAllMembers(Type type, int expectedCount)
    {
        _ = expectedCount;
        foreach (var name in Enum.GetNames(type))
        {
            Enum.IsDefined(type, name).Should().BeTrue();
        }
    }

    [Fact]
    public void AIModelCapability_IsFlagsAttribute()
    {
        typeof(AIModelCapability).GetCustomAttribute<FlagsAttribute>().Should().NotBeNull();
    }

    [Fact]
    public void AIModelCapability_HasExpectedBitValues()
    {
        ((int)AIModelCapability.TextGeneration).Should().Be(1);
        ((int)AIModelCapability.CodeGeneration).Should().Be(2);
        ((int)AIModelCapability.ImageGeneration).Should().Be(4);
        ((int)AIModelCapability.ImageAnalysis).Should().Be(8);
        ((int)AIModelCapability.AudioTranscription).Should().Be(16);
        ((int)AIModelCapability.Embedding).Should().Be(32);
        ((int)AIModelCapability.Reasoning).Should().Be(64);
        ((int)AIModelCapability.FunctionCalling).Should().Be(128);
        ((int)AIModelCapability.Vision).Should().Be(256);
    }

    [Fact]
    public void AIModelCapability_FlagsCombineAndTest()
    {
        var combined = AIModelCapability.TextGeneration | AIModelCapability.Embedding | AIModelCapability.Reasoning;
        combined.HasFlag(AIModelCapability.TextGeneration).Should().BeTrue();
        combined.HasFlag(AIModelCapability.Embedding).Should().BeTrue();
        combined.HasFlag(AIModelCapability.Reasoning).Should().BeTrue();
        combined.HasFlag(AIModelCapability.Vision).Should().BeFalse();
        combined.HasFlag(AIModelCapability.FunctionCalling).Should().BeFalse();
    }

    [Theory]
    [InlineData(typeof(AIProviderType), "OpenAI")]
    [InlineData(typeof(AIProviderType), "Custom")]
    [InlineData(typeof(MessageRole), "System")]
    [InlineData(typeof(MessageRole), "Assistant")]
    [InlineData(typeof(MessageRole), "Tool")]
    [InlineData(typeof(ConversationStatus), "Active")]
    [InlineData(typeof(ConversationStatus), "Archived")]
    [InlineData(typeof(WorkflowStatus), "Draft")]
    [InlineData(typeof(WorkflowStatus), "Active")]
    [InlineData(typeof(AgentStatus), "Draft")]
    [InlineData(typeof(AgentStatus), "Active")]
    public void Enum_ContainsExpectedMember(Type type, string memberName)
    {
        Enum.GetNames(type).Should().Contain(memberName);
    }
}
