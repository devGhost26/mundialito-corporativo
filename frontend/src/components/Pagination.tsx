import type { PagedResult } from "@/lib/api/types";

interface Props {
  page: PagedResult<unknown>;
  onPageChange: (pageNumber: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function Pagination({ page, onPageChange, onPageSizeChange }: Props) {
  return (
    <div className="pagination">
      <button disabled={page.pageNumber <= 1} onClick={() => onPageChange(page.pageNumber - 1)}>
        ← Anterior
      </button>
      <span>
        Página {page.pageNumber} de {Math.max(page.totalPages, 1)} · {page.totalRecords} registros
      </span>
      <button disabled={page.pageNumber >= page.totalPages} onClick={() => onPageChange(page.pageNumber + 1)}>
        Siguiente →
      </button>
      <select value={page.pageSize} onChange={(e) => onPageSizeChange(Number(e.target.value))}>
        {[5, 10, 20, 50].map((size) => (
          <option key={size} value={size}>
            {size} por página
          </option>
        ))}
      </select>
    </div>
  );
}
