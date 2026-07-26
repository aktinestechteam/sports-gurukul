using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateSocialLinks;

public class UpdateSocialLinksCommand : IRequest<Result<IReadOnlyList<SocialLinkDto>>>
{
    public Guid AcademyId { get; set; }
    public List<SocialLinkInput> Links { get; set; } = [];
}

public class SocialLinkInput
{
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
