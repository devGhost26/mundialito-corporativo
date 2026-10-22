import type { ApiError } from "@/lib/api/http";

export function ErrorBox({ error }: { error: ApiError | null }) {
  if (!error) return null;

  return (
    <div className="alert" role="alert">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" aria-hidden="true">
        <circle cx="12" cy="12" r="9" />
        <path d="M12 7v6M12 16.5v.5" />
      </svg>
      <div>
        <strong>{error.message}</strong>
        {(error.code || error.correlationId) && (
          <small>
            {error.code && <>Código: {error.code}</>}
            {error.code && error.correlationId && " · "}
            {error.correlationId && <>Referencia: {error.correlationId}</>}
          </small>
        )}
      </div>
    </div>
  );
}
