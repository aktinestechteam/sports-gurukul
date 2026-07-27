using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.UpdateTrainingSession;

public record UpdateTrainingSessionCommand(
    Guid Id,
    string SessionTitle,
    SessionType SessionType,
    DateTime SessionDate,
    TimeSpan StartTime,
    TimeSpan EndTime
) : IRequest<Result<TrainingSessionDto>>;
