using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetMembershipPlans;

public class GetMembershipPlansQueryHandler : IRequestHandler<GetMembershipPlansQuery, Result<IReadOnlyList<MembershipPlanDto>>>
{
    private readonly IAcademyMembershipRepository _membershipRepository;
    private readonly ILogger<GetMembershipPlansQueryHandler> _logger;

    public GetMembershipPlansQueryHandler(
        IAcademyMembershipRepository membershipRepository,
        ILogger<GetMembershipPlansQueryHandler> logger)
    {
        _membershipRepository = membershipRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<MembershipPlanDto>>> Handle(GetMembershipPlansQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching membership plans for academy: {AcademyId}", request.AcademyId);

        var memberships = await _membershipRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);

        var dtos = memberships.Select(m => new MembershipPlanDto
        {
            Id = m.Id,
            AcademyId = m.AcademyId,
            MembershipName = m.MembershipName,
            Description = m.Description,
            Price = m.Price,
            Duration = m.Duration,
            Benefits = m.Benefits,
            Status = m.Status.ToString(),
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} membership plans for academy: {AcademyId}", dtos.Count, request.AcademyId);

        return Result<IReadOnlyList<MembershipPlanDto>>.Success(dtos);
    }
}
