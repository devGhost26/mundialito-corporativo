using System.Text.Json.Serialization;
using Mundialito.Api.Endpoints;
using Mundialito.Api.Observability;
using Mundialito.Application;
using Mundialito.Infrastructure;
using Scalar.AspNetCore;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("Location", "X-Correlation-Id", "X-Trace-Id", "Idempotent-Replayed")));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseObservability();

await app.Services.InitializeDatabaseAsync();

app.UseCors(FrontendCorsPolicy);

app.MapOpenApi();
app.MapScalarApiReference();
app.MapHealthChecks("/health");

app.MapTeamEndpoints();
app.MapMatchEndpoints();
app.MapStandingEndpoints();

app.Run();