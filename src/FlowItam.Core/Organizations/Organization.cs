namespace FlowItam.Core.Organizations;

public sealed class Organization
{
    private Organization()
    {
    }

    public Organization(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Organization name is required.", nameof(name));
        }

        Name = name.Trim();
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = null!;

    public DateTimeOffset CreatedAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    public ICollection<OrganizationMembership> Memberships { get; } =
        new List<OrganizationMembership>();
}