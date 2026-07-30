namespace SportsGurukul.Platform.PaymentGateway.Models;

public class InvoiceGenerationRequest
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerGstin { get; set; }
    public string? CustomerAddress { get; set; }
    public List<InvoiceLineItem> LineItems { get; set; } = [];
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public string Currency { get; set; } = "INR";
    public string? PurchaseOrderNumber { get; set; }
}

public class InvoiceLineItem
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal? TaxRate { get; set; }
    public string? TaxName { get; set; }
    public string? HsnCode { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
}

public class InvoiceResult
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal Total { get; set; }
    public decimal AmountDue { get; set; }
    public string Currency { get; set; } = "INR";
    public List<TaxBreakdown> TaxBreakdown { get; set; } = [];
    public List<DiscountBreakdown> DiscountBreakdown { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class TaxBreakdown
{
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class DiscountBreakdown
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Amount { get; set; }
    public string? Source { get; set; }
}

public class InstallmentPlan
{
    public string PlanId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int NumberOfInstallments { get; set; }
    public decimal InstallmentAmount { get; set; }
    public decimal? InterestRate { get; set; }
    public decimal? ProcessingFee { get; set; }
    public List<InstallmentSchedule> Schedule { get; set; } = [];
    public string Frequency { get; set; } = "monthly";
}

public class InstallmentSchedule
{
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "pending";
}

public class LateFeeResult
{
    public decimal LateFee { get; set; }
    public decimal PenaltyAmount { get; set; }
    public int DaysOverdue { get; set; }
    public decimal TotalDue { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

public class RecurringBillingProfile
{
    public string ProfileId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Frequency { get; set; } = "monthly";
    public int Interval { get; set; } = 1;
    public int? MaxCycles { get; set; }
    public int CurrentCycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public string Status { get; set; } = "active";
    public string? GatewaySubscriptionId { get; set; }
    public string? PaymentMethodToken { get; set; }
}
