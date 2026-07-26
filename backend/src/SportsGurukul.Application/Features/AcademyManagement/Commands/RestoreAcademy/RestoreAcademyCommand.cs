using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;

public class RestoreAcademyCommand : IRequest<Result<Unit>>
{
    public Guid AcademyId { get; set; }
}
