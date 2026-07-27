using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CompleteTrainingSession;

public record CompleteTrainingSessionCommand(
    Guid Id
) : IRequest<Result<TrainingSessionDto>>;
