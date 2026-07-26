using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.AddFacilityImage;

public class AddFacilityImageCommand : IRequest<Result<ImageDto>>
{
    public Guid FacilityId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
}
