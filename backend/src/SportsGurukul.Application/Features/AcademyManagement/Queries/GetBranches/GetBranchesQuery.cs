using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetBranches;

public class GetBranchesQuery : IRequest<Result<IReadOnlyList<BranchDto>>>
{
    public Guid AcademyId { get; set; }
}
