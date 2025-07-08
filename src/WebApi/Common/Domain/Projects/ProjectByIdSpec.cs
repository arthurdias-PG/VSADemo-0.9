namespace VSADemo.Common.Domain.Projects;

// For more on the Specification Pattern see: https://www.ssw.com.au/rules/use-specification-pattern/
public sealed class ProjectByIdSpec : SingleResultSpecification<Project>
{
    public ProjectByIdSpec(ProjectId projectId)
    {
        Query.Where(t => t.Id == projectId);
    }
}