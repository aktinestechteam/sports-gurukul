using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IDiscountService _discountService;
    private readonly ITaxCalculationService _taxCalculationService;
    private readonly ILedgerService _ledgerService;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        ICouponRepository couponRepository,
        IDiscountService discountService,
        ITaxCalculationService taxCalculationService,
        ILedgerService ledgerService)
    {
        _invoiceRepository = invoiceRepository;
        _couponRepository = couponRepository;
        _discountService = discountService;
        _taxCalculationService = taxCalculationService;
        _ledgerService = ledgerService;
    }

    public async Task<Result<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = new Invoice
        {
            AthleteId = request.AthleteId,
            AcademyId = request.AcademyId,
            Notes = request.Description,
            DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(30),
            Currency = request.Currency ?? "INR",
            Status = InvoiceStatus.Draft,
            IssueDate = DateTime.UtcNow,
        };

        foreach (var item in request.LineItems)
        {
            invoice.Items.Add(new InvoiceItem
            {
                Description = item.Description ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalAmount = item.Quantity * item.UnitPrice,
                ReferenceType = item.ItemType,
                ReferenceId = string.IsNullOrEmpty(item.ItemReference) ? null : Guid.TryParse(item.ItemReference, out var refId) ? refId : null,
            });
        }

        invoice.SubTotal = invoice.Items.Sum(x => x.TotalAmount);

        if (!string.IsNullOrEmpty(request.CouponCode))
        {
            var discountResult = await _discountService.ApplyDiscountAsync(invoice.SubTotal, request.CouponCode, cancellationToken: cancellationToken);
            if (!discountResult.IsSuccess)
                return Result<InvoiceDto>.Failure(discountResult.Error!);

            invoice.DiscountTotal = discountResult.Value.DiscountAmount;
        }

        if (request.ScholarshipId.HasValue)
        {
            var scholarshipResult = await _discountService.ApplyScholarshipAsync(invoice.SubTotal, request.ScholarshipId.Value, cancellationToken);
            if (!scholarshipResult.IsSuccess)
                return Result<InvoiceDto>.Failure(scholarshipResult.Error!);

            invoice.DiscountTotal += scholarshipResult.Value.DiscountAmount;
        }

        var taxResult = await _taxCalculationService.CalculateInvoiceTaxesAsync(invoice.SubTotal - invoice.DiscountTotal, invoice.Currency, cancellationToken);
        if (!taxResult.IsSuccess)
            return Result<InvoiceDto>.Failure(taxResult.Error!);

        invoice.TaxTotal = taxResult.Value.Sum(t => t.TaxAmount);
        invoice.Total = invoice.SubTotal - invoice.DiscountTotal + invoice.TaxTotal;
        invoice.AmountDue = invoice.Total;

        var created = await _invoiceRepository.AddAsync(invoice, cancellationToken);
        return Result<InvoiceDto>.Success(MapToDto(created));
    }

    public async Task<Result<InvoiceDto>> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceDto>.Failure("Invoice not found");

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<InvoiceDto>.Failure("Only draft invoices can be updated");

        if (request.Description is not null)
            invoice.Notes = request.Description;
        if (request.DueDate.HasValue)
            invoice.DueDate = request.DueDate.Value;

        if (request.LineItems is not null)
        {
            invoice.Items.Clear();
            foreach (var item in request.LineItems)
            {
                invoice.Items.Add(new InvoiceItem
                {
                    Description = item.Description ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalAmount = item.Quantity * item.UnitPrice,
                    ReferenceType = item.ItemType,
                    ReferenceId = string.IsNullOrEmpty(item.ItemReference) ? null : Guid.TryParse(item.ItemReference, out var refId) ? refId : null,
                });
            }

            invoice.SubTotal = invoice.Items.Sum(x => x.TotalAmount);
            invoice.Total = invoice.SubTotal - invoice.DiscountTotal + invoice.TaxTotal;
            invoice.AmountDue = invoice.Total;
        }

        _invoiceRepository.Update(invoice);
        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<InvoiceDto>> IssueInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceDto>.Failure("Invoice not found");

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<InvoiceDto>.Failure("Only draft invoices can be issued");

        invoice.Status = InvoiceStatus.Issued;
        invoice.InvoiceNumber = await GenerateInvoiceNumberInternalAsync(cancellationToken);

        _invoiceRepository.Update(invoice);

        await PostInvoiceLedgerEntries(invoice, cancellationToken);

        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<InvoiceDto>> CancelInvoiceAsync(Guid invoiceId, string reason, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceDto>.Failure("Invoice not found");

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<InvoiceDto>.Failure("Cannot cancel a paid invoice");

        invoice.Status = InvoiceStatus.Cancelled;
        _invoiceRepository.Update(invoice);

        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<InvoiceDto>> MarkAsPaidAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceDto>.Failure("Invoice not found");

        if (invoice.Status != InvoiceStatus.Issued)
            return Result<InvoiceDto>.Failure("Only issued invoices can be marked as paid");

        invoice.Status = InvoiceStatus.Paid;
        invoice.AmountPaid = invoice.Total;
        invoice.AmountDue = 0;

        _invoiceRepository.Update(invoice);
        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<InvoiceDto>> VoidInvoiceAsync(Guid invoiceId, string reason, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceDto>.Failure("Invoice not found");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Result<InvoiceDto>.Failure("Invoice is already voided");

        invoice.Status = InvoiceStatus.Cancelled;
        invoice.Notes = reason;

        _invoiceRepository.Update(invoice);
        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<string>> GenerateInvoiceNumberAsync(CancellationToken cancellationToken)
    {
        var number = await GenerateInvoiceNumberInternalAsync(cancellationToken);
        return Result<string>.Success(number);
    }

    public async Task<Result<InvoiceDto>> GetByIdAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceDto>.Failure("Invoice not found");

        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<IReadOnlyList<InvoiceSummaryDto>>> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Invoice> invoices;
        if (request.Status.HasValue)
            invoices = await _invoiceRepository.GetByStatusAsync(request.Status.Value, cancellationToken);
        else if (request.AthleteId.HasValue)
            invoices = await _invoiceRepository.GetByAthleteIdAsync(request.AthleteId.Value, cancellationToken);
        else if (request.AcademyId.HasValue)
            invoices = await _invoiceRepository.GetByAcademyIdAsync(request.AcademyId.Value, cancellationToken);
        else
            invoices = await _invoiceRepository.GetAllAsync(cancellationToken);

        var filtered = invoices.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(i => (i.InvoiceNumber?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                || (i.Notes?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        if (request.FromDate.HasValue)
            filtered = filtered.Where(i => i.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            filtered = filtered.Where(i => i.CreatedAt <= request.ToDate.Value);

        var paged = filtered.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return Result<IReadOnlyList<InvoiceSummaryDto>>.Success(paged.Select(MapToSummaryDto).ToList());
    }

    public async Task<Result<IReadOnlyList<InvoiceSummaryDto>>> GetOutstandingInvoicesAsync(CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetOverdueInvoicesAsync(cancellationToken);
        return Result<IReadOnlyList<InvoiceSummaryDto>>.Success(invoices.Select(MapToSummaryDto).ToList());
    }

    private async Task<string> GenerateInvoiceNumberInternalAsync(CancellationToken cancellationToken)
    {
        var count = await _invoiceRepository.CountAsync(cancellationToken: cancellationToken);
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D5}";
    }

    private async Task PostInvoiceLedgerEntries(Invoice invoice, CancellationToken cancellationToken)
    {
        var receivableLedger = await _ledgerService.GetOrCreateLedgerAsync("AR", "Accounts Receivable", LedgerType.Asset, "Accounts Receivable", cancellationToken);
        var revenueLedger = await _ledgerService.GetOrCreateLedgerAsync("REV", "Revenue", LedgerType.Income, "Revenue from operations", cancellationToken);
        var taxLedger = await _ledgerService.GetOrCreateLedgerAsync("TAX", "Tax Payable", LedgerType.Liability, "Tax Payable", cancellationToken);

        if (receivableLedger.IsSuccess && revenueLedger.IsSuccess)
        {
            await _ledgerService.PostLedgerEntryAsync(receivableLedger.Value!, new LedgerEntry
            {
                DebitAmount = invoice.Total,
                CreditAmount = 0,
                Description = $"Invoice {invoice.InvoiceNumber}",
                Reference = invoice.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);

            await _ledgerService.PostLedgerEntryAsync(revenueLedger.Value!, new LedgerEntry
            {
                DebitAmount = 0,
                CreditAmount = invoice.SubTotal,
                Description = $"Revenue - Invoice {invoice.InvoiceNumber}",
                Reference = invoice.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);

            if (invoice.TaxTotal > 0 && taxLedger.IsSuccess)
            {
                await _ledgerService.PostLedgerEntryAsync(taxLedger.Value!, new LedgerEntry
                {
                    DebitAmount = 0,
                    CreditAmount = invoice.TaxTotal,
                    Description = $"Tax - Invoice {invoice.InvoiceNumber}",
                    Reference = invoice.Id.ToString(),
                    EntryDate = DateTime.UtcNow,
                }, cancellationToken);
            }
        }
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber ?? string.Empty,
            invoice.AthleteId,
            invoice.AcademyId,
            null,
            null,
            invoice.Notes,
            invoice.SubTotal,
            invoice.TaxTotal,
            invoice.DiscountTotal,
            invoice.Total,
            invoice.AmountPaid,
            invoice.AmountDue,
            invoice.Status,
            invoice.DueDate,
            invoice.IssueDate,
            null,
            null,
            null,
            invoice.Currency,
            invoice.CreatedAt,
            invoice.Items.Select(li => new InvoiceLineItemDto(
                li.Id, li.Description, li.ReferenceType ?? string.Empty, li.ReferenceId?.ToString(), li.Quantity, li.UnitPrice, li.TotalAmount, null, null, li.TotalAmount
            )).ToList(),
            invoice.InvoicePayments.Select(p => new InvoicePaymentDto(
                p.Id, p.AmountApplied, Domain.Enums.Finance.PaymentMethod.Cash, PaymentStatus.Captured, string.Empty, p.CreatedAt
            )).ToList()
        );
    }

    private static InvoiceSummaryDto MapToSummaryDto(Invoice invoice)
    {
        return new InvoiceSummaryDto(
            invoice.Id,
            invoice.InvoiceNumber ?? string.Empty,
            null,
            invoice.Total,
            invoice.AmountPaid,
            invoice.AmountDue,
            invoice.Status,
            invoice.DueDate,
            invoice.CreatedAt
        );
    }
}
