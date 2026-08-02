namespace SportsGurukul.Platform.AI.Models;

public enum ModelRole
{
    System,
    User,
    Assistant,
    Tool
}

public class ModelMessage
{
    public ModelRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Name { get; set; }

    public static ModelMessage System(string content) => new() { Role = ModelRole.System, Content = content };
    public static ModelMessage User(string content) => new() { Role = ModelRole.User, Content = content };
    public static ModelMessage Assistant(string content) => new() { Role = ModelRole.Assistant, Content = content };
}

public class ModelOptions
{
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public IReadOnlyList<string>? StopSequences { get; set; }
    public IReadOnlyList<ToolDescriptor>? Tools { get; set; }
}

public class ModelUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens => PromptTokens + CompletionTokens;
}

public class ModelResponse
{
    public string Content { get; set; } = string.Empty;
    public ModelUsage Usage { get; set; } = new();
    public string? FinishReason { get; set; }
    public decimal? Cost { get; set; }
    public string? Provider { get; set; }
    public string? Model { get; set; }
}
