using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeactivateMembershipPlan;

public class DeactivateMembershipPlanCommandHandler : IRequestHandler<DeactivateMembershipPlanCommand, Result<MembershipPlanDto>>
{
    private readonly IAcademyMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateMembershipPlanCommandHandler> _logger;

    public DeactivateMembershipPlanCommandHandler(
        IAcademyMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateMembershipPlanCommandHandler> logger)
    {
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<MembershipPlanDto>> Handle(DeactivateMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating membership plan with Id: {MembershipId}", request.MembershipId);

        var membership = await _membershipRepository.GetByIdAsync(request.MembershipId, cancellationToken);
        if (membership is null)
            return Result<MembershipPlanDto>.Failure("Membership plan not found.");

        if (membership.IsDeleted)
            return Result<MembershipPlanDto>.Failure("Membership plan is deleted.");

        membership.Status = AcademyMembershipStatus.Inactive;
        membership.UpdatedAt = DateTime.UtcNow;

        _membershipRepository.Update(membership);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Membership plan deactivated with Id: {MembershipId}", request.MembershipId);

        return Result<MembershipPlanDto>.Success(MapToDto(membership));
    }

    private static MembershipPlanDto MapToDto(Domain.Entities.AcademyMembership membership)
    {
        return new MembershipPlanDto
        {
            Id = membership.Id,
            AcademyId = membership.AcademyId,
            MembershipName = membership.MembershipName,
            Description = membership.Description,
            Price = membership.Price,
            Duration = membership.Duration,
            Benefits = membership.Benefits,
            Status = membership.Status.ToString(),
            CreatedAt = membership.CreatedAt,
            UpdatedAt = membership.UpdatedAt
        };
    }
}
