using FlowItam.Infrastructure;
using FlowItam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FlowItam.IntegrationTests;

public class DatabaseConfigurationTests
{
    [Test]
    public void AddInfrastructure_requires_a_connection_string()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddInfrastructure(configuration, isDevelopment: true));

        Assert.That(exception!.Message, Does.Contain("Sqlite"));
    }

    [Test]
    public void AddInfrastructure_enforces_verified_tls()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Database:Provider"] = "PostgreSql",
            ["ConnectionStrings:PostgreSql"] =
                "Host=db.example.invalid;Database=flowitam;Username=test;Password=test;SSL Mode=Disable"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration, isDevelopment: false);

        using var provider = services.BuildServiceProvider();
        using var dbContext = provider.GetRequiredService<FlowItamDbContext>();
        var connectionString = new NpgsqlConnectionStringBuilder(
            dbContext.Database.GetDbConnection().ConnectionString);

        Assert.That(connectionString.SslMode, Is.EqualTo(SslMode.VerifyFull));
    }

    [Test]
    public void AddInfrastructure_uses_sqlite_in_development()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Database:Provider"] = "Sqlite",
            ["ConnectionStrings:Sqlite"] = "Data Source=:memory:"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration, isDevelopment: true);

        using var provider = services.BuildServiceProvider();
        using var dbContext = provider.GetRequiredService<FlowItamDbContext>();

        Assert.That(dbContext.Database.IsSqlite(), Is.True);
        Assert.That(dbContext.Database.CanConnect(), Is.True);
    }

    [Test]
    public void AddInfrastructure_rejects_sqlite_outside_development()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Database:Provider"] = "Sqlite"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddInfrastructure(configuration, isDevelopment: false));

        Assert.That(exception!.Message, Does.Contain("Development"));
    }
}
