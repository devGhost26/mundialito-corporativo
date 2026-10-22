import type { SortDirection } from "@/lib/api/types";

interface Props {
  options: { value: string; label: string }[];
  sortBy: string;
  sortDirection: SortDirection;
  onChange: (sortBy: string, sortDirection: SortDirection) => void;
}

export function SortControls({ options, sortBy, sortDirection, onChange }: Props) {
  return (
    <label>
      Ordenar por
      <span className="inline-fields">
        <select value={sortBy} onChange={(e) => onChange(e.target.value, sortDirection)}>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
        <select aria-label="Dirección" value={sortDirection} onChange={(e) => onChange(sortBy, e.target.value as SortDirection)}>
          <option value="asc">Ascendente</option>
          <option value="desc">Descendente</option>
        </select>
      </span>
    </label>
  );
}
