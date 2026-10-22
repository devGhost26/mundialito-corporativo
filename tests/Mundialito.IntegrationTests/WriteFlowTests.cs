using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Mundialito.Application.Common.Paging;
using Mundialito.Application.Standings.GetStandings;
using Mundialito.Domain.Matches;

namespace Mundialito.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class WriteFlowTests(MundialitoApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Repetir_un_POST_con_la_misma_llave_no_duplica_el_equipo()
    {
        var name = $"Idempotente {Guid.NewGuid():N}";
        var key = Guid.NewGuid().ToString();

        var first = await _client.PostAsync("/api/teams", new { name, city = (string?)null }, key);
        var second = await _client.PostAsync("/api/teams", new { name, city = (string?)null }, key);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.Equal(await first.Content.ReadAsStringAsync(), await second.Content.ReadAsStringAsync());
        Assert.Equal(1, await factory.QueryDatabaseAsync(db => db.Teams.CountAsync(t => t.Name == name)));
    }

    [Fact]
    public async Task Crear_el_mismo_nombre_al_mismo_tiempo_deja_un_solo_equipo_y_responde_409_al_resto()
    {
        var name = $"Concurrente {Guid.NewGuid():N}";

        var responses = await Task.WhenAll(Enumerable.Range(0, 5)
            .Select(_ => _client.PostAsync("/api/teams", new { name, city = (string?)null })));

        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Created);
        Assert.All(responses.Where(r => r.StatusCode != HttpStatusCode.Created),
            r => Assert.Equal(HttpStatusCode.Conflict, r.StatusCode));
        Assert.Equal(1, await factory.QueryDatabaseAsync(db => db.Teams.CountAsync(t => t.Name == name)));
    }

    [Fact]
    public async Task Registrar_un_resultado_actualiza_la_tabla_y_guarda_el_evento_con_su_trazabilidad()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var correlationId = $"it-{suffix}";

        var home = await _client.CreateAsync("/api/teams", new { name = $"Local {suffix}", city = (string?)null });
        var away = await _client.CreateAsync("/api/teams", new { name = $"Visita {suffix}", city = (string?)null });
        var scorer = await _client.CreateAsync($"/api/teams/{home}/players",
            new { firstName = "Ana", lastName = "López", number = 9, position = "Forward" });
        var match = await _client.CreateAsync("/api/matches",
            new { homeTeamId = home, awayTeamId = away, scheduledAtUtc = DateTime.UtcNow.AddDays(-1), venue = "Cancha 1" });

        var result = await _client.PostAsync($"/api/matches/{match}/result",
            new { homeGoals = 1, awayGoals = 0, goals = new[] { new { playerId = scorer, minute = 30 } } },
            correlationId: correlationId);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var table = await _client.GetJsonAsync<PagedResult<StandingItem>>("/api/standings?pageSize=100");
        var winner = table.Data.Single(s => s.TeamId == home);
        Assert.Equal((1, 1, 1, 3), (winner.Played, winner.Won, winner.GoalDifference, winner.Points));

        var storedEvent = await factory.QueryDatabaseAsync(db => db.DomainEvents
            .SingleAsync(e => e.Type == nameof(MatchResultRegisteredDomainEvent) && e.Payload.Contains(match.ToString())));
        Assert.Equal(correlationId, storedEvent.CorrelationId);
        Assert.False(string.IsNullOrEmpty(storedEvent.TraceId));
    }

    [Fact]
    public async Task Un_error_de_negocio_responde_ProblemDetails_con_su_codigo_y_correlationId()
    {
        var response = await _client.GetAsync($"/api/teams/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.Equal("Team.NotFound", problem!["errorCode"].ToString());
        Assert.True(problem.ContainsKey("correlationId"));
    }
}
