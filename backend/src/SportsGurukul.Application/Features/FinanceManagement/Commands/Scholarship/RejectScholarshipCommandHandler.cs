using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public class RejectScholarshipCommandHandler : IRequestHandler<RejectScholarshipCommand, Result<ScholarshipDto>>
{
    public async Task<Result<ScholarshipDto>> Handle(RejectScholarshipCommand request, CancellationToken cancellationToken)
    {
        // Placeholder: would update scholarship status
        return Result<ScholarshipDto>.Success(new ScholarshipDto(
            request.ScholarshipId, Guid.Empty, null, "Rejected", request.Reason,
            DiscountType.Percentage, 0, null, null, null, false, DateTime.UtcNow
        ));
    }
}
