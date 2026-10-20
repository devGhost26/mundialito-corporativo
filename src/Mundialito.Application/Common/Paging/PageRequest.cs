using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Common.Paging
{
    public sealed record PageRequest(int PageNumber, int PageSize, string SortBy, SortDirection SortDirection)
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

        public int Offset => (PageNumber - 1) * PageSize;

        public static Result<PageRequest> Create(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            string? sortDirection,
            IReadOnlyList<string> allowedSortFields,
            SortDirection defaultDirection = SortDirection.Ascending)
        {
            var number = pageNumber ?? 1;
            if (number < 1)
                return PagingErrors.InvalidPageNumber;

            var size = pageSize ?? DefaultPageSize;
            if (size is < 1 or > MaxPageSize)
                return PagingErrors.InvalidPageSize;

            var field = string.IsNullOrWhiteSpace(sortBy) ? allowedSortFields[0] : sortBy.Trim();
            var allowedField = allowedSortFields.FirstOrDefault(f => f.Equals(field, StringComparison.OrdinalIgnoreCase));
            if (allowedField is null)
                return PagingErrors.InvalidSortBy(allowedSortFields);

            SortDirection direction;
            if (string.IsNullOrWhiteSpace(sortDirection))
                direction = defaultDirection;
            else if (sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase))
                direction = SortDirection.Ascending;
            else if (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                direction = SortDirection.Descending;
            else
                return PagingErrors.InvalidSortDirection;

            return new PageRequest(number, size, allowedField, direction);
        }
    }
}
