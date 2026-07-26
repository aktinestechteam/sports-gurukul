using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateAcademy;

public class UpdateAcademyCommand : IRequest<Result<AcademyDto>>
{
    public Guid AcademyId { get; set; }
    public string? Name { get; set; }
    public string? LegalName { get; set; }
    public string? Description { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? GSTNumber { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
}
