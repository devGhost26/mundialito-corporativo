"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { PageHeader } from "@/components/PageHeader";
import { Pagination } from "@/components/Pagination";
import { SortControls } from "@/components/SortControls";
import { teamsApi, type PlayerFilters } from "@/lib/api/teams";
import { playerPositions, type PlayerPosition } from "@/lib/api/types";
import { positionLabel } from "@/lib/format";
import { useApi, useSubmit } from "@/lib/hooks/useApi";

const sortOptions = [
  { value: "number", label: "Dorsal" },
  { value: "name", label: "Apellido" },
  { value: "position", label: "Posición" },
];

export default function TeamDetailPage() {
  const { teamId } = useParams<{ teamId: string }>();
  const router = useRouter();

  const team = useApi(() => teamsApi.get(teamId), [teamId]);
  const [filters, setFilters] = useState<PlayerFilters>({
    pageNumber: 1, pageSize: 10, sortBy: "number", sortDirection: "asc", name: "", position: "",
  });
  const players = useApi(() => teamsApi.listPlayers(teamId, filters), [teamId, filters]);

  const [teamName, setTeamName] = useState("");
  const [teamCity, setTeamCity] = useState("");
  useEffect(() => {
    if (team.data) {
      setTeamName(team.data.name);
      setTeamCity(team.data.city ?? "");
    }
  }, [team.data]);

  const update = useSubmit();
  const remove = useSubmit();
  const addPlayer = useSubmit();
  const [confirmDelete, setConfirmDelete] = useState(false);

  const [player, setPlayer] = useState({ firstName: "", lastName: "", number: 1, position: "Forward" as PlayerPosition });

  async function onUpdate(e: React.FormEvent) {
    e.preventDefault();
    if (await update.submit(() => teamsApi.update(teamId, { name: teamName, city: teamCity || null }))) {
      await team.reload();
    }
  }

  async function onDelete() {
    if (await remove.submit(() => teamsApi.remove(teamId))) router.push("/teams");
    else setConfirmDelete(false);
  }

  async function onAddPlayer(e: React.FormEvent) {
    e.preventDefault();
    if (await addPlayer.submit(() => teamsApi.addPlayer(teamId, player))) {
      setPlayer({ ...player, firstName: "", lastName: "" });
      await Promise.all([players.reload(), team.reload()]);
    }
  }

  if (team.error) return <ErrorBox error={team.error} />;
  if (!team.data) return <div className="loading">Cargando…</div>;

  return (
    <>
      <PageHeader
        breadcrumb={<Link href="/teams">← Equipos</Link>}
        title={team.data.name}
        description={`${team.data.city ?? "Sin ciudad"} · ${team.data.playerCount} integrantes`}
      />

      <div className="grid-2">
        <section className="card">
          <div className="card-header"><h2>Datos del equipo</h2></div>
          <div className="card-body">
            <form className="goal-list" onSubmit={onUpdate}>
              <label>Nombre<input value={teamName} onChange={(e) => setTeamName(e.target.value)} required /></label>
              <label>Ciudad<input placeholder="Opcional" value={teamCity} onChange={(e) => setTeamCity(e.target.value)} /></label>
              <div className="actions">
                <button className="btn btn-primary" disabled={update.submitting}>Guardar cambios</button>
                {confirmDelete ? (
                  <>
                    <button type="button" className="btn btn-danger" onClick={onDelete} disabled={remove.submitting}>Sí, eliminar</button>
                    <button type="button" className="btn" onClick={() => setConfirmDelete(false)}>Cancelar</button>
                  </>
                ) : (
                  <button type="button" className="btn btn-danger" onClick={() => setConfirmDelete(true)}>Eliminar equipo</button>
                )}
              </div>
            </form>
            <ErrorBox error={update.error} />
            <ErrorBox error={remove.error} />
          </div>
        </section>

        <section className="card">
          <div className="card-header"><h2>Agregar integrante</h2></div>
          <div className="card-body">
            <form className="form-grid" onSubmit={onAddPlayer}>
              <label>Nombre<input value={player.firstName} onChange={(e) => setPlayer({ ...player, firstName: e.target.value })} required /></label>
              <label>Apellido<input value={player.lastName} onChange={(e) => setPlayer({ ...player, lastName: e.target.value })} required /></label>
              <label>
                Dorsal
                <input type="number" min={1} max={99} value={player.number}
                  onChange={(e) => setPlayer({ ...player, number: Number(e.target.value) })} required />
              </label>
              <label>
                Posición
                <select value={player.position} onChange={(e) => setPlayer({ ...player, position: e.target.value as PlayerPosition })}>
                  {playerPositions.map((p) => <option key={p.value} value={p.value}>{p.label}</option>)}
                </select>
              </label>
              <button className="btn btn-primary" disabled={addPlayer.submitting}>Agregar</button>
            </form>
            <ErrorBox error={addPlayer.error} />
          </div>
        </section>
      </div>

      <section className="card">
        <div className="card-header">
          <h2>Integrantes</h2>
          <div className="filters">
            <label>
              Nombre
              <input placeholder="Buscar…" value={filters.name} onChange={(e) => setFilters({ ...filters, name: e.target.value, pageNumber: 1 })} />
            </label>
            <label>
              Posición
              <select value={filters.position} onChange={(e) => setFilters({ ...filters, position: e.target.value as PlayerPosition | "", pageNumber: 1 })}>
                <option value="">Todas</option>
                {playerPositions.map((p) => <option key={p.value} value={p.value}>{p.label}</option>)}
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
        {players.error && <div className="card-body"><ErrorBox error={players.error} /></div>}
        {players.data && (
          <>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr><th className="num">Dorsal</th><th>Nombre</th><th>Posición</th></tr>
                </thead>
                <tbody>
                  {players.data.data.map((p) => (
                    <tr key={p.id}>
                      <td className="num"><span className="position">{p.number}</span></td>
                      <td className="strong">{p.firstName} {p.lastName}</td>
                      <td className="muted">{positionLabel(p.position)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {players.data.totalRecords === 0 && <div className="empty">No hay integrantes con esos filtros.</div>}
            <Pagination
              page={players.data}
              onPageChange={(pageNumber) => setFilters({ ...filters, pageNumber })}
              onPageSizeChange={(pageSize) => setFilters({ ...filters, pageSize, pageNumber: 1 })}
            />
          </>
        )}
      </section>
    </>
  );
}
