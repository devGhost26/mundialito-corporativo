import type { PagedResult } from "@/lib/api/types";

interface Props {
  page: PagedResult<unknown>;
  onPageChange: (pageNumber: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function Pagination({ page, onPageChange, onPageSizeChange }: Props) {
  const first = page.totalRecords === 0 ? 0 : (page.pageNumber - 1) * page.pageSize + 1;
  const last = Math.min(page.pageNumber * page.pageSize, page.totalRecords);

  return (
    <div className="card-footer pagination">
      <span className="pagination-info">
        Mostrando {first}–{last} de {page.totalRecords}
      </span>
      <div className="pagination-controls">
        <select aria-label="Registros por página" value={page.pageSize} onChange={(e) => onPageSizeChange(Number(e.target.value))}>
          {[5, 10, 20, 50].map((size) => (
            <option key={size} value={size}>
              {size} por página
            </option>
          ))}
        </select>
        <button className="btn btn-sm" disabled={page.pageNumber <= 1} onClick={() => onPageChange(page.pageNumber - 1)}>
          ← Anterior
        </button>
        <span className="muted">
          {page.pageNumber} / {Math.max(page.totalPages, 1)}
        </span>
        <button className="btn btn-sm" disabled={page.pageNumber >= page.totalPages} onClick={() => onPageChange(page.pageNumber + 1)}>
          Siguiente →
        </button>
      </div>
    </div>
  );
}
