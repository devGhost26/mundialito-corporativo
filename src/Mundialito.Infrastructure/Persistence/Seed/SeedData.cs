using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Persistence.Seed
{
    internal static class SeedData
    {
        public sealed record TeamSeed(string Name, string? City, PlayerSeed[] Players);

        public sealed record PlayerSeed(string FirstName, string LastName, int Number, PlayerPosition Position);

        public static readonly TeamSeed[] Teams =
        [
            new("Tecnología", "San Salvador",
            [
                new("Carlos", "Martínez", 1, PlayerPosition.Goalkeeper),
                new("José", "Hernández", 4, PlayerPosition.Defender),
                new("Luis", "Ramírez", 5, PlayerPosition.Defender),
                new("Andrés", "López", 8, PlayerPosition.Midfielder),
                new("Diego", "Flores", 9, PlayerPosition.Forward)
            ]),
            new("Finanzas", "Santa Ana",
            [
                new("Mario", "Pérez", 1, PlayerPosition.Goalkeeper),
                new("Óscar", "Guzmán", 4, PlayerPosition.Defender),
                new("Raúl", "Castro", 5, PlayerPosition.Defender),
                new("Héctor", "Rivas", 8, PlayerPosition.Midfielder),
                new("Ernesto", "Molina", 9, PlayerPosition.Forward)
            ]),
            new("Operaciones", "San Miguel",
            [
                new("Fernando", "Ortiz", 1, PlayerPosition.Goalkeeper),
                new("Ricardo", "Mejía", 4, PlayerPosition.Defender),
                new("Javier", "Alvarado", 5, PlayerPosition.Defender),
                new("Manuel", "Cruz", 8, PlayerPosition.Midfielder),
                new("Roberto", "Aguilar", 9, PlayerPosition.Forward)
            ]),
            new("Recursos Humanos", null,
            [
                new("Daniel", "Reyes", 1, PlayerPosition.Goalkeeper),
                new("Pablo", "Morales", 4, PlayerPosition.Defender),
                new("Sergio", "Vásquez", 5, PlayerPosition.Defender),
                new("Alberto", "Romero", 8, PlayerPosition.Midfielder),
                new("Hugo", "Navarro", 9, PlayerPosition.Forward)
            ])
        ];
    }
}
