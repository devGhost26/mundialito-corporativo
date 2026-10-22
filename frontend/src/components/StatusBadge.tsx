import type { MatchStatus } from "@/lib/api/types";
import { statusLabel } from "@/lib/format";

export function StatusBadge({ status }: { status: MatchStatus }) {
  return <span className={`badge ${status === "Played" ? "badge-success" : "badge-info"}`}>{statusLabel(status)}</span>;
}
