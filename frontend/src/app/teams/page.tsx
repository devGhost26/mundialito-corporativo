"use client";

import Link from "next/link";
import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { teamsApi, type TeamFilters } from "@/lib/api/teams";
import { formatDateTime } from "@/lib/format";
import { useApi, useSubmit } from "@/lib/hooks/useApi";

const sortOptions = [
  { value: "name", label: "Nombre" },
  { value: "city", label: "Ciudad" },
  { value: "createdAt", label: "Fecha de creación" },
];

export default function TeamsPage() {
  const [filters, setFilters] = useState<TeamFilters>({
    pageNumber: 1, pageSize: 10, sortBy: "name", sortDirection: "asc", name: "", city: "",
  });
  const { data, error, loading, reload } = useApi(() => teamsApi.list(filters), [filters]);

  const [name, setName] = useState("");
  const [city, setCity] = useState("");
  const create = useSubmit();

  async function onCreate(e: React.FormEvent) {
    e.preventDefault();
    const ok = await create.submit(() => teamsApi.create({ name, city: city || null }));
    if (ok) {
      setName("");
      setCity("");
      await reload();
    }
  }

  return (
    <>
      <h1>Equipos</h1>

      <section>
        <h2>Nuevo equipo</h2>
        <form onSubmit={onCreate}>
          <label>Nombre<input value={name} onChange={(e) => setName(e.target.value)} required /></label>
          <label>Ciudad<input value={city} onChange={(e) => setCity(e.target.value)} /></label>
          <button disabled={create.submitting}>Crear</button>
        </form>
        <ErrorBox error={create.error} />
      </section>

      <section>
        <div className="filters">
          <label>
            Nombre
            <input value={filters.name} onChange={(e) => setFilters({ ...filters, name: e.target.value, pageNumber: 1 })} />
          </label>
          <label>
            Ciudad
            <input value={filters.city} onChange={(e) => setFilters({ ...filters, city: e.target.value, pageNumber: 1 })} />
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
                <tr><th>Equipo</th><th>Ciudad</th><th>Jugadores</th><th>Creado</th></tr>
              </thead>
              <tbody>
                {data.data.map((team) => (
                  <tr key={team.id}>
                    <td><Link href={`/teams/${team.id}`}>{team.name}</Link></td>
                    <td>{team.city ?? "—"}</td>
                    <td>{team.playerCount}</td>
                    <td>{formatDateTime(team.createdAtUtc)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {data.totalRecords === 0 && <p className="muted">No hay equipos con esos filtros.</p>}
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
