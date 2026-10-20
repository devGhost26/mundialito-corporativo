using Mundialito.Application.Common.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Queries
{
    internal static class SqlSort
    {
        public static string OrderBy(PageRequest page, IReadOnlyDictionary<string, string> columns, params string[] tieBreakers)
        {
            var column = columns[page.SortBy];
            var direction = page.SortDirection == SortDirection.Descending ? "DESC" : "ASC";

            var orderBy = new List<string> { $"{column} {direction}" };
            orderBy.AddRange(tieBreakers.Where(tieBreaker => tieBreaker != column));

            return "ORDER BY " + string.Join(", ", orderBy);
        }
    }
}
