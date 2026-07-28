using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetRegistrationsByEvent;

public class GetRegistrationsByEventQueryHandler : IRequestHandler<GetRegistrationsByEventQuery, Result<PagedResult<RegistrationDto>>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly ILogger<GetRegistrationsByEventQueryHandler> _logger;

    public GetRegistrationsByEventQueryHandler(
        IEventRegistrationRepository registrationRepository,
        ILogger<GetRegistrationsByEventQueryHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<RegistrationDto>>> Handle(GetRegistrationsByEventQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting registrations for event: {EventId}, Status={Status}, Page={Page}, PageSize={PageSize}",
            request.EventId, request.Status, request.Page, request.PageSize);

        var registrations = await _registrationRepository.SearchAsync(
            request.EventId,
            request.Status,
            null,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _registrationRepository.CountSearchAsync(
            request.EventId,
            request.Status,
            null,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var items = registrations.Select(r => new RegistrationDto
        {
            Id = r.Id,
            EventId = r.EventId,
            RegistrationNumber = r.RegistrationNumber,
            Status = r.Status.ToString(),
            AmountPaid = r.AmountPaid,
            PaymentReference = r.PaymentReference,
            Notes = r.Notes,
            RegistrationDate = r.RegistrationDate,
            ApprovalDate = r.ApprovalDate,
            RejectionReason = r.RejectionReason,
            WaitlistPosition = r.WaitlistPosition,
            CreatedAt = r.CreatedAt
        }).ToList();

        var result = new PagedResult<RegistrationDto>
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<RegistrationDto>>.Success(result);
    }
}
