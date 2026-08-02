using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.Security;

public interface IOutputValidator
{
    Task<OutputValidationResult> ValidateAsync(string output, CancellationToken cancellationToken = default);
}
