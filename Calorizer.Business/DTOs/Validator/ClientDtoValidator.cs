using FluentValidation;
using Calorizer.Business.Services;

namespace Calorizer.Business.DTOs.Validations
{
    public class ClientDtoValidator : AbstractValidator<ClientDto>
    {
        public ClientDtoValidator(Localizer localizer)
        {
            RuleFor(x => x.FullNameEn)
                .NotEmpty().WithMessage(x => localizer["FullNameEnRequired"])
                .MaximumLength(500).WithMessage(x => localizer["FullNameEnMaxLength"]);

            RuleFor(x => x.FullNameAr)
                .MaximumLength(500).WithMessage(x => localizer["FullNameArMaxLength"])
                .When(x => !string.IsNullOrEmpty(x.FullNameAr));

            RuleFor(x => x.MobileNumber)
                .MaximumLength(20).WithMessage(x => localizer["MobileNumberMaxLength"])
                .Matches(@"^\+?[0-9\s\-()]*$").WithMessage(x => localizer["MobileNumberInvalidFormat"])
                .When(x => !string.IsNullOrEmpty(x.MobileNumber));

            RuleFor(x => x.GenderId)
                .GreaterThan(0).WithMessage(x => localizer["GenderRequired"]);

            RuleFor(x => x.Address)
                .MaximumLength(2000).WithMessage(x => localizer["AddressMaxLength"])
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage(x => localizer["DateOfBirthRequired"])
                .LessThan(DateTime.Today).WithMessage(x => localizer["DateOfBirthMustBePast"]);

            RuleFor(x => x.Height)
                    .NotNull().WithMessage(x => localizer["HeightRequired"])
                    .NotEmpty().WithMessage(x => localizer["HeightRequired"])
                    .InclusiveBetween(0.01m, 999.99m).WithMessage(x => localizer["HeightRange"]);

            RuleFor(x => x.Weight)
                .NotNull().WithMessage(x => localizer["WeightRequired"])
                .NotEmpty().WithMessage(x => localizer["WeightRequired"])
                .InclusiveBetween(0.01m, 999.99m).WithMessage(x => localizer["WeightRange"]);
        }
    }

    public class WeightHistoryDtoValidator : AbstractValidator<WeightHistoryDto>
    {
        public WeightHistoryDtoValidator(Localizer localizer)
        {
            RuleFor(x => x.Weight)
                .InclusiveBetween(0.01m, 999.99m).WithMessage(x => localizer["WeightRange"])
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Height)
                .InclusiveBetween(0.01m, 999.99m).WithMessage(x => localizer["HeightRange"])
                .When(x => x.Height.HasValue);
        }
    }

    public class BiochemicalMedicalTestDtoValidator : AbstractValidator<BiochemicalMedicalTestDto>
    {
        public BiochemicalMedicalTestDtoValidator(Localizer localizer)
        {
            RuleFor(x => x.MedicalData)
                .NotEmpty().WithMessage(x => localizer["MedicalDataRequired"])
                .MaximumLength(4000).WithMessage(x => localizer["MedicalDataMaxLength"]);
        }
    }

    public class DrugsSupplementDtoValidator : AbstractValidator<DrugsSupplementDto>
    {
        public DrugsSupplementDtoValidator(Localizer localizer)
        {
            RuleFor(x => x.Drug)
                .NotEmpty().WithMessage(x => localizer["DrugSupplementRequired"])
                .MaximumLength(500).WithMessage(x => localizer["DrugSupplementMaxLength"]);
        }
    }

    public class MedicalHistoryDtoValidator : AbstractValidator<MedicalHistoryDto>
    {
        public MedicalHistoryDtoValidator(Localizer localizer)
        {
            RuleFor(x => x.MedicalNote)
                .NotEmpty().WithMessage(x => localizer["MedicalNoteRequired"])
                .MaximumLength(4000).WithMessage(x => localizer["MedicalNoteMaxLength"]);
        }
    }
}