using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public record CreateInvoiceCommand(string? Description, DateTime? DueDate, string? Currency, Guid? AthleteId, Guid? AcademyId, List<CreateInvoiceLineItemDto> LineItems, string? CouponCode, Guid? ScholarshipId) : IRequest<Result<InvoiceDto>>;
