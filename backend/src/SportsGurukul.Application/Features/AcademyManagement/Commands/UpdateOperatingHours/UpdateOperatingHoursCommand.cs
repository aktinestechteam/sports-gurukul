using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateOperatingHours;

public class UpdateOperatingHoursCommand : IRequest<Result<OperatingHoursDto>>
{
    public Guid AcademyId { get; set; }
    public string? MondayOpening { get; set; }
    public string? MondayClosing { get; set; }
    public string? TuesdayOpening { get; set; }
    public string? TuesdayClosing { get; set; }
    public string? WednesdayOpening { get; set; }
    public string? WednesdayClosing { get; set; }
    public string? ThursdayOpening { get; set; }
    public string? ThursdayClosing { get; set; }
    public string? FridayOpening { get; set; }
    public string? FridayClosing { get; set; }
    public string? SaturdayOpening { get; set; }
    public string? SaturdayClosing { get; set; }
    public string? SundayOpening { get; set; }
    public string? SundayClosing { get; set; }
    public string? HolidaySchedule { get; set; }
}
