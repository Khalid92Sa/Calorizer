using FluentValidation;

namespace Calorizer.Business.DTOs.Validations
{
    public class DrugsSupplementDtoValidator : AbstractValidator<DrugsSupplementDto>
    {
        public DrugsSupplementDtoValidator()
        {
            RuleFor(x => x.Drug)
                .NotEmpty().WithMessage("Drug/Supplement name is required")
                .MaximumLength(500).WithMessage("Drug/Supplement name must not exceed 500 characters");
        }
    }
}