using FluentValidation;
using PadelBooking.Api.DTOs.FixedReservations;

namespace PadelBooking.Api.Validators.FixedReservations;

public class CreateFixedReservationRequestValidator : AbstractValidator<CreateFixedReservationRequest>
{
    public CreateFixedReservationRequestValidator()
    {
        RuleFor(x => x.CourtId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DayOfWeek).InclusiveBetween(0, 6);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue).WithMessage("End date must be after start date.");
    }
}

public class UpdateFixedReservationRequestValidator : AbstractValidator<UpdateFixedReservationRequest>
{
    public UpdateFixedReservationRequestValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DayOfWeek).InclusiveBetween(0, 6);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue).WithMessage("End date must be after start date.");
    }
}
