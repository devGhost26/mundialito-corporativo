using Mundialito.Domain.Matches;
using Mundialito.Domain.Standings;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.UnitTests.Domain
{
    public class StandingsCalculatorTests
    {
        private readonly Team _alfa = NewTeam("Alfa");
        private readonly Team _beta = NewTeam("Beta");
        private readonly Team _gamma = NewTeam("Gamma");
        private readonly Team _delta = NewTeam("Delta");

        [Fact]
        public void Ordena_por_puntos()
        {
            var matches = new[]
            {
                Played(_alfa, 1, _beta, 0),
                Played(_gamma, 0, _delta, 0)
            };

            var table = StandingsCalculator.Calculate([_alfa, _beta, _gamma, _delta], matches);

            Assert.Equal("Alfa", table[0].TeamName);
            Assert.Equal(3, table[0].Points);
            Assert.Equal("Beta", table[^1].TeamName);
        }

        [Fact]
        public void Con_los_mismos_puntos_desempata_la_diferencia_de_gol()
        {
            var matches = new[]
            {
                Played(_alfa, 1, _gamma, 0),
                Played(_beta, 4, _delta, 0)
            };

            var table = StandingsCalculator.Calculate([_alfa, _beta, _gamma, _delta], matches);

            Assert.Equal(3, table[0].Points);
            Assert.Equal(3, table[1].Points);
            Assert.Equal("Beta", table[0].TeamName);
            Assert.Equal("Alfa", table[1].TeamName);
        }

        [Fact]
        public void Con_los_mismos_puntos_y_diferencia_desempatan_los_goles_a_favor()
        {
            var matches = new[]
            {
                Played(_alfa, 1, _gamma, 0),
                Played(_beta, 3, _delta, 2)
            };

            var table = StandingsCalculator.Calculate([_alfa, _beta, _gamma, _delta], matches);

            Assert.Equal(table[0].GoalDifference, table[1].GoalDifference);
            Assert.Equal("Beta", table[0].TeamName);
            Assert.Equal("Alfa", table[1].TeamName);
        }

        [Fact]
        public void Si_todo_empata_ordena_por_nombre()
        {
            var table = StandingsCalculator.Calculate([_gamma, _alfa, _delta, _beta], []);

            Assert.Equal(["Alfa", "Beta", "Delta", "Gamma"], table.Select(s => s.TeamName));
        }

        [Fact]
        public void Un_equipo_sin_partidos_aparece_en_cero()
        {
            var table = StandingsCalculator.Calculate([_alfa, _beta, _gamma], [Played(_alfa, 2, _beta, 0)]);

            var gamma = Assert.Single(table, s => s.TeamName == "Gamma");
            Assert.Equal(0, gamma.Played);
            Assert.Equal(0, gamma.Points);
        }

        [Fact]
        public void Los_partidos_sin_jugar_no_cuentan()
        {
            var scheduled = Match.Schedule(_alfa.Id, _beta.Id, DateTime.UtcNow.AddDays(3), null).Value;

            var table = StandingsCalculator.Calculate([_alfa, _beta], [scheduled]);

            Assert.All(table, s => Assert.Equal(0, s.Played));
        }

        [Fact]
        public void Cada_partido_cuenta_para_los_dos_equipos_con_los_goles_invertidos()
        {
            var table = StandingsCalculator.Calculate([_alfa, _beta], [Played(_alfa, 3, _beta, 1)]);

            var alfa = table.Single(s => s.TeamName == "Alfa");
            var beta = table.Single(s => s.TeamName == "Beta");

            Assert.Equal((3, 1, 2), (alfa.GoalsFor, alfa.GoalsAgainst, alfa.GoalDifference));
            Assert.Equal((1, 3, -2), (beta.GoalsFor, beta.GoalsAgainst, beta.GoalDifference));
        }

        private static Team NewTeam(string name)
        {
            var team = Team.Create(name, null).Value;
            for (var number = 1; number <= 5; number++)
                team.AddPlayer("Jugador", $"{name}{number}", number, PlayerPosition.Forward);
            return team;
        }

        private static Match Played(Team home, int homeGoals, Team away, int awayGoals)
        {
            var match = Match.Schedule(home.Id, away.Id, DateTime.UtcNow.AddDays(-1), null).Value;

            var goals = Goals(home, homeGoals).Concat(Goals(away, awayGoals)).ToList();
            match.RegisterResult(homeGoals, awayGoals, goals);

            return match;
        }

        private static IEnumerable<ScoredGoal> Goals(Team team, int count) =>
            Enumerable.Range(1, count).Select(minute => new ScoredGoal(team.Players.First().Id, team.Id, minute));
    }
}
