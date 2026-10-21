using Mundialito.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.UnitTests.Domain
{
    public class MatchTests
    {
        private readonly Guid _home = Guid.NewGuid();
        private readonly Guid _away = Guid.NewGuid();

        [Fact]
        public void Un_equipo_no_puede_jugar_contra_si_mismo()
        {
            var result = Match.Schedule(_home, _home, DateTime.UtcNow, null);

            Assert.Equal(MatchErrors.SameTeams, result.Error);
        }

        [Fact]
        public void Registrar_resultado_marca_el_partido_como_jugado_y_levanta_el_evento()
        {
            var match = Match.Schedule(_home, _away, DateTime.UtcNow, null).Value;

            var result = match.RegisterResult(1, 0, [new ScoredGoal(Guid.NewGuid(), _home, 30)]);

            Assert.True(result.IsSuccess);
            Assert.Equal(MatchStatus.Played, match.Status);
            var domainEvent = Assert.IsType<MatchResultRegisteredDomainEvent>(Assert.Single(match.DomainEvents));
            Assert.Equal((1, 0), (domainEvent.HomeGoals, domainEvent.AwayGoals));
        }

        [Fact]
        public void No_se_registra_un_resultado_dos_veces()
        {
            var match = Match.Schedule(_home, _away, DateTime.UtcNow, null).Value;
            match.RegisterResult(0, 0, []);

            var result = match.RegisterResult(0, 0, []);

            Assert.Equal(MatchErrors.AlreadyPlayed, result.Error);
        }

        [Fact]
        public void Los_goles_deben_cuadrar_con_el_marcador_y_el_partido_no_cambia()
        {
            var match = Match.Schedule(_home, _away, DateTime.UtcNow, null).Value;

            var result = match.RegisterResult(2, 0, [new ScoredGoal(Guid.NewGuid(), _home, 10)]);

            Assert.Equal(MatchErrors.ScoreMismatch, result.Error);
            Assert.Equal(MatchStatus.Scheduled, match.Status);
            Assert.Empty(match.Goals);
        }
    }
}
