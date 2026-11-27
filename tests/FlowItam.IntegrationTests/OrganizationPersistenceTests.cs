using FlowItam.Core.Organizations;
using FlowItam.Infrastructure;
using FlowItam.Infrastructure.Identity;
using FlowItam.Infrastructure.Persistence;
using FlowItam.Web.Services;
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
            var registrationService = new OrganizationRegistrationService(userManager, dbContext);

            var user = new ApplicationUser
            {
                UserName = "owner@example.test",
                Email = "owner@example.test"
            };
            var createUserResult = await registrationService.CreateOrganizationAccountAsync(
                user,
                "A-long-test-password-2026!",
                "Example Organization");

            Assert.That(
                createUserResult.Succeeded,
                Is.True,
                string.Join("; ", createUserResult.Errors.Select(error => error.Description)));

            var persistedMembership = await dbContext.OrganizationMemberships
                .SingleAsync();
            Assert.That(persistedMembership.Role, Is.EqualTo(OrganizationRole.Owner));
            Assert.That(await dbContext.Users.AnyAsync(storedUser => storedUser.Id == user.Id), Is.True);
            Assert.That(
                await dbContext.Organizations.AnyAsync(organization => organization.Name == "Example Organization"),
                Is.True);

            var rejectedUser = new ApplicationUser
            {
                UserName = "rejected@example.test",
                Email = "rejected@example.test"
            };
            var rejectedResult = await registrationService.CreateOrganizationAccountAsync(
                rejectedUser,
                "weak",
                "Organization Must Not Be Created");

            Assert.That(rejectedResult.Succeeded, Is.False);
            Assert.That(await dbContext.Users.CountAsync(), Is.EqualTo(1));
            Assert.That(await dbContext.Organizations.CountAsync(), Is.EqualTo(1));

            dbContext.OrganizationMemberships.Add(new OrganizationMembership(
                persistedMembership.OrganizationId,
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