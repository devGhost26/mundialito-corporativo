using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Mundialito.Api.Contracts.Teams;
using Mundialito.Api.Idempotency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.UnitTests.Api
{
    public class IdempotencyFilterTests
    {
        private readonly IdempotencyFilter _filter = new(new InMemoryIdempotencyStore(), NullLogger<IdempotencyFilter>.Instance);
        private int _endpointCalls;

        [Fact]
        public async Task Sin_llave_responde_400_y_no_ejecuta_el_endpoint()
        {
            var response = await SendAsync(key: null, new CreateTeamRequest("Halcones", null));

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(0, _endpointCalls);
        }

        [Fact]
        public async Task La_primera_peticion_ejecuta_el_endpoint()
        {
            var response = await SendAsync("k1", new CreateTeamRequest("Halcones", null));

            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(1, _endpointCalls);
        }

        [Fact]
        public async Task Repetir_la_llave_devuelve_la_misma_respuesta_sin_ejecutar_otra_vez()
        {
            var first = await SendAsync("k1", new CreateTeamRequest("Halcones", null));
            var second = await SendAsync("k1", new CreateTeamRequest("Halcones", null));

            Assert.Equal(1, _endpointCalls);
            Assert.Equal(first.StatusCode, second.StatusCode);
            Assert.Equal(first.Body, second.Body);
            Assert.Equal(first.Location, second.Location);
            Assert.Equal("true", second.Replayed);
        }

        [Fact]
        public async Task Reutilizar_la_llave_con_otro_body_responde_409()
        {
            await SendAsync("k1", new CreateTeamRequest("Halcones", null));

            var response = await SendAsync("k1", new CreateTeamRequest("Jaguares", null));

            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            Assert.Contains("Idempotency.KeyReused", response.Body);
            Assert.Equal(1, _endpointCalls);
        }

        [Fact]
        public async Task Llaves_distintas_ejecutan_el_endpoint_cada_vez()
        {
            await SendAsync("k1", new CreateTeamRequest("Halcones", null));
            await SendAsync("k2", new CreateTeamRequest("Halcones", null));

            Assert.Equal(2, _endpointCalls);
        }

        [Fact]
        public async Task Un_error_500_libera_la_llave_para_reintentar()
        {
            await SendAsync("k1", new CreateTeamRequest("Halcones", null), endpointStatus: 500);
            var retry = await SendAsync("k1", new CreateTeamRequest("Halcones", null));

            Assert.Equal(2, _endpointCalls);
            Assert.Equal(StatusCodes.Status201Created, retry.StatusCode);
        }

        private async Task<TestResponse> SendAsync(string? key, CreateTeamRequest request, int endpointStatus = 201)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider()
            };
            httpContext.Request.Method = HttpMethods.Post;
            httpContext.Request.Path = "/api/teams";
            httpContext.Response.Body = new MemoryStream();

            if (key is not null)
                httpContext.Request.Headers[IdempotencyFilter.HeaderName] = key;

            var invocation = EndpointFilterInvocationContext.Create(httpContext, request);

            var result = await _filter.InvokeAsync(invocation, _ =>
            {
                _endpointCalls++;
                IResult endpointResult = endpointStatus == 201
                    ? Results.Created("/api/teams/abc", new { id = "abc" })
                    : Results.StatusCode(endpointStatus);
                return ValueTask.FromResult<object?>(endpointResult);
            });

            if (result is IResult httpResult)
                await httpResult.ExecuteAsync(httpContext);

            httpContext.Response.Body.Position = 0;
            var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            return new TestResponse(
                httpContext.Response.StatusCode,
                body,
                httpContext.Response.Headers.Location.ToString(),
                httpContext.Response.Headers[IdempotencyFilter.ReplayedHeaderName].ToString());
        }

        private sealed record TestResponse(int StatusCode, string Body, string Location, string Replayed);
    }
}
