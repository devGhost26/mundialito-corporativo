using Microsoft.EntityFrameworkCore;
using Mundialito.Application.Common.Paging;
using Mundialito.Application.Standings.GetStandings;
using Mundialito.Application.Standings.GetTopScorers;
using Mundialito.Domain.Standings;

namespace Mundialito.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class StandingsQueryTests(MundialitoApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task La_tabla_del_SQL_coincide_con_el_calculador_del_dominio()
    {
        var fromSql = await _client.GetJsonAsync<PagedResult<StandingItem>>("/api/standings?pageSize=100");

        var fromDomain = await factory.QueryDatabaseAsync(async db => StandingsCalculator.Calculate(
            await db.Teams.ToListAsync(),
            await db.Matches.ToListAsync()));

        Assert.NotEmpty(fromSql.Data);
        Assert.Equal(fromDomain.Count, fromSql.TotalRecords);
        Assert.Equal(
            fromDomain.Select(s => (s.TeamName, s.Played, s.Won, s.Drawn, s.Lost, s.GoalsFor, s.GoalsAgainst, s.GoalDifference, s.Points)),
            fromSql.Data.Select(s => (s.TeamName, s.Played, s.Won, s.Drawn, s.Lost, s.GoalsFor, s.GoalsAgainst, s.GoalDifference, s.Points)));
        Assert.Equal(Enumerable.Range(1, fromSql.Data.Count), fromSql.Data.Select(s => s.Position));
    }

    [Fact]
    public async Task Los_goleadores_del_SQL_coinciden_con_los_goles_guardados()
    {
        var fromSql = await _client.GetJsonAsync<PagedResult<TopScorerItem>>("/api/top-scorers?pageSize=100");

        var goalsByPlayer = await factory.QueryDatabaseAsync(db => db.Goals
            .GroupBy(g => g.PlayerId)
            .Select(g => new { PlayerId = g.Key, Goals = g.Count() })
            .ToDictionaryAsync(g => g.PlayerId, g => g.Goals));

        Assert.NotEmpty(fromSql.Data);
        Assert.Equal(goalsByPlayer.Count, fromSql.TotalRecords);
        Assert.All(fromSql.Data, scorer => Assert.Equal(goalsByPlayer[scorer.PlayerId], scorer.Goals));
        Assert.Equal(fromSql.Data.Select(s => s.Goals).OrderDescending(), fromSql.Data.Select(s => s.Goals));
    }

    [Fact]
    public async Task La_paginacion_sale_de_la_base_con_el_total_correcto()
    {
        var totalTeams = await factory.QueryDatabaseAsync(db => db.Teams.CountAsync());

        var page = await _client.GetJsonAsync<PagedResult<StandingItem>>("/api/standings?pageSize=3&pageNumber=2");

        Assert.Equal(totalTeams, page.TotalRecords);
        Assert.Equal((int)Math.Ceiling(totalTeams / 3.0), page.TotalPages);
        Assert.Equal(Math.Min(3, totalTeams - 3), page.Data.Count);
        Assert.Equal(4, page.Data[0].Position);
    }
}
