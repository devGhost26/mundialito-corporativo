import { buildQuery, http } from "./http";
import type { PagedResult, PageQuery, Standing, TopScorer } from "./types";

export interface TopScorerFilters extends PageQuery {
  teamId?: string;
}

export const standingsApi = {
  table: (query: PageQuery) => http.get<PagedResult<Standing>>(`/api/standings${buildQuery({ ...query })}`),

  topScorers: (filters: TopScorerFilters) =>
    http.get<PagedResult<TopScorer>>(`/api/top-scorers${buildQuery({ ...filters })}`),
};
