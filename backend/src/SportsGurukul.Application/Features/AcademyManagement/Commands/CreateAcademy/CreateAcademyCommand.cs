using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;

public class CreateAcademyCommand : IRequest<Result<AcademyDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Description { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? GSTNumber { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public string? Website { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
