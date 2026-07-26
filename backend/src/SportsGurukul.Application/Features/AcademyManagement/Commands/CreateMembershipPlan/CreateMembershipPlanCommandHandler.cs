using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;

public class CreateMembershipPlanCommandHandler : IRequestHandler<CreateMembershipPlanCommand, Result<MembershipPlanDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyMembershipRepository _academyMembershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateMembershipPlanCommandHandler> _logger;

    public CreateMembershipPlanCommandHandler(
        IAcademyRepository academyRepository,
        IAcademyMembershipRepository academyMembershipRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateMembershipPlanCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _academyMembershipRepository = academyMembershipRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<MembershipPlanDto>> Handle(CreateMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating membership plan '{MembershipName}' for AcademyId: {AcademyId}", request.MembershipName, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId);
        if (academy is null)
            return Result<MembershipPlanDto>.Failure("Academy not found.");

        var existingPlans = await _academyMembershipRepository.GetByAcademyIdAsync(request.AcademyId, cancellationToken);
        if (existingPlans.Any(p => p.MembershipName.Equals(request.MembershipName, StringComparison.OrdinalIgnoreCase)))
            return Result<MembershipPlanDto>.Failure("A membership plan with the same name already exists for this academy.");

        var membershipPlan = new AcademyMembership
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            MembershipName = request.MembershipName,
            Description = request.Description,
            Price = request.Price,
            Duration = request.Duration,
            Benefits = request.Benefits,
            Status = AcademyMembershipStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _academyMembershipRepository.AddAsync(membershipPlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Membership plan created with Id: {MembershipPlanId}, Name: {MembershipName}", membershipPlan.Id, membershipPlan.MembershipName);

        return Result<MembershipPlanDto>.Success(new MembershipPlanDto
        {
            Id = membershipPlan.Id,
            AcademyId = membershipPlan.AcademyId,
            MembershipName = membershipPlan.MembershipName,
            Description = membershipPlan.Description,
            Price = membershipPlan.Price,
            Duration = membershipPlan.Duration,
            Benefits = membershipPlan.Benefits,
            Status = membershipPlan.Status.ToString(),
            CreatedAt = membershipPlan.CreatedAt,
            UpdatedAt = membershipPlan.UpdatedAt
        });
    }
}
