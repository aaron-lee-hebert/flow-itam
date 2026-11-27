using FlowItam.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FlowItam.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            SslMode = SslMode.VerifyFull
        };

        services.AddDbContext<FlowItamDbContext>(options =>
            options.UseNpgsql(connectionStringBuilder.ConnectionString));

        return services;
    }
}