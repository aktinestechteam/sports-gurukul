using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public record UpdateInvoiceCommand(Guid InvoiceId, string? Description, DateTime? DueDate, List<CreateInvoiceLineItemDto>? LineItems) : IRequest<Result<InvoiceDto>>;
