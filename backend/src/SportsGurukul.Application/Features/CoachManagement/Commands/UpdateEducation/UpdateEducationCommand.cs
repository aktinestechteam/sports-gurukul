using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateEducation;

public class UpdateEducationCommand : IRequest<Result<EducationDto>>
{
    public Guid EducationId { get; set; }
    public string? Degree { get; set; }
    public string? Institution { get; set; }
    public string? FieldOfStudy { get; set; }
    public int? YearCompleted { get; set; }
}
