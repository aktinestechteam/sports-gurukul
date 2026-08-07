using System.Text.Json.Nodes;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Features.AIManagement;

internal static class AssistantAssignmentStore
{
    private const string KnowledgeBaseIdsKey = "knowledge_base_ids";
    private const string ToolIdsKey = "tool_ids";

    internal static List<Guid> GetKnowledgeBaseIds(AIAssistant assistant) =>
        Read(assistant.MetadataJson, KnowledgeBaseIdsKey);

    internal static void SetKnowledgeBaseIds(AIAssistant assistant, IEnumerable<Guid> ids)
        => assistant.MetadataJson = Write(assistant.MetadataJson, KnowledgeBaseIdsKey, ids);

    internal static List<Guid> GetToolIds(AIAssistant assistant) =>
        Read(assistant.MetadataJson, ToolIdsKey);

    internal static void SetToolIds(AIAssistant assistant, IEnumerable<Guid> ids)
        => assistant.MetadataJson = Write(assistant.MetadataJson, ToolIdsKey, ids);

    private static List<Guid> Read(string? metadataJson, string key)
    {
        if (string.IsNullOrWhiteSpace(metadataJson))
            return new List<Guid>();

        var node = JsonNode.Parse(metadataJson) as JsonObject;
        if (node is null || node[key] is not JsonArray array)
            return new List<Guid>();

        var result = new List<Guid>();
        foreach (var item in array)
        {
            if (Guid.TryParse(item?.GetValue<string>(), out var id))
                result.Add(id);
        }

        return result;
    }

    private static string Write(string? metadataJson, string key, IEnumerable<Guid> ids)
    {
        var node = string.IsNullOrWhiteSpace(metadataJson)
            ? new JsonObject()
            : JsonNode.Parse(metadataJson) as JsonObject ?? new JsonObject();

        node[key] = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(ids.ToList()));
        return node.ToJsonString();
    }
}
