using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public record CreateScholarshipCommand(Guid AthleteId, string? Name, string? Description, DiscountType DiscountType, decimal Value, decimal? MaxAmount, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<Result<ScholarshipDto>>;
