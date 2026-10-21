const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080";

export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly code?: string,
    readonly correlationId?: string,
  ) {
    super(message);
  }
}

type QueryValue = string | number | undefined | null;

export function buildQuery(params: Record<string, QueryValue>): string {
  const query = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== null && value !== "") query.set(key, String(value));
  }
  const text = query.toString();
  return text ? `?${text}` : "";
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  let response: Response;

  try {
    response = await fetch(`${API_URL}${path}`, {
      ...init,
      headers: { "Content-Type": "application/json", ...init.headers },
    });
  } catch {
    throw new ApiError("No se pudo conectar con la API. Revisa que esté levantada.", 0);
  }

  if (response.status === 204) return undefined as T;

  const body = await response.json().catch(() => null);

  if (!response.ok) {
    throw new ApiError(
      body?.detail ?? body?.title ?? `Error ${response.status}`,
      response.status,
      body?.errorCode ?? body?.title,
      body?.correlationId ?? response.headers.get("X-Correlation-Id") ?? undefined,
    );
  }

  return body as T;
}

export const http = {
  get: <T>(path: string) => request<T>(path),

  post: <T>(path: string, body: unknown) =>
    request<T>(path, {
      method: "POST",
      body: JSON.stringify(body),
      headers: { "Idempotency-Key": crypto.randomUUID() },
    }),

  put: (path: string, body: unknown) => request<void>(path, { method: "PUT", body: JSON.stringify(body) }),

  patch: (path: string, body: unknown) => request<void>(path, { method: "PATCH", body: JSON.stringify(body) }),

  delete: (path: string) => request<void>(path, { method: "DELETE" }),
};
