# Arquitectura

## 1. Vista general

Los cuatro contenedores de `docker-compose`, las capas de la API y por dónde pasa cada cosa: escritura con EF Core y Unit of Work, lectura con Dapper, idempotencia, eventos, logs y trazas.

![Diagrama de arquitectura](arquitectura.svg)

## 2. Dependencias entre capas

Las flechas apuntan hacia adentro: Domain no conoce a nadie y la Api solo conecta las piezas.

```mermaid
flowchart LR
    Api["Api<br/>endpoints, middleware,<br/>composition root"]
    Application["Application<br/>commands, queries,<br/>contratos"]
    Domain["Domain<br/>entidades, reglas,<br/>eventos, Result"]
    Infrastructure["Infrastructure<br/>EF Core, Dapper,<br/>SQL Server"]

    Api --> Application
    Api --> Infrastructure
    Application --> Domain
    Infrastructure --> Application
```

## 3. Recorrido de un request (trace flow)

Registrar un resultado de punta a punta. El `TraceId` y el `CorrelationId` acompañan cada paso: van en todos los logs, en la traza de Jaeger y en el evento guardado.

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuario
    participant F as Frontend
    participant M as Middleware
    participant I as IdempotencyFilter
    participant E as Endpoint
    participant H as Handler
    participant D as Match (Domain)
    participant W as Unit of Work
    participant S as SQL Server
    participant J as Jaeger

    U->>F: Registra 1-0 con su goleador
    F->>M: POST /api/matches/{id}/result<br/>Idempotency-Key + X-Correlation-Id
    M->>M: CorrelationId y TraceId al LogContext<br/>y al baggage de la traza
    M->>I: pasa el request
    I->>S: ¿existe la llave? (Dapper)
    I->>S: reserva la llave (EF + Unit of Work)
    I->>E: ejecuta el endpoint
    E->>H: Mediator.Send(command)<br/>LoggingBehavior mide la duración
    H->>S: carga el partido y los goleadores (EF)
    H->>D: RegisterResult(1, 0, goles)
    D-->>H: Result + evento MatchResultRegistered
    H->>W: CommitAsync()
    W->>S: BEGIN · UPDATE Matches · INSERT Goals<br/>INSERT DomainEvents · COMMIT
    W->>W: publica el evento → log "Resultado registrado"
    W-->>H: Result.Success
    H-->>E: Result.Success
    E-->>I: 200 OK
    I->>S: guarda la respuesta con la llave
    I-->>M: 200 OK
    M-->>F: 200 + X-Correlation-Id + X-Trace-Id
    M--)J: spans del request y de cada consulta SQL
```

Si el mismo request se repite con la misma `Idempotency-Key`, el filtro encuentra la llave en el paso 5, devuelve la respuesta guardada y el endpoint no se ejecuta.
