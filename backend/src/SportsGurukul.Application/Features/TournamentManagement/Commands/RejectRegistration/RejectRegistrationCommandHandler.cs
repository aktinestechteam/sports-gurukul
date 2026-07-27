using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RejectRegistration;

public class RejectRegistrationCommandHandler : IRequestHandler<RejectRegistrationCommand, Result<Unit>>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectRegistrationCommandHandler> _logger;

    public RejectRegistrationCommandHandler(
        IRegistrationRepository registrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectRegistrationCommandHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RejectRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting registration: {RegistrationId}, Reason: {Reason}", request.RegistrationId, request.Reason);

        var registration = await _registrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
            return Result<Unit>.Failure("Registration not found.");

        if (registration.RegistrationStatus != TournamentRegistrationStatus.Pending)
            return Result<Unit>.Failure("Only pending registrations can be rejected.");

        registration.RegistrationStatus = TournamentRegistrationStatus.Rejected;
        _registrationRepository.Update(registration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration rejected: {RegistrationId}", request.RegistrationId);
        return Result<Unit>.Success(Unit.Value);
    }
}
