"use client";

import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
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

  return (
    <>
      <h1>Goleadores</h1>
      <section>
        <div className="filters">
          <TeamSelect
            label="Equipo"
            value={filters.teamId ?? ""}
            emptyLabel="Todos"
            onChange={(teamId) => setFilters({ ...filters, teamId, pageNumber: 1 })}
          />
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
                <tr><th>Jugador</th><th>Equipo</th><th>Goles</th></tr>
              </thead>
              <tbody>
                {data.data.map((s) => (
                  <tr key={`${s.playerId}-${s.teamId}`}>
                    <td>{s.playerName}</td>
                    <td>{s.teamName}</td>
                    <td><strong>{s.goals}</strong></td>
                  </tr>
                ))}
              </tbody>
            </table>
            {data.totalRecords === 0 && <p className="muted">Todavía no hay goles registrados.</p>}
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
