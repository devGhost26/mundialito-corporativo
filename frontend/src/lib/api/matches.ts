import { buildQuery, http } from "./http";
import type { Created, Match, MatchDetails, MatchStatus, PagedResult, PageQuery } from "./types";

export interface MatchFilters extends PageQuery {
  from?: string;
  to?: string;
  teamId?: string;
  status?: MatchStatus | "";
}

export interface ScheduleMatchInput {
  homeTeamId: string;
  awayTeamId: string;
  scheduledAtUtc: string;
  venue: string | null;
}

export interface RegisterResultInput {
  homeGoals: number;
  awayGoals: number;
  goals: { playerId: string; minute: number }[];
}

export const matchesApi = {
  list: (filters: MatchFilters) => http.get<PagedResult<Match>>(`/api/matches${buildQuery({ ...filters })}`),

  get: (matchId: string) => http.get<MatchDetails>(`/api/matches/${matchId}`),

  schedule: (input: ScheduleMatchInput) => http.post<Created>("/api/matches", input),

  registerResult: (matchId: string, input: RegisterResultInput) =>
    http.post<{ matchId: string }>(`/api/matches/${matchId}/result`, input),
};
