using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Mundialito.Infrastructure.Queries
{
    internal sealed class SqlConnectionFactory(string connectionString)
    {
        public async Task<DbConnection> OpenAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
