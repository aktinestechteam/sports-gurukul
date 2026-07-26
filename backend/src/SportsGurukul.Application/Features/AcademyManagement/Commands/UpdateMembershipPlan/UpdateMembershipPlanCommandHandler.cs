using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateMembershipPlan;

public class UpdateMembershipPlanCommandHandler : IRequestHandler<UpdateMembershipPlanCommand, Result<MembershipPlanDto>>
{
    private readonly IAcademyMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMembershipPlanCommandHandler> _logger;

    public UpdateMembershipPlanCommandHandler(
        IAcademyMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMembershipPlanCommandHandler> logger)
    {
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<MembershipPlanDto>> Handle(UpdateMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating membership plan with Id: {MembershipId}", request.MembershipId);

        var membership = await _membershipRepository.GetByIdAsync(request.MembershipId, cancellationToken);
        if (membership is null)
            return Result<MembershipPlanDto>.Failure("Membership plan not found.");

        if (membership.IsDeleted)
            return Result<MembershipPlanDto>.Failure("Membership plan is deleted.");

        if (request.MembershipName is not null)
            membership.MembershipName = request.MembershipName;

        if (request.Description is not null)
            membership.Description = request.Description;

        if (request.Price.HasValue)
            membership.Price = request.Price.Value;

        if (request.Duration.HasValue)
            membership.Duration = request.Duration.Value;

        if (request.Benefits is not null)
            membership.Benefits = request.Benefits;

        membership.UpdatedAt = DateTime.UtcNow;

        _membershipRepository.Update(membership);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Membership plan updated with Id: {MembershipId}", request.MembershipId);

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
