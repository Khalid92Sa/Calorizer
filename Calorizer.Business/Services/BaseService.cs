using Calorizer.Business.Models;
using Calorizer.Business.Validation;
using FluentValidation;

namespace Calorizer.Business.Services
{
    public abstract class BaseService
    {
        protected readonly IValidatorFactory? _validatorFactory;

        protected BaseService(IValidatorFactory? validatorFactory = null)
        {
            _validatorFactory = validatorFactory;
        }

        public async Task<Response<object>> Validate(object obj)
        {
            var valid = await ValidateAsync(obj);
            return valid.ConvertToResponseOf<object>(obj);
        }

        protected async Task<Response<bool>> ValidateAsync<T>(T dto)
        {
            if (_validatorFactory == null)
                return new Response<bool>(true);

            var validator = _validatorFactory.GetValidator(dto.GetType());
            if (validator != null)
            {
                var context = new ValidationContext<T>(dto);
                var validationResult = validator.Validate(context);

                if (!validationResult.IsValid)
                    return await new ValidationProcessor<bool>().ProcessValidationResultOnFailureAsync(validationResult);

                return new Response<bool>(true);
            }

            return new Response<bool>(true);
        }
    }
}