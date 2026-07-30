using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class Wallet : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "INR";
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
    public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
}
