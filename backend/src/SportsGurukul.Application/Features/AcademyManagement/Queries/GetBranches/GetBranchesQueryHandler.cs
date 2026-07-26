using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetBranches;

public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, Result<IReadOnlyList<BranchDto>>>
{
    private readonly IAcademyBranchRepository _branchRepository;
    private readonly ILogger<GetBranchesQueryHandler> _logger;

    public GetBranchesQueryHandler(
        IAcademyBranchRepository branchRepository,
        ILogger<GetBranchesQueryHandler> logger)
    {
        _branchRepository = branchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching branches for academy: {AcademyId}", request.AcademyId);

        var branches = await _branchRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);

        var dtos = branches.Select(b => new BranchDto
        {
            Id = b.Id,
            AcademyId = b.AcademyId,
            BranchName = b.BranchName,
            Address = b.Address,
            Country = b.Country,
            State = b.State,
            City = b.City,
            District = b.District,
            PostalCode = b.PostalCode,
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} branches for academy: {AcademyId}", dtos.Count, request.AcademyId);

        return Result<IReadOnlyList<BranchDto>>.Success(dtos);
    }
}
