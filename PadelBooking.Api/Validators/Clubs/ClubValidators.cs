using FluentValidation;
using PadelBooking.Api.DTOs.Clubs;

namespace PadelBooking.Api.Validators.Clubs;

public class CreateClubRequestValidator : AbstractValidator<CreateClubRequest>
{
    public CreateClubRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(200);
        RuleFor(x => x.OpenTime).LessThan(x => x.CloseTime).WithMessage("Open time must be before close time.");
    }
}

public class UpdateClubRequestValidator : AbstractValidator<UpdateClubRequest>
{
    public UpdateClubRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(200);
        RuleFor(x => x.OpenTime).LessThan(x => x.CloseTime).WithMessage("Open time must be before close time.");
    }
}
