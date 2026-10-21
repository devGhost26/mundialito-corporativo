using Mundialito.Domain.Standings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.UnitTests.Domain
{
    public class TeamStandingTests
    {
        [Fact]
        public void Una_victoria_suma_tres_puntos()
        {
            var standing = new TeamStanding(Guid.NewGuid(), "Tecnología");

            standing.AddResult(goalsFor: 2, goalsAgainst: 1);

            Assert.Equal(1, standing.Won);
            Assert.Equal(3, standing.Points);
        }

        [Fact]
        public void Un_empate_suma_un_punto()
        {
            var standing = new TeamStanding(Guid.NewGuid(), "Tecnología");

            standing.AddResult(goalsFor: 1, goalsAgainst: 1);

            Assert.Equal(1, standing.Drawn);
            Assert.Equal(1, standing.Points);
        }

        [Fact]
        public void Una_derrota_no_suma_puntos()
        {
            var standing = new TeamStanding(Guid.NewGuid(), "Tecnología");

            standing.AddResult(goalsFor: 0, goalsAgainst: 3);

            Assert.Equal(1, standing.Lost);
            Assert.Equal(0, standing.Points);
        }

        [Fact]
        public void Los_puntos_acumulan_varios_partidos()
        {
            var standing = new TeamStanding(Guid.NewGuid(), "Tecnología");

            standing.AddResult(3, 0);
            standing.AddResult(1, 1);
            standing.AddResult(0, 2);

            Assert.Equal(3, standing.Played);
            Assert.Equal(TournamentRules.PointsPerWin + TournamentRules.PointsPerDraw, standing.Points);
        }

        [Theory]
        [InlineData(3, 1, 2)]
        [InlineData(0, 2, -2)]
        [InlineData(1, 1, 0)]
        public void La_diferencia_de_gol_es_goles_a_favor_menos_en_contra(int goalsFor, int goalsAgainst, int expected)
        {
            var standing = new TeamStanding(Guid.NewGuid(), "Tecnología");

            standing.AddResult(goalsFor, goalsAgainst);

            Assert.Equal(expected, standing.GoalDifference);
        }

        [Fact]
        public void La_diferencia_de_gol_acumula_varios_partidos()
        {
            var standing = new TeamStanding(Guid.NewGuid(), "Tecnología");

            standing.AddResult(4, 1);
            standing.AddResult(0, 2);

            Assert.Equal(4, standing.GoalsFor);
            Assert.Equal(3, standing.GoalsAgainst);
            Assert.Equal(1, standing.GoalDifference);
        }
    }
}
