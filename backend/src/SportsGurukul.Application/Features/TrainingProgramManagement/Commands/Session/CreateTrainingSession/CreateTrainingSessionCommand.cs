using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CreateTrainingSession;

public record CreateTrainingSessionCommand(
    Guid BatchId,
    string SessionTitle,
    SessionType SessionType,
    DateTime SessionDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    Guid? FacilityId,
    Guid CoachId
) : IRequest<Result<TrainingSessionDto>>;
