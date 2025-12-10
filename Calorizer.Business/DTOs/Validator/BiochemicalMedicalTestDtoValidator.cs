using FluentValidation;

namespace Calorizer.Business.DTOs.Validations
{
    public class BiochemicalMedicalTestDtoValidator : AbstractValidator<BiochemicalMedicalTestDto>
    {
        public BiochemicalMedicalTestDtoValidator()
        {
            RuleFor(x => x.MedicalData)
                .NotEmpty().WithMessage("Medical data is required")
                .MaximumLength(4000).WithMessage("Medical data must not exceed 4000 characters");
        }
    }
}