"use client";

import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
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

  return (
    <>
      <h1>Tabla de posiciones</h1>
      <section>
        <div className="filters">
          <SortControls
            options={sortOptions}
            sortBy={query.sortBy}
            sortDirection={query.sortDirection}
            onChange={(sortBy, sortDirection) => setQuery({ ...query, sortBy, sortDirection, pageNumber: 1 })}
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
                <tr>
                  <th>#</th><th>Equipo</th><th>PJ</th><th>G</th><th>E</th><th>P</th>
                  <th>GF</th><th>GC</th><th>DG</th><th>Pts</th>
                </tr>
              </thead>
              <tbody>
                {data.data.map((s) => (
                  <tr key={s.teamId}>
                    <td>{s.position}</td>
                    <td>{s.teamName}</td>
                    <td>{s.played}</td>
                    <td>{s.won}</td>
                    <td>{s.drawn}</td>
                    <td>{s.lost}</td>
                    <td>{s.goalsFor}</td>
                    <td>{s.goalsAgainst}</td>
                    <td>{s.goalDifference > 0 ? `+${s.goalDifference}` : s.goalDifference}</td>
                    <td><strong>{s.points}</strong></td>
                  </tr>
                ))}
              </tbody>
            </table>
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
