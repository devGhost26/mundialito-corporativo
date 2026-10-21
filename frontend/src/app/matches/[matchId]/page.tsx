"use client";

import { useParams } from "next/navigation";
import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { matchesApi } from "@/lib/api/matches";
import { teamsApi } from "@/lib/api/teams";
import type { Match, Player } from "@/lib/api/types";
import { formatDateTime, score, statusLabel } from "@/lib/format";
import { useApi, useSubmit } from "@/lib/hooks/useApi";

interface GoalRow {
  playerId: string;
  minute: number;
}

export default function MatchDetailPage() {
  const { matchId } = useParams<{ matchId: string }>();
  const details = useApi(() => matchesApi.get(matchId), [matchId]);

  if (details.error) return <ErrorBox error={details.error} />;
  if (!details.data) return <p className="muted">Cargando…</p>;

  const { match, goals } = details.data;

  return (
    <>
      <h1>
        {match.homeTeamName} {score(match.homeGoals, match.awayGoals)} {match.awayTeamName}
      </h1>
      <section>
        <p>
          {formatDateTime(match.scheduledAtUtc)} · {match.venue ?? "Sin cancha"} · {statusLabel(match.status)}
        </p>
        {goals.length > 0 && (
          <table>
            <thead><tr><th>Minuto</th><th>Goleador</th><th>Equipo</th></tr></thead>
            <tbody>
              {goals.map((g, i) => (
                <tr key={i}>
                  <td>{g.minute}&apos;</td>
                  <td>{g.playerName}</td>
                  <td>{g.teamId === match.homeTeamId ? match.homeTeamName : match.awayTeamName}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
      {match.status === "Scheduled" && <RegisterResult match={match} onRegistered={details.reload} />}
    </>
  );
}

function RegisterResult({ match, onRegistered }: { match: Match; onRegistered: () => Promise<void> }) {
  const players = useApi(async () => {
    const [home, away] = await Promise.all([
      teamsApi.listPlayers(match.homeTeamId, { pageSize: 100, sortBy: "number" }),
      teamsApi.listPlayers(match.awayTeamId, { pageSize: 100, sortBy: "number" }),
    ]);
    return { home: home.data, away: away.data };
  }, [match.homeTeamId, match.awayTeamId]);

  const [homeGoals, setHomeGoals] = useState(0);
  const [awayGoals, setAwayGoals] = useState(0);
  const [goals, setGoals] = useState<GoalRow[]>([]);
  const register = useSubmit();

  const updateGoal = (index: number, change: Partial<GoalRow>) =>
    setGoals(goals.map((goal, i) => (i === index ? { ...goal, ...change } : goal)));

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (await register.submit(() => matchesApi.registerResult(match.id, { homeGoals, awayGoals, goals }))) {
      await onRegistered();
    }
  }

  const playerOptions = (list: Player[]) =>
    list.map((p) => <option key={p.id} value={p.id}>{p.number} · {p.firstName} {p.lastName}</option>);

  return (
    <section>
      <h2>Registrar resultado</h2>
      <ErrorBox error={players.error} />
      <form onSubmit={onSubmit}>
        <label>
          Goles {match.homeTeamName}
          <input type="number" min={0} value={homeGoals} onChange={(e) => setHomeGoals(Number(e.target.value))} />
        </label>
        <label>
          Goles {match.awayTeamName}
          <input type="number" min={0} value={awayGoals} onChange={(e) => setAwayGoals(Number(e.target.value))} />
        </label>
        <div style={{ flexBasis: "100%" }}>
          <p className="muted">Agrega un goleador por cada gol: deben cuadrar con el marcador.</p>
          {goals.map((goal, index) => (
            <div className="goal-row" key={index}>
              <label>
                Goleador
                <select value={goal.playerId} required onChange={(e) => updateGoal(index, { playerId: e.target.value })}>
                  <option value="">Selecciona…</option>
                  <optgroup label={match.homeTeamName}>{playerOptions(players.data?.home ?? [])}</optgroup>
                  <optgroup label={match.awayTeamName}>{playerOptions(players.data?.away ?? [])}</optgroup>
                </select>
              </label>
              <label>
                Minuto
                <input type="number" min={1} max={120} value={goal.minute} required
                  onChange={(e) => updateGoal(index, { minute: Number(e.target.value) })} />
              </label>
              <button type="button" onClick={() => setGoals(goals.filter((_, i) => i !== index))}>Quitar</button>
            </div>
          ))}
          <button type="button" onClick={() => setGoals([...goals, { playerId: "", minute: 1 }])}>+ Agregar gol</button>
        </div>
        <button disabled={register.submitting}>Registrar resultado</button>
      </form>
      <ErrorBox error={register.error} />
    </section>
  );
}
