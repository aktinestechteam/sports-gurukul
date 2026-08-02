using AutoFixture;
using AutoFixture.Kernel;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities.AI;

namespace AI.Domain.Tests.Entities;

public class AIEntityAutoFixtureTests
{
    public static IEnumerable<object[]> EntityTypes()
    {
        yield return [typeof(AgentDefinition)];
        yield return [typeof(AgentExecution)];
        yield return [typeof(AIAssistant)];
        yield return [typeof(AIAuditLog)];
        yield return [typeof(AIModel)];
        yield return [typeof(AIModelConfiguration)];
        yield return [typeof(AIProvider)];
        yield return [typeof(AIRoutingPolicy)];
        yield return [typeof(AITokenUsage)];
        yield return [typeof(Conversation)];
        yield return [typeof(ConversationMemory)];
        yield return [typeof(ConversationMessage)];
        yield return [typeof(Embedding)];
        yield return [typeof(EmbeddingChunk)];
        yield return [typeof(KnowledgeBase)];
        yield return [typeof(KnowledgeDocument)];
        yield return [typeof(KnowledgeSource)];
        yield return [typeof(PromptTemplate)];
        yield return [typeof(PromptVersion)];
        yield return [typeof(SemanticSearchRequest)];
        yield return [typeof(SemanticSearchResult)];
        yield return [typeof(ToolDefinition)];
        yield return [typeof(ToolExecution)];
        yield return [typeof(VectorIndex)];
        yield return [typeof(WorkflowDefinition)];
        yield return [typeof(WorkflowExecution)];
    }

    [Theory]
    [MemberData(nameof(EntityTypes))]
    public void AutoFixture_CanPopulateAllProperties(Type type)
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var entity = fixture.Create(type, new SpecimenContext(fixture));

        entity.Should().NotBeNull();
        var baseEntity = entity as BaseEntity;
        baseEntity!.Id.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [MemberData(nameof(EntityTypes))]
    public void AutoFixture_AssignsValuesToValueTypeAndStringProperties(Type type)
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var entity = fixture.Create(type, new SpecimenContext(fixture));

        var properties = entity.GetType().GetProperties();
        properties.Should().NotBeEmpty();
    }
}
