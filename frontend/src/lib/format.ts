import { playerPositions, type MatchStatus, type PlayerPosition } from "./api/types";

export const formatDateTime = (iso: string) =>
  new Date(iso).toLocaleString("es-SV", { dateStyle: "medium", timeStyle: "short" });

export const positionLabel = (position: PlayerPosition) =>
  playerPositions.find((p) => p.value === position)?.label ?? position;

export const statusLabel = (status: MatchStatus) => (status === "Played" ? "Jugado" : "Programado");

export const score = (home: number | null, away: number | null) =>
  home === null || away === null ? "vs" : `${home} - ${away}`;

export const localInputToUtcIso = (value: string) => new Date(value).toISOString();
