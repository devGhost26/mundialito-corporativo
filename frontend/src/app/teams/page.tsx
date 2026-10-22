"use client";

import Link from "next/link";
import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { PageHeader } from "@/components/PageHeader";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { TeamAvatar } from "@/components/TeamAvatar";
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
      <PageHeader title="Equipos" description="Equipos participantes y sus integrantes." />

      <section className="card">
        <div className="card-header">
          <h2>Nuevo equipo</h2>
        </div>
        <div className="card-body">
          <form className="form-grid" onSubmit={onCreate}>
            <label>Nombre<input value={name} onChange={(e) => setName(e.target.value)} required /></label>
            <label>Ciudad<input placeholder="Opcional" value={city} onChange={(e) => setCity(e.target.value)} /></label>
            <button className="btn btn-primary" disabled={create.submitting}>Crear equipo</button>
          </form>
          <ErrorBox error={create.error} />
        </div>
      </section>

      <section className="card">
        <div className="card-header">
          <h2>Listado</h2>
          <div className="filters">
            <label>
              Nombre
              <input placeholder="Buscar…" value={filters.name} onChange={(e) => setFilters({ ...filters, name: e.target.value, pageNumber: 1 })} />
            </label>
            <label>
              Ciudad
              <input placeholder="Buscar…" value={filters.city} onChange={(e) => setFilters({ ...filters, city: e.target.value, pageNumber: 1 })} />
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
                  <tr><th>Equipo</th><th>Ciudad</th><th className="num">Jugadores</th><th>Creado</th></tr>
                </thead>
                <tbody>
                  {data.data.map((team) => (
                    <tr key={team.id}>
                      <td>
                        <Link className="team-cell" href={`/teams/${team.id}`}>
                          <TeamAvatar name={team.name} />{team.name}
                        </Link>
                      </td>
                      <td className="muted">{team.city ?? "—"}</td>
                      <td className="num">{team.playerCount}</td>
                      <td className="muted">{formatDateTime(team.createdAtUtc)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {data.totalRecords === 0 && <div className="empty">No hay equipos con esos filtros.</div>}
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
