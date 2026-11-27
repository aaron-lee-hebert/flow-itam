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
            () => services.AddInfrastructure(configuration));

        Assert.That(exception!.Message, Does.Contain("DefaultConnection"));
    }

    [Test]
    public void AddInfrastructure_enforces_verified_tls()
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] =
                "Host=db.example.invalid;Database=flowitam;Username=test;Password=test;SSL Mode=Disable"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var dbContext = provider.GetRequiredService<FlowItamDbContext>();
        var connectionString = new NpgsqlConnectionStringBuilder(
            dbContext.Database.GetDbConnection().ConnectionString);

        Assert.That(connectionString.SslMode, Is.EqualTo(SslMode.VerifyFull));
    }
}
