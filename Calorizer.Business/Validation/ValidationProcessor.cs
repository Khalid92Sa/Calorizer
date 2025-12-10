using Calorizer.Business.Models;
using FluentValidation.Results;

namespace Calorizer.Business.Validation
{
    public class ValidationProcessor<T>
    {
        public Task<Response<T>> ProcessValidationResultOnFailureAsync(ValidationResult validationResult)
        {
            return Task.FromResult(ProcessValidationResultOnFailure(validationResult));
        }

        public Response<T> ProcessValidationResultOnFailure(ValidationResult validationResult)
        {
            var response = new Response<T>
            {
                Succeeded = false,
                Message = "ValidationError",
                StatusCode = HttpStatusCode.BusinessRuleViolation
            };

            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    response.BrokenRules.Add(new ValidationRule
                    {
                        PropertyName = failure.PropertyName,
                        Message = failure.ErrorMessage
                    });
                }
            }

            return response;
        }
    }

    public static class ResponseExtensions
    {
        public static Response<TTarget> ConvertToResponseOf<TTarget>(this Response<bool> source, object data)
        {
            return new Response<TTarget>
            {
                Succeeded = source.Succeeded,
                Message = source.Message,
                StatusCode = source.StatusCode,
                BrokenRules = source.BrokenRules,
                Data = data is TTarget target ? target : default
            };
        }
    }
}