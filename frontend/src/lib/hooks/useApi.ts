"use client";

import { useCallback, useEffect, useState } from "react";
import { ApiError } from "@/lib/api/http";

export function useApi<T>(load: () => Promise<T>, deps: unknown[]) {
  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<ApiError | null>(null);
  const [loading, setLoading] = useState(true);

  const run = useCallback(load, deps);

  const reload = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setData(await run());
    } catch (e) {
      setError(e instanceof ApiError ? e : new ApiError("Ocurrió un error inesperado.", 0));
    } finally {
      setLoading(false);
    }
  }, [run]);

  useEffect(() => {
    void reload();
  }, [reload]);

  return { data, error, loading, reload };
}

export function useSubmit() {
  const [error, setError] = useState<ApiError | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const submit = useCallback(async (action: () => Promise<unknown>) => {
    setSubmitting(true);
    setError(null);
    try {
      await action();
      return true;
    } catch (e) {
      setError(e instanceof ApiError ? e : new ApiError("Ocurrió un error inesperado.", 0));
      return false;
    } finally {
      setSubmitting(false);
    }
  }, []);

  return { submit, error, submitting };
}
