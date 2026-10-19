using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Teams
{
    public static class PlayerErrors
    {
        public static readonly Error FirstNameRequired =
            Error.Validation("Player.FirstNameRequired", "El nombre del jugador es obligatorio.");

        public static readonly Error LastNameRequired =
            Error.Validation("Player.LastNameRequired", "El apellido del jugador es obligatorio.");

        public static readonly Error InvalidNumber =
            Error.Validation("Player.InvalidNumber", "El dorsal debe estar entre 1 y 99.");

        public static readonly Error InvalidPosition =
            Error.Validation("Player.InvalidPosition", "La posición no es válida.");

        public static Error NotFound(Guid playerId) =>
            Error.NotFound("Player.NotFound", $"No existe el jugador con Id {playerId}.");
    }
}
