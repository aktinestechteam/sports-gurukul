using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using PaymentMethodEnum = SportsGurukul.Domain.Enums.Finance.PaymentMethod;

namespace SportsGurukul.Finance.Domain.Tests.Builders;

public static class FinanceEntityBuilder
{
    private static int _counter;

    public static Payment CreatePayment(
        Guid? id = null, decimal amount = 1000, PaymentStatus status = PaymentStatus.Pending,
        PaymentMethodEnum method = PaymentMethodEnum.UPI, Guid? invoiceId = null)
    {
        _counter++;
        return new Payment
        {
            Id = id ?? Guid.NewGuid(),
            PaymentReference = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{_counter:D5}",
            PaymentDate = DateTime.UtcNow,
            Amount = amount,
            Currency = "INR",
            PaymentMethod = method,
            Status = status,
            InvoiceId = invoiceId ?? Guid.NewGuid(),
            Description = "Test payment",
            IsIdempotent = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Invoice CreateInvoice(
        Guid? id = null, decimal total = 5000, InvoiceStatus status = InvoiceStatus.Draft,
        decimal paidAmount = 0, decimal amountDue = 5000)
    {
        _counter++;
        return new Invoice
        {
            Id = id ?? Guid.NewGuid(),
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{_counter:D5}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = status,
            SubTotal = total,
            TaxTotal = total * 0.18m,
            DiscountTotal = 0,
            Total = total * 1.18m,
            AmountPaid = paidAmount,
            AmountDue = amountDue,
            Currency = "INR",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Wallet CreateWallet(
        Guid? id = null, Guid? userId = null, decimal balance = 10000)
    {
        return new Wallet
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            Balance = balance,
            Currency = "INR",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static WalletTransaction CreateWalletTransaction(
        Guid walletId, decimal amount, decimal balanceBefore, TransactionType type = TransactionType.Credit)
    {
        return new WalletTransaction
        {
            WalletId = walletId,
            TransactionType = type,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = type == TransactionType.Credit ? balanceBefore + amount : balanceBefore - amount,
            Reference = "TXN-REF",
            Description = "Test transaction",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Refund CreateRefund(
        Guid? id = null, Guid? paymentId = null, decimal amount = 500,
        RefundStatus status = RefundStatus.Requested)
    {
        return new Refund
        {
            Id = id ?? Guid.NewGuid(),
            RefundNumber = $"RFN-{DateTime.UtcNow:yyyyMMdd}-{_counter:D5}",
            PaymentId = paymentId ?? Guid.NewGuid(),
            Reason = "Test refund",
            Status = status,
            TotalAmount = amount,
            RefundDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Coupon CreateCoupon(
        string? code = null, DiscountType type = DiscountType.Percentage,
        decimal value = 10, bool isActive = true, int? maxUsage = 100)
    {
        return new Coupon
        {
            Id = Guid.NewGuid(),
            Code = code ?? $"TEST{_counter++:D4}",
            Type = type,
            Value = value,
            MaxUsage = maxUsage,
            CurrentUsage = 0,
            MaxUsagePerUser = 1,
            IsActive = isActive,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            MinOrderAmount = 100,
            MaxDiscountAmount = 5000,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static SettlementBatch CreateSettlementBatch(
        Guid? id = null, SettlementStatus status = SettlementStatus.Pending, decimal total = 10000)
    {
        return new SettlementBatch
        {
            Id = id ?? Guid.NewGuid(),
            BatchNumber = $"STL-{DateTime.UtcNow:yyyyMMdd}-{_counter:D5}",
            TotalAmount = total,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Settlement CreateSettlement(Guid batchId, Guid paymentId, decimal amount = 5000)
    {
        return new Settlement
        {
            SettlementBatchId = batchId,
            PaymentId = paymentId,
            Amount = amount,
            Status = SettlementStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static InvoiceItem CreateInvoiceItem(
        Guid invoiceId, string description = "Service Fee", int quantity = 1, decimal unitPrice = 1000)
    {
        return new InvoiceItem
        {
            InvoiceId = invoiceId,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalAmount = quantity * unitPrice
        };
    }

    public static InvoiceTax CreateInvoiceTax(Guid invoiceId, string name = "GST 18%", decimal rate = 18, decimal amount = 180)
    {
        return new InvoiceTax
        {
            InvoiceId = invoiceId,
            TaxName = name,
            TaxRate = rate,
            TaxAmount = amount
        };
    }

    public static InvoiceDiscount CreateInvoiceDiscount(Guid invoiceId, string name = "Coupon", DiscountType type = DiscountType.Percentage, decimal value = 10, decimal amount = 100)
    {
        return new InvoiceDiscount
        {
            InvoiceId = invoiceId,
            DiscountName = name,
            DiscountType = type,
            DiscountValue = value,
            DiscountAmount = amount
        };
    }

    public static Ledger CreateLedger(string code = "CASH001", string name = "Cash", LedgerType type = LedgerType.Asset)
    {
        return new Ledger
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Type = type,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static LedgerEntry CreateLedgerEntry(Guid ledgerId, decimal debit = 0, decimal credit = 1000)
    {
        return new LedgerEntry
        {
            LedgerId = ledgerId,
            EntryDate = DateTime.UtcNow,
            DebitAmount = debit,
            CreditAmount = credit,
            Reference = "REF-001",
            Description = "Test entry",
            CreatedAt = DateTime.UtcNow
        };
    }
}
