"use client";

import Link from "next/link";
import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { TeamSelect } from "@/components/TeamSelect";
import { matchesApi, type MatchFilters } from "@/lib/api/matches";
import type { MatchStatus } from "@/lib/api/types";
import { formatDateTime, localInputToUtcIso, score, statusLabel } from "@/lib/format";
import { useApi, useSubmit } from "@/lib/hooks/useApi";

const sortOptions = [
  { value: "date", label: "Fecha" },
  { value: "status", label: "Estado" },
];

export default function MatchesPage() {
  const [filters, setFilters] = useState<MatchFilters>({
    pageNumber: 1, pageSize: 10, sortBy: "date", sortDirection: "asc", from: "", to: "", teamId: "", status: "",
  });
  const { data, error, loading, reload } = useApi(() => matchesApi.list(filters), [filters]);

  const [form, setForm] = useState({ homeTeamId: "", awayTeamId: "", scheduledAt: "", venue: "" });
  const schedule = useSubmit();

  async function onSchedule(e: React.FormEvent) {
    e.preventDefault();
    const ok = await schedule.submit(() => matchesApi.schedule({
      homeTeamId: form.homeTeamId,
      awayTeamId: form.awayTeamId,
      scheduledAtUtc: localInputToUtcIso(form.scheduledAt),
      venue: form.venue || null,
    }));
    if (ok) {
      setForm({ homeTeamId: "", awayTeamId: "", scheduledAt: "", venue: "" });
      await reload();
    }
  }

  return (
    <>
      <h1>Partidos</h1>

      <section>
        <h2>Programar partido</h2>
        <form onSubmit={onSchedule}>
          <TeamSelect label="Local" value={form.homeTeamId} required onChange={(homeTeamId) => setForm({ ...form, homeTeamId })} />
          <TeamSelect label="Visitante" value={form.awayTeamId} required onChange={(awayTeamId) => setForm({ ...form, awayTeamId })} />
          <label>
            Fecha y hora
            <input type="datetime-local" value={form.scheduledAt} required
              onChange={(e) => setForm({ ...form, scheduledAt: e.target.value })} />
          </label>
          <label>Cancha<input value={form.venue} onChange={(e) => setForm({ ...form, venue: e.target.value })} /></label>
          <button disabled={schedule.submitting}>Programar</button>
        </form>
        <ErrorBox error={schedule.error} />
      </section>

      <section>
        <div className="filters">
          <label>
            Desde
            <input type="date" value={filters.from} onChange={(e) => setFilters({ ...filters, from: e.target.value, pageNumber: 1 })} />
          </label>
          <label>
            Hasta
            <input type="date" value={filters.to} onChange={(e) => setFilters({ ...filters, to: e.target.value, pageNumber: 1 })} />
          </label>
          <TeamSelect label="Equipo" value={filters.teamId ?? ""} emptyLabel="Todos"
            onChange={(teamId) => setFilters({ ...filters, teamId, pageNumber: 1 })} />
          <label>
            Estado
            <select value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value as MatchStatus | "", pageNumber: 1 })}>
              <option value="">Todos</option>
              <option value="Scheduled">Programado</option>
              <option value="Played">Jugado</option>
            </select>
          </label>
          <SortControls
            options={sortOptions}
            sortBy={filters.sortBy!}
            sortDirection={filters.sortDirection!}
            onChange={(sortBy, sortDirection) => setFilters({ ...filters, sortBy, sortDirection, pageNumber: 1 })}
          />
        </div>
      </section>

      <section>
        <ErrorBox error={error} />
        {loading && <p className="muted">Cargando…</p>}
        {data && (
          <>
            <table>
              <thead>
                <tr><th>Fecha</th><th>Local</th><th>Marcador</th><th>Visitante</th><th>Cancha</th><th>Estado</th><th></th></tr>
              </thead>
              <tbody>
                {data.data.map((m) => (
                  <tr key={m.id}>
                    <td>{formatDateTime(m.scheduledAtUtc)}</td>
                    <td>{m.homeTeamName}</td>
                    <td><strong>{score(m.homeGoals, m.awayGoals)}</strong></td>
                    <td>{m.awayTeamName}</td>
                    <td>{m.venue ?? "—"}</td>
                    <td>{statusLabel(m.status)}</td>
                    <td><Link href={`/matches/${m.id}`}>{m.status === "Scheduled" ? "Registrar resultado" : "Ver detalle"}</Link></td>
                  </tr>
                ))}
              </tbody>
            </table>
            {data.totalRecords === 0 && <p className="muted">No hay partidos con esos filtros.</p>}
            <Pagination
              page={data}
              onPageChange={(pageNumber) => setFilters({ ...filters, pageNumber })}
              onPageSizeChange={(pageSize) => setFilters({ ...filters, pageSize, pageNumber: 1 })}
            />
          </>
        )}
      </section>
    </>
  );
}
