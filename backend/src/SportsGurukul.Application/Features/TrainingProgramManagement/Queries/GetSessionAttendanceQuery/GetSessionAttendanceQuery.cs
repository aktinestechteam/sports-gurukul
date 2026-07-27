using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionAttendanceQuery
{
    public class GetSessionAttendanceQuery : IRequest<Result<IReadOnlyList<AttendanceDto>>>
    {
        public Guid SessionId { get; set; }
    }
}
