using FluentValidation;

namespace Calorizer.Business.DTOs.Validations
{
    public class ClientDtoValidator : AbstractValidator<ClientDto>
    {
        public ClientDtoValidator()
        {
            RuleFor(x => x.FullNameEn)
                .NotEmpty().WithMessage("FullNameEnRequired")
                .MaximumLength(500).WithMessage("FullNameEnMaxLength");

            RuleFor(x => x.FullNameAr)
                .NotEmpty().WithMessage("FullNameArRequired")
                .MaximumLength(500).WithMessage("FullNameArMaxLength");

            RuleFor(x => x.MobileNumber)
                .MaximumLength(20).WithMessage("MobileNumberMaxLength")
                .Matches(@"^\+?[0-9\s\-()]*$").WithMessage("MobileNumberInvalidFormat")
                .When(x => !string.IsNullOrEmpty(x.MobileNumber));

            RuleFor(x => x.GenderId)
                .GreaterThan(0).WithMessage("GenderRequired");

            RuleFor(x => x.Address)
                .MaximumLength(2000).WithMessage("AddressMaxLength")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirthRequired")
                .LessThan(DateTime.Today).WithMessage("DateOfBirthMustBePast");

            RuleFor(x => x.Height)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("HeightRange")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.Weight)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("WeightRange")
                .When(x => x.Weight.HasValue);
        }
    }

    public class WeightHistoryDtoValidator : AbstractValidator<WeightHistoryDto>
    {
        public WeightHistoryDtoValidator()
        {
            RuleFor(x => x.Weight)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("WeightRange")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Height)
                .InclusiveBetween(0.01m, 999.99m).WithMessage("HeightRange")
                .When(x => x.Height.HasValue);
        }
    }

    public class BiochemicalMedicalTestDtoValidator : AbstractValidator<BiochemicalMedicalTestDto>
    {
        public BiochemicalMedicalTestDtoValidator()
        {
            RuleFor(x => x.MedicalData)
                .NotEmpty().WithMessage("MedicalDataRequired")
                .MaximumLength(4000).WithMessage("MedicalDataMaxLength");
        }
    }

    public class DrugsSupplementDtoValidator : AbstractValidator<DrugsSupplementDto>
    {
        public DrugsSupplementDtoValidator()
        {
            RuleFor(x => x.Drug)
                .NotEmpty().WithMessage("DrugSupplementRequired")
                .MaximumLength(500).WithMessage("DrugSupplementMaxLength");
        }
    }

    public class MedicalHistoryDtoValidator : AbstractValidator<MedicalHistoryDto>
    {
        public MedicalHistoryDtoValidator()
        {
            RuleFor(x => x.MedicalNote)
                .NotEmpty().WithMessage("MedicalNoteRequired")
                .MaximumLength(4000).WithMessage("MedicalNoteMaxLength");
        }
    }
}