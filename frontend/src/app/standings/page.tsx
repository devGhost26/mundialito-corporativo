"use client";

import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { PageHeader } from "@/components/PageHeader";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { TeamAvatar } from "@/components/TeamAvatar";
import { standingsApi } from "@/lib/api/standings";
import type { PageQuery } from "@/lib/api/types";
import { useApi } from "@/lib/hooks/useApi";

const sortOptions = [
  { value: "position", label: "Posición" },
  { value: "points", label: "Puntos" },
  { value: "goalsFor", label: "Goles a favor" },
  { value: "name", label: "Nombre" },
];

export default function StandingsPage() {
  const [query, setQuery] = useState<Required<PageQuery>>({
    pageNumber: 1, pageSize: 10, sortBy: "position", sortDirection: "asc",
  });
  const { data, error, loading } = useApi(() => standingsApi.table(query), [query]);
  const summary = useApi(() => standingsApi.table({ pageSize: 100 }), []);
  const scorers = useApi(() => standingsApi.topScorers({ pageSize: 1 }), []);

  const all = summary.data?.data ?? [];
  const leader = all.find((s) => s.position === 1);
  const matchesPlayed = all.reduce((total, s) => total + s.played, 0) / 2;
  const goals = all.reduce((total, s) => total + s.goalsFor, 0);
  const topScorer = scorers.data?.data[0];

  return (
    <>
      <PageHeader title="Tabla de posiciones" description="Se calcula al momento con los resultados registrados." />

      <div className="stats">
        <div className="card stat">
          <span className="stat-label">Líder</span>
          <span className="stat-value">{leader?.teamName ?? "—"}</span>
          <span className="stat-hint">{leader ? `${leader.points} puntos` : "Sin datos"}</span>
        </div>
        <div className="card stat">
          <span className="stat-label">Partidos jugados</span>
          <span className="stat-value">{matchesPlayed}</span>
          <span className="stat-hint">{all.length} equipos</span>
        </div>
        <div className="card stat">
          <span className="stat-label">Goles</span>
          <span className="stat-value">{goals}</span>
          <span className="stat-hint">{matchesPlayed ? `${(goals / matchesPlayed).toFixed(1)} por partido` : "—"}</span>
        </div>
        <div className="card stat">
          <span className="stat-label">Goleador</span>
          <span className="stat-value">{topScorer?.playerName ?? "—"}</span>
          <span className="stat-hint">{topScorer ? `${topScorer.goals} goles · ${topScorer.teamName}` : "Sin goles"}</span>
        </div>
      </div>

      <section className="card">
        <div className="card-header">
          <div>
            <h2>Clasificación</h2>
            <p>Puntos, diferencia de gol, goles a favor y nombre.</p>
          </div>
          <SortControls
            options={sortOptions}
            sortBy={query.sortBy}
            sortDirection={query.sortDirection}
            onChange={(sortBy, sortDirection) => setQuery({ ...query, sortBy, sortDirection, pageNumber: 1 })}
          />
        </div>
        {error && <div className="card-body"><ErrorBox error={error} /></div>}
        {loading && !data && <div className="loading">Cargando…</div>}
        {data && (
          <>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>#</th><th>Equipo</th>
                    <th className="num">PJ</th><th className="num">G</th><th className="num">E</th><th className="num">P</th>
                    <th className="num">GF</th><th className="num">GC</th><th className="num">DG</th><th className="num">Pts</th>
                  </tr>
                </thead>
                <tbody>
                  {data.data.map((s) => (
                    <tr key={s.teamId}>
                      <td><span className={`position ${s.position <= 3 ? `p${s.position}` : ""}`}>{s.position}</span></td>
                      <td><span className="team-cell"><TeamAvatar name={s.teamName} />{s.teamName}</span></td>
                      <td className="num">{s.played}</td>
                      <td className="num">{s.won}</td>
                      <td className="num">{s.drawn}</td>
                      <td className="num">{s.lost}</td>
                      <td className="num">{s.goalsFor}</td>
                      <td className="num">{s.goalsAgainst}</td>
                      <td className={`num ${s.goalDifference > 0 ? "positive" : s.goalDifference < 0 ? "negative" : "muted"}`}>
                        {s.goalDifference > 0 ? `+${s.goalDifference}` : s.goalDifference}
                      </td>
                      <td className="num strong">{s.points}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <Pagination
              page={data}
              onPageChange={(pageNumber) => setQuery({ ...query, pageNumber })}
              onPageSizeChange={(pageSize) => setQuery({ ...query, pageSize, pageNumber: 1 })}
            />
          </>
        )}
      </section>
    </>
  );
}
