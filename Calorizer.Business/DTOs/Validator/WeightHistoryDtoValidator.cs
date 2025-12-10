using FluentValidation;

namespace Calorizer.Business.DTOs.Validations
{
    public class WeightHistoryDtoValidator : AbstractValidator<WeightHistoryDto>
    {
        public WeightHistoryDtoValidator()
        {
            RuleFor(x => x.Weight)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("Weight must be between 0.01 and 999.99")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Height)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("Height must be between 0.01 and 999.99")
                .When(x => x.Height.HasValue);
        }
    }
}