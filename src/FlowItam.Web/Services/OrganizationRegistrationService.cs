using FlowItam.Core.Organizations;
using FlowItam.Infrastructure.Identity;
using FlowItam.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlowItam.Web.Services;

public sealed class OrganizationRegistrationService(
    UserManager<ApplicationUser> userManager,
    FlowItamDbContext dbContext)
{
    public async Task<IdentityResult> CreateOrganizationAccountAsync(
        ApplicationUser user,
        string password,
        string organizationName,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return result;
        }

        var organization = new Organization(organizationName);
        dbContext.Organizations.Add(organization);
        dbContext.OrganizationMemberships.Add(new OrganizationMembership(
            organization.Id,
            user.Id,
            OrganizationRole.Owner));

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return IdentityResult.Success;
    }
}