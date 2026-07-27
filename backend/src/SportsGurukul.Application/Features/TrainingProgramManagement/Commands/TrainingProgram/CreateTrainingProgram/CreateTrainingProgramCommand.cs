using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;

public class CreateTrainingProgramCommand : IRequest<Result<TrainingProgramDto>>
{
    public string ProgramName { get; set; } = string.Empty;
    public Guid SportId { get; set; }
    public Guid AcademyId { get; set; }
    public string? Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    public int DurationWeeks { get; set; }
    public int Capacity { get; set; }
}
