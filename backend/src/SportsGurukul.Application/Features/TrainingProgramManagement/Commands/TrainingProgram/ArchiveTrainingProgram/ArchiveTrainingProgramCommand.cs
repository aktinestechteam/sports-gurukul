using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.ArchiveTrainingProgram;

public class ArchiveTrainingProgramCommand : IRequest<Result<TrainingProgramDto>>
{
    public Guid Id { get; set; }
}
