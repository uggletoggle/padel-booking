using FluentValidation;
using PadelBooking.Api.DTOs.Courts;

namespace PadelBooking.Api.Validators.Courts;

public class CreateCourtRequestValidator : AbstractValidator<CreateCourtRequest>
{
    public CreateCourtRequestValidator()
    {
        RuleFor(x => x.ClubId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SurfaceType).MaximumLength(50);
    }
}

public class UpdateCourtRequestValidator : AbstractValidator<UpdateCourtRequest>
{
    public UpdateCourtRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SurfaceType).MaximumLength(50);
    }
}
