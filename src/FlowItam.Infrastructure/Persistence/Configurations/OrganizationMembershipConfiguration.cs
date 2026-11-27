using FlowItam.Core.Organizations;
using FlowItam.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowItam.Infrastructure.Persistence.Configurations;

public sealed class OrganizationMembershipConfiguration
    : IEntityTypeConfiguration<OrganizationMembership>
{
    public void Configure(EntityTypeBuilder<OrganizationMembership> builder)
    {
        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(membership => membership.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(membership => new { membership.OrganizationId, membership.UserId })
            .IsUnique();

        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.OrganizationMemberships)
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}