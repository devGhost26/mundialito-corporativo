using System.Text.Json.Serialization;
using Mundialito.Api.Endpoints;
using Mundialito.Api.Observability;
using Mundialito.Application;
using Mundialito.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseObservability();

await app.Services.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapTeamEndpoints();
app.MapMatchEndpoints();
app.MapStandingEndpoints();

app.Run();