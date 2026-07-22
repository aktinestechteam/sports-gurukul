using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
    string GenerateRefreshToken();
}
