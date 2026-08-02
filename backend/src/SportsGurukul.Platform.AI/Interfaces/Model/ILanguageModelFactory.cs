using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Model;

public interface ILanguageModelFactory
{
    void Register(string provider, Func<ILanguageModel> factory);

    ILanguageModel? Get(string provider);

    ILanguageModel Create(string provider, string model, IDictionary<string, string?>? config = null);

    IReadOnlyList<string> Providers { get; }
}
