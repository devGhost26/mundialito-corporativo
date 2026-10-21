"use client";

import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
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

  const [player, setPlayer] = useState({ firstName: "", lastName: "", number: 1, position: "Forward" as PlayerPosition });

  async function onUpdate(e: React.FormEvent) {
    e.preventDefault();
    if (await update.submit(() => teamsApi.update(teamId, { name: teamName, city: teamCity || null }))) {
      await team.reload();
    }
  }

  async function onDelete() {
    if (!confirm("¿Eliminar este equipo?")) return;
    if (await remove.submit(() => teamsApi.remove(teamId))) router.push("/teams");
  }

  async function onAddPlayer(e: React.FormEvent) {
    e.preventDefault();
    if (await addPlayer.submit(() => teamsApi.addPlayer(teamId, player))) {
      setPlayer({ ...player, firstName: "", lastName: "" });
      await Promise.all([players.reload(), team.reload()]);
    }
  }

  if (team.error) return <ErrorBox error={team.error} />;
  if (!team.data) return <p className="muted">Cargando…</p>;

  return (
    <>
      <h1>{team.data.name}</h1>

      <section>
        <h2>Datos del equipo</h2>
        <form onSubmit={onUpdate}>
          <label>Nombre<input value={teamName} onChange={(e) => setTeamName(e.target.value)} required /></label>
          <label>Ciudad<input value={teamCity} onChange={(e) => setTeamCity(e.target.value)} /></label>
          <button disabled={update.submitting}>Guardar</button>
          <button type="button" onClick={onDelete} disabled={remove.submitting}>Eliminar equipo</button>
        </form>
        <ErrorBox error={update.error} />
        <ErrorBox error={remove.error} />
      </section>

      <section>
        <h2>Agregar jugador</h2>
        <form onSubmit={onAddPlayer}>
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
          <button disabled={addPlayer.submitting}>Agregar</button>
        </form>
        <ErrorBox error={addPlayer.error} />
      </section>

      <section>
        <h2>Integrantes ({team.data.playerCount})</h2>
        <div className="filters">
          <label>
            Nombre
            <input value={filters.name} onChange={(e) => setFilters({ ...filters, name: e.target.value, pageNumber: 1 })} />
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
        <ErrorBox error={players.error} />
        {players.data && (
          <>
            <table>
              <thead>
                <tr><th>Dorsal</th><th>Nombre</th><th>Posición</th></tr>
              </thead>
              <tbody>
                {players.data.data.map((p) => (
                  <tr key={p.id}>
                    <td>{p.number}</td>
                    <td>{p.firstName} {p.lastName}</td>
                    <td>{positionLabel(p.position)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {players.data.totalRecords === 0 && <p className="muted">No hay jugadores con esos filtros.</p>}
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
