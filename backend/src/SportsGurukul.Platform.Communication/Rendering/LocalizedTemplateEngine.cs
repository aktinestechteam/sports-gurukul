using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.Communication.Rendering;

public class LocalizedTemplateEngine
{
    private readonly ITemplateEngine _innerEngine;
    private readonly ILogger<LocalizedTemplateEngine> _logger;
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new(StringComparer.OrdinalIgnoreCase);

    public LocalizedTemplateEngine(
        ITemplateEngine innerEngine,
        ILogger<LocalizedTemplateEngine> logger)
    {
        _innerEngine = innerEngine;
        _logger = logger;
    }

    public void RegisterTranslations(string locale, Dictionary<string, string> translations)
    {
        _translations[locale] = translations;
    }

    public async Task<string> RenderLocalizedAsync(
        string template,
        IReadOnlyDictionary<string, object> variables,
        string locale)
    {
        var localizedTemplate = ApplyLocalization(template, locale);

        return await _innerEngine.RenderAsync(localizedTemplate, variables);
    }

    private string ApplyLocalization(string template, string locale)
    {
        if (!_translations.TryGetValue(locale, out var translations))
            return template;

        var result = template;

        foreach (var (key, value) in translations)
        {
            result = result.Replace($"{{t {key}}}", value, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    public async Task<string> RenderWithLocaleDetectionAsync(
        string template,
        IReadOnlyDictionary<string, object> variables,
        string? preferredLocale = null)
    {
        var locale = preferredLocale ?? "en";

        if (!_translations.ContainsKey(locale))
            locale = "en";

        return await RenderLocalizedAsync(template, variables, locale);
    }
}
