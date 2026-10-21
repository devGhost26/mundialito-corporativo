using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace Mundialito.Api.Observability
{
    public static class ObservabilityExtensions
    {
        public const string ServiceName = "Mundialito.Api";

        public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
        {
            builder.Services.AddSerilog((services, configuration) => configuration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", ServiceName));

            builder.Services.AddProblemDetails(options =>
                options.CustomizeProblemDetails = context =>
                {
                    if (context.HttpContext.Items[CorrelationIdMiddleware.CorrelationIdItem] is string correlationId)
                        context.ProblemDetails.Extensions["correlationId"] = correlationId;
                });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(ServiceName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddSqlClientInstrumentation();

                    if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
                        tracing.AddOtlpExporter();
                });

            return builder;
        }

        public static WebApplication UseObservability(this WebApplication app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} respondió {StatusCode} en {Elapsed:0.0} ms";

                options.GetLevel = (httpContext, _, exception) =>
                    exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest
                            ? LogEventLevel.Warning
                            : LogEventLevel.Information;
            });

            app.UseExceptionHandler();

            return app;
        }
    }
}
