using FlowItam.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FlowItam.Infrastructure;

public static class DependencyInjection
{
    private const string SqliteMigrationsAssembly =
        "FlowItam.Infrastructure.Migrations.Sqlite";
    private const string PostgreSqlMigrationsAssembly =
        "FlowItam.Infrastructure.Migrations.PostgreSql";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment,
        string contentRootPath)
    {
        var databaseProvider = configuration["Database:Provider"]
            ?? (isDevelopment ? "Sqlite" : "PostgreSql");
        var useSqlite = databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase);
        var usePostgreSql = databaseProvider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase);

        if (!useSqlite && !usePostgreSql)
        {
            throw new InvalidOperationException(
                $"Database provider '{databaseProvider}' is not supported.");
        }

        if (useSqlite && !isDevelopment)
        {
            throw new InvalidOperationException(
                "SQLite is only supported in the Development environment.");
        }

        var connectionName = useSqlite ? "Sqlite" : "PostgreSql";
        var connectionString = configuration.GetConnectionString(connectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionName}' is not configured.");
        }

        services.AddDbContext<FlowItamDbContext>(options =>
        {
            if (useSqlite)
            {
                var sqliteConnectionString = new SqliteConnectionStringBuilder(connectionString);
                if (sqliteConnectionString.DataSource != ":memory:"
                    && !Path.IsPathRooted(sqliteConnectionString.DataSource))
                {
                    sqliteConnectionString.DataSource = Path.GetFullPath(
                        sqliteConnectionString.DataSource,
                        contentRootPath);
                }

                options.UseSqlite(
                    sqliteConnectionString.ConnectionString,
                    sqlite => sqlite.MigrationsAssembly(SqliteMigrationsAssembly));
                return;
            }

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                SslMode = SslMode.VerifyFull
            };

            options.UseNpgsql(
                connectionStringBuilder.ConnectionString,
                postgres => postgres.MigrationsAssembly(PostgreSqlMigrationsAssembly));
        });

        return services;
    }
}