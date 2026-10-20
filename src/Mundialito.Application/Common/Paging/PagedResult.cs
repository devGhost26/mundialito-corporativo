using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Common.Paging
{
    public sealed record PagedResult<T>(IReadOnlyList<T> Data, int PageNumber, int PageSize, int TotalRecords)
    {
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }
}
