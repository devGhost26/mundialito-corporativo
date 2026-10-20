using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mundialito.Domain.Matches
{
    public static class MatchErrors
    {
        public static readonly Error SameTeams =
            Error.Validation("Match.SameTeams", "Un equipo no puede jugar contra sí mismo.");

        public static readonly Error VenueTooLong =
            Error.Validation("Match.VenueTooLong", $"El estadio no puede pasar de {Match.VenueMaxLength} caracteres.");

        public static readonly Error AlreadyPlayed =
            Error.Conflict("Match.AlreadyPlayed", "El partido ya tiene un resultado registrado.");

        public static readonly Error NegativeScore =
            Error.Validation("Match.NegativeScore", "El marcador no puede ser negativo.");

        public static readonly Error ScoreMismatch =
            Error.Validation("Match.ScoreMismatch", "Los goles registrados no cuadran con el marcador.");

        public static readonly Error GoalTeamNotInMatch =
            Error.Validation("Match.GoalTeamNotInMatch", "Hay un gol de un equipo que no juega este partido.");

        public static readonly Error InvalidMinute =
            Error.Validation("Match.InvalidMinute", $"El minuto del gol debe estar entre 1 y {Goal.MaxMinute}.");

        public static Error NotFound(Guid matchId) =>
            Error.NotFound("Match.NotFound", $"No existe el partido con Id {matchId}.");
    }
}
