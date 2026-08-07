using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetMyAcademy;

public class GetMyAcademyQueryHandler : IRequestHandler<GetMyAcademyQuery, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly ILogger<GetMyAcademyQueryHandler> _logger;

    public GetMyAcademyQueryHandler(
        IAcademyRepository academyRepository,
        ILogger<GetMyAcademyQueryHandler> logger)
    {
        _academyRepository = academyRepository;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(GetMyAcademyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching academy for user: {UserId}", request.UserId);

        var academy = await _academyRepository.GetByOwnerUserIdAsync(request.UserId, cancellationToken);
        if (academy is null)
            return Result<AcademyDto>.Failure("Academy not found.");

        var dto = AcademyDtoMapper.Map(academy);

        dto.Branches = academy.Branches?.Select(b => new BranchDto
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
        }).ToList() ?? [];

        dto.Sports = academy.AcademySports?.Select(s => new AcademySportDto
        {
            Id = s.Id,
            SportId = s.SportId,
            Name = s.Sport?.Name ?? string.Empty,
            Code = s.Sport?.Code ?? string.Empty,
            CategoryName = s.Sport?.SportCategory?.Name,
            OlympicSport = s.Sport?.OlympicSport ?? false,
            IsPrimarySport = s.IsPrimarySport,
            JoinedDate = s.JoinedDate
        }).ToList() ?? [];

        return Result<AcademyDto>.Success(dto);
    }
}
