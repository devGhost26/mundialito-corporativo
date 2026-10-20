using Mediator;
using Mundialito.Api.Extensions;
using Mundialito.Application.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mundialito.Api.Idempotency
{
    public sealed class IdempotencyFilter(IIdempotencyStore store, ILogger<IdempotencyFilter> logger) : IEndpointFilter
    {
        public const string HeaderName = "Idempotency-Key";
        public const string ReplayedHeaderName = "Idempotent-Replayed";
        public const int KeyMaxLength = 100;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;
            var cancellationToken = httpContext.RequestAborted;

            var key = httpContext.Request.Headers[HeaderName].ToString();
            if (string.IsNullOrWhiteSpace(key) || key.Length > KeyMaxLength)
                return IdempotencyErrors.KeyRequired.ToProblem();

            var requestHash = ComputeRequestHash(httpContext.Request, context.Arguments);

            var stored = await store.GetAsync(key, cancellationToken);
            if (stored is not null)
                return Replay(httpContext, key, stored, requestHash);

            if (!await store.TryReserveAsync(key, requestHash, cancellationToken))
            {
                stored = await store.GetAsync(key, cancellationToken);
                return stored is null
                    ? IdempotencyErrors.InProgress.ToProblem()
                    : Replay(httpContext, key, stored, requestHash);
            }

            try
            {
                var result = await next(context);

                if (result is not IResult httpResult)
                {
                    await store.ReleaseAsync(key, CancellationToken.None);
                    return result;
                }

                var originalBody = httpContext.Response.Body;
                using var buffer = new MemoryStream();
                httpContext.Response.Body = buffer;

                try
                {
                    await httpResult.ExecuteAsync(httpContext);
                }
                finally
                {
                    httpContext.Response.Body = originalBody;
                }

                var statusCode = httpContext.Response.StatusCode;

                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    await store.ReleaseAsync(key, CancellationToken.None);
                }
                else
                {
                    await store.CompleteAsync(
                        key,
                        statusCode,
                        httpContext.Response.ContentType,
                        Encoding.UTF8.GetString(buffer.ToArray()),
                        httpContext.Response.Headers.Location.ToString(),
                        CancellationToken.None);
                }

                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody, cancellationToken);

                return Results.Empty;
            }
            catch
            {
                await store.ReleaseAsync(key, CancellationToken.None);
                throw;
            }
        }

        private IResult Replay(HttpContext httpContext, string key, IdempotentResponse stored, string requestHash)
        {
            if (stored.RequestHash != requestHash)
            {
                logger.LogWarning("Llave de idempotencia {IdempotencyKey} reutilizada con otra petición", key);
                return IdempotencyErrors.KeyReused.ToProblem();
            }

            if (!stored.IsCompleted)
                return IdempotencyErrors.InProgress.ToProblem();

            logger.LogInformation("Respuesta repetida para la llave de idempotencia {IdempotencyKey}", key);

            httpContext.Response.Headers[ReplayedHeaderName] = "true";
            if (!string.IsNullOrEmpty(stored.Location))
                httpContext.Response.Headers.Location = stored.Location;

            return Results.Content(stored.Body, stored.ContentType, Encoding.UTF8, stored.StatusCode);
        }

        private static string ComputeRequestHash(HttpRequest request, IList<object?> arguments)
        {
            var payload = arguments
                .Where(argument => argument is not null and not ISender and not CancellationToken)
                .ToList();

            var content = $"{request.Method}:{request.Path}:{JsonSerializer.Serialize(payload)}";

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
        }
    }
}
