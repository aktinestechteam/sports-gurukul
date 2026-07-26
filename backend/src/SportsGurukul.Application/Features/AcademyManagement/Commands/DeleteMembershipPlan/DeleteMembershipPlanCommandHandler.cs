using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteMembershipPlan;

public class DeleteMembershipPlanCommandHandler : IRequestHandler<DeleteMembershipPlanCommand, Result<Unit>>
{
    private readonly IAcademyMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMembershipPlanCommandHandler> _logger;

    public DeleteMembershipPlanCommandHandler(
        IAcademyMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteMembershipPlanCommandHandler> logger)
    {
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting membership plan with Id: {MembershipId}", request.MembershipId);

        var membership = await _membershipRepository.GetByIdAsync(request.MembershipId, cancellationToken);
        if (membership is null)
            return Result<Unit>.Failure("Membership plan not found.");

        if (membership.IsDeleted)
            return Result<Unit>.Failure("Membership plan is already deleted.");

        _membershipRepository.Remove(membership);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Membership plan deleted with Id: {MembershipId}", request.MembershipId);

        return Result<Unit>.Success(Unit.Value);
    }
}
