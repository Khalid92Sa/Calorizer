using FluentValidation;

namespace Calorizer.Business.DTOs.Validations
{
    public class ClientDtoValidator : AbstractValidator<ClientDto>
    {
        public ClientDtoValidator()
        {
            RuleFor(x => x.FullNameEn)
                .NotEmpty().WithMessage("English name is required")
                .MaximumLength(500).WithMessage("English name must not exceed 500 characters");

            RuleFor(x => x.FullNameAr)
                .NotEmpty().WithMessage("Arabic name is required")
                .MaximumLength(500).WithMessage("Arabic name must not exceed 500 characters");

            RuleFor(x => x.MobileNumber)
                .MaximumLength(20).WithMessage("Mobile number must not exceed 20 characters")
                .Matches(@"^\+?[0-9\s\-()]*$").WithMessage("Invalid mobile number format")
                .When(x => !string.IsNullOrEmpty(x.MobileNumber));

            RuleFor(x => x.GenderId)
                .GreaterThan(0).WithMessage("Gender is required");

            RuleFor(x => x.Address)
                .MaximumLength(2000).WithMessage("Address must not exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past");

            RuleFor(x => x.Height)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("Height must be between 0.01 and 999.99")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.Weight)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("Weight must be between 0.01 and 999.99")
                .When(x => x.Weight.HasValue);
        }
    }
}