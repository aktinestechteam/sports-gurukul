using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public record MarkInvoiceAsPaidCommand(Guid InvoiceId) : IRequest<Result<InvoiceDto>>;
