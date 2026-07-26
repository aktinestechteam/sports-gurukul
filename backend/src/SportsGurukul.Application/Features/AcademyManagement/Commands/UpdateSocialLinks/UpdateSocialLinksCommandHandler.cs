using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateSocialLinks;

public class UpdateSocialLinksCommandHandler : IRequestHandler<UpdateSocialLinksCommand, Result<IReadOnlyList<SocialLinkDto>>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IRepository<AcademySocialLink> _socialLinkRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSocialLinksCommandHandler> _logger;

    public UpdateSocialLinksCommandHandler(
        IAcademyRepository academyRepository,
        IRepository<AcademySocialLink> socialLinkRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateSocialLinksCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _socialLinkRepository = socialLinkRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SocialLinkDto>>> Handle(UpdateSocialLinksCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating social links for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<IReadOnlyList<SocialLinkDto>>.Failure("Academy not found.");

        var existingLinks = await _socialLinkRepository.FindAsync(
            s => s.AcademyId == request.AcademyId, cancellationToken);

        foreach (var link in existingLinks)
        {
            _socialLinkRepository.Remove(link);
        }

        var newLinks = new List<AcademySocialLink>();

        foreach (var input in request.Links)
        {
            var socialLink = new AcademySocialLink
            {
                Id = Guid.NewGuid(),
                AcademyId = request.AcademyId,
                Platform = input.Platform,
                Url = input.Url
            };

            await _socialLinkRepository.AddAsync(socialLink, cancellationToken);
            newLinks.Add(socialLink);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Social links updated for academy: {AcademyId}. Count: {Count}", request.AcademyId, newLinks.Count);

        var dtos = newLinks.Select(sl => new SocialLinkDto
        {
            Id = sl.Id,
            AcademyId = sl.AcademyId,
            Platform = sl.Platform,
            Url = sl.Url,
            CreatedAt = sl.CreatedAt,
            UpdatedAt = sl.UpdatedAt
        }).ToList();

        return Result<IReadOnlyList<SocialLinkDto>>.Success(dtos);
    }
}
