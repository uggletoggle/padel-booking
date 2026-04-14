using FluentValidation;
using PadelBooking.Api.DTOs.Users;
using PadelBooking.Api.Data.Enums;

namespace PadelBooking.Api.Validators.Users;

public class CreateAdminUserRequestValidator : AbstractValidator<CreateAdminUserRequest>
{
    public CreateAdminUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).IsInEnum();
        
        RuleFor(x => x.ClubId)
            .NotEmpty()
            .When(x => x.Role is UserRole.Attendant or UserRole.Owner)
            .WithMessage("ClubId is required for Owner or Attendant roles.");
            
        RuleFor(x => x.ClubId)
            .Empty()
            .When(x => x.Role == UserRole.Admin)
            .WithMessage("ClubId should not be provided for Admin users.");
    }
}
