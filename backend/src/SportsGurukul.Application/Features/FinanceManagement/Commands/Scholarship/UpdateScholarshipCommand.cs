using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public record UpdateScholarshipCommand(Guid ScholarshipId, string? Name, string? Description, decimal? Value, decimal? MaxAmount, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<Result<ScholarshipDto>>;
