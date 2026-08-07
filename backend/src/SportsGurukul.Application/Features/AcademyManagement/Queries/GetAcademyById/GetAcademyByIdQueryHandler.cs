using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyById;

public class GetAcademyByIdQueryHandler : IRequestHandler<GetAcademyByIdQuery, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IRepository<AcademySocialLink> _socialLinkRepository;
    private readonly ILogger<GetAcademyByIdQueryHandler> _logger;

    public GetAcademyByIdQueryHandler(
        IAcademyRepository academyRepository,
        IRepository<AcademySocialLink> socialLinkRepository,
        ILogger<GetAcademyByIdQueryHandler> logger)
    {
        _academyRepository = academyRepository;
        _socialLinkRepository = socialLinkRepository;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(GetAcademyByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting academy with Id: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<AcademyDto>.Failure("Academy not found.");

        var socialLinks = await _socialLinkRepository.FindAsync(
            s => s.AcademyId == request.AcademyId, cancellationToken);

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

        dto.Facilities = academy.Facilities?.Select(f => new FacilityDto
        {
            Id = f.Id,
            AcademyId = f.AcademyId,
            FacilityName = f.FacilityName,
            FacilityType = f.FacilityType.ToString(),
            IndoorOutdoor = f.IndoorOutdoor,
            Capacity = f.Capacity,
            Available = f.Available,
            Description = f.Description,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        }).ToList() ?? [];

        dto.Memberships = academy.Memberships?.Select(m => new MembershipPlanDto
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
        }).ToList() ?? [];

        dto.SocialLinks = socialLinks.Select(sl => new SocialLinkDto
        {
            Id = sl.Id,
            AcademyId = sl.AcademyId,
            Platform = sl.Platform,
            Url = sl.Url,
            CreatedAt = sl.CreatedAt,
            UpdatedAt = sl.UpdatedAt
        }).ToList();

        return Result<AcademyDto>.Success(dto);
    }
}
