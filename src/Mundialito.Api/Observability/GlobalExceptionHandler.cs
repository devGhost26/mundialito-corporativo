using Microsoft.AspNetCore.Diagnostics;

namespace Mundialito.Api.Observability
{
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Error no controlado en {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails =
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server.Error",
                    Detail = "Ocurrió un error inesperado. Usa el correlationId para reportarlo."
                }
            });
        }
    }
}
