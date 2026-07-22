using Microsoft.AspNetCore.Identity;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Authentication;

public class ApplicationPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _inner = new();

    public string HashPassword(string password)
    {
        return _inner.HashPassword(new User(), password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _inner.VerifyHashedPassword(new User(), hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
