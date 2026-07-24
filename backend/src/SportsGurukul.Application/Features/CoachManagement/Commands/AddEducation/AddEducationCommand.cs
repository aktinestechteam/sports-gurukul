using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AddEducation;

public class AddEducationCommand : IRequest<Result<EducationDto>>
{
    public Guid CoachId { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public string? FieldOfStudy { get; set; }
    public int? YearCompleted { get; set; }
}
