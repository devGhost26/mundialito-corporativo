"use client";

import { teamsApi } from "@/lib/api/teams";
import { useApi } from "@/lib/hooks/useApi";

interface Props {
  label: string;
  value: string;
  onChange: (teamId: string) => void;
  emptyLabel?: string;
  required?: boolean;
}

export function TeamSelect({ label, value, onChange, emptyLabel = "Selecciona…", required }: Props) {
  const { data } = useApi(() => teamsApi.list({ pageSize: 100, sortBy: "name" }), []);

  return (
    <label>
      {label}
      <select value={value} required={required} onChange={(e) => onChange(e.target.value)}>
        <option value="">{emptyLabel}</option>
        {data?.data.map((team) => (
          <option key={team.id} value={team.id}>
            {team.name}
          </option>
        ))}
      </select>
    </label>
  );
}
