using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;

public class DeleteEducationCommand : IRequest<Result<Unit>>
{
    public Guid EducationId { get; set; }
}
