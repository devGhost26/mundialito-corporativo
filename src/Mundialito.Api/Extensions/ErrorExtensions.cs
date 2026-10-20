using Mundialito.Domain.Common.Results;

namespace Mundialito.Api.Extensions
{
    public static class ErrorExtensions
    {
        public static IResult ToProblem(this Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return Results.Problem(
                statusCode: statusCode,
                title: error.Code,
                detail: error.Message,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code });
        }
    }
}
