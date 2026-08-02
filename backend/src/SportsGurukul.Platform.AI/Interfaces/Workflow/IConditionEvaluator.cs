namespace SportsGurukul.Platform.AI.Interfaces.Workflow;

public interface IConditionEvaluator
{
    bool Evaluate(string? condition, IReadOnlyDictionary<string, object?> state);
}
