using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCertification;

public class DeleteCertificationCommand : IRequest<Result<Unit>>
{
    public Guid CertificationId { get; set; }
}
