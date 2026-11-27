namespace FlowItam.Core.Organizations;

public sealed class OrganizationMembership
{
    private OrganizationMembership()
    {
    }

    public OrganizationMembership(Guid organizationId, Guid userId, OrganizationRole role)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID is required.", nameof(organizationId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        OrganizationId = organizationId;
        UserId = userId;
        Role = role;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid OrganizationId { get; private set; }

    public Guid UserId { get; private set; }

    public OrganizationRole Role { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    public Organization? Organization { get; private set; }
}