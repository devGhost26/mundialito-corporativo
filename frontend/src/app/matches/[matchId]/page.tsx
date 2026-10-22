"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useState } from "react";
import { ErrorBox } from "@/components/ErrorBox";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";
import { matchesApi } from "@/lib/api/matches";
import { teamsApi } from "@/lib/api/teams";
import type { Match, Player } from "@/lib/api/types";
import { formatDateTime } from "@/lib/format";
import { useApi, useSubmit } from "@/lib/hooks/useApi";

interface GoalRow {
  playerId: string;
  minute: number;
}

export default function MatchDetailPage() {
  const { matchId } = useParams<{ matchId: string }>();
  const details = useApi(() => matchesApi.get(matchId), [matchId]);

  if (details.error) return <ErrorBox error={details.error} />;
  if (!details.data) return <div className="loading">Cargando…</div>;

  const { match, goals } = details.data;
  const played = match.status === "Played";

  return (
    <>
      <PageHeader
        breadcrumb={<Link href="/matches">← Partidos</Link>}
        title={`${match.homeTeamName} vs ${match.awayTeamName}`}
        description={played ? "Resultado registrado." : "Partido pendiente de resultado."}
      />

      <section className="card">
        <div className="scoreboard">
          <div className="team-name">{match.homeTeamName}</div>
          <div className="big-score">{played ? `${match.homeGoals} – ${match.awayGoals}` : "vs"}</div>
          <div className="team-name">{match.awayTeamName}</div>
        </div>
        <div className="meta">
          <span>{formatDateTime(match.scheduledAtUtc)}</span>
          <span>{match.venue ?? "Sin cancha asignada"}</span>
          <StatusBadge status={match.status} />
        </div>
        {goals.length > 0 && (
          <div className="table-wrap">
            <table>
              <thead><tr><th className="num">Minuto</th><th>Goleador</th><th>Equipo</th></tr></thead>
              <tbody>
                {goals.map((g, i) => (
                  <tr key={i}>
                    <td className="num strong">{g.minute}&apos;</td>
                    <td>{g.playerName}</td>
                    <td className="muted">{g.teamId === match.homeTeamId ? match.homeTeamName : match.awayTeamName}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>

      {!played && <RegisterResult match={match} onRegistered={details.reload} />}
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
    <section className="card">
      <div className="card-header">
        <div>
          <h2>Registrar resultado</h2>
          <p>Agrega un goleador por cada gol: deben cuadrar con el marcador.</p>
        </div>
      </div>
      <form className="card-body goal-list" onSubmit={onSubmit}>
        <ErrorBox error={players.error} />
        <div className="form-grid">
          <label>
            Goles {match.homeTeamName}
            <input type="number" min={0} value={homeGoals} onChange={(e) => setHomeGoals(Number(e.target.value))} />
          </label>
          <label>
            Goles {match.awayTeamName}
            <input type="number" min={0} value={awayGoals} onChange={(e) => setAwayGoals(Number(e.target.value))} />
          </label>
        </div>

        {goals.map((goal, index) => (
          <div className="goal-row" key={index}>
            <label>
              Goleador {index + 1}
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
            <button type="button" className="btn btn-danger" onClick={() => setGoals(goals.filter((_, i) => i !== index))}>
              Quitar
            </button>
          </div>
        ))}

        <div className="actions">
          <button type="button" className="btn" onClick={() => setGoals([...goals, { playerId: "", minute: 1 }])}>
            + Agregar gol
          </button>
          <button className="btn btn-primary" disabled={register.submitting}>Registrar resultado</button>
        </div>
        <ErrorBox error={register.error} />
      </form>
    </section>
  );
}
