using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.RescheduleTrainingSession;

public record RescheduleTrainingSessionCommand(
    Guid Id,
    DateTime SessionDate,
    TimeSpan StartTime,
    TimeSpan EndTime
) : IRequest<Result<TrainingSessionDto>>;
