import type { ApiError } from "@/lib/api/http";

export function ErrorBox({ error }: { error: ApiError | null }) {
  if (!error) return null;

  return (
    <div className="error" role="alert">
      <strong>{error.message}</strong>
      {(error.code || error.correlationId) && (
        <small>
          {error.code && <>Código: {error.code}</>}
          {error.code && error.correlationId && " · "}
          {error.correlationId && <>Referencia: {error.correlationId}</>}
        </small>
      )}
    </div>
  );
}
