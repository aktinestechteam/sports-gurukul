using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public record ApproveScholarshipCommand(Guid ScholarshipId) : IRequest<Result<ScholarshipDto>>;
