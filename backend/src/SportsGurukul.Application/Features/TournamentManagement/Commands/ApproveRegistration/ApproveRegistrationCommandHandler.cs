using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.ApproveRegistration;

public class ApproveRegistrationCommandHandler : IRequestHandler<ApproveRegistrationCommand, Result<Unit>>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveRegistrationCommandHandler> _logger;

    public ApproveRegistrationCommandHandler(
        IRegistrationRepository registrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApproveRegistrationCommandHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ApproveRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving registration: {RegistrationId}", request.RegistrationId);

        var registration = await _registrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
            return Result<Unit>.Failure("Registration not found.");

        if (registration.RegistrationStatus != TournamentRegistrationStatus.Pending)
            return Result<Unit>.Failure("Only pending registrations can be approved.");

        registration.RegistrationStatus = TournamentRegistrationStatus.Approved;
        _registrationRepository.Update(registration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration approved: {RegistrationId}", request.RegistrationId);
        return Result<Unit>.Success(Unit.Value);
    }
}
