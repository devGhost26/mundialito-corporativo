export type SortDirection = "asc" | "desc";

export interface PageQuery {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: SortDirection;
}

export interface PagedResult<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export type PlayerPosition = "Goalkeeper" | "Defender" | "Midfielder" | "Forward";

export const playerPositions: { value: PlayerPosition; label: string }[] = [
  { value: "Goalkeeper", label: "Portero" },
  { value: "Defender", label: "Defensa" },
  { value: "Midfielder", label: "Medio" },
  { value: "Forward", label: "Delantero" },
];

export type MatchStatus = "Scheduled" | "Played";

export interface Team {
  id: string;
  name: string;
  city: string | null;
  playerCount: number;
  createdAtUtc: string;
}

export interface Player {
  id: string;
  teamId: string;
  firstName: string;
  lastName: string;
  number: number;
  position: PlayerPosition;
}

export interface Match {
  id: string;
  homeTeamId: string;
  homeTeamName: string;
  awayTeamId: string;
  awayTeamName: string;
  scheduledAtUtc: string;
  venue: string | null;
  status: MatchStatus;
  homeGoals: number | null;
  awayGoals: number | null;
}

export interface MatchGoal {
  playerId: string;
  playerName: string;
  teamId: string;
  minute: number;
}

export interface MatchDetails {
  match: Match;
  goals: MatchGoal[];
}

export interface Standing {
  position: number;
  teamId: string;
  teamName: string;
  played: number;
  won: number;
  drawn: number;
  lost: number;
  goalsFor: number;
  goalsAgainst: number;
  goalDifference: number;
  points: number;
}

export interface TopScorer {
  playerId: string;
  playerName: string;
  teamId: string;
  teamName: string;
  goals: number;
}

export interface Created {
  id: string;
}
