using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Mundialito.Infrastructure.Persistence;
using Testcontainers.MsSql;
namespace Mundialito.IntegrationTests
{
    public sealed class MundialitoApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _database =
            new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

        public async Task InitializeAsync() => await _database.StartAsync();

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();
            await _database.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connectionString = new SqlConnectionStringBuilder(_database.GetConnectionString())
            {
                InitialCatalog = "Mundialito"
            }.ConnectionString;

            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:Mundialito", connectionString);
        }

        internal async Task<T> QueryDatabaseAsync<T>(Func<AppDbContext, Task<T>> query)
        {
            using var scope = Services.CreateScope();
            return await query(scope.ServiceProvider.GetRequiredService<AppDbContext>());
        }
    }
}
