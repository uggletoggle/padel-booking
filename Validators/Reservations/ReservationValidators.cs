using FluentValidation;
using PadelBooking.Api.DTOs.Reservations;

namespace PadelBooking.Api.Validators.Reservations;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.CourtId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(50);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.TotalAmount);
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.Status).NotEmpty().MaximumLength(20);
    }
}

public class UpdateReservationRequestValidator : AbstractValidator<UpdateReservationRequest>
{
    public UpdateReservationRequestValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(50);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.TotalAmount);
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.Status).NotEmpty().MaximumLength(20);
    }
}
