namespace VSADemo.Common.Domain.Managers;

// For more on the Specification Pattern see: https://www.ssw.com.au/rules/use-specification-pattern/
public sealed class ManagerByIdSpec : SingleResultSpecification<Manager>
{
    public ManagerByIdSpec(ManagerId managerId)
    {
        Query.Where(t => t.Id == managerId);
    }
}