using System.Net;

namespace Calorizer.Business.Models
{
    public class Response
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public List<ValidationRule> BrokenRules { get; set; } = new();

        public Response()
        {
            Succeeded = true;
            StatusCode = (int)HttpStatusCode.OK;
        }

        public Response(bool succeeded)
        {
            Succeeded = succeeded;
            StatusCode = succeeded ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest;
        }
    }

    public class Response<T> : Response
    {
        public T? Data { get; set; }

        public Response() : base() { }

        public Response(bool succeeded) : base(succeeded) { }

        public Response(T data)
        {
            Succeeded = true;
            Data = data;
            StatusCode = (int)HttpStatusCode.OK;
        }
    }

    public class ValidationRule
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public static class HttpStatusCode
    {
        public const int OK = 200;
        public const int BadRequest = 400;
        public const int BusinessRuleViolation = 422;
    }
}