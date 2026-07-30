using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Validators;

public class CreateInvoiceCommandValidatorTests
{
    private readonly CreateInvoiceCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyLineItems_ShouldHaveError()
    {
        var command = new CreateInvoiceCommand(
            null, null, null, null, null,
            new List<CreateInvoiceLineItemDto>(),
            null, null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LineItems);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new CreateInvoiceCommand(
            "Test", DateTime.UtcNow.AddDays(10), "INR", Guid.NewGuid(), null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Tuition", null, 1, 5000, null)
            },
            null, null);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PastDueDate_ShouldHaveError()
    {
        var command = new CreateInvoiceCommand(
            "Test", DateTime.UtcNow.AddDays(-1), "INR", Guid.NewGuid(), null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Tuition", null, 1, 5000, null)
            },
            null, null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }
}

public class CancelInvoiceCommandValidatorTests
{
    private readonly CancelInvoiceCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyReason_ShouldHaveError()
    {
        var command = new CancelInvoiceCommand(Guid.NewGuid(), string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Validate_ValidReason_ShouldNotHaveError()
    {
        var command = new CancelInvoiceCommand(Guid.NewGuid(), "Customer requested cancellation");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class InitiatePaymentCommandValidatorTests
{
    private readonly InitiatePaymentCommandValidator _validator = new();

    [Fact]
    public void Validate_ZeroAmount_ShouldHaveError()
    {
        var command = new InitiatePaymentCommand(Guid.NewGuid(), 0, PaymentMethod.UPI, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_EmptyInvoiceId_ShouldHaveError()
    {
        var command = new InitiatePaymentCommand(Guid.Empty, 100, PaymentMethod.UPI, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.InvoiceId);
    }

    [Fact]
    public void Validate_InvalidPaymentMethod_ShouldHaveError()
    {
        var command = new InitiatePaymentCommand(Guid.NewGuid(), 100, (PaymentMethod)999, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new InitiatePaymentCommand(Guid.NewGuid(), 5000, PaymentMethod.UPI, "key-123", "Test payment");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CancelPaymentCommandValidatorTests
{
    private readonly CancelPaymentCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyReason_ShouldHaveError()
    {
        var command = new CancelPaymentCommand(Guid.NewGuid(), string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Validate_ValidReason_ShouldNotHaveError()
    {
        var command = new CancelPaymentCommand(Guid.NewGuid(), "Payment cancelled by user");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class RecordOfflinePaymentCommandValidatorTests
{
    private readonly RecordOfflinePaymentCommandValidator _validator = new();

    [Fact]
    public void Validate_ZeroAmount_ShouldHaveError()
    {
        var command = new RecordOfflinePaymentCommand(Guid.NewGuid(), 0, PaymentMethod.Cash, null, DateTime.UtcNow, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_EmptyInvoiceId_ShouldHaveError()
    {
        var command = new RecordOfflinePaymentCommand(Guid.Empty, 100, PaymentMethod.Cash, null, DateTime.UtcNow, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.InvoiceId);
    }

    [Fact]
    public void Validate_InvalidPaymentMethod_ShouldHaveError()
    {
        var command = new RecordOfflinePaymentCommand(Guid.NewGuid(), 100, (PaymentMethod)999, null, DateTime.UtcNow, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new RecordOfflinePaymentCommand(Guid.NewGuid(), 5000, PaymentMethod.Cash, "REF-001", DateTime.UtcNow, "Offline payment");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class RequestRefundCommandValidatorTests
{
    private readonly RequestRefundCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyPaymentId_ShouldHaveError()
    {
        var command = new RequestRefundCommand(Guid.Empty, 100, "Damaged item", null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PaymentId);
    }

    [Fact]
    public void Validate_ZeroAmount_ShouldHaveError()
    {
        var command = new RequestRefundCommand(Guid.NewGuid(), 0, "Damaged item", null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new RequestRefundCommand(Guid.NewGuid(), 500, "Damaged item", null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class ApproveRefundCommandValidatorTests
{
    private readonly ApproveRefundCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyApprovedBy_ShouldHaveError()
    {
        var command = new ApproveRefundCommand(Guid.NewGuid(), string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ApprovedBy);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new ApproveRefundCommand(Guid.NewGuid(), "Admin User");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class RejectRefundCommandValidatorTests
{
    private readonly RejectRefundCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyReason_ShouldHaveError()
    {
        var command = new RejectRefundCommand(Guid.NewGuid(), string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Validate_ValidReason_ShouldNotHaveError()
    {
        var command = new RejectRefundCommand(Guid.NewGuid(), "Refund policy does not apply");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreditWalletCommandValidatorTests
{
    private readonly CreditWalletCommandValidator _validator = new();

    [Fact]
    public void Validate_ZeroAmount_ShouldHaveError()
    {
        var command = new CreditWalletCommand(Guid.NewGuid(), 0, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_ValidAmount_ShouldNotHaveError()
    {
        var command = new CreditWalletCommand(Guid.NewGuid(), 1000, "REF-001", "Wallet top-up");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DebitWalletCommandValidatorTests
{
    private readonly DebitWalletCommandValidator _validator = new();

    [Fact]
    public void Validate_ZeroAmount_ShouldHaveError()
    {
        var command = new DebitWalletCommand(Guid.NewGuid(), 0, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_ValidAmount_ShouldNotHaveError()
    {
        var command = new DebitWalletCommand(Guid.NewGuid(), 500, "REF-002", "Wallet debit for purchase");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class TransferWalletBalanceCommandValidatorTests
{
    private readonly TransferWalletBalanceCommandValidator _validator = new();

    [Fact]
    public void Validate_ZeroAmount_ShouldHaveError()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();
        var command = new TransferWalletBalanceCommand(fromId, toId, 0, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_EmptyFromWalletId_ShouldHaveError()
    {
        var command = new TransferWalletBalanceCommand(Guid.Empty, Guid.NewGuid(), 500, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FromWalletId);
    }

    [Fact]
    public void Validate_EmptyToWalletId_ShouldHaveError()
    {
        var command = new TransferWalletBalanceCommand(Guid.NewGuid(), Guid.Empty, 500, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ToWalletId);
    }

    [Fact]
    public void Validate_SameWalletIds_ShouldHaveError()
    {
        var id = Guid.NewGuid();
        var command = new TransferWalletBalanceCommand(id, id, 500, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FromWalletId);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new TransferWalletBalanceCommand(Guid.NewGuid(), Guid.NewGuid(), 500, "Transfer to savings");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateSettlementBatchCommandValidatorTests
{
    private readonly CreateSettlementBatchCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyPaymentIds_ShouldHaveError()
    {
        var command = new CreateSettlementBatchCommand(Array.Empty<Guid>());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PaymentIds);
    }

    [Fact]
    public void Validate_ValidPaymentIds_ShouldNotHaveError()
    {
        var command = new CreateSettlementBatchCommand(new[] { Guid.NewGuid(), Guid.NewGuid() });
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateCouponCommandValidatorTests
{
    private readonly CreateCouponCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyCode_ShouldHaveError()
    {
        var command = new CreateCouponCommand(string.Empty, null, DiscountType.Percentage, 10, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_CodeTooLong_ShouldHaveError()
    {
        var command = new CreateCouponCommand(new string('A', 51), null, DiscountType.Percentage, 10, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_ZeroValue_ShouldHaveError()
    {
        var command = new CreateCouponCommand("SAVE10", null, DiscountType.Percentage, 0, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Validate_InvalidDiscountType_ShouldHaveError()
    {
        var command = new CreateCouponCommand("SAVE10", null, (DiscountType)999, 10, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiscountType);
    }

    [Fact]
    public void Validate_ValidToBeforeValidFrom_ShouldHaveError()
    {
        var command = new CreateCouponCommand("SAVE10", null, DiscountType.Percentage, 10, null, null, null, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(5));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValidTo);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new CreateCouponCommand("SAVE10", "10% off", DiscountType.Percentage, 10, 500, 200, 100, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class ApplyCouponCommandValidatorTests
{
    private readonly ApplyCouponCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyCode_ShouldHaveError()
    {
        var command = new ApplyCouponCommand(string.Empty, null, 1000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_ZeroOrderAmount_ShouldHaveError()
    {
        var command = new ApplyCouponCommand("SAVE10", null, 0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.OrderAmount);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new ApplyCouponCommand("SAVE10", "user-123", 5000);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateScholarshipCommandValidatorTests
{
    private readonly CreateScholarshipCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyAthleteId_ShouldHaveError()
    {
        var command = new CreateScholarshipCommand(Guid.Empty, "Merit", "Top performer", DiscountType.Percentage, 20, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
    }

    [Fact]
    public void Validate_ZeroValue_ShouldHaveError()
    {
        var command = new CreateScholarshipCommand(Guid.NewGuid(), "Merit", "Top performer", DiscountType.Percentage, 0, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Validate_InvalidDiscountType_ShouldHaveError()
    {
        var command = new CreateScholarshipCommand(Guid.NewGuid(), "Merit", "Top performer", (DiscountType)999, 20, null, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DiscountType);
    }

    [Fact]
    public void Validate_ValidToBeforeValidFrom_ShouldHaveError()
    {
        var command = new CreateScholarshipCommand(Guid.NewGuid(), "Merit", "Top performer", DiscountType.Percentage, 20, null, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(5));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValidTo);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveError()
    {
        var command = new CreateScholarshipCommand(Guid.NewGuid(), "Merit", "Top performer", DiscountType.Percentage, 20, 1000, DateTime.UtcNow, DateTime.UtcNow.AddDays(365));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
