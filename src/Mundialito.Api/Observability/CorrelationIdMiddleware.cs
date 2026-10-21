using Serilog.Context;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Mundialito.Api.Observability
{
    public sealed partial class CorrelationIdMiddleware(RequestDelegate next)
    {
        public const string CorrelationIdHeader = "X-Correlation-Id";
        public const string TraceIdHeader = "X-Trace-Id";
        public const string CorrelationIdItem = "CorrelationId";

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = ReadCorrelationId(context) ?? Guid.NewGuid().ToString("N");
            var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

            context.Items[CorrelationIdItem] = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
                context.Response.Headers[TraceIdHeader] = traceId;
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("TraceId", traceId))
            {
                await next(context);
            }
        }

        private static string? ReadCorrelationId(HttpContext context)
        {
            var value = context.Request.Headers[CorrelationIdHeader].ToString();

            return SafeCorrelationId().IsMatch(value) ? value : null;
        }

        [GeneratedRegex("^[A-Za-z0-9-]{1,64}$")]
        private static partial Regex SafeCorrelationId();
    }
}
