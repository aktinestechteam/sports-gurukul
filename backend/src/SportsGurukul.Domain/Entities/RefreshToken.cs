using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public User User { get; set; } = null!;
}
