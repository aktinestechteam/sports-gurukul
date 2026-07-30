using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public class UpdateScholarshipCommandHandler : IRequestHandler<UpdateScholarshipCommand, Result<ScholarshipDto>>
{
    public async Task<Result<ScholarshipDto>> Handle(UpdateScholarshipCommand request, CancellationToken cancellationToken)
    {
        // Placeholder: would use a scholarship repository/service
        return Result<ScholarshipDto>.Success(new ScholarshipDto(
            request.ScholarshipId, Guid.Empty, null, request.Name, request.Description,
            Domain.Enums.Finance.DiscountType.Percentage, request.Value ?? 0, request.MaxAmount,
            request.ValidFrom, request.ValidTo, true, DateTime.UtcNow
        ));
    }
}
