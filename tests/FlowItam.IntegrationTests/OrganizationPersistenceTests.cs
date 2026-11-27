using FlowItam.Core.Organizations;
using FlowItam.Infrastructure;
using FlowItam.Infrastructure.Identity;
using FlowItam.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowItam.IntegrationTests;

public class OrganizationPersistenceTests
{
    [Test]
    public async Task Identity_user_and_organization_membership_persist_with_unique_membership()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"flowitam-{Guid.NewGuid():N}.db");
        var settings = new Dictionary<string, string?>
        {
            ["Database:Provider"] = "Sqlite",
            ["ConnectionStrings:Sqlite"] = $"Data Source={databasePath}"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(
            configuration,
            isDevelopment: true,
            contentRootPath: Path.GetTempPath());
            services.AddDefaultIdentity<ApplicationUser>(options =>
                    options.User.RequireUniqueEmail = true)
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<FlowItamDbContext>();

        try
        {
            await using var serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<FlowItamDbContext>();
            await dbContext.Database.MigrateAsync();

            var user = new ApplicationUser
            {
                UserName = "owner@example.test",
                Email = "owner@example.test"
            };
            var createUserResult = await userManager.CreateAsync(user, "A-long-test-password-2026!");

            Assert.That(
                createUserResult.Succeeded,
                Is.True,
                string.Join("; ", createUserResult.Errors.Select(error => error.Description)));

            var organization = new Organization("Example Organization");
            var membership = new OrganizationMembership(
                organization.Id,
                user.Id,
                OrganizationRole.Owner);
            dbContext.Organizations.Add(organization);
            dbContext.OrganizationMemberships.Add(membership);
            await dbContext.SaveChangesAsync();

            var persistedMembership = await dbContext.OrganizationMemberships
                .SingleAsync();
            Assert.That(persistedMembership.Role, Is.EqualTo(OrganizationRole.Owner));
            Assert.That(await dbContext.Users.AnyAsync(storedUser => storedUser.Id == user.Id), Is.True);

            dbContext.OrganizationMemberships.Add(new OrganizationMembership(
                organization.Id,
                user.Id,
                OrganizationRole.ReadOnly));

            Assert.ThrowsAsync<DbUpdateException>(async () => await dbContext.SaveChangesAsync());
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }
}