using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Security;

public interface IPromptInjectionGuard
{
    Task<PromptInjectionAssessment> InspectAsync(string input, CancellationToken cancellationToken = default);
}
