using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Api.Common.Models;

public class CreateInvoiceRequest
{
    public Guid? AthleteId { get; set; }
    public Guid? AcademyId { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Currency { get; set; }
    public List<InvoiceLineItemInput> LineItems { get; set; } = [];
    public string? CouponCode { get; set; }
    public Guid? ScholarshipId { get; set; }
}

public class InvoiceLineItemInput
{
    public string? Description { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string? ItemReference { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public string? CouponCode { get; set; }
}

public class UpdateInvoiceRequest
{
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<InvoiceLineItemInput>? LineItems { get; set; }
}

public class InvoiceSearchRequestModel
{
    public string? SearchTerm { get; set; }
    public InvoiceStatus? Status { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? AcademyId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class InitiatePaymentRequest
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? Description { get; set; }
}

public class RecordOfflinePaymentRequest
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public DateTime PaidAt { get; set; }
    public string? Description { get; set; }
}

public class RequestRefundRequest
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public List<RefundItemInput>? Items { get; set; }
}

public class RefundItemInput
{
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}

public class CreditWalletRequest
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
}

public class DebitWalletRequest
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
}

public class TransferWalletRequest
{
    public Guid FromWalletId { get; set; }
    public Guid ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

public class CreateCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public int? MaxUsages { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateCouponRequest
{
    public string? Description { get; set; }
    public decimal? Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public int? MaxUsages { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class ApplyCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public decimal OrderAmount { get; set; }
}

public class CreateScholarshipRequest
{
    public Guid AthleteId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class CreateSettlementBatchRequest
{
    public Guid[] PaymentIds { get; set; } = [];
}

public class ApproveRefundRequest
{
    public Guid RefundId { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
}

public class RejectRefundRequest
{
    public Guid RefundId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CompleteRefundRequest
{
    public Guid RefundId { get; set; }
    public string? GatewayReference { get; set; }
}

public class CancelInvoiceRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class VoidInvoiceRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class FinanceSearchRequest
{
    public string? SearchTerm { get; set; }
    public string? EntityType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PaymentSearchRequest
{
    public string? SearchTerm { get; set; }
    public PaymentStatus? Status { get; set; }
    public Guid? InvoiceId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
