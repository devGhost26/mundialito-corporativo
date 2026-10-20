using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Queries
{
    internal static class SqlLike
    {
        public static string Contains(string value)
        {
            var escaped = value.Trim()
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");

            return $"%{escaped}%";
        }
    }
}
