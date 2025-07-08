using VSADemo.Common.Domain.Projects;

// Preserve the namespace across partial classes
// ReSharper disable once CheckNamespace
namespace VSADemo.Common.Persistence;

public partial class ApplicationDbContext
{
    public DbSet<Project> Projects => AggregateRootSet<Project>();
}