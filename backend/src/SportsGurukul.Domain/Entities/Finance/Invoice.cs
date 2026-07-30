using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string Currency { get; set; } = "INR";
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? BillingAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? AcademyId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? TournamentId { get; set; }
    public Guid? TrainingProgramId { get; set; }
    public Guid? MembershipId { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Athlete? Athlete { get; set; }
    public Academy? Academy { get; set; }
    public Event? Event { get; set; }
    public Tournament? Tournament { get; set; }
    public TrainingProgram? TrainingProgram { get; set; }
    public AcademyMembership? Membership { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<InvoiceTax> Taxes { get; set; } = new List<InvoiceTax>();
    public ICollection<InvoiceDiscount> Discounts { get; set; } = new List<InvoiceDiscount>();
    public ICollection<InvoicePayment> InvoicePayments { get; set; } = new List<InvoicePayment>();
    public ICollection<PaymentReminder> Reminders { get; set; } = new List<PaymentReminder>();
    public ICollection<CreditNote> CreditNotes { get; set; } = new List<CreditNote>();
    public ICollection<DebitNote> DebitNotes { get; set; } = new List<DebitNote>();
}
