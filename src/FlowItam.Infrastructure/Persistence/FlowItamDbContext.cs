using Microsoft.EntityFrameworkCore;

namespace FlowItam.Infrastructure.Persistence;

public sealed class FlowItamDbContext(DbContextOptions<FlowItamDbContext> options)
    : DbContext(options)
{
}