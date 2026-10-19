using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Teams
{
    public static class TeamErrors
    {
        public static readonly Error NameRequired =
            Error.Validation("Team.NameRequired", "El nombre del equipo es obligatorio.");

        public static readonly Error NameTooLong =
            Error.Validation("Team.NameTooLong", $"El nombre no puede pasar de {Team.NameMaxLength} caracteres.");

        public static readonly Error CityTooLong =
            Error.Validation("Team.CityTooLong", $"La ciudad no puede pasar de {Team.CityMaxLength} caracteres.");

        public static Error NotFound(Guid teamId) =>
            Error.NotFound("Team.NotFound", $"No existe el equipo con Id {teamId}.");

        public static Error NameAlreadyExists(string name) =>
            Error.Confilct("Team.NameAlreadyExists", $"Ya existe un equipo llamado '{name}'.");

        public static Error NumberTaken(int number) =>
            Error.Confilct("Team.NumberTaken", $"El dorsal {number} ya está ocupado en este equipo.");
    }
}
