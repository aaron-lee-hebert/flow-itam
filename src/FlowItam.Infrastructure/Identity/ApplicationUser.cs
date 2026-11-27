using FlowItam.Core.Organizations;
using Microsoft.AspNetCore.Identity;

namespace FlowItam.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid();
    }

    public ICollection<OrganizationMembership> OrganizationMemberships { get; } =
        new List<OrganizationMembership>();
}