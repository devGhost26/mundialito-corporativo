using Mediator;
using Microsoft.Extensions.Logging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mundialito.Application.Behaviors
{
    internal sealed class LoggingBehavior<TMessage, TResponse>(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
        : IPipelineBehavior<TMessage, TResponse>
        where TMessage : notnull, IMessage
    {
        public async ValueTask<TResponse> Handle(
            TMessage message,
            MessageHandlerDelegate<TMessage, TResponse> next,
            CancellationToken cancellationToken)
        {
            var messageName = typeof(TMessage).Name;
            logger.LogDebug("Ejecutando {MessageName}", messageName);

            var startedAt = Stopwatch.GetTimestamp();
            var response = await next(message, cancellationToken);
            var elapsedMs = Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;

            if (response is Result { IsFailure: true } result)
            {
                logger.LogWarning(
                    "{MessageName} terminó con error {ErrorCode} en {ElapsedMs:0.0} ms",
                    messageName, result.Error.Code, elapsedMs);
            }
            else
            {
                logger.LogInformation("{MessageName} terminó en {ElapsedMs:0.0} ms", messageName, elapsedMs);
            }

            return response;
        }
    }
}
