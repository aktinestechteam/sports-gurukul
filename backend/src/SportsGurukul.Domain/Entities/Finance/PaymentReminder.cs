using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class PaymentReminder : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public PaymentReminderType ReminderType { get; set; }
    public DateTime SentAt { get; set; }
    public string SentTo { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? Notes { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
