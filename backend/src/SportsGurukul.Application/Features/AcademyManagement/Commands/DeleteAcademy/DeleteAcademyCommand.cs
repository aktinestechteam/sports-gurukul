using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteAcademy;

public class DeleteAcademyCommand : IRequest<Result<Unit>>
{
    public Guid AcademyId { get; set; }
}
