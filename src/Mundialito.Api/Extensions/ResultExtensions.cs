using Mundialito.Domain.Common.Results;

namespace Mundialito.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToOk<T>(this Result<T> result) =>
            result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblem();
    }
}
