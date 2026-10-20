namespace Mundialito.Api.Idempotency
{
    public static class IdempotencyExtensions
    {
        public static RouteHandlerBuilder WithIdempotency(this RouteHandlerBuilder builder) =>
            builder.AddEndpointFilter<IdempotencyFilter>();
    }
}
