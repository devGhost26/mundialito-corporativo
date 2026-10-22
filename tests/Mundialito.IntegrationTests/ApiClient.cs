using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mundialito.IntegrationTests;

internal static class ApiClient
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static Task<HttpResponseMessage> PostAsync(
        this HttpClient client, string url, object body, string? idempotencyKey = null, string? correlationId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body, options: Json) };
        request.Headers.Add("Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString());
        if (correlationId is not null)
            request.Headers.Add("X-Correlation-Id", correlationId);

        return client.SendAsync(request);
    }

    public static async Task<Guid> CreateAsync(this HttpClient client, string url, object body, string? correlationId = null)
    {
        var response = await client.PostAsync(url, body, correlationId: correlationId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Created>(Json))!.Id;
    }

    public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>(Json))!;
    }

    public sealed record Created(Guid Id);
}
