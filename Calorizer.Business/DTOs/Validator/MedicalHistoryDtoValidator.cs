using FluentValidation;

namespace Calorizer.Business.DTOs.Validations
{
    public class MedicalHistoryDtoValidator : AbstractValidator<MedicalHistoryDto>
    {
        public MedicalHistoryDtoValidator()
        {
            RuleFor(x => x.MedicalNote)
                .NotEmpty().WithMessage("Medical note is required")
                .MaximumLength(4000).WithMessage("Medical note must not exceed 4000 characters");
        }
    }
}