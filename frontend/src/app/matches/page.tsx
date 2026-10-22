"use client";

import Link from "next/link";
import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { PageHeader } from "@/components/PageHeader";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { StatusBadge } from "@/components/StatusBadge";
import { TeamSelect } from "@/components/TeamSelect";
import { matchesApi, type MatchFilters } from "@/lib/api/matches";
import type { MatchStatus } from "@/lib/api/types";
import { formatDateTime, localInputToUtcIso, score } from "@/lib/format";
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
      <PageHeader title="Partidos" description="Calendario del torneo y registro de resultados." />

      <section className="card">
        <div className="card-header">
          <div>
            <h2>Programar partido</h2>
            <p>La fecha se guarda en UTC y se muestra en tu hora local.</p>
          </div>
        </div>
        <div className="card-body">
          <form className="form-grid" onSubmit={onSchedule}>
            <TeamSelect label="Local" value={form.homeTeamId} required onChange={(homeTeamId) => setForm({ ...form, homeTeamId })} />
            <TeamSelect label="Visitante" value={form.awayTeamId} required onChange={(awayTeamId) => setForm({ ...form, awayTeamId })} />
            <label>
              Fecha y hora
              <input type="datetime-local" value={form.scheduledAt} required
                onChange={(e) => setForm({ ...form, scheduledAt: e.target.value })} />
            </label>
            <label>
              Cancha
              <input placeholder="Opcional" value={form.venue} onChange={(e) => setForm({ ...form, venue: e.target.value })} />
            </label>
            <button className="btn btn-primary" disabled={schedule.submitting}>Programar partido</button>
          </form>
          <ErrorBox error={schedule.error} />
        </div>
      </section>

      <section className="card">
        <div className="card-header">
          <h2>Calendario</h2>
        </div>
        <div className="card-body">
          <div className="filters">
            <label>
              Desde
              <input type="date" value={filters.from} onChange={(e) => setFilters({ ...filters, from: e.target.value, pageNumber: 1 })} />
            </label>
            <label>
              Hasta
              <input type="date" value={filters.to} onChange={(e) => setFilters({ ...filters, to: e.target.value, pageNumber: 1 })} />
            </label>
            <TeamSelect label="Equipo" value={filters.teamId ?? ""} emptyLabel="Todos los equipos"
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
        </div>
        {error && <div className="card-body"><ErrorBox error={error} /></div>}
        {loading && !data && <div className="loading">Cargando…</div>}
        {data && (
          <>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr><th>Fecha</th><th>Local</th><th className="num">Marcador</th><th>Visitante</th><th>Cancha</th><th>Estado</th><th></th></tr>
                </thead>
                <tbody>
                  {data.data.map((m) => (
                    <tr key={m.id}>
                      <td className="muted">{formatDateTime(m.scheduledAtUtc)}</td>
                      <td className="strong">{m.homeTeamName}</td>
                      <td className="num">
                        <span className={`score ${m.status === "Played" ? "" : "pending"}`}>{score(m.homeGoals, m.awayGoals)}</span>
                      </td>
                      <td className="strong">{m.awayTeamName}</td>
                      <td className="muted">{m.venue ?? "—"}</td>
                      <td><StatusBadge status={m.status} /></td>
                      <td>
                        <Link className={`btn btn-sm ${m.status === "Scheduled" ? "btn-primary" : ""}`} href={`/matches/${m.id}`}>
                          {m.status === "Scheduled" ? "Registrar resultado" : "Ver detalle"}
                        </Link>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {data.totalRecords === 0 && <div className="empty">No hay partidos con esos filtros.</div>}
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
