using VSADemo.Common.Domain.Managers;

// Preserve the namespace across partial classes
// ReSharper disable once CheckNamespace
namespace VSADemo.Common.Persistence;

public partial class ApplicationDbContext
{
    public DbSet<Manager> Managers => AggregateRootSet<Manager>();
}