using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Infrastructure.Tests.Builders;

namespace SportsGurukul.Finance.Infrastructure.Tests.Repositories;

public class InvoiceRepositoryTests
{
    private static int _counter;

    private static Invoice CreateInvoice(InvoiceStatus status = InvoiceStatus.Draft)
    {
        _counter++;
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INV-TEST-{_counter:D5}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = status,
            SubTotal = 5000,
            TaxTotal = 900,
            DiscountTotal = 0,
            Total = 5900,
            AmountPaid = 0,
            AmountDue = 5900,
            Currency = "INR",
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IInvoiceRepository> _mock;
    private readonly List<Invoice> _invoices;

    public InvoiceRepositoryTests()
    {
        _invoices =
        [
            CreateInvoice(InvoiceStatus.Draft),
            CreateInvoice(InvoiceStatus.Issued),
            CreateInvoice(InvoiceStatus.Paid)
        ];
        _mock = MockRepositoryBuilder.CreateInvoiceRepository(_invoices);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnInvoice_WhenFound()
    {
        var expected = _invoices[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllInvoices()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnInvoice()
    {
        var invoice = CreateInvoice();
        var result = await _mock.Object.AddAsync(invoice);
        result.Should().Be(invoice);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var invoice = _invoices[0];
        _mock.Object.Update(invoice);
        _mock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var invoice = _invoices[0];
        _mock.Object.Remove(invoice);
        _mock.Verify(r => r.Remove(invoice), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredInvoices()
    {
        var result = await _mock.Object.FindAsync(i => i.Status == InvoiceStatus.Draft);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(i => i.Status == InvoiceStatus.Draft);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByInvoiceNumberAsync_ShouldReturnInvoice_WhenFound()
    {
        var expected = _invoices[0];
        _mock.Setup(r => r.GetByInvoiceNumberAsync(expected.InvoiceNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByInvoiceNumberAsync(expected.InvoiceNumber);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByInvoiceNumberAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByInvoiceNumberAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        var result = await _mock.Object.GetByInvoiceNumberAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnInvoiceWithItems()
    {
        var invoice = _invoices[0];
        invoice.Items.Add(new InvoiceItem
        {
            InvoiceId = invoice.Id,
            Description = "Service Fee",
            Quantity = 1,
            UnitPrice = 5000,
            TotalAmount = 5000
        });
        _mock.Setup(r => r.GetByIdWithDetailsAsync(invoice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        var result = await _mock.Object.GetByIdWithDetailsAsync(invoice.Id);
        result.Should().Be(invoice);
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByAthleteIdAsync_ShouldReturnAthleteInvoices()
    {
        var athleteId = Guid.NewGuid();
        var athleteInvoices = _invoices.Take(2).ToList();
        athleteInvoices.ForEach(i => i.AthleteId = athleteId);
        _mock.Setup(r => r.GetByAthleteIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athleteInvoices);
        var result = await _mock.Object.GetByAthleteIdAsync(athleteId);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByAcademyIdAsync_ShouldReturnAcademyInvoices()
    {
        var academyId = Guid.NewGuid();
        var academyInvoices = _invoices.Take(1).ToList();
        academyInvoices.ForEach(i => i.AcademyId = academyId);
        _mock.Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academyInvoices);
        var result = await _mock.Object.GetByAcademyIdAsync(academyId);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnInvoicesByStatus()
    {
        var status = InvoiceStatus.Draft;
        var draftInvoices = _invoices.Where(i => i.Status == status).ToList();
        _mock.Setup(r => r.GetByStatusAsync(status, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draftInvoices);
        var result = await _mock.Object.GetByStatusAsync(status);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetOverdueInvoicesAsync_ShouldReturnOverdueInvoices()
    {
        var overdue = _invoices.Take(1).ToList();
        _mock.Setup(r => r.GetOverdueInvoicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(overdue);
        var result = await _mock.Object.GetOverdueInvoicesAsync();
        result.Should().ContainSingle();
    }
}

public class PaymentRepositoryTests
{
    private static int _counter;

    private static Payment CreatePayment(PaymentStatus status = PaymentStatus.Pending)
    {
        _counter++;
        return new Payment
        {
            Id = Guid.NewGuid(),
            PaymentReference = $"PAY-TEST-{_counter:D5}",
            PaymentDate = DateTime.UtcNow,
            Amount = 1000,
            Currency = "INR",
            PaymentMethod = Domain.Enums.Finance.PaymentMethod.UPI,
            Status = status,
            InvoiceId = Guid.NewGuid(),
            Description = "Test payment",
            IsIdempotent = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IPaymentRepository> _mock;
    private readonly List<Payment> _payments;

    public PaymentRepositoryTests()
    {
        _payments =
        [
            CreatePayment(PaymentStatus.Pending),
            CreatePayment(PaymentStatus.Captured),
            CreatePayment(PaymentStatus.Failed)
        ];
        _mock = MockRepositoryBuilder.CreatePaymentRepository(_payments);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPayment_WhenFound()
    {
        var expected = _payments[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPayments()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnPayment()
    {
        var payment = CreatePayment();
        var result = await _mock.Object.AddAsync(payment);
        result.Should().Be(payment);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var payment = _payments[0];
        _mock.Object.Update(payment);
        _mock.Verify(r => r.Update(payment), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var payment = _payments[0];
        _mock.Object.Remove(payment);
        _mock.Verify(r => r.Remove(payment), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredPayments()
    {
        var result = await _mock.Object.FindAsync(p => p.Status == PaymentStatus.Captured);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(p => p.Status == PaymentStatus.Captured);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByPaymentReferenceAsync_ShouldReturnPayment_WhenFound()
    {
        var expected = _payments[0];
        _mock.Setup(r => r.GetByPaymentReferenceAsync(expected.PaymentReference, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByPaymentReferenceAsync(expected.PaymentReference);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByPaymentReferenceAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByPaymentReferenceAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);
        var result = await _mock.Object.GetByPaymentReferenceAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithTransactionsAsync_ShouldReturnPaymentWithTransactions()
    {
        var payment = _payments[0];
        payment.Transactions.Add(new PaymentTransaction
        {
            PaymentId = payment.Id,
            TransactionType = TransactionType.Debit,
            Amount = payment.Amount,
            Status = "Success"
        });
        _mock.Setup(r => r.GetByIdWithTransactionsAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        var result = await _mock.Object.GetByIdWithTransactionsAsync(payment.Id);
        result.Should().Be(payment);
        result!.Transactions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByInvoiceIdAsync_ShouldReturnInvoicePayments()
    {
        var invoiceId = Guid.NewGuid();
        var invoicePayments = _payments.Take(2).ToList();
        invoicePayments.ForEach(p => p.InvoiceId = invoiceId);
        _mock.Setup(r => r.GetByInvoiceIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoicePayments);
        var result = await _mock.Object.GetByInvoiceIdAsync(invoiceId);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdempotencyKeyAsync_ShouldReturnPayment_WhenFound()
    {
        var key = "IDEMP-001";
        var payment = _payments[0];
        payment.IdempotencyKey = key;
        _mock.Setup(r => r.GetByIdempotencyKeyAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        var result = await _mock.Object.GetByIdempotencyKeyAsync(key);
        result.Should().Be(payment);
    }

    [Fact]
    public async Task GetByIdempotencyKeyAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByIdempotencyKeyAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);
        var result = await _mock.Object.GetByIdempotencyKeyAsync("NONEXISTENT");
        result.Should().BeNull();
    }
}

public class RefundRepositoryTests
{
    private static int _counter;

    private static Refund CreateRefund(RefundStatus status = RefundStatus.Requested)
    {
        _counter++;
        return new Refund
        {
            Id = Guid.NewGuid(),
            RefundNumber = $"RFN-TEST-{_counter:D5}",
            PaymentId = Guid.NewGuid(),
            Reason = "Test refund",
            Status = status,
            TotalAmount = 500,
            RefundDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IRefundRepository> _mock;
    private readonly List<Refund> _refunds;

    public RefundRepositoryTests()
    {
        _refunds =
        [
            CreateRefund(RefundStatus.Requested),
            CreateRefund(RefundStatus.Approved),
            CreateRefund(RefundStatus.Completed)
        ];
        _mock = MockRepositoryBuilder.CreateRefundRepository(_refunds);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRefund_WhenFound()
    {
        var expected = _refunds[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRefunds()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnRefund()
    {
        var refund = CreateRefund();
        var result = await _mock.Object.AddAsync(refund);
        result.Should().Be(refund);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var refund = _refunds[0];
        _mock.Object.Update(refund);
        _mock.Verify(r => r.Update(refund), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var refund = _refunds[0];
        _mock.Object.Remove(refund);
        _mock.Verify(r => r.Remove(refund), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredRefunds()
    {
        var result = await _mock.Object.FindAsync(r => r.Status == RefundStatus.Approved);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(r => r.Status == RefundStatus.Approved);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByRefundNumberAsync_ShouldReturnRefund_WhenFound()
    {
        var expected = _refunds[0];
        _mock.Setup(r => r.GetByRefundNumberAsync(expected.RefundNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByRefundNumberAsync(expected.RefundNumber);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByRefundNumberAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByRefundNumberAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Refund?)null);
        var result = await _mock.Object.GetByRefundNumberAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithItemsAsync_ShouldReturnRefundWithItems()
    {
        var refund = _refunds[0];
        refund.RefundItems.Add(new RefundItem
        {
            RefundId = refund.Id,
            Description = "Test item",
            Quantity = 1,
            Amount = 500
        });
        _mock.Setup(r => r.GetByIdWithItemsAsync(refund.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);
        var result = await _mock.Object.GetByIdWithItemsAsync(refund.Id);
        result.Should().Be(refund);
        result!.RefundItems.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByPaymentIdAsync_ShouldReturnPaymentRefunds()
    {
        var paymentId = Guid.NewGuid();
        var paymentRefunds = _refunds.Take(2).ToList();
        paymentRefunds.ForEach(r => r.PaymentId = paymentId);
        _mock.Setup(r => r.GetByPaymentIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentRefunds);
        var result = await _mock.Object.GetByPaymentIdAsync(paymentId);
        result.Should().HaveCount(2);
    }
}

public class CouponRepositoryTests
{
    private static int _counter;

    private static Coupon CreateCoupon(bool isActive = true)
    {
        _counter++;
        return new Coupon
        {
            Id = Guid.NewGuid(),
            Code = $"TEST{_counter:D4}",
            Type = DiscountType.Percentage,
            Value = 10,
            MaxUsage = 100,
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

    private readonly Mock<ICouponRepository> _mock;
    private readonly List<Coupon> _coupons;

    public CouponRepositoryTests()
    {
        _coupons =
        [
            CreateCoupon(true),
            CreateCoupon(true),
            CreateCoupon(false)
        ];
        _mock = MockRepositoryBuilder.CreateCouponRepository(_coupons);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCoupon_WhenFound()
    {
        var expected = _coupons[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCoupons()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnCoupon()
    {
        var coupon = CreateCoupon();
        var result = await _mock.Object.AddAsync(coupon);
        result.Should().Be(coupon);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var coupon = _coupons[0];
        _mock.Object.Update(coupon);
        _mock.Verify(r => r.Update(coupon), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var coupon = _coupons[0];
        _mock.Object.Remove(coupon);
        _mock.Verify(r => r.Remove(coupon), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredCoupons()
    {
        var result = await _mock.Object.FindAsync(c => c.IsActive);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(c => c.IsActive);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnCoupon_WhenFound()
    {
        var expected = _coupons[0];
        _mock.Setup(r => r.GetByCodeAsync(expected.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByCodeAsync(expected.Code);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByCodeAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);
        var result = await _mock.Object.GetByCodeAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeWithUsagesAsync_ShouldReturnCouponWithUsages()
    {
        var coupon = _coupons[0];
        coupon.Usages.Add(new CouponUsage
        {
            CouponId = coupon.Id,
            UserId = Guid.NewGuid(),
            UsedAt = DateTime.UtcNow,
            DiscountAmount = 100
        });
        _mock.Setup(r => r.GetByCodeWithUsagesAsync(coupon.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);
        var result = await _mock.Object.GetByCodeWithUsagesAsync(coupon.Code);
        result.Should().Be(coupon);
        result!.Usages.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetActiveCouponsAsync_ShouldReturnOnlyActiveCoupons()
    {
        var active = _coupons.Where(c => c.IsActive).ToList();
        _mock.Setup(r => r.GetActiveCouponsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(active);
        var result = await _mock.Object.GetActiveCouponsAsync();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.IsActive.Should().BeTrue());
    }
}

public class WalletRepositoryTests
{
    private static int _counter;

    private static Wallet CreateWallet(decimal balance = 10000)
    {
        _counter++;
        return new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Balance = balance,
            Currency = "INR",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static WalletTransaction CreateWalletTransaction(
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

    private readonly Mock<IWalletRepository> _mock;
    private readonly List<Wallet> _wallets;

    public WalletRepositoryTests()
    {
        _wallets =
        [
            CreateWallet(10000),
            CreateWallet(5000),
            CreateWallet(0)
        ];
        _mock = MockRepositoryBuilder.CreateWalletRepository(_wallets);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnWallet_WhenFound()
    {
        var expected = _wallets[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllWallets()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnWallet()
    {
        var wallet = CreateWallet();
        var result = await _mock.Object.AddAsync(wallet);
        result.Should().Be(wallet);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var wallet = _wallets[0];
        _mock.Object.Update(wallet);
        _mock.Verify(r => r.Update(wallet), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var wallet = _wallets[0];
        _mock.Object.Remove(wallet);
        _mock.Verify(r => r.Remove(wallet), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredWallets()
    {
        var result = await _mock.Object.FindAsync(w => w.Balance > 0);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(w => w.Balance > 0);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnWallet_WhenFound()
    {
        var expected = _wallets[0];
        _mock.Setup(r => r.GetByUserIdAsync(expected.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByUserIdAsync(expected.UserId);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);
        var result = await _mock.Object.GetByUserIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithTransactionsAsync_ShouldReturnWalletWithTransactions()
    {
        var wallet = _wallets[0];
        wallet.Transactions.Add(CreateWalletTransaction(
            wallet.Id, 1000, wallet.Balance, TransactionType.Credit));
        _mock.Setup(r => r.GetByIdWithTransactionsAsync(wallet.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);
        var result = await _mock.Object.GetByIdWithTransactionsAsync(wallet.Id);
        result.Should().Be(wallet);
        result!.Transactions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLastTransactionAsync_ShouldReturnMostRecentTransaction()
    {
        var walletId = _wallets[0].Id;
        var transaction = CreateWalletTransaction(
            walletId, 500, 9500, TransactionType.Debit);
        _mock.Setup(r => r.GetLastTransactionAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        var result = await _mock.Object.GetLastTransactionAsync(walletId);
        result.Should().Be(transaction);
        result!.WalletId.Should().Be(walletId);
    }
}

public class LedgerRepositoryTests
{
    private static int _counter;

    private static Ledger CreateLedger(string code, string name, LedgerType type = LedgerType.Asset)
    {
        _counter++;
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

    private static LedgerEntry CreateLedgerEntry(Guid ledgerId)
    {
        return new LedgerEntry
        {
            LedgerId = ledgerId,
            EntryDate = DateTime.UtcNow,
            DebitAmount = 0,
            CreditAmount = 1000,
            Reference = "REF-001",
            Description = "Test entry",
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<ILedgerRepository> _mock;
    private readonly List<Ledger> _ledgers;

    public LedgerRepositoryTests()
    {
        _ledgers =
        [
            CreateLedger("CASH001", "Cash", LedgerType.Asset),
            CreateLedger("BANK001", "Bank", LedgerType.Asset),
            CreateLedger("REV001", "Revenue", LedgerType.Income)
        ];
        _mock = MockRepositoryBuilder.CreateLedgerRepository(_ledgers);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnLedger_WhenFound()
    {
        var expected = _ledgers[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLedgers()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnLedger()
    {
        var ledger = CreateLedger("TEST001", "Test", LedgerType.Liability);
        var result = await _mock.Object.AddAsync(ledger);
        result.Should().Be(ledger);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var ledger = _ledgers[0];
        _mock.Object.Update(ledger);
        _mock.Verify(r => r.Update(ledger), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var ledger = _ledgers[0];
        _mock.Object.Remove(ledger);
        _mock.Verify(r => r.Remove(ledger), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredLedgers()
    {
        var result = await _mock.Object.FindAsync(l => l.Type == LedgerType.Asset);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(l => l.Type == LedgerType.Income);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnLedger_WhenFound()
    {
        var expected = _ledgers[0];
        _mock.Setup(r => r.GetByCodeAsync(expected.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByCodeAsync(expected.Code);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByCodeAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger?)null);
        var result = await _mock.Object.GetByCodeAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithEntriesAsync_ShouldReturnLedgerWithEntries()
    {
        var ledger = _ledgers[0];
        ledger.Entries.Add(CreateLedgerEntry(ledger.Id));
        _mock.Setup(r => r.GetByIdWithEntriesAsync(ledger.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ledger);
        var result = await _mock.Object.GetByIdWithEntriesAsync(ledger.Id);
        result.Should().Be(ledger);
        result!.Entries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetActiveLedgersAsync_ShouldReturnOnlyActiveLedgers()
    {
        var active = _ledgers.Where(l => l.IsActive).ToList();
        _mock.Setup(r => r.GetActiveLedgersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(active);
        var result = await _mock.Object.GetActiveLedgersAsync();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(l => l.IsActive.Should().BeTrue());
    }
}

public class SettlementRepositoryTests
{
    private static int _counter;

    private static SettlementBatch CreateBatch(SettlementStatus status = SettlementStatus.Pending, decimal total = 10000)
    {
        _counter++;
        return new SettlementBatch
        {
            Id = Guid.NewGuid(),
            BatchNumber = $"STL-TEST-{_counter:D5}",
            TotalAmount = total,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Settlement CreateSettlement(Guid batchId, Guid paymentId, decimal amount = 5000)
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

    private readonly Mock<ISettlementRepository> _mock;
    private readonly List<SettlementBatch> _batches;

    public SettlementRepositoryTests()
    {
        _batches =
        [
            CreateBatch(SettlementStatus.Pending, 10000),
            CreateBatch(SettlementStatus.Completed, 25000),
            CreateBatch(SettlementStatus.Failed, 5000)
        ];
        _mock = MockRepositoryBuilder.CreateSettlementRepository(_batches);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBatch_WhenFound()
    {
        var expected = _batches[0];
        var result = await _mock.Object.GetByIdAsync(expected.Id);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBatches()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAndReturnBatch()
    {
        var batch = CreateBatch();
        var result = await _mock.Object.AddAsync(batch);
        result.Should().Be(batch);
    }

    [Fact]
    public void Update_ShouldWork()
    {
        var batch = _batches[0];
        _mock.Object.Update(batch);
        _mock.Verify(r => r.Update(batch), Times.Once);
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var batch = _batches[0];
        _mock.Object.Remove(batch);
        _mock.Verify(r => r.Remove(batch), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var result = await _mock.Object.CountAsync();
        result.Should().Be(3);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredBatches()
    {
        var result = await _mock.Object.FindAsync(b => b.Status == SettlementStatus.Completed);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue()
    {
        var result = await _mock.Object.AnyAsync(b => b.Status == SettlementStatus.Completed);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByBatchNumberAsync_ShouldReturnBatch_WhenFound()
    {
        var expected = _batches[0];
        _mock.Setup(r => r.GetByBatchNumberAsync(expected.BatchNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByBatchNumberAsync(expected.BatchNumber);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByBatchNumberAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByBatchNumberAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementBatch?)null);
        var result = await _mock.Object.GetByBatchNumberAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithSettlementsAsync_ShouldReturnBatchWithSettlements()
    {
        var batch = _batches[0];
        var paymentId = Guid.NewGuid();
        batch.Settlements.Add(CreateSettlement(batch.Id, paymentId));
        _mock.Setup(r => r.GetByIdWithSettlementsAsync(batch.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        var result = await _mock.Object.GetByIdWithSettlementsAsync(batch.Id);
        result.Should().Be(batch);
        result!.Settlements.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSettlementsByBatchIdAsync_ShouldReturnSettlementsForBatch()
    {
        var batchId = _batches[0].Id;
        var paymentId = Guid.NewGuid();
        var settlements = new List<Settlement>
        {
            CreateSettlement(batchId, paymentId, 5000),
            CreateSettlement(batchId, Guid.NewGuid(), 5000)
        };
        _mock.Setup(r => r.GetSettlementsByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlements);
        var result = await _mock.Object.GetSettlementsByBatchIdAsync(batchId);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(s => s.SettlementBatchId.Should().Be(batchId));
    }
}
