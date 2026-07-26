using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteBranch;

public class DeleteBranchCommand : IRequest<Result<Unit>>
{
    public Guid BranchId { get; set; }
}
