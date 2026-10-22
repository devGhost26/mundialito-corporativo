"use client";

import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { PageHeader } from "@/components/PageHeader";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { TeamAvatar } from "@/components/TeamAvatar";
import { TeamSelect } from "@/components/TeamSelect";
import { standingsApi, type TopScorerFilters } from "@/lib/api/standings";
import { useApi } from "@/lib/hooks/useApi";

const sortOptions = [
  { value: "goals", label: "Goles" },
  { value: "name", label: "Nombre" },
];

export default function ScorersPage() {
  const [filters, setFilters] = useState<TopScorerFilters>({
    pageNumber: 1, pageSize: 10, sortBy: "goals", sortDirection: "desc", teamId: "",
  });
  const { data, error, loading } = useApi(() => standingsApi.topScorers(filters), [filters]);
  const offset = data ? (data.pageNumber - 1) * data.pageSize : 0;

  return (
    <>
      <PageHeader title="Goleadores" description="Goles registrados en los resultados de cada partido." />

      <section className="card">
        <div className="card-header">
          <h2>Ranking</h2>
          <div className="filters">
            <TeamSelect
              label="Equipo"
              value={filters.teamId ?? ""}
              emptyLabel="Todos los equipos"
              onChange={(teamId) => setFilters({ ...filters, teamId, pageNumber: 1 })}
            />
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
                  <tr><th>#</th><th>Jugador</th><th>Equipo</th><th className="num">Goles</th></tr>
                </thead>
                <tbody>
                  {data.data.map((s, index) => {
                    const rank = offset + index + 1;
                    return (
                      <tr key={`${s.playerId}-${s.teamId}`}>
                        <td><span className={`position ${rank <= 3 && filters.sortBy === "goals" ? `p${rank}` : ""}`}>{rank}</span></td>
                        <td className="strong">{s.playerName}</td>
                        <td><span className="team-cell"><TeamAvatar name={s.teamName} />{s.teamName}</span></td>
                        <td className="num strong">{s.goals}</td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
            {data.totalRecords === 0 && <div className="empty">Todavía no hay goles registrados.</div>}
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
