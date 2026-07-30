using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetAllScholarshipsQueryHandler : IRequestHandler<GetAllScholarshipsQuery, Result<IReadOnlyList<ScholarshipDto>>>
{
    public async Task<Result<IReadOnlyList<ScholarshipDto>>> Handle(GetAllScholarshipsQuery request, CancellationToken cancellationToken)
    {
        // Placeholder: would fetch from scholarship repository
        return Result<IReadOnlyList<ScholarshipDto>>.Success(new List<ScholarshipDto>());
    }
}
