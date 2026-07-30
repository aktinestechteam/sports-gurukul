namespace SportsGurukul.Platform.Communication.Rendering;

public class VariableResolver
{
    private readonly Dictionary<string, Func<object>> _globalProviders = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterGlobal(string name, Func<object> provider)
    {
        _globalProviders[name] = provider;
    }

    public Dictionary<string, object> Resolve(IReadOnlyDictionary<string, string> variables)
    {
        var resolved = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var kv in variables)
        {
            resolved[kv.Key] = kv.Value;
        }

        foreach (var global in _globalProviders)
        {
            if (!resolved.ContainsKey(global.Key))
            {
                try
                {
                    resolved[global.Key] = global.Value();
                }
                catch
                {
                    resolved[global.Key] = string.Empty;
                }
            }
        }

        resolved["now"] = DateTime.UtcNow;
        resolved["today"] = DateTime.UtcNow.Date;
        resolved["year"] = DateTime.UtcNow.Year;

        return resolved;
    }

    public Dictionary<string, object> ResolveWithContext(
        IReadOnlyDictionary<string, string> variables,
        IReadOnlyDictionary<string, object> context)
    {
        var resolved = Resolve(variables);

        foreach (var kv in context)
        {
            resolved[kv.Key] = kv.Value;
        }

        return resolved;
    }

    public static string ResolveFromDictionary(string key, IReadOnlyDictionary<string, string> variables)
    {
        return variables.TryGetValue(key, out var value) ? value : string.Empty;
    }
}
