using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateContact;

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result<ContactDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContactCommandHandler> _logger;

    public UpdateContactCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateContactCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContactDto>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating contact for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<ContactDto>.Failure("Academy not found.");

        var contact = academy.Contact;

        if (contact is null)
        {
            contact = new AcademyContact
            {
                Id = Guid.NewGuid(),
                AcademyId = request.AcademyId,
                PrimaryContactName = request.PrimaryContactName,
                PrimaryPhone = request.PrimaryPhone,
                PrimaryEmail = request.PrimaryEmail,
                SecondaryContactName = request.SecondaryContactName,
                SecondaryPhone = request.SecondaryPhone,
                SecondaryEmail = request.SecondaryEmail,
                Address = request.Address,
                Country = request.Country,
                State = request.State,
                City = request.City,
                PostalCode = request.PostalCode,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            academy.Contact = contact;
        }
        else
        {
            if (request.PrimaryContactName is not null)
                contact.PrimaryContactName = request.PrimaryContactName;

            if (request.PrimaryPhone is not null)
                contact.PrimaryPhone = request.PrimaryPhone;

            if (request.PrimaryEmail is not null)
                contact.PrimaryEmail = request.PrimaryEmail;

            if (request.SecondaryContactName is not null)
                contact.SecondaryContactName = request.SecondaryContactName;

            if (request.SecondaryPhone is not null)
                contact.SecondaryPhone = request.SecondaryPhone;

            if (request.SecondaryEmail is not null)
                contact.SecondaryEmail = request.SecondaryEmail;

            if (request.Address is not null)
                contact.Address = request.Address;

            if (request.Country is not null)
                contact.Country = request.Country;

            if (request.State is not null)
                contact.State = request.State;

            if (request.City is not null)
                contact.City = request.City;

            if (request.PostalCode is not null)
                contact.PostalCode = request.PostalCode;

            if (request.Latitude.HasValue)
                contact.Latitude = request.Latitude.Value;

            if (request.Longitude.HasValue)
                contact.Longitude = request.Longitude.Value;

            contact.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact updated for academy: {AcademyId}", request.AcademyId);

        var dto = new ContactDto
        {
            Id = contact.Id,
            AcademyId = contact.AcademyId,
            PrimaryContactName = contact.PrimaryContactName,
            PrimaryPhone = contact.PrimaryPhone,
            PrimaryEmail = contact.PrimaryEmail,
            SecondaryContactName = contact.SecondaryContactName,
            SecondaryPhone = contact.SecondaryPhone,
            SecondaryEmail = contact.SecondaryEmail,
            Address = contact.Address,
            Country = contact.Country,
            State = contact.State,
            City = contact.City,
            PostalCode = contact.PostalCode,
            Latitude = contact.Latitude,
            Longitude = contact.Longitude,
            CreatedAt = contact.CreatedAt,
            UpdatedAt = contact.UpdatedAt
        };

        return Result<ContactDto>.Success(dto);
    }
}
