using System.Text.Json;
using System.Text.Json.Nodes;

namespace SportsGurukul.Application.Features.AIManagement;

internal static class AiJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    internal static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    internal static T? Deserialize<T>(string? json) =>
        string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, Options);

    internal static string? ToJsonOrNull<T>(T value) => value is null ? null : Serialize(value);
}
