using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class UpdateAttendanceCommandValidator : AbstractValidator<UpdateAttendanceCommand>
{
    public UpdateAttendanceCommandValidator()
    {
        RuleFor(x => x.AttendanceId)
            .NotEmpty().WithMessage("Attendance ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid attendance status.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.");
    }
}
