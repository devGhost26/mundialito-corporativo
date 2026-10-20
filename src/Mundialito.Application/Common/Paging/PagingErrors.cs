using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Common.Paging
{
    public static class PagingErrors
    {
        public static readonly Error InvalidPageNumber =
            Error.Validation("Paging.InvalidPageNumber", "pageNumber debe ser 1 o mayor.");

        public static readonly Error InvalidPageSize =
            Error.Validation("Paging.InvalidPageSize", $"pageSize debe estar entre 1 y {PageRequest.MaxPageSize}.");

        public static readonly Error InvalidSortDirection =
            Error.Validation("Paging.InvalidSortDirection", "sortDirection debe ser 'asc' o 'desc'.");

        public static Error InvalidSortBy(IEnumerable<string> allowed) =>
            Error.Validation("Paging.InvalidSortBy", $"sortBy debe ser uno de: {string.Join(", ", allowed)}.");
    }
}
