import { buildQuery, http } from "./http";
import type { Created, PagedResult, PageQuery, Player, PlayerPosition, Team } from "./types";

export interface TeamFilters extends PageQuery {
  name?: string;
  city?: string;
}

export interface PlayerFilters extends PageQuery {
  name?: string;
  position?: PlayerPosition | "";
}

export interface TeamInput {
  name: string;
  city: string | null;
}

export interface PlayerInput {
  firstName: string;
  lastName: string;
  number: number;
  position: PlayerPosition;
}

export const teamsApi = {
  list: (filters: TeamFilters) => http.get<PagedResult<Team>>(`/api/teams${buildQuery({ ...filters })}`),

  get: (teamId: string) => http.get<Team>(`/api/teams/${teamId}`),

  create: (input: TeamInput) => http.post<Created>("/api/teams", input),

  update: (teamId: string, input: TeamInput) => http.put(`/api/teams/${teamId}`, input),

  remove: (teamId: string) => http.delete(`/api/teams/${teamId}`),

  listPlayers: (teamId: string, filters: PlayerFilters) =>
    http.get<PagedResult<Player>>(`/api/teams/${teamId}/players${buildQuery({ ...filters })}`),

  addPlayer: (teamId: string, input: PlayerInput) => http.post<Created>(`/api/teams/${teamId}/players`, input),
};
