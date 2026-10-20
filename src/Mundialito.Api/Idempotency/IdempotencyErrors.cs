using Mundialito.Domain.Common.Results;

namespace Mundialito.Api.Idempotency
{
    public static class IdempotencyErrors
    {
        public static readonly Error KeyRequired = Error.Validation(
            "Idempotency.KeyRequired",
            $"El header {IdempotencyFilter.HeaderName} es obligatorio y no puede pasar de {IdempotencyFilter.KeyMaxLength} caracteres.");

        public static readonly Error KeyReused = Error.Conflict(
            "Idempotency.KeyReused",
            "Esta llave de idempotencia ya se usó con una petición distinta.");

        public static readonly Error InProgress = Error.Conflict(
            "Idempotency.InProgress",
            "Ya hay una petición en curso con esta llave de idempotencia. Vuelve a intentar en unos segundos.");
    }
}
