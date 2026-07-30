using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public class ApproveScholarshipCommandHandler : IRequestHandler<ApproveScholarshipCommand, Result<ScholarshipDto>>
{
    public async Task<Result<ScholarshipDto>> Handle(ApproveScholarshipCommand request, CancellationToken cancellationToken)
    {
        // Placeholder: would update scholarship status
        return Result<ScholarshipDto>.Success(new ScholarshipDto(
            request.ScholarshipId, Guid.Empty, null, "Approved", null,
            DiscountType.Percentage, 0, null, null, null, true, DateTime.UtcNow
        ));
    }
}
